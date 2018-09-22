using System;
using System.Collections.Generic;

namespace CMSSolutions.Web.UI.Grid
{
    /// <summary>
    /// Default model for grid
    /// </summary>
    public class GridModel<T> : IGridModel<T> where T : class
    {
        private readonly ColumnBuilder<T> columnBuilder;
        private readonly GridSections<T> sections = new GridSections<T>();
        private IGridRenderer<T> gridRenderer = new HtmlTableGridRenderer<T>();
        private string emptyText;
        private IDictionary<string, object> attributes = new Dictionary<string, object>();
        private GridSortOptions sortOptions;
        private string sortPrefix;

        GridSortOptions IGridModel<T>.SortOptions
        {
            get { return sortOptions; }
            set { sortOptions = value; }
        }

        IList<GridColumn<T>> IGridModel<T>.Columns
        {
            get { return columnBuilder; }
        }

        IGridRenderer<T> IGridModel<T>.Renderer
        {
            get { return gridRenderer; }
            set { gridRenderer = value; }
        }

        string IGridModel<T>.EmptyText
        {
            get { return emptyText; }
            set { emptyText = value; }
        }

        IDictionary<string, object> IGridModel<T>.Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        string IGridModel<T>.SortPrefix
        {
            get { return sortPrefix; }
            set { sortPrefix = value; }
        }

        /// <summary>
        /// Creates a new instance of the GridModel class
        /// </summary>
        public GridModel()
        {
            emptyText = "There is no data available.";
            columnBuilder = CreateColumnBuilderWrap();
        }

        /// <summary>
        /// Column builder for this grid model
        /// </summary>
        public ColumnBuilder<T> Column
        {
            get { return columnBuilder; }
        }

        /// <summary>
        /// Section overrides for this grid model.
        /// </summary>
        IGridSections<T> IGridModel<T>.Sections
        {
            get { return sections; }
        }

        /// <summary>
        /// Section overrides for this grid model.
        /// </summary>
        public GridSections<T> Sections
        {
            get { return sections; }
        }

        /// <summary>
        /// Text that will be displayed when the grid has no data.
        /// </summary>
        /// <param name="text">Text to display</param>
        public void Empty(string text)
        {
            emptyText = text;
        }

        /// <summary>
        /// Defines additional attributes for the grid.
        /// </summary>
        /// <param name="hash"></param>
        public void Attributes(params Func<object, object>[] hash)
        {
            Attributes(new Hash(hash));
        }

        /// <summary>
        /// Defines additional attributes for the grid
        /// </summary>
        /// <param name="attrs"></param>
        public void Attributes(IDictionary<string, object> attrs)
        {
            attributes = attrs;
        }

        /// <summary>
        /// Specifies the Renderer to use with this grid. If omitted, the HtmlTableGridRenderer will be used.
        /// </summary>
        /// <param name="renderer">The Renderer to use</param>
        public void RenderUsing(IGridRenderer<T> renderer)
        {
            gridRenderer = renderer;
        }

        /// <summary>
        /// Secifies that the grid is currently being sorted by the specified column in a particular direction.
        /// </summary>
        public void Sort(GridSortOptions options)
        {
            sortOptions = options;
        }

        /// <summary>
        /// Specifies that the grid is currently being sorted by the specified column in a particular direction.
        /// This overload allows you to specify a prefix.
        /// </summary>
        public void Sort(GridSortOptions options, string prefix)
        {
            sortOptions = options;
            sortPrefix = prefix;
        }

        private ColumnBuilder<T> CreateColumnBuilderWrap()
        {
            return CreateColumnBuilder();
        }

        protected virtual ColumnBuilder<T> CreateColumnBuilder()
        {
            return new ColumnBuilder<T>();
        }
    }
}