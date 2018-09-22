using System;

namespace CMSSolutions.Data.QueryBuilder
{
    [Serializable]
    public class SqlLiteral
    {
        public string Value { get; set; }

        public SqlLiteral()
        {
        }

        public SqlLiteral(string value)
        {
            Value = value;
        }
    }
}