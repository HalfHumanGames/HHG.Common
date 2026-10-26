using System.Collections.Generic;
using System.Text;

namespace HHG.Common.Editor
{
    public static class FontUtil
    {
        /// <param name="extraSpacing">Font units added to every glyph's advance (split evenly on both sides).</param>
        /// <param name="preventOverhang">Widen/shift glyphs whose outline sticks out past their advance box
        /// (e.g. a T whose bar hangs over its neighbour) so letters can't overlap.</param>
        public static byte[] Scale(byte[] data, float sx, float sy, string nameSuffix, List<string> warnings,
                                   int extraSpacing = 0, bool preventOverhang = false)
        {
            if (sx <= 0f || sy <= 0f)
            {
                throw new System.ArgumentException("Scale values must be greater than zero.");
            }

            if (warnings == null)
            {
                warnings = new List<string>();
            }

            uint sfnt = BE.U32(data, 0);
            if (sfnt == 0x4F54544F)
            {
                throw new System.NotSupportedException("This is a CFF-based OpenType font (.otf with PostScript outlines). Use the TrueType (.ttf) version of the font.");
            }
            if (sfnt == 0x74746366)
            {
                throw new System.NotSupportedException("Font collections (.ttc) are not supported. Extract a single .ttf first.");
            }
            if (sfnt != 0x00010000 && sfnt != 0x74727565)
            {
                throw new System.NotSupportedException("Not a TrueType font file.");
            }

            int numTables = BE.U16(data, 4);
            Dictionary<string, byte[]> tables = new Dictionary<string, byte[]>();
            for (int i = 0; i < numTables; i++)
            {
                int rec = 12 + 16 * i;
                string tag = Encoding.ASCII.GetString(data, rec, 4);
                int off = (int)BE.U32(data, rec + 8);
                int len = (int)BE.U32(data, rec + 12);
                byte[] tableBytes = new byte[len];
                System.Buffer.BlockCopy(data, off, tableBytes, 0, len);
                tables[tag] = tableBytes;
            }
            foreach (string req in new[] { "head", "maxp", "hhea", "hmtx", "loca", "glyf" })
            {
                if (!tables.ContainsKey(req))
                {
                    throw new System.NotSupportedException("Font is missing required table '" + req + "'.");
                }
            }

            byte[] head = tables["head"], maxp = tables["maxp"], hhea = tables["hhea"];
            byte[] hmtx = tables["hmtx"], locaT = tables["loca"], glyf = tables["glyf"];

            int numGlyphs = BE.U16(maxp, 4);
            bool longLoca = BE.S16(head, 50) == 1;
            int nHM = BE.U16(hhea, 34);

            int[] loca = new int[numGlyphs + 1];
            for (int g = 0; g <= numGlyphs; g++)
            {
                loca[g] = longLoca ? (int)BE.U32(locaT, 4 * g) : BE.U16(locaT, 2 * g) * 2;
            }

            int[] adv = new int[numGlyphs];
            int[] lsb = new int[numGlyphs];
            for (int g = 0; g < numGlyphs; g++)
            {
                if (g < nHM)
                {
                    adv[g] = BE.U16(hmtx, 4 * g);
                    lsb[g] = BE.S16(hmtx, 4 * g + 2);
                }
                else
                {
                    adv[g] = adv[nHM - 1];
                    lsb[g] = BE.S16(hmtx, 4 * nHM + 2 * (g - nHM));
                }
            }

            int[] shift = new int[numGlyphs];
            int fixedOverhangs = 0;
            for (int g = 0; g < numGlyphs; g++)
            {
                int advance = ScaleUnsigned(adv[g], sx) + extraSpacing;
                int dx = (int)System.Math.Round(extraSpacing / 2.0, System.MidpointRounding.AwayFromZero);
                if (loca[g + 1] > loca[g])
                {
                    int xMin = ScaleSigned(BE.S16(glyf, loca[g] + 2), sx) + dx;
                    int xMax = ScaleSigned(BE.S16(glyf, loca[g] + 6), sx) + dx;
                    if (preventOverhang && advance > 0)
                    {
                        bool fixedOne = false;
                        if (xMin < 0)
                        {
                            advance += -xMin;
                            dx += -xMin;
                            xMax += -xMin;
                            fixedOne = true;
                        }
                        if (xMax > advance)
                        {
                            advance = xMax;
                            fixedOne = true;
                        }
                        if (fixedOne)
                        {
                            fixedOverhangs++;
                        }
                    }
                }
                adv[g] = System.Math.Max(0, System.Math.Min(65535, advance));
                shift[g] = dx;
            }
            if (fixedOverhangs > 0)
            {
                warnings.Add($"Widened {fixedOverhangs} glyphs whose outlines hung past their advance width.");
            }

            ByteWriter glyfOut = new ByteWriter();
            int[] newLoca = new int[numGlyphs + 1];
            for (int g = 0; g < numGlyphs; g++)
            {
                newLoca[g] = glyfOut.Length;
                int start = loca[g], end = loca[g + 1];
                if (end > start)
                {
                    byte[] scaled = ScaleGlyph(glyf, start, end - start, sx, sy, shift[g], shift);
                    glyfOut.Bytes(scaled);
                    glyfOut.PadTo4();
                    lsb[g] = BE.S16(scaled, 2); // new xMin = new left side bearing
                }
                else
                {
                    lsb[g] = ScaleSigned(lsb[g], sx) + shift[g];
                }
            }
            newLoca[numGlyphs] = glyfOut.Length;
            tables["glyf"] = glyfOut.ToArray();

            ByteWriter locaOut = new ByteWriter();
            for (int g = 0; g <= numGlyphs; g++)
            {
                locaOut.U32((uint)newLoca[g]);
            }
            tables["loca"] = locaOut.ToArray();

            ByteWriter hmtxOut = new ByteWriter();
            for (int g = 0; g < numGlyphs; g++)
            {
                if (g < nHM)
                {
                    hmtxOut.U16(adv[g]);
                    hmtxOut.S16(lsb[g]);
                }
                else
                {
                    hmtxOut.S16(lsb[g]);
                }
            }
            tables["hmtx"] = hmtxOut.ToArray();

            BE.W16(head, 50, 1);                                  // indexToLocFormat = long
            BE.W32(head, 8, 0);                                   // checkSumAdjustment (recomputed at the end)
            BE.W16(head, 36, ScaleSigned(BE.S16(head, 36), sx));  // xMin
            BE.W16(head, 38, ScaleSigned(BE.S16(head, 38), sy));  // yMin
            BE.W16(head, 40, ScaleSigned(BE.S16(head, 40), sx));  // xMax
            BE.W16(head, 42, ScaleSigned(BE.S16(head, 42), sy));  // yMax

            int maxAdv = 0;
            foreach (int advanceWidth in adv)
            {
                maxAdv = System.Math.Max(maxAdv, advanceWidth);
            }
            BE.W16(hhea, 4, ScaleSigned(BE.S16(hhea, 4), sy));    // ascender
            BE.W16(hhea, 6, ScaleSigned(BE.S16(hhea, 6), sy));    // descender
            BE.W16(hhea, 8, ScaleSigned(BE.S16(hhea, 8), sy));    // lineGap
            BE.W16(hhea, 10, maxAdv);                             // advanceWidthMax
            BE.W16(hhea, 12, ScaleSigned(BE.S16(hhea, 12), sx));  // minLeftSideBearing
            BE.W16(hhea, 14, ScaleSigned(BE.S16(hhea, 14), sx));  // minRightSideBearing
            BE.W16(hhea, 16, ScaleSigned(BE.S16(hhea, 16), sx));  // xMaxExtent
            BE.W16(hhea, 18, ScaleSigned(BE.S16(hhea, 18), sy));  // caretSlopeRise
            BE.W16(hhea, 20, ScaleSigned(BE.S16(hhea, 20), sx));  // caretSlopeRun
            BE.W16(hhea, 22, ScaleSigned(BE.S16(hhea, 22), sx));  // caretOffset

            if (BE.U32(maxp, 0) == 0x00010000 && maxp.Length >= 32) // no instructions any more
            {
                BE.W16(maxp, 26, 0);
            }

            if (tables.TryGetValue("OS/2", out byte[] os2))
            {
                void ScaleX(int offset)
                {
                    if (offset + 2 <= os2.Length)
                    {
                        BE.W16(os2, offset, ScaleSigned(BE.S16(os2, offset), sx));
                    }
                }
                void ScaleY(int offset)
                {
                    if (offset + 2 <= os2.Length)
                    {
                        BE.W16(os2, offset, ScaleSigned(BE.S16(os2, offset), sy));
                    }
                }
                void ScaleYUnsigned(int offset)
                {
                    if (offset + 2 <= os2.Length)
                    {
                        BE.W16(os2, offset, ScaleUnsigned(BE.U16(os2, offset), sy));
                    }
                }
                ScaleX(2);                                       // xAvgCharWidth
                ScaleX(10); ScaleY(12); ScaleX(14); ScaleY(16);   // subscript size / offset
                ScaleX(18); ScaleY(20); ScaleX(22); ScaleY(24);   // superscript size / offset
                ScaleY(26); ScaleY(28);                           // strikeout size / position
                ScaleY(68); ScaleY(70); ScaleY(72);               // typo ascender / descender / line gap
                ScaleYUnsigned(74); ScaleYUnsigned(76);           // win ascent / descent
                if (BE.U16(os2, 0) >= 2)
                {
                    ScaleY(86); ScaleY(88); // x-height, cap height
                }
            }

            if (tables.TryGetValue("post", out byte[] post) && post.Length >= 12)
            {
                BE.W16(post, 8, ScaleSigned(BE.S16(post, 8), sy));   // underlinePosition
                BE.W16(post, 10, ScaleSigned(BE.S16(post, 10), sy)); // underlineThickness
            }

            if (tables.TryGetValue("kern", out byte[] kern))
            {
                ScaleKern(kern, sx, sy, warnings);
            }
            if (tables.TryGetValue("GPOS", out byte[] gpos))
            {
                ScaleGpos(gpos, sx, sy, warnings);
            }

            // TrueType hinting is removed: hints are tied to the original coordinates and would
            // distort the scaled outlines, and Unity/TMP doesn't use them anyway. DSIG is dropped
            // because a signature is invalid once the file changes.
            bool hadHints = false;
            foreach (string tag in new[] { "fpgm", "prep", "cvt ", "hdmx", "LTSH", "VDMX", "DSIG" })
            {
                if (tables.Remove(tag) && tag != "DSIG")
                {
                    hadHints = true;
                }
            }
            if (hadHints)
            {
                warnings.Add("TrueType hinting was removed (it doesn't survive scaling; Unity/TMP doesn't use it).");
            }
            if (tables.ContainsKey("vhea") || tables.ContainsKey("vmtx"))
            {
                warnings.Add("Vertical metrics (vhea/vmtx) were left unscaled.");
            }

            if (!string.IsNullOrEmpty(nameSuffix) && tables.TryGetValue("name", out byte[] name))
            {
                byte[] renamed = RenameFont(name, nameSuffix, warnings);
                if (renamed != null)
                {
                    tables["name"] = renamed;
                }
            }

            return WriteSfnt(tables, sfnt);
        }

        public struct PairKern
        {
            public int First, Second;            // glyph indices
            public int FirstXPlacement, FirstXAdvance, SecondXPlacement; // font units
        }

        /// <summary>Reads pair kerning from 'kern' (format 0) and GPOS pair adjustment lookups.
        /// GPOS pairs take priority over 'kern' pairs for the same glyph pair.</summary>
        public static List<PairKern> ReadPairKerning(byte[] data)
        {
            Dictionary<long, PairKern> result = new Dictionary<long, PairKern>();
            int numTables = BE.U16(data, 4);
            int kernOff = -1, gposOff = -1;
            for (int i = 0; i < numTables; i++)
            {
                int rec = 12 + 16 * i;
                string tag = Encoding.ASCII.GetString(data, rec, 4);
                if (tag == "kern")
                {
                    kernOff = (int)BE.U32(data, rec + 8);
                }
                if (tag == "GPOS")
                {
                    gposOff = (int)BE.U32(data, rec + 8);
                }
            }

            if (kernOff >= 0 && BE.U16(data, kernOff) == 0)
            {
                int subtableCount = BE.U16(data, kernOff + 2), pos = kernOff + 4;
                for (int t = 0; t < subtableCount; t++)
                {
                    int length = BE.U16(data, pos + 2), coverage = BE.U16(data, pos + 4);
                    if ((coverage >> 8) == 0 && (coverage & 1) != 0 && (coverage & 4) == 0)
                    {
                        int pairs = BE.U16(data, pos + 6);
                        for (int k = 0; k < pairs; k++)
                        {
                            int pairOffset = pos + 14 + 6 * k;
                            Add(result, BE.U16(data, pairOffset), BE.U16(data, pairOffset + 2), 0, BE.S16(data, pairOffset + 4), 0);
                        }
                    }
                    if (length <= 0)
                    {
                        break;
                    }
                    pos += length;
                }
            }

            if (gposOff >= 0)
            {
                int ll = gposOff + BE.U16(data, gposOff + 8);
                int lookupCount = BE.U16(data, ll);
                for (int i = 0; i < lookupCount; i++)
                {
                    int lk = ll + BE.U16(data, ll + 2 + 2 * i);
                    int type = BE.U16(data, lk), subCount = BE.U16(data, lk + 4);
                    for (int j = 0; j < subCount; j++)
                    {
                        int st = lk + BE.U16(data, lk + 6 + 2 * j);
                        int effectiveType = type;
                        if (effectiveType == 9)
                        {
                            effectiveType = BE.U16(data, st + 2);
                            st += (int)BE.U32(data, st + 4);
                        }
                        if (effectiveType != 2)
                        {
                            continue;
                        }

                        int fmt = BE.U16(data, st);
                        List<int> cov = ReadCoverage(data, st + BE.U16(data, st + 2));
                        int vf1 = BE.U16(data, st + 4), vf2 = BE.U16(data, st + 6);
                        int s1 = ValueSize(vf1), s2 = ValueSize(vf2);
                        if (fmt == 1)
                        {
                            int setCount = BE.U16(data, st + 8);
                            for (int k = 0; k < setCount && k < cov.Count; k++)
                            {
                                int ps = st + BE.U16(data, st + 10 + 2 * k);
                                int pvc = BE.U16(data, ps);
                                for (int m = 0; m < pvc; m++)
                                {
                                    int recordOffset = ps + 2 + m * (2 + s1 + s2);
                                    int second = BE.U16(data, recordOffset);
                                    ReadValue(data, recordOffset + 2, vf1, out int p1, out int a1);
                                    ReadValue(data, recordOffset + 2 + s1, vf2, out int p2, out _);
                                    Add(result, cov[k], second, p1, a1, p2, overwrite: true);
                                }
                            }
                        }
                        else if (fmt == 2)
                        {
                            Dictionary<int, int> cd1 = ReadClassDef(data, st + BE.U16(data, st + 8));
                            Dictionary<int, int> cd2 = ReadClassDef(data, st + BE.U16(data, st + 10));
                            int c1 = BE.U16(data, st + 12), c2 = BE.U16(data, st + 14);
                            Dictionary<int, List<int>> byClass2 = new Dictionary<int, List<int>>();
                            foreach (KeyValuePair<int, int> kv in cd2)
                            {
                                if (!byClass2.TryGetValue(kv.Value, out List<int> list))
                                {
                                    byClass2[kv.Value] = list = new List<int>();
                                }
                                list.Add(kv.Key);
                            }
                            foreach (int first in cov)
                            {
                                int cls1 = cd1.TryGetValue(first, out int classValue) ? classValue : 0;
                                if (cls1 >= c1)
                                {
                                    continue;
                                }
                                for (int cls2 = 1; cls2 < c2; cls2++) // class 0 = "everything else", skipped
                                {
                                    int recordOffset = st + 16 + (cls1 * c2 + cls2) * (s1 + s2);
                                    ReadValue(data, recordOffset, vf1, out int p1, out int a1);
                                    ReadValue(data, recordOffset + s1, vf2, out int p2, out _);
                                    if (p1 == 0 && a1 == 0 && p2 == 0)
                                    {
                                        continue;
                                    }
                                    if (!byClass2.TryGetValue(cls2, out List<int> seconds))
                                    {
                                        continue;
                                    }
                                    foreach (int second in seconds)
                                    {
                                        Add(result, first, second, p1, a1, p2, overwrite: true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new List<PairKern>(result.Values);
        }

        private static void Add(Dictionary<long, PairKern> kerningPairs, int first, int second, int p1, int a1, int p2, bool overwrite = false)
        {
            if (p1 == 0 && a1 == 0 && p2 == 0)
            {
                return;
            }
            long key = ((long)first << 16) | (uint)second;
            if (!overwrite && kerningPairs.ContainsKey(key))
            {
                return;
            }
            kerningPairs[key] = new PairKern { First = first, Second = second, FirstXPlacement = p1, FirstXAdvance = a1, SecondXPlacement = p2 };
        }

        private static void ReadValue(byte[] data, int pos, int vf, out int xPlacement, out int xAdvance)
        {
            xPlacement = 0;
            xAdvance = 0;
            int offset = pos;
            for (int b = 0; b < 8; b++)
            {
                if ((vf & (1 << b)) == 0)
                {
                    continue;
                }
                if (b == 0)
                {
                    xPlacement = BE.S16(data, offset);
                }
                if (b == 2)
                {
                    xAdvance = BE.S16(data, offset);
                }
                offset += 2;
            }
        }

        private static List<int> ReadCoverage(byte[] data, int off)
        {
            List<int> list = new List<int>();
            int fmt = BE.U16(data, off);
            if (fmt == 1)
            {
                int count = BE.U16(data, off + 2);
                for (int i = 0; i < count; i++)
                {
                    list.Add(BE.U16(data, off + 4 + 2 * i));
                }
            }
            else if (fmt == 2)
            {
                int count = BE.U16(data, off + 2);
                for (int i = 0; i < count; i++)
                {
                    int entryOffset = off + 4 + 6 * i;
                    int start = BE.U16(data, entryOffset), end = BE.U16(data, entryOffset + 2);
                    for (int g = start; g <= end; g++)
                    {
                        list.Add(g);
                    }
                }
            }
            return list;
        }

        private static Dictionary<int, int> ReadClassDef(byte[] data, int off)
        {
            Dictionary<int, int> classes = new Dictionary<int, int>();
            int fmt = BE.U16(data, off);
            if (fmt == 1)
            {
                int start = BE.U16(data, off + 2), count = BE.U16(data, off + 4);
                for (int i = 0; i < count; i++)
                {
                    int classValue = BE.U16(data, off + 6 + 2 * i);
                    if (classValue != 0)
                    {
                        classes[start + i] = classValue;
                    }
                }
            }
            else if (fmt == 2)
            {
                int count = BE.U16(data, off + 2);
                for (int i = 0; i < count; i++)
                {
                    int entryOffset = off + 4 + 6 * i;
                    int start = BE.U16(data, entryOffset), end = BE.U16(data, entryOffset + 2), classValue = BE.U16(data, entryOffset + 4);
                    if (classValue != 0)
                    {
                        for (int g = start; g <= end; g++)
                        {
                            classes[g] = classValue;
                        }
                    }
                }
            }
            return classes;
        }

        private static byte[] ScaleGlyph(byte[] src, int off, int len, float sx, float sy, int dx, int[] shifts)
        {
            byte[] glyph = new byte[len];
            System.Buffer.BlockCopy(src, off, glyph, 0, len);
            short nc = BE.S16(glyph, 0);
            return nc >= 0 ? ScaleSimple(glyph, nc, sx, sy, dx) : ScaleComposite(glyph, sx, sy, dx, shifts);
        }

        private static byte[] ScaleSimple(byte[] glyph, int nc, float sx, float sy, int dx)
        {
            int pos = 10;
            int[] endPts = new int[nc];
            for (int i = 0; i < nc; i++)
            {
                endPts[i] = BE.U16(glyph, pos);
                pos += 2;
            }
            int numPts = nc > 0 ? endPts[nc - 1] + 1 : 0;
            int insLen = BE.U16(glyph, pos);
            pos += 2 + insLen;

            byte[] flags = new byte[numPts];
            for (int i = 0; i < numPts;)
            {
                byte flag = glyph[pos++];
                flags[i++] = flag;
                if ((flag & 0x08) != 0)
                {
                    int repeat = glyph[pos++];
                    while (repeat-- > 0 && i < numPts)
                    {
                        flags[i++] = flag;
                    }
                }
            }

            int[] xs = new int[numPts];
            int[] ys = new int[numPts];
            int coord = 0;
            for (int i = 0; i < numPts; i++)
            {
                byte flag = flags[i];
                if ((flag & 0x02) != 0)
                {
                    int delta = glyph[pos++];
                    coord += (flag & 0x10) != 0 ? delta : -delta;
                }
                else if ((flag & 0x10) == 0)
                {
                    coord += BE.S16(glyph, pos);
                    pos += 2;
                }
                xs[i] = coord;
            }
            coord = 0;
            for (int i = 0; i < numPts; i++)
            {
                byte flag = flags[i];
                if ((flag & 0x04) != 0)
                {
                    int delta = glyph[pos++];
                    coord += (flag & 0x20) != 0 ? delta : -delta;
                }
                else if ((flag & 0x20) == 0)
                {
                    coord += BE.S16(glyph, pos);
                    pos += 2;
                }
                ys[i] = coord;
            }

            int xMin = 0, yMin = 0, xMax = 0, yMax = 0;
            for (int i = 0; i < numPts; i++)
            {
                xs[i] = ScaleSigned(xs[i], sx) + dx;
                ys[i] = ScaleSigned(ys[i], sy);
                if (i == 0)
                {
                    xMin = xMax = xs[0];
                    yMin = yMax = ys[0];
                }
                else
                {
                    xMin = System.Math.Min(xMin, xs[i]); xMax = System.Math.Max(xMax, xs[i]);
                    yMin = System.Math.Min(yMin, ys[i]); yMax = System.Math.Max(yMax, ys[i]);
                }
            }

            // re-encode (instructions dropped)
            byte[] newFlags = new byte[numPts];
            ByteWriter xb = new ByteWriter();
            ByteWriter yb = new ByteWriter();
            int px = 0, py = 0;
            for (int i = 0; i < numPts; i++)
            {
                byte flag = (byte)(flags[i] & 0x41); // keep ON_CURVE and OVERLAP_SIMPLE
                int ddx = xs[i] - px, dy = ys[i] - py;
                px = xs[i]; py = ys[i];

                if (ddx == 0)
                {
                    flag |= 0x10;
                }
                else if (ddx >= -255 && ddx <= 255)
                {
                    flag |= 0x02;
                    if (ddx > 0)
                    {
                        flag |= 0x10;
                    }
                    xb.U8(System.Math.Abs(ddx));
                }
                else
                {
                    xb.S16(ddx);
                }

                if (dy == 0)
                {
                    flag |= 0x20;
                }
                else if (dy >= -255 && dy <= 255)
                {
                    flag |= 0x04;
                    if (dy > 0)
                    {
                        flag |= 0x20;
                    }
                    yb.U8(System.Math.Abs(dy));
                }
                else
                {
                    yb.S16(dy);
                }

                newFlags[i] = flag;
            }

            ByteWriter writer = new ByteWriter();
            writer.S16(nc); writer.S16(xMin); writer.S16(yMin); writer.S16(xMax); writer.S16(yMax);
            foreach (int endPoint in endPts)
            {
                writer.U16(endPoint);
            }
            writer.U16(0); // instructionLength
            for (int i = 0; i < numPts;)
            {
                byte flag = newFlags[i];
                int run = 1;
                while (i + run < numPts && newFlags[i + run] == flag && run < 256)
                {
                    run++;
                }
                if (run > 1)
                {
                    writer.U8(flag | 0x08);
                    writer.U8(run - 1);
                }
                else
                {
                    writer.U8(flag);
                }
                i += run;
            }
            writer.Bytes(xb.ToArray());
            writer.Bytes(yb.ToArray());
            return writer.ToArray();
        }

        private static byte[] ScaleComposite(byte[] glyph, float sx, float sy, int dx, int[] shifts)
        {
            ByteWriter writer = new ByteWriter();
            writer.S16(BE.S16(glyph, 0));
            writer.S16(ScaleSigned(BE.S16(glyph, 2), sx) + dx); writer.S16(ScaleSigned(BE.S16(glyph, 4), sy));
            writer.S16(ScaleSigned(BE.S16(glyph, 6), sx) + dx); writer.S16(ScaleSigned(BE.S16(glyph, 8), sy));

            int pos = 10;
            int flags;
            do
            {
                flags = BE.U16(glyph, pos);
                int glyphIndex = BE.U16(glyph, pos + 2);
                pos += 4;
                bool words = (flags & 0x0001) != 0;
                bool xy = (flags & 0x0002) != 0;
                int a1, a2;
                if (words)
                {
                    a1 = xy ? BE.S16(glyph, pos) : BE.U16(glyph, pos);
                    a2 = xy ? BE.S16(glyph, pos + 2) : BE.U16(glyph, pos + 2);
                    pos += 4;
                }
                else
                {
                    a1 = xy ? (sbyte)glyph[pos] : glyph[pos];
                    a2 = xy ? (sbyte)glyph[pos + 1] : glyph[pos + 1];
                    pos += 2;
                }
                if (xy)
                {
                    // components were already shifted by their own amount; correct to this glyph's shift
                    int compShift = glyphIndex < shifts.Length ? shifts[glyphIndex] : 0;
                    a1 = ScaleSigned(a1, sx) + dx - compShift;
                    a2 = ScaleSigned(a2, sy);
                }

                int newFlags = (flags | 0x0001) & ~0x0100; // always word args, no instructions
                writer.U16(newFlags); writer.U16(glyphIndex);
                writer.U16(a1 & 0xFFFF); writer.U16(a2 & 0xFFFF);

                if ((flags & 0x0008) != 0) // uniform scale: unchanged
                {
                    writer.Bytes(glyph, pos, 2); pos += 2;
                }
                else if ((flags & 0x0040) != 0) // x/y scale: unchanged
                {
                    writer.Bytes(glyph, pos, 4); pos += 4;
                }
                else if ((flags & 0x0080) != 0) // 2x2: M' = S·M·S⁻¹
                {
                    double xscale = ReadF2Dot14(glyph, pos), scale01 = ReadF2Dot14(glyph, pos + 2), scale10 = ReadF2Dot14(glyph, pos + 4), yscale = ReadF2Dot14(glyph, pos + 6);
                    scale01 *= sy / sx;
                    scale10 *= sx / sy;
                    writer.S16(ToF2Dot14(xscale)); writer.S16(ToF2Dot14(scale01)); writer.S16(ToF2Dot14(scale10)); writer.S16(ToF2Dot14(yscale));
                    pos += 8;
                }
            } while ((flags & 0x0020) != 0);

            return writer.ToArray();
        }

        private static void ScaleKern(byte[] kern, float sx, float sy, List<string> warnings)
        {
            if (BE.U16(kern, 0) != 0)
            {
                warnings.Add("Apple-format 'kern' table was left unscaled.");
                return;
            }
            int subtableCount = BE.U16(kern, 2);
            int pos = 4;
            for (int t = 0; t < subtableCount && pos + 6 <= kern.Length; t++)
            {
                int length = BE.U16(kern, pos + 2);
                int coverage = BE.U16(kern, pos + 4);
                if ((coverage >> 8) == 0)
                {
                    float scale = (coverage & 1) != 0 ? sx : sy;
                    int pairs = BE.U16(kern, pos + 6);
                    for (int i = 0; i < pairs; i++)
                    {
                        int pairOffset = pos + 14 + 6 * i + 4;
                        if (pairOffset + 2 <= kern.Length)
                        {
                            BE.W16(kern, pairOffset, ScaleSigned(BE.S16(kern, pairOffset), scale));
                        }
                    }
                }
                else
                {
                    warnings.Add("A 'kern' subtable in format " + (coverage >> 8) + " was left unscaled.");
                }
                if (length <= 0)
                {
                    break;
                }
                pos += length;
            }
        }

        private static void ScaleGpos(byte[] gpos, float sx, float sy, List<string> warnings)
        {
            HashSet<int> patched = new HashSet<int>();
            bool warnedAnchors = false;
            int ll = BE.U16(gpos, 8);
            if (ll == 0)
            {
                return;
            }
            int lookupCount = BE.U16(gpos, ll);
            for (int i = 0; i < lookupCount; i++)
            {
                int lk = ll + BE.U16(gpos, ll + 2 + 2 * i);
                int type = BE.U16(gpos, lk);
                int subCount = BE.U16(gpos, lk + 4);
                for (int j = 0; j < subCount; j++)
                {
                    int st = lk + BE.U16(gpos, lk + 6 + 2 * j);
                    int effectiveType = type;
                    if (effectiveType == 9)
                    {
                        effectiveType = BE.U16(gpos, st + 2);
                        st += (int)BE.U32(gpos, st + 4);
                    }

                    if (effectiveType == 1)
                    {
                        int fmt = BE.U16(gpos, st), vf = BE.U16(gpos, st + 4);
                        if (fmt == 1)
                        {
                            PatchValue(gpos, st + 6, vf, sx, sy, patched);
                        }
                        else
                        {
                            int cnt = BE.U16(gpos, st + 6), sz = ValueSize(vf);
                            for (int k = 0; k < cnt; k++)
                            {
                                PatchValue(gpos, st + 8 + k * sz, vf, sx, sy, patched);
                            }
                        }
                    }
                    else if (effectiveType == 2)
                    {
                        int fmt = BE.U16(gpos, st);
                        int vf1 = BE.U16(gpos, st + 4), vf2 = BE.U16(gpos, st + 6);
                        int s1 = ValueSize(vf1), s2 = ValueSize(vf2);
                        if (fmt == 1)
                        {
                            int setCount = BE.U16(gpos, st + 8);
                            for (int k = 0; k < setCount; k++)
                            {
                                int ps = st + BE.U16(gpos, st + 10 + 2 * k);
                                int pvc = BE.U16(gpos, ps);
                                for (int m = 0; m < pvc; m++)
                                {
                                    int recordOffset = ps + 2 + m * (2 + s1 + s2);
                                    PatchValue(gpos, recordOffset + 2, vf1, sx, sy, patched);
                                    PatchValue(gpos, recordOffset + 2 + s1, vf2, sx, sy, patched);
                                }
                            }
                        }
                        else if (fmt == 2)
                        {
                            int c1 = BE.U16(gpos, st + 12), c2 = BE.U16(gpos, st + 14);
                            int baseOffset = st + 16;
                            for (int class1 = 0; class1 < c1; class1++)
                            {
                                for (int class2 = 0; class2 < c2; class2++)
                                {
                                    int recordOffset = baseOffset + (class1 * c2 + class2) * (s1 + s2);
                                    PatchValue(gpos, recordOffset, vf1, sx, sy, patched);
                                    PatchValue(gpos, recordOffset + s1, vf2, sx, sy, patched);
                                }
                            }
                        }
                    }
                    else if (effectiveType >= 3 && effectiveType <= 8 && !warnedAnchors)
                    {
                        warnings.Add("GPOS attachment lookups (cursive/mark anchors) were left unscaled.");
                        warnedAnchors = true;
                    }
                }
            }
        }

        private static int ValueSize(int vf)
        {
            int count = 0;
            for (int b = 0; b < 8; b++)
            {
                if ((vf & (1 << b)) != 0)
                {
                    count++;
                }
            }
            return count * 2;
        }

        private static void PatchValue(byte[] table, int pos, int vf, float sx, float sy, HashSet<int> patched)
        {
            int offset = pos;
            for (int b = 0; b < 8; b++)
            {
                int bit = 1 << b;
                if ((vf & bit) == 0)
                {
                    continue;
                }
                if (b < 4 && offset + 2 <= table.Length && patched.Add(offset))
                {
                    float scale = (bit == 0x1 || bit == 0x4) ? sx : sy; // XPlacement, XAdvance vs YPlacement, YAdvance
                    BE.W16(table, offset, ScaleSigned(BE.S16(table, offset), scale));
                }
                offset += 2; // device table offsets (bits 4-7) are skipped
            }
        }

        private static byte[] RenameFont(byte[] name, string suffix, List<string> warnings)
        {
            if (BE.U16(name, 0) != 0)
            {
                warnings.Add("Name table format 1: font names were not changed.");
                return null;
            }
            int count = BE.U16(name, 2);
            int storage = BE.U16(name, 4);
            string psSuffix = MakePostScript(suffix);

            List<(int plat, int enc, int lang, int id, byte[] bytes)> recs = new List<(int plat, int enc, int lang, int id, byte[] bytes)>();
            for (int i = 0; i < count; i++)
            {
                int recordOffset = 6 + 12 * i;
                int plat = BE.U16(name, recordOffset), enc = BE.U16(name, recordOffset + 2), lang = BE.U16(name, recordOffset + 4), id = BE.U16(name, recordOffset + 6);
                int len = BE.U16(name, recordOffset + 8), off = BE.U16(name, recordOffset + 10);
                byte[] bytes = new byte[len];
                System.Buffer.BlockCopy(name, storage + off, bytes, 0, len);

                bool unicode = plat == 0 || plat == 3;
                bool mac = plat == 1 && enc == 0;
                if ((unicode || mac) && (id == 1 || id == 3 || id == 4 || id == 6 || id == 16))
                {
                    string text = unicode ? Encoding.BigEndianUnicode.GetString(bytes) : Latin1(bytes);
                    text = id == 6 ? text + psSuffix : text + suffix;
                    bytes = unicode ? Encoding.BigEndianUnicode.GetBytes(text) : Latin1(text);
                }
                recs.Add((plat, enc, lang, id, bytes));
            }

            ByteWriter writer = new ByteWriter();
            writer.U16(0); writer.U16(count); writer.U16(6 + 12 * count);
            int offset = 0;
            foreach (var record in recs)
            {
                writer.U16(record.plat); writer.U16(record.enc); writer.U16(record.lang); writer.U16(record.id);
                writer.U16(record.bytes.Length); writer.U16(offset);
                offset += record.bytes.Length;
            }
            foreach (var record in recs)
            {
                writer.Bytes(record.bytes);
            }
            return writer.ToArray();
        }

        private static string MakePostScript(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in value)
            {
                if (ch < 128 && (char.IsLetterOrDigit(ch) || ch == '-'))
                {
                    sb.Append(ch);
                }
            }
            return sb.Length > 0 ? "-" + sb : "";
        }

        private static string Latin1(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }
            return new string(chars);
        }

        private static byte[] Latin1(string value)
        {
            byte[] bytes = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                bytes[i] = value[i] < 256 ? (byte)value[i] : (byte)'?';
            }
            return bytes;
        }

        private static byte[] WriteSfnt(Dictionary<string, byte[]> tables, uint sfntVersion)
        {
            List<string> tags = new List<string>(tables.Keys);
            tags.Sort(string.CompareOrdinal);
            int tableCount = tags.Count;
            int entrySelector = 0;
            while ((1 << (entrySelector + 1)) <= tableCount)
            {
                entrySelector++;
            }
            int searchRange = (1 << entrySelector) * 16;

            ByteWriter writer = new ByteWriter();
            writer.U32(sfntVersion); writer.U16(tableCount); writer.U16(searchRange); writer.U16(entrySelector); writer.U16(tableCount * 16 - searchRange);

            int offset = 12 + 16 * tableCount;
            int headOffset = -1;
            foreach (string tag in tags)
            {
                byte[] tableBytes = tables[tag];
                if (tag == "head")
                {
                    headOffset = offset;
                }
                writer.Bytes(Encoding.ASCII.GetBytes(tag));
                writer.U32(Checksum(tableBytes));
                writer.U32((uint)offset);
                writer.U32((uint)tableBytes.Length);
                offset += (tableBytes.Length + 3) & ~3;
            }
            foreach (string tag in tags)
            {
                writer.Bytes(tables[tag]);
                writer.PadTo4();
            }

            byte[] file = writer.ToArray();
            if (headOffset >= 0)
            {
                BE.W32(file, headOffset + 8, 0xB1B0AFBA - Checksum(file));
            }
            return file;
        }

        private static uint Checksum(byte[] bytes)
        {
            uint sum = 0;
            int wordCount = (bytes.Length + 3) / 4;
            for (int i = 0; i < wordCount; i++)
            {
                int offset = i * 4;
                uint value = 0;
                for (int k = 0; k < 4; k++)
                {
                    value = (value << 8) | (offset + k < bytes.Length ? bytes[offset + k] : 0u);
                }
                sum = unchecked(sum + value);
            }
            return sum;
        }

        private static int ScaleSigned(int value, float scale) => System.Math.Max(-32768, System.Math.Min(32767, (int)System.Math.Round(value * (double)scale, System.MidpointRounding.AwayFromZero)));
        private static int ScaleUnsigned(int value, float scale) => System.Math.Max(0, System.Math.Min(65535, (int)System.Math.Round(value * (double)scale, System.MidpointRounding.AwayFromZero)));
        private static double ReadF2Dot14(byte[] bytes, int offset) => BE.S16(bytes, offset) / 16384.0;
        private static int ToF2Dot14(double value) => (int)System.Math.Round(System.Math.Max(-2.0, System.Math.Min(1.99993896484375, value)) * 16384.0);

        private static class BE
        {
            public static ushort U16(byte[] bytes, int offset) => (ushort)((bytes[offset] << 8) | bytes[offset + 1]);
            public static short S16(byte[] bytes, int offset) => (short)((bytes[offset] << 8) | bytes[offset + 1]);
            public static uint U32(byte[] bytes, int offset) => ((uint)bytes[offset] << 24) | ((uint)bytes[offset + 1] << 16) | ((uint)bytes[offset + 2] << 8) | bytes[offset + 3];

            public static void W16(byte[] bytes, int offset, int value)
            {
                bytes[offset] = (byte)(value >> 8);
                bytes[offset + 1] = (byte)value;
            }

            public static void W32(byte[] bytes, int offset, uint value)
            {
                bytes[offset] = (byte)(value >> 24);
                bytes[offset + 1] = (byte)(value >> 16);
                bytes[offset + 2] = (byte)(value >> 8);
                bytes[offset + 3] = (byte)value;
            }
        }

        private class ByteWriter
        {
            private readonly List<byte> buf = new List<byte>();
            public int Length => buf.Count;
            public void U8(int value) => buf.Add((byte)value);

            public void U16(int value)
            {
                buf.Add((byte)(value >> 8));
                buf.Add((byte)value);
            }

            public void S16(int value) => U16(value & 0xFFFF);

            public void U32(uint value)
            {
                buf.Add((byte)(value >> 24));
                buf.Add((byte)(value >> 16));
                buf.Add((byte)(value >> 8));
                buf.Add((byte)value);
            }

            public void Bytes(byte[] bytes) => buf.AddRange(bytes);

            public void Bytes(byte[] bytes, int off, int len)
            {
                for (int i = 0; i < len; i++)
                {
                    buf.Add(bytes[off + i]);
                }
            }

            public void PadTo4()
            {
                while ((buf.Count & 3) != 0)
                {
                    buf.Add(0);
                }
            }

            public byte[] ToArray() => buf.ToArray();
        }
    }
}
