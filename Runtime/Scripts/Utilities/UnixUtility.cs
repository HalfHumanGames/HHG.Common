namespace HHG.Common
{
    public static class UnixUtility
    {
        public static int GetUnixPermissions(int unixPermissions)
        {
            return ((ushort)(unixPermissions % 1000) & 0x1FF) << 16;
        }
    }
}