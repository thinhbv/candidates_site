using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMSSolutions.Collections;
using CMSSolutions.Extensions;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public struct Condition<TQueryType>
        where TQueryType : Query<TQueryType>
    {
        public string Field { get; set; }

        public ComparisonOperator CompareOperator { get; set; }

        public LogicalOperator LogicOperator { get; set; }

        public object Value { get; set; }

        public List<WhereClause<TQueryType>> SubClauses { get; set; }

        public Condition(string field, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, object value)
            : this()
        {
            SubClauses = new List<WhereClause<TQueryType>>();

            Field = field;
            CompareOperator = comparisonOperator;
            LogicOperator = logicalOperator;
            Value = value;
        }

        public override string ToString()
        {
            // Strip any "[" or "]" so we don't get double
            string fieldName = Field
                .Replace(Query<TQueryType>.SpaceEscapeStart, string.Empty)
                .Replace(Query<TQueryType>.SpaceEscapeEnd, string.Empty);
            string returnValue = fieldName;

            // If format is Table.Column (example: Customers.CustomerID)
            if (returnValue.Contains('.'))
            {
                string separator = string.Concat(Query<TQueryType>.SpaceEscapeEnd, ".", Query<TQueryType>.SpaceEscapeStart);
                returnValue = returnValue.Prepend(Query<TQueryType>.SpaceEscapeStart).Replace(".", separator).Append(Query<TQueryType>.SpaceEscapeEnd);
            }
            else { returnValue = Query<TQueryType>.SpaceEscape(returnValue); }

            switch (CompareOperator)
            {
                #region EqualTo

                case ComparisonOperator.EqualTo:
                    returnValue = returnValue.Append(" = ", Query<TQueryType>.GetSqlFormattedValue(Value), " ");
                    break;

                #endregion EqualTo

                #region EndsWith

                case ComparisonOperator.EndsWith:
                    returnValue = returnValue.Append(
                        " LIKE ",
                        Query<TQueryType>.GetSqlFormattedValue(string.Concat('%', Value)),
                        " ");
                    break;

                #endregion EndsWith

                #region GreaterThan

                case ComparisonOperator.GreaterThan:
                    returnValue = returnValue.Append(" > ", Query<TQueryType>.GetSqlFormattedValue(Value), " ");
                    break;

                #endregion GreaterThan

                #region GreaterThanOrEqualTo

                case ComparisonOperator.GreaterThanOrEqualTo:
                    returnValue = returnValue.Append(" >= ", Query<TQueryType>.GetSqlFormattedValue(Value), " ");
                    break;

                #endregion GreaterThanOrEqualTo

                #region In

                case ComparisonOperator.In:
                    returnValue = returnValue.Append(" IN (", Query<TQueryType>.GetSqlFormattedValue(Value), ") ");
                    break;

                #endregion In

                #region IsEmpty

                case ComparisonOperator.IsEmpty:
                    returnValue = returnValue.Append(" = '' ");
                    break;

                #endregion IsEmpty

                #region IsNotEmpty

                case ComparisonOperator.IsNotEmpty:
                    returnValue = returnValue.Append(" <> '' ");
                    break;

                #endregion IsNotEmpty

                #region IsNotNull

                case ComparisonOperator.IsNotNull:
                    returnValue = returnValue.Append(" IS NOT NULL ");
                    break;

                #endregion IsNotNull

                #region IsNotNullOrEmpty

                case ComparisonOperator.IsNotNullOrEmpty:
                    returnValue = returnValue.Append(" IS NOT NULL AND ", Query<TQueryType>.SpaceEscapeStart, fieldName, Query<TQueryType>.SpaceEscapeEnd, " <> '' ");
                    break;

                #endregion IsNotNullOrEmpty

                #region IsNull

                case ComparisonOperator.IsNull:
                    returnValue = returnValue.Append(" IS NULL ");
                    break;

                #endregion IsNull

                #region IsNullOrEmpty

                case ComparisonOperator.IsNullOrEmpty:
                    returnValue = returnValue.Append(" IS NULL OR ", Query<TQueryType>.SpaceEscapeStart, fieldName, Query<TQueryType>.SpaceEscapeEnd, " = '' ");
                    break;

                #endregion IsNullOrEmpty

                #region LessThan

                case ComparisonOperator.LessThan:
                    returnValue = returnValue.Append(" < ", Query<TQueryType>.GetSqlFormattedValue(Value), " ");
                    break;

                #endregion LessThan

                #region LessThanOrEqualTo

                case ComparisonOperator.LessThanOrEqualTo:
                    returnValue = returnValue.Append(" <= ", Query<TQueryType>.GetSqlFormattedValue(Value), " ");
                    break;

                #endregion LessThanOrEqualTo

                #region Like

                case ComparisonOperator.Like:
                    returnValue = returnValue.Append(
                        " LIKE ",
                        Query<TQueryType>.GetSqlFormattedValue(string.Concat('%', Value, '%')),
                        " ");
                    break;

                #endregion Like

                #region NotEndsWith

                case ComparisonOperator.NotEndsWith:
                    returnValue = returnValue.Prepend("NOT ").Append(
                        " LIKE ",
                        Query<TQueryType>.GetSqlFormattedValue(string.Concat('%', Value)),
                        " ");
                    break;

                #endregion NotEndsWith

                #region NotEqualTo

                case ComparisonOperator.NotEqualTo:
                    returnValue = returnValue.Append(" <> ", Query<TQueryType>.GetSqlFormattedValue(Value), " ");
                    break;

                #endregion NotEqualTo

                #region NotLike

                case ComparisonOperator.NotLike:
                    returnValue = returnValue.Prepend("NOT ").Append(
                        " LIKE ",
                        Query<TQueryType>.GetSqlFormattedValue(string.Concat('%', Value, '%')),
                        " ");
                    break;

                #endregion NotLike

                #region NotStartsWith

                case ComparisonOperator.NotStartsWith:
                    returnValue = returnValue.Prepend("NOT ").Append(
                        " LIKE ",
                        Query<TQueryType>.GetSqlFormattedValue(string.Concat(Value, '%')),
                        " ");
                    break;

                #endregion NotStartsWith

                #region StartsWith

                case ComparisonOperator.StartsWith:
                    returnValue = returnValue.Append(
                        " LIKE ",
                        Query<TQueryType>.GetSqlFormattedValue(string.Concat(Value, '%')),
                        " ");
                    break;

                #endregion StartsWith
            }

            return returnValue;
        }
    }

    [Serializable]
    public class WhereClause<TQueryType> : List<Condition<TQueryType>>
        where TQueryType : Query<TQueryType>
    {
        public LogicalOperator LogicalOperator { get; set; }

        public WhereClause()
        {
        }

        public WhereClause(string field, ComparisonOperator comparisonOperator, object value)
        {
            var condition = new Condition<TQueryType>
            {
                Field = field,
                CompareOperator = comparisonOperator,
                Value = value
            };

            LogicalOperator = LogicalOperator.And;
            Add(condition);
        }

        public WhereClause(string field, ComparisonOperator comparisonOperator, object value, LogicalOperator logicOperator)
        {
            var condition = new Condition<TQueryType>
            {
                Field = field,
                CompareOperator = comparisonOperator,
                Value = value
            };

            LogicalOperator = logicOperator;
            Add(condition);
        }

        public override string ToString()
        {
            if (Count == 0)
            {
                return string.Empty;
            }

            var query = new StringBuilder();

            Queue<Condition<TQueryType>> queue = this.ToQueue();
            Condition<TQueryType> condition = queue.Dequeue();
            query.Append(condition.ToString());
            query.Append(ProcessSubClauses(condition));

            while (queue.Count > 0)
            {
                condition = queue.Dequeue();
                query.Append(condition.ToString());
                query.Append(ProcessSubClauses(condition));
            }

            return query.ToString();
        }

        public virtual string ProcessSubClauses(Condition<TQueryType> condition)
        {
            var sb = new StringBuilder();

            if (!condition.SubClauses.IsNullOrEmpty())
            {
                foreach (WhereClause<TQueryType> subclause in condition.SubClauses)
                {
                    sb.Append(subclause.LogicalOperator == LogicalOperator.Or ? "OR " : "AND ");
                    sb.Append(subclause);
                }
            }

            return sb.ToString();
        }
    }

    [Serializable]
    public class WhereClauseCollection<TQueryType> : List<WhereClause<TQueryType>>
        where TQueryType : Query<TQueryType>
    {
        public virtual string ToHavingString()
        {
            var query = new StringBuilder();

            query.Append("HAVING ");
            foreach (WhereClause<TQueryType> whereClause in this)
            {
                query.Append('(', whereClause.ToString(), ") ");
                query.Append(whereClause.LogicalOperator == LogicalOperator.Or ? "OR " : "AND ");
            }
            return query.ToString();
        }

        public virtual string ToWhereString()
        {
            var query = new StringBuilder();

            query.Append("WHERE ");

            bool notFirst = false;
            foreach (WhereClause<TQueryType> whereClause in this)
            {
                if (notFirst)
                {
                    query.Append(whereClause.LogicalOperator == LogicalOperator.Or ? "OR " : "AND ");
                }

                query.Append('(', whereClause.ToString(), ") ");
                notFirst = true;
            }

            return query.ToString();
        }
    }
}