using System;

namespace CMSSolutions.Data
{
    public class TableNameAttribute : Attribute
    {
        private readonly string name;

        public TableNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}