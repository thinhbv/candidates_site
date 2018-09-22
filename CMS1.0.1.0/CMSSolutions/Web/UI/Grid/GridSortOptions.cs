using System.Web.UI.WebControls;

namespace CMSSolutions.Web.UI.Grid
{
    /// <summary>
    /// Sorting information for use with the grid.
    /// </summary>
    public class GridSortOptions
    {
        public string Column { get; set; }

        public SortDirection Direction { get; set; }
    }
}