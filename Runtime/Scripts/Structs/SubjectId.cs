using System;

namespace HHG.Common
{
    public struct SubjectId : IEquatable<SubjectId>
    {
        public Type Type { get; set; }
        public object Id { get; set; }

        public SubjectId(Type type, object id)
        {
            Type = type;
            Id = id;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + Type.GetHashCode();
                hash = hash * 29 + (Id == null ? 0 : Id.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is SubjectId)
            {
                SubjectId otherId = (SubjectId) other;
                return otherId == this;
            }

            return false;
        }

        public bool Equals(SubjectId other)
        {
            return this == other;
        }

        public static bool operator ==(SubjectId left, SubjectId right)
        {
            return left.Type == right.Type && Equals(left.Id, right.Id);
        }

        public static bool operator !=(SubjectId left, SubjectId right)
        {
            return !left.Equals(right);
        }
    }
}