using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using CMSSolutions.Collections;
using CMSSolutions.Extensions;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public abstract class Query<TQueryType> : ISerializable
        where TQueryType : Query<TQueryType>
    {
        #region Non-Public Members

        protected string select = string.Empty;
        protected string from = string.Empty;
        protected WhereClauseCollection<TQueryType> where = new WhereClauseCollection<TQueryType>();
        protected OrderByClauseCollection<TQueryType> orderBy = new OrderByClauseCollection<TQueryType>();
        protected JoinClauseCollection<TQueryType> joins = new JoinClauseCollection<TQueryType>();
        protected WhereClauseCollection<TQueryType> having = new WhereClauseCollection<TQueryType>();
        protected string groupBy = string.Empty;
        protected TopClause top = TopClause.Empty;
        protected uint rowStartIndex = default(uint);
        protected uint pageSize = default(ushort);
        protected bool isPagedQuery = false;

        #endregion Non-Public Members

        #region Public Properties

        public bool DoFormat { get; set; }

        public bool IsDistinct { get; set; }

        public static string SpaceEscapeStart { get; set; }

        public static string SpaceEscapeEnd { get; set; }

        #endregion Public Properties

        #region Constructors

        public Query()
        {
            SpaceEscapeStart = "[";
            SpaceEscapeEnd = "]";
        }

        public Query(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            select = info.GetString("select");
            from = info.GetString("from");
            where = (WhereClauseCollection<TQueryType>)info.GetValue("where", typeof(WhereClauseCollection<TQueryType>));
            orderBy = (OrderByClauseCollection<TQueryType>)info.GetValue("orderBy", typeof(OrderByClauseCollection<TQueryType>));
            joins = (JoinClauseCollection<TQueryType>)info.GetValue("joins", typeof(JoinClauseCollection<TQueryType>));
            having = (WhereClauseCollection<TQueryType>)info.GetValue("having", typeof(WhereClauseCollection<TQueryType>));
            groupBy = info.GetString("groupBy");
            top = (TopClause)info.GetValue("top", typeof(TopClause));
            rowStartIndex = info.GetUInt32("rowStartIndex");
            pageSize = info.GetUInt32("pageSize");
            isPagedQuery = info.GetBoolean("isPagedQuery");
            DoFormat = info.GetBoolean("DoFormat");
            IsDistinct = info.GetBoolean("IsDistinct");
        }

        #endregion Constructors

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("select", select);
            info.AddValue("from", from);
            info.AddValue("where", where);
            info.AddValue("orderBy", orderBy);
            info.AddValue("joins", joins);
            info.AddValue("having", having);
            info.AddValue("groupBy", groupBy);
            info.AddValue("top", top);
            info.AddValue("rowStartIndex", rowStartIndex);
            info.AddValue("pageSize", pageSize);
            info.AddValue("isPagedQuery", isPagedQuery);
            info.AddValue("DoFormat", DoFormat);
            info.AddValue("IsDistinct", IsDistinct);
        }

        #endregion ISerializable Members

        #region Methods

        #region SELECT

        public virtual TQueryType Select(params string[] fields)
        {
            if (!string.IsNullOrEmpty(select))
            { select = select.Append(", "); }

            select = select.Append(fields.Join()
                .Prepend(SpaceEscapeStart)
                .Replace(",", string.Concat(SpaceEscapeEnd, ", ", SpaceEscapeStart))
                //.Replace(".", string.Concat(SpaceEscapeEnd, '.', SpaceEscapeStart))
                .Append(SpaceEscapeEnd));

            return (TQueryType)this;
        }

        public virtual TQueryType Select(string tableName, params string[] fields)
        {
            // pre-allocate StringBuilder capacity for better performance
            var sb = new StringBuilder(select.Length + (50 * fields.Length));
            sb.Append(select);

            if (!string.IsNullOrEmpty(select))
            { sb.Append(", "); }

            foreach (string field in fields)
            {
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    sb.Append(SpaceEscapeStart);
                    sb.Append(tableName);
                    sb.Append(SpaceEscapeEnd);
                    sb.Append('.');
                }

                sb.Append(SpaceEscapeStart);
                sb.Append(field);
                sb.Append(SpaceEscapeEnd);
                sb.Append(", ");
            }
            sb.Length = sb.Length - 2;

            select = sb.ToString();

            return (TQueryType)this;
        }

        public virtual TQueryType SelectAll()
        {
            if (!string.IsNullOrEmpty(select))
            { select = select.Append(", "); }

            select = select.Append("*");
            return (TQueryType)this;
        }

        public virtual TQueryType SelectAverage(string field)
        {
            return SelectAverage(field, string.Concat(field, "_Average"));
        }

        public virtual TQueryType SelectAverage(string field, string asField)
        {
            return SelectFunction("AVG", field, asField);
        }

        public virtual TQueryType SelectCount(string field)
        {
            return SelectCount(field, string.Concat(field, "_Count"));
        }

        public virtual TQueryType SelectCount(string field, string asField)
        {
            return SelectFunction("COUNT", field, asField);
        }

        public virtual TQueryType SelectCountAll()
        {
            return SelectFunction("COUNT", "*", "RecordCount");
        }

        public virtual TQueryType SelectMax(string field)
        {
            return SelectMax(field, string.Concat(field, "_Max"));
        }

        public virtual TQueryType SelectMax(string field, string asField)
        {
            return SelectFunction("MAX", field, asField);
        }

        public virtual TQueryType SelectMin(string field)
        {
            return SelectMin(field, string.Concat(field, "_Min"));
        }

        public virtual TQueryType SelectMin(string field, string asField)
        {
            return SelectFunction("MIN", field, asField);
        }

        public virtual TQueryType SelectSum(string field)
        {
            return SelectSum(field, string.Concat(field, "_Sum"));
        }

        public virtual TQueryType SelectSum(string field, string asField)
        {
            return SelectFunction("SUM", field, asField);
        }

        protected virtual TQueryType SelectFunction(string functionName, string field, string asField)
        {
            if (!string.IsNullOrEmpty(select))
            { select = select.Append(", "); }

            if (field == "*")
            {
                select = select.Append(functionName, "(", field, ") AS ", asField);
            }
            else { select = select.Append(functionName, "(", SpaceEscape(field), ") AS ", asField); }

            return (TQueryType)this;
        }

        public virtual TQueryType SelectLiteral(string literal)
        {
            if (!string.IsNullOrEmpty(select))
            { select = select.Append(", "); }

            select = select.Append(literal);
            return (TQueryType)this;
        }

        public virtual TQueryType GetPagingQuery(uint pageNumber, uint pageSize)
        {
            uint maxPage = pageNumber * pageSize;
            uint rowStartIndex = (maxPage - pageSize) + 1;

            isPagedQuery = true;
            this.rowStartIndex = rowStartIndex;
            this.pageSize = pageSize;
            return (TQueryType)this;
        }

        #endregion SELECT

        #region FROM

        public virtual TQueryType From(params string[] tables)
        {
            from = tables.Join()
                .Prepend(SpaceEscapeStart)
                .Replace(",", string.Concat(SpaceEscapeEnd, ", ", SpaceEscapeStart))
                .Replace(".", string.Concat(SpaceEscapeEnd, '.', SpaceEscapeStart))
                .Append(SpaceEscapeEnd);

            return (TQueryType)this;
        }

        public virtual TQueryType FromQuery(TQueryType fromQuery, string alias)
        {
            from = string.Concat('(', fromQuery.ToString(), ") AS ", alias);
            return (TQueryType)this;
        }

        #endregion FROM

        #region JOIN

        public virtual TQueryType AddJoin(string fromTable, string fromField, string toTable, string toField)
        {
            joins.Add(new JoinClause
            {
                JoinType = JoinType.InnerJoin,
                FromTable = fromTable,
                FromField = fromField,
                CompareOperator = ComparisonOperator.EqualTo,
                ToTable = toTable,
                ToField = toField
            });
            return (TQueryType)this;
        }

        public virtual TQueryType AddJoin(string fromTable, string fromField, string toTable, string toField, ComparisonOperator comparisonOperator)
        {
            joins.Add(new JoinClause
            {
                JoinType = JoinType.InnerJoin,
                FromTable = fromTable,
                FromField = fromField,
                CompareOperator = comparisonOperator,
                ToTable = toTable,
                ToField = toField
            });
            return (TQueryType)this;
        }

        public virtual TQueryType AddJoin(string fromTable, string fromField, string toTable, string toField, ComparisonOperator comparisonOperator, JoinType joinType)
        {
            joins.Add(new JoinClause
            {
                JoinType = joinType,
                FromTable = fromTable,
                FromField = fromField,
                CompareOperator = comparisonOperator,
                ToTable = toTable,
                ToField = toField
            });
            return (TQueryType)this;
        }

        public virtual TQueryType AddJoin(string fromTable, string fromField, string toTable, string toField, JoinType joinType)
        {
            return AddJoin(fromTable, fromField, toTable, toField, ComparisonOperator.EqualTo, joinType);
        }

        #endregion JOIN

        #region WHERE

        public virtual TQueryType AddWhere(string field, ComparisonOperator comparisonOperator, object value)
        {
            return AddWhere(field, comparisonOperator, value, null);
        }

        public virtual TQueryType AddWhere(string field, ComparisonOperator comparisonOperator, object value, IEnumerable<WhereClause<TQueryType>> subClauses)
        {
            Condition<TQueryType> condition = new Condition<TQueryType>
            {
                Field = field,
                CompareOperator = comparisonOperator,
                Value = value
            };

            if (!subClauses.IsNullOrEmpty())
            {
                condition.SubClauses = new List<WhereClause<TQueryType>>();
                condition.SubClauses.AddRange(subClauses);
            }

            WhereClause<TQueryType> clause = new WhereClause<TQueryType> { LogicalOperator = LogicalOperator.And };
            clause.Add(condition);
            where.Add(clause);

            return (TQueryType)this;
        }

        public virtual TQueryType AddWhere(string field, ComparisonOperator comparisonOperator, object value, LogicalOperator logicOperator)
        {
            return AddWhere(field, comparisonOperator, value, logicOperator, null);
        }

        public virtual TQueryType AddWhere(string field, ComparisonOperator comparisonOperator, object value, LogicalOperator logicOperator, IEnumerable<WhereClause<TQueryType>> subClauses)
        {
            Condition<TQueryType> condition = new Condition<TQueryType>
            {
                Field = field,
                CompareOperator = comparisonOperator,
                Value = value
            };

            if (!subClauses.IsNullOrEmpty())
            {
                condition.SubClauses = new List<WhereClause<TQueryType>>();
                condition.SubClauses.AddRange(subClauses);
            }

            WhereClause<TQueryType> clause = new WhereClause<TQueryType> { LogicalOperator = logicOperator };
            clause.Add(condition);
            where.Add(clause);

            return (TQueryType)this;
        }

        public virtual TQueryType AddWhere(Condition<TQueryType> condition)
        {
            WhereClause<TQueryType> whereClause = new WhereClause<TQueryType> { LogicalOperator = LogicalOperator.And };
            whereClause.Add(condition);
            where.Add(whereClause);
            return (TQueryType)this;
        }

        public virtual TQueryType AddWhere(IEnumerable<Condition<TQueryType>> conditions)
        {
            WhereClause<TQueryType> whereClause = new WhereClause<TQueryType> { LogicalOperator = LogicalOperator.And };
            whereClause.AddRange(conditions);
            where.Add(whereClause);
            return (TQueryType)this;
        }

        public virtual TQueryType AddWhere(WhereClause<TQueryType> whereClause)
        {
            where.Add(whereClause);
            return (TQueryType)this;
        }

        #endregion WHERE

        #region ORDER BY

        public virtual TQueryType AddOrderBy(string field)
        {
            return AddOrderBy(field, SortDirection.Ascending);
        }

        public virtual TQueryType AddOrderBy(string field, SortDirection sortDirection)
        {
            orderBy.Add(new OrderByClause
            {
                Field = field,
                SortDirection = sortDirection
            });
            return (TQueryType)this;
        }

        #endregion ORDER BY

        #region GROUP BY

        public virtual TQueryType GroupBy(params string[] fields)
        {
            // Default is comma with no space
            // Here, I want space
            groupBy = fields.Join()
                .Prepend(SpaceEscapeStart)
                .Replace(",", string.Concat(SpaceEscapeEnd, ", ", SpaceEscapeStart))
                .Append(SpaceEscapeEnd);
            return (TQueryType)this;
        }

        #endregion GROUP BY

        #region HAVING

        public virtual TQueryType AddHaving(string field, ComparisonOperator comparisonOperator, object value)
        {
            Condition<TQueryType> condition = new Condition<TQueryType>
            {
                Field = field,
                CompareOperator = comparisonOperator,
                Value = value
            };

            WhereClause<TQueryType> clause = new WhereClause<TQueryType> { LogicalOperator = LogicalOperator.And };
            clause.Add(condition);
            having.Add(clause);

            return (TQueryType)this;
        }

        public virtual TQueryType AddHaving(string field, ComparisonOperator comparisonOperator, object value, LogicalOperator logicOperator)
        {
            Condition<TQueryType> condition = new Condition<TQueryType>
            {
                Field = field,
                CompareOperator = comparisonOperator,
                Value = value
            };

            WhereClause<TQueryType> clause = new WhereClause<TQueryType> { LogicalOperator = logicOperator };
            clause.Add(condition);
            having.Add(clause);

            return (TQueryType)this;
        }

        public virtual TQueryType AddHaving(Condition<TQueryType> condition)
        {
            WhereClause<TQueryType> clause = new WhereClause<TQueryType> { LogicalOperator = LogicalOperator.And };
            clause.Add(condition);
            having.Add(clause);
            return (TQueryType)this;
        }

        public virtual TQueryType AddHaving(IEnumerable<Condition<TQueryType>> conditions)
        {
            WhereClause<TQueryType> whereClause = new WhereClause<TQueryType> { LogicalOperator = LogicalOperator.And };
            whereClause.AddRange(conditions);
            having.Add(whereClause);
            return (TQueryType)this;
        }

        #endregion HAVING

        public virtual TQueryType Distinct()
        {
            IsDistinct = true;
            return (TQueryType)this;
        }

        public virtual TQueryType Top(int count)
        {
            return Top(count, TopType.Records);
        }

        public virtual TQueryType Top(int count, TopType topType)
        {
            top.Count = count;
            top.TopType = topType;
            return (TQueryType)this;
        }

        internal static string GetSqlFormattedValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                switch (value.GetType().Name)
                {
                    case "String": return string.Concat("'", ((string)value).Replace("'", "''"), "'");
                    case "DateTime": return string.Concat("'", ((DateTime)value).ToISO8601DateString(), "'");
                    case "DBNull": return "NULL";
                    case "Boolean": return (bool)value ? "1" : "0";
                    case "SqlLiteral": return ((SqlLiteral)value).Value;
                    default: return value.ToString();
                }
            }
        }

        public static string SpaceEscape(string value)
        {
            return string.Concat(SpaceEscapeStart, value, SpaceEscapeEnd);
        }

        #endregion Methods

        public abstract new string ToString();
    }
}