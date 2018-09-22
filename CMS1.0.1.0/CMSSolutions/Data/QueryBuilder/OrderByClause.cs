using System;
using System.Collections.Generic;
using System.Text;
using CMSSolutions.Extensions;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public struct OrderByClause
    {
        public string Field { get; set; }

        public SortDirection SortDirection { get; set; }
    }

    [Serializable]
    public class OrderByClauseCollection<TQueryType> : List<OrderByClause>
        where TQueryType : Query<TQueryType>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            string separator = string.Concat(Query<TQueryType>.SpaceEscapeEnd, ".", Query<TQueryType>.SpaceEscapeStart);

            sb.Append("ORDER BY ");
            foreach (OrderByClause orderByClause in this)
            {
                sb.Append(
                    Query<TQueryType>.SpaceEscapeStart, orderByClause.Field.Replace(".", separator), Query<TQueryType>.SpaceEscapeEnd,
                    orderByClause.SortDirection == SortDirection.Descending ? " DESC " : " ASC ");
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2); //Remove last comma

            return sb.ToString();
        }
    }
}