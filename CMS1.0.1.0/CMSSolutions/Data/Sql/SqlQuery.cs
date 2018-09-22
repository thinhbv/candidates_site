using System;
using System.Runtime.Serialization;
using System.Text;
using CMSSolutions.Collections;
using CMSSolutions.Data.QueryBuilder;
using CMSSolutions.Extensions;

namespace CMSSolutions.Data.Sql
{
    [Serializable]
    public class SqlQuery : Query<SqlQuery>
    {
        #region Constructors

        public SqlQuery()
        {
        }

        public SqlQuery(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors

        public override string ToString()
        {
            if (select.IsNullOrEmpty())
            {
                throw new ArgumentException("Please select some columns.");
            }

            if (from.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("Please select a table with From().");
            }

            if (isPagedQuery)
            {
                if (orderBy.IsNullOrEmpty())
                {
                    throw new ArgumentException("Cannot have a paged query without an ORDER BY clause.");
                }
            }

            //SELECT
            StringBuilder query = new StringBuilder(200);
            query.Append("SELECT ");

            //DISTINCT
            if (IsDistinct)
            {
                query.Append("DISTINCT ");
            }

            //TOP
            query.Append(top.ToString());

            if (isPagedQuery)
            {
                query.Append("ROW_NUMBER() OVER (", orderBy.ToString(), ") AS Row, ");
            }

            //FROM

            if (DoFormat)
            {
                query.Append(select, System.Environment.NewLine, "FROM ", from, " ");
            }
            else
            {
                query.Append(select, " FROM ", from, " ");
            }

            //JOINS
            if (!joins.IsNullOrEmpty())
            {
                if (DoFormat)
                { query.Append(System.Environment.NewLine); }

                query.Append(joins);
            }

            //WHERE
            if (!where.IsNullOrEmpty())
            {
                if (DoFormat)
                { query.Append(System.Environment.NewLine); }

                query.Append(where.ToWhereString());
            }

            //GROUP BY
            if (!groupBy.IsNullOrWhiteSpace())
            {
                if (DoFormat)
                { query.Append(System.Environment.NewLine); }

                query.Append("GROUP BY ", groupBy, " ");

                //HAVING
                if (!having.IsNullOrEmpty())
                {
                    if (DoFormat)
                    { query.Append(System.Environment.NewLine); }

                    query.Append(having.ToHavingString());
                }
            }

            if (!isPagedQuery)
            {
                //ORDER BY
                if (!orderBy.IsNullOrEmpty())
                {
                    if (DoFormat)
                    { query.Append(System.Environment.NewLine); }

                    query.Append(orderBy);
                }
            }

            #region Paging Query

            const string pageQueryFormat =
@"SELECT {0}
        FROM
        (
            {1}
        ) AS PagingQuery
        WHERE Row >= {2} AND Row < {3}";

            if (isPagedQuery)
            {
                return string.Format(
                    pageQueryFormat,
                    select,
                    query,
                    rowStartIndex,
                    rowStartIndex + pageSize);
            }

            #endregion Paging Query

            return query.ToString();
        }
    }
}