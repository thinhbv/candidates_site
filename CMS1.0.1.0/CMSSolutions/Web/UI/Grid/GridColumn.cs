using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CMSSolutions.Web.UI.Grid
{
    /// <summary>
    /// Column for the grid
    /// </summary>
    public class GridColumn<T> : IGridColumn<T> where T : class
    {
        private readonly string name;
        private string displayName;
        private bool doNotSplit;
        private readonly Func<T, object> columnValueFunc;
        private readonly Type dataType;
        private Func<T, bool> cellCondition = x => true;
        private string format;
        private bool visible = true;
        private bool htmlEncode = true;
        private readonly IDictionary<string, object> headerAttributes = new Dictionary<string, object>();
        private readonly List<Func<GridRowViewData<T>, IDictionary<string, object>>> attributes = new List<Func<GridRowViewData<T>, IDictionary<string, object>>>();
        private bool sortable = true;
        private string sortColumnName;
        private int? position;
        private IGridCellRender cellRender;
        private Func<object, object> headerRenderer = x => null;

        /// <summary>
        /// Creates a new instance of the GridColumn class
        /// </summary>
        public GridColumn(Func<T, object> columnValueFunc, string name, Type type)
        {
            this.name = name;
            displayName = name;
            dataType = type;
            this.columnValueFunc = columnValueFunc;
        }

        public IGridCellRender CellRender { get { return cellRender; } }

        public bool Sortable
        {
            get { return sortable; }
        }

        public bool Visible
        {
            get { return visible; }
        }

        public string SortColumnName
        {
            get { return sortColumnName; }
        }

        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Display name for the column
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (doNotSplit)
                {
                    return displayName;
                }
                return SplitPascalCase(displayName);
            }
        }

        /// <summary>
        /// The type of the object being rendered for thsi column.
        /// Note: this will return null if the type cannot be inferred.
        /// </summary>
        public Type ColumnType
        {
            get { return dataType; }
        }

        public int? Position
        {
            get { return position; }
        }

        IGridColumn<T> IGridColumn<T>.Attributes(Func<GridRowViewData<T>, IDictionary<string, object>> attrs)
        {
            attributes.Add(attrs);
            return this;
        }

        IGridColumn<T> IGridColumn<T>.Sortable(bool isColumnSortable)
        {
            sortable = isColumnSortable;
            return this;
        }

        IGridColumn<T> IGridColumn<T>.SortColumnName(string columnName)
        {
            sortColumnName = columnName;
            return this;
        }

        IGridColumn<T> IGridColumn<T>.InsertAt(int index)
        {
            position = index;
            return this;
        }

        /// <summary>
        /// Additional attributes for the column header
        /// </summary>
        public IDictionary<string, object> HeaderAttributes
        {
            get { return headerAttributes; }
        }

        /// <summary>
        /// Additional attributes for the cell
        /// </summary>
        public Func<GridRowViewData<T>, IDictionary<string, object>> Attributes
        {
            get { return GetAttributesFromRow; }
        }

        private IDictionary<string, object> GetAttributesFromRow(GridRowViewData<T> row)
        {
            var dictionary = new Dictionary<string, object>();
            var pairs = attributes.SelectMany(attributeFunc => attributeFunc(row));

            foreach (var pair in pairs)
            {
                dictionary[pair.Key] = pair.Value;
            }

            return dictionary;
        }

        public IGridColumn<T> Named(string newName)
        {
            displayName = newName;
            doNotSplit = true;
            return this;
        }

        public IGridColumn<T> DoNotSplit()
        {
            doNotSplit = true;
            return this;
        }

        public IGridColumn<T> Format(string formatString)
        {
            format = formatString;
            return this;
        }

        public IGridColumn<T> CellCondition(Func<T, bool> func)
        {
            cellCondition = func;
            return this;
        }

        IGridColumn<T> IGridColumn<T>.Visible(bool isVisible)
        {
            visible = isVisible;
            return this;
        }

        public IGridColumn<T> Header(Func<object, object> renderer)
        {
            headerRenderer = renderer;
            return this;
        }

        public IGridColumn<T> Encode(bool shouldEncode)
        {
            htmlEncode = shouldEncode;
            return this;
        }

        IGridColumn<T> IGridColumn<T>.HeaderAttributes(IDictionary<string, object> attrs)
        {
            foreach (var attribute in attrs)
            {
                headerAttributes.Add(attribute);
            }

            return this;
        }

        IGridColumn<T> IGridColumn<T>.HasCellRender(IGridCellRender render)
        {
            cellRender = render;
            return this;
        }

        IGridColumn<T> IGridColumn<T>.Centered()
        {
            headerAttributes.Add("style", "text-align: center;");
            attributes.Add(x => new Hash(style => "text-align: center;"));
            return this;
        }

        private static string SplitPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Gets the value for a particular cell in this column
        /// </summary>
        /// <param name="instance">Instance from which the value should be obtained</param>
        /// <returns>Item to be rendered</returns>
        public object GetValue(T instance)
        {
            if (!cellCondition(instance))
            {
                return null;
            }

            var value = columnValueFunc(instance);

            if (!string.IsNullOrEmpty(format))
            {
                value = string.Format(format, value);
            }

            if (htmlEncode && value != null && !(value is IHtmlString))
            {
                value = HttpUtility.HtmlEncode(value.ToString());
            }

            return value;
        }

        public string GetHeader()
        {
            var header = headerRenderer(null);
            return header == null ? null : header.ToString();
        }
    }
}