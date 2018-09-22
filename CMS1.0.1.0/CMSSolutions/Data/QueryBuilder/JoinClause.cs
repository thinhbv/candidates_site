using System;
using System.Collections.Generic;
using System.Text;
using CMSSolutions.Extensions;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public enum JoinType
    {
        CrossJoin,
        InnerJoin,
        LeftJoin,
        OuterJoin,
        RightJoin
    }

    [Serializable]
    public struct JoinClause
    {
        public JoinType JoinType { get; set; }

        public string FromTable { get; set; }

        public string FromField { get; set; }

        public ComparisonOperator CompareOperator { get; set; }

        public string ToTable { get; set; }

        public string ToField { get; set; }

        public static JoinClause Empty
        {
            get { return new JoinClause(); }
        }
    }

    [Serializable]
    public class JoinClauseCollection<TQueryType> : List<JoinClause>
        where TQueryType : Query<TQueryType>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string separator = string.Concat(Query<TQueryType>.SpaceEscapeEnd, ".", Query<TQueryType>.SpaceEscapeStart);

            foreach (JoinClause joinClause in this)
            {
                SqlLiteral literal = new SqlLiteral
                {
                    Value = string.Concat(Query<TQueryType>.SpaceEscapeStart, joinClause.ToTable, separator, joinClause.ToField, Query<TQueryType>.SpaceEscapeEnd)
                };
                Condition<TQueryType> condition = new Condition<TQueryType>
                {
                    CompareOperator = joinClause.CompareOperator,
                    Field = string.Concat(Query<TQueryType>.SpaceEscapeStart, joinClause.FromTable, separator, joinClause.FromField, Query<TQueryType>.SpaceEscapeEnd),
                    Value = literal
                };

                switch (joinClause.JoinType)
                {
                    case JoinType.CrossJoin: sb.Append("CROSS JOIN "); break;
                    case JoinType.InnerJoin: sb.Append("INNER JOIN "); break;
                    case JoinType.LeftJoin: sb.Append("LEFT JOIN "); break;
                    case JoinType.OuterJoin: sb.Append("OUTER JOIN "); break;
                    case JoinType.RightJoin: sb.Append("RIGHT JOIN "); break;
                }

                sb.Append(Query<TQueryType>.SpaceEscapeStart, joinClause.FromTable, Query<TQueryType>.SpaceEscapeEnd, " ON ");
                sb.Append(condition.ToString());
            }

            return sb.ToString();
        }
    }
}