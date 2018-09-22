using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMSSolutions.Web.UI.Grid
{
    /// <summary>
    /// Defines a grid to be rendered.
    /// </summary>
    /// <typeparam name="T">Type of datasource for the grid</typeparam>
    public class Grid<T> : IGrid<T> where T : class
    {
        private readonly ViewContext context;
        private IGridModel<T> gridModel = new GridModel<T>();

        /// <summary>
        /// The GridModel that holds the internal representation of this grid.
        /// </summary>
        public IGridModel<T> Model
        {
            get { return gridModel; }
        }

        /// <summary>
        /// Creates a new instance of the Grid class.
        /// </summary>
        /// <param name="dataSource">The datasource for the grid</param>
        /// <param name="context"></param>
        public Grid(IEnumerable<T> dataSource, ViewContext context)
        {
            this.context = context;
            DataSource = dataSource;
        }

        /// <summary>
        /// The datasource for the grid.
        /// </summary>
        public IEnumerable<T> DataSource { get; private set; }

        public IGridWithOptions<T> RenderUsing(IGridRenderer<T> renderer)
        {
            gridModel.Renderer = renderer;
            return this;
        }

        public IGridWithOptions<T> Columns(Action<ColumnBuilder<T>> columnBuilder)
        {
            var builder = new ColumnBuilder<T>();
            columnBuilder(builder);

            foreach (var column in builder)
            {
                if (column.Position == null)
                {
                    gridModel.Columns.Add(column);
                }
                else
                {
                    gridModel.Columns.Insert(column.Position.Value, column);
                }
            }

            return this;
        }

        public IGridWithOptions<T> Empty(string emptyText)
        {
            gridModel.EmptyText = emptyText;
            return this;
        }

        public IGridWithOptions<T> Attributes(object attributes)
        {
            return Attributes(new RouteValueDictionary(attributes));
        }

        public IGridWithOptions<T> Attributes(IDictionary<string, object> attributes)
        {
            gridModel.Attributes = attributes;
            return this;
        }

        public IGrid<T> WithModel(IGridModel<T> model)
        {
            gridModel = model;
            return this;
        }

        public IGridWithOptions<T> Sort(GridSortOptions sortOptions)
        {
            gridModel.SortOptions = sortOptions;
            return this;
        }

        public IGridWithOptions<T> Sort(GridSortOptions sortOptions, string prefix)
        {
            gridModel.SortOptions = sortOptions;
            gridModel.SortPrefix = prefix;
            return this;
        }

        public override string ToString()
        {
            return ToHtmlString();
        }

        public string ToHtmlString()
        {
            var writer = new StringWriter();
            gridModel.Renderer.Render(gridModel, DataSource, writer, context);
            return writer.ToString();
        }

        public IGridWithOptions<T> HeaderRowAttributes(IDictionary<string, object> attributes)
        {
            gridModel.Sections.HeaderRowAttributes(attributes);
            return this;
        }

        public IGridWithOptions<T> RowAttributes(Func<GridRowViewData<T>, IDictionary<string, object>> attributes)
        {
            gridModel.Sections.RowAttributes(attributes);
            return this;
        }
    }
}