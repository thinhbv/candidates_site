using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Lists.Fields;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.JQueryBuilder;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Media
{
    [Feature(Constants.Areas.Media)]
    public class PictureTooltipsListField : BaseListField
    {
        public PictureTooltipsListField()
        {
            MaxPhotos = 10;
        }

        public override string FieldType
        {
            get { return "iPicture Tooltips Field"; }
        }

        [ControlNumeric(Required = true, MinimumValue = "0", MaximumValue = "255", LabelText = "Max Photos")]
        public byte MaxPhotos { get; set; }

        public override void BindControlField(ControlFormResult controlForm, object value)
        {
            var attribute = new ControlGridAttribute(0, MaxPhotos)
            {
                PropertyName = Name,
                LabelText = Title,
                PropertyType = typeof(List<PictureTooltip>),
                ShowLabelControl = false,
                EnabledScroll = true
            };

            controlForm.RegisterExternalDataSource(Name + ".Button", "moregold", "moregrey", "moreblack", "moredarkblue", "moreblue", "morelightblue", "morelightblue2", "morewatergreen", "morelightgreen", "moregreen", "moreyellow", "moreorange", "morered", "morepurple", "moreviolet", "morelightviolet", "morefucsia");
            controlForm.RegisterExternalDataSource(Name + ".Background", "bgblack", "bgwhite");
            controlForm.RegisterExternalDataSource(Name + ".Round", "roundBgW", "roundBgB");
            controlForm.RegisterExternalDataSource(Name + ".AnimationType", "ltr-slide", "rtl-slide", "btt-slide", "ttb-slide");

            controlForm.AddProperty(Name, attribute, value);
        }

        public override object GetControlFormValue(Controller controller, WorkContext workContext)
        {
            var model = new List<PictureTooltip>();

            var binder = new DefaultModelBinder();
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelState = controller.ViewData.ModelState,
                ValueProvider = controller.ValueProvider,
                ModelName = Name
            };

            binder.BindModel(controller.ControllerContext, bindingContext);

            return model;
        }

        public override object RenderField(object value, object[] args)
        {
            var model = value as List<PictureTooltip>;
            if (model == null || model.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("$(document).ready(function(){");

            var pictures = model.GroupBy(x => x.PictureId);
            foreach (var picture in pictures)
            {
                sb.AppendLine(string.Format("var picture = $('#{0}').wrap('<div class=\"ip_slide\" />');", picture.Key));
                foreach (var tooltip in picture)
                {
                    sb.AppendLine(string.Format("picture.after(\"<div class=\\\"ip_tooltip ip_img16\\\" style=\\\"top: {1}px; left: {2}px;\\\" data-button=\\\"{3}\\\" data-tooltipbg=\\\"{4}\\\" data-round=\\\"{5}\\\" data-animationtype=\\\"{6}\\\">{0}</div>\");",
                        JQueryUtility.EncodeJsString(tooltip.Content, false),
                        tooltip.Top,
                        tooltip.Left,
                        tooltip.Button,
                        tooltip.Background,
                        tooltip.Round,
                        tooltip.AnimationType));
                }
                sb.AppendLine("picture.parent().iPicture();");
            }

            sb.AppendLine("});");
            sb.AppendLine("</script>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public class PictureTooltip
        {
            [ControlText(Required = true)]
            public string PictureId { get; set; }

            [ControlNumeric(Required = true, MinimumValue = "0")]
            public int Top { get; set; }

            [ControlNumeric(Required = true, MinimumValue = "0")]
            public int Left { get; set; }

            [ControlChoice(ControlChoice.DropDownList, Required = true)]
            public string Button { get; set; }

            [ControlChoice(ControlChoice.DropDownList, Required = true)]
            public string Background { get; set; }

            [ControlChoice(ControlChoice.DropDownList, Required = true)]
            public string Round { get; set; }

            [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Animation Type")]
            public string AnimationType { get; set; }

            [ControlText(Type = ControlText.MultiText)]
            public string Content { get; set; }
        }
    }
}