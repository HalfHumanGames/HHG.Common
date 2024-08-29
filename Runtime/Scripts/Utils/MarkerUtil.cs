using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public static class MarkerUtil
    {
        private static HashSet<Mark> marked = new HashSet<Mark>();

        public static bool HasMark(object id, Vector2Int position)
        {
            return marked.Contains(new Mark(id, position));
        }

        public static void Mark(object id, Vector2Int position)
        {
            marked.Add(new Mark(id, position));
        }

        public static void Unmark(object id, Vector2Int position)
        {
            marked.Remove(new Mark(id, position));
        }
    }

    public struct Mark : IEquatable<Mark>
    {
        public object Id { get; set; }
        public Vector2Int Position { get; set; }

        public Mark(object id, Vector2Int position)
        {
            Id = id;
            Position = position;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + Id.GetHashCode();
                hash = hash * 29 + Position.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is Mark otherMark)
            {
                return this == otherMark;
            }

            return false;
        }

        public bool Equals(Mark other)
        {
            return this == other;
        }

        public static bool operator ==(Mark left, Mark right)
        {
            return Equals(left.Id, right.Id) && left.Position == right.Position;
        }

        public static bool operator !=(Mark left, Mark right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"MarkId(Id = {Id}, Position = {Position})";
        }
    }
}