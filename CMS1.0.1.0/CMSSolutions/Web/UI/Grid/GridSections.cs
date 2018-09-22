namespace CMSSolutions.Web.UI.Grid
{
    /// <summary>
    /// Sections for a grid.
    /// </summary>
    public class GridSections<T> : IGridSections<T> where T : class
    {
        private readonly GridRow<T> headerRow = new GridRow<T>();
        private readonly GridRow<T> row = new GridRow<T>();

        #region IGridSections<T> Members

        GridRow<T> IGridSections<T>.Row
        {
            get { return row; }
        }

        GridRow<T> IGridSections<T>.HeaderRow
        {
            get { return headerRow; }
        }

        #endregion IGridSections<T> Members
    }

    public interface IGridSections<T> where T : class
    {
        GridRow<T> Row { get; }

        GridRow<T> HeaderRow { get; }
    }
}