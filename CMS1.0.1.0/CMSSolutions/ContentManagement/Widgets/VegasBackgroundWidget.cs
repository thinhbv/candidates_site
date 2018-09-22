using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class VegasBackgroundWidget : WidgetBase
    {
        public VegasBackgroundWidget()
        {
            Delay = 5000;
        }

        public override string Name
        {
            get { return "Vegas Background Widget"; }
        }

        [ControlNumeric(Required = true, MinimumValue = "1000", LabelText = "Delay", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public int Delay { get; set; }

        [ControlGrid(1, 10, 5, CssClass = "table table-striped table-bordered", ContainerCssClass = "col-xs-9 col-sm-9", ContainerRowIndex = 4)]
        public List<BackgroundPart> Backgrounds { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.VegasBackground;
        }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (Backgrounds == null || Backgrounds.Count == 0)
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var scriptRegister = new ScriptRegister(workContext);

            var options = new JObject
                          {
                              {"delay", Delay},
                              {"backgrounds", new JArray(Backgrounds.Select(x => new JObject(new JProperty("src", x.Url), new JProperty("fade", x.Fade))))}
                          };
            scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $.vegas('slideshow', {0})('overlay'); }});", options));
        }

        [UsedImplicitly]
        public class BackgroundPart
        {
            public BackgroundPart()
            {
                Fade = 5000;
            }

            [ControlFileUpload(EnableFineUploader = true, Required = true, ColumnWidth = 350, AllowBrowseOnServer = true, Order = 1)]
            public string Url { get; set; }

            [ControlNumeric(LabelText = "Fade", MinimumValue = "1000", Order = 2)]
            public int Fade { get; set; }
        }
    }
}
