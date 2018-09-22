using System;
using CMSSolutions.Extensions;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public enum TopType
    {
        Records = 0,
        Percent
    }

    [Serializable]
    public struct TopClause
    {
        public int Count { get; set; }

        public TopType TopType { get; set; }

        public static TopClause Empty
        {
            get
            {
                return new TopClause
                {
                    Count = 100,
                    TopType = TopType.Percent
                };
            }
        }

        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        public static bool operator ==(TopClause clause1, TopClause clause2)
        {
            if (clause1.Count != clause2.Count)
            {
                return false;
            }

            if (clause1.TopType != clause2.TopType)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(TopClause clause1, TopClause clause2)
        {
            return !(clause1 == clause2);
        }

        public override string ToString()
        {
            var query = string.Empty;
            if (this != Empty)
            {
                query = string.Concat("TOP ", Count, " ");
                if (TopType == TopType.Percent)
                {
                    query = query.Append("PERCENT ");
                }
            }
            return query;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is TopClause))
            {
                return false;
            }

            return this == (TopClause)obj;
        }

        public override int GetHashCode()
        {
            return Count.GetHashCode() ^ TopType.GetHashCode();
        }
    }
}