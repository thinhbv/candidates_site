using System.IO;

namespace CMSSolutions.Web.UI.Grid
{
    internal class ImageColumnCellRender : IGridCellRender
    {
        private readonly int flag;
        private readonly string trueImageUrl;
        private readonly string falseImageUrl;

        public ImageColumnCellRender(string trueImageUrl, string falseImageUrl)
        {
            this.trueImageUrl = trueImageUrl;
            this.falseImageUrl = falseImageUrl;
            flag = 1;
        }

        #region IGridCellRender Members

        public void Render(TextWriter writer, object value)
        {
            switch (flag)
            {
                case 1:
                    {
                        var booleanValue = (bool)value;
                        if (booleanValue && !string.IsNullOrEmpty(trueImageUrl))
                        {
                            writer.Write(string.Format("<img src=\"{0}\" alt=\"\" />", trueImageUrl));
                        }

                        if (!booleanValue && !string.IsNullOrEmpty(falseImageUrl))
                        {
                            writer.Write(string.Format("<img src=\"{0}\" alt=\"\" />", falseImageUrl));
                        }
                        break;
                    }
            }
        }

        #endregion IGridCellRender Members
    }

    public static class GridImageColumnExtensions
    {
        public static IGridColumn<T> RenderAsImage<T>(this IGridColumn<T> column, string trueImageUrl, string falseImageUrl)
        {
            column.Encode(false).HasCellRender(new ImageColumnCellRender(trueImageUrl, falseImageUrl));
            return column;
        }
    }
}