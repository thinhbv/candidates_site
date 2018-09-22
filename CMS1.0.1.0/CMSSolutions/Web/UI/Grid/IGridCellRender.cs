using System.IO;

namespace CMSSolutions.Web.UI.Grid
{
    public interface IGridCellRender
    {
        void Render(TextWriter writer, object value);
    }
}