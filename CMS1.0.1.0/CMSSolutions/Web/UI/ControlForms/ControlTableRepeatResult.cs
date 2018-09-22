using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlTableRepeatResult<TModel> : BaseControlFormResult
    {
        private readonly IList<TModel> items;
        private readonly int cols;
        private Func<TModel, string> cellValueRender;

        public ControlTableRepeatResult(IList<TModel> items, int cols)
        {
            if (cols <= 0)
            {
                throw new ArgumentOutOfRangeException("cols", "Columns must be great than 0.");
            }

            this.items = items;
            this.cols = cols;
            CssClass = "table table-bordered";
        }

        public string CssClass { get; set; }

        public ControlTableRepeatResult<TModel> HasCellValueRender(Func<TModel, string> func)
        {
            cellValueRender = func;
            return this;
        }

        public ControlTableRepeatResult<TModel> HasCellValueRender<TValue>(Func<TModel, TValue> text, Func<TModel, string> href)
        {
            cellValueRender = x => string.Format("<a href=\"{1}\">{0}</a>", text(x), href(x));
            return this;
        }

        public ControlTableRepeatResult<TModel> HasCssClass(string cssClass)
        {
            CssClass = cssClass;
            return this;
        }

        #region Overrides of BaseControlFormResult

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            if (items.Count == 0)
            {
                return null;
            }

            var stringBuilder = new StringBuilder();
            var rows = (int)Math.Ceiling((double)items.Count / cols);

            if (string.IsNullOrEmpty(CssClass))
            {
                stringBuilder.Append("<table>");
            }
            else
            {
                stringBuilder.AppendFormat("<table class=\"{0}\">", CssClass);
            }

            var index = 0;
            for (var i = 0; i < rows; i++)
            {
                stringBuilder.Append("<tr>");

                for (var j = 0; j < cols; j++)
                {
                    stringBuilder.Append("<td>");

                    if (index < items.Count)
                    {
                        var item = items[index];
                        if (cellValueRender != null)
                        {
                            stringBuilder.Append(cellValueRender(item));
                        }
                        else
                        {
                            stringBuilder.Append(item);
                        }
                    }
                    else
                    {
                        stringBuilder.Append("&nbsp;");
                    }
                    index++;

                    stringBuilder.Append("</td>");
                }

                stringBuilder.Append("</tr>");
            }

            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        #endregion Overrides of BaseControlFormResult
    }
}