using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Sliders.Domain;
using CMSSolutions.ContentManagement.Sliders.Models;
using CMSSolutions.ContentManagement.Sliders.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Url(BaseUrl = "{DashboardBaseUrl}/sliders/slides/{sliderId}")]
    [Feature(Constants.Areas.Sliders)]
    public class SlideController : BaseControlController<Guid, Slide, SlideModel>
    {
        public SlideController(IWorkContextAccessor workContextAccessor, ISlideService service)
            : base(workContextAccessor, service)
        {
             
        }

        protected override int DialogModalWidth
        {
            get
            {
                return 800;
            }
        }

        protected override int DialogModalHeight
        {
            get
            {
                return 700;
            }
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override void OnViewIndex(ControlGridFormResult<Slide> controlGrid)
        {
            controlGrid.Title = T("Manage Slides");
            controlGrid.ActionsColumnWidth = 200;
            controlGrid.AddColumn(x => x.Position);
            controlGrid.AddColumn(x => x.Title);
            controlGrid.AddColumn(x => x.BackgroundUrl).HasHeaderText(T("Background Url"));
            controlGrid.AddColumn(x => x.SlideDirection).AlignCenter().HasHeaderText(T("Slide Direction"));
            controlGrid.AddColumn(x => x.SlideDelay).AlignCenter().HasHeaderText(T("Slide Delay"));

            controlGrid.AddRowAction()
                .HasText(T("Sublayers"))
                .HasUrl(x => Url.Action("Sublayers", new { slideId = x.Id }))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Primary);

            WorkContext.Breadcrumbs.Add(T("Slideshows"), Url.Action("Index", "Slider"));
            WorkContext.Breadcrumbs.Add(T("Slides"));
        }

        protected override ControlGridAjaxData<Slide> GetRecords(ControlGridFormRequest options)
        {
            var slideId = new Guid(RouteData.GetRequiredString("sliderId"));
            int totals;
            var records = Service.GetRecords(options, out totals, x => x.SliderId == slideId);
            return new ControlGridAjaxData<Slide>(records, totals);
        }

        protected override void OnCreating(ControlFormResult<SlideModel> controlForm)
        {
            base.OnCreating(controlForm);
            controlForm.RegisterExternalDataSource(x => x.SlideDirection, "left", "right", "top", "bottom", "fade");
            controlForm.FormModel.Title = string.Empty;
            controlForm.FormModel.SlideDirection = "right";
            controlForm.RegisterFileUploadOptions("BackgroundUrl.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });
        }

        protected override void OnEditing(ControlFormResult<SlideModel> controlForm)
        {
            controlForm.RegisterExternalDataSource(x => x.SlideDirection, "left", "right", "top", "bottom", "fade");
            controlForm.RegisterFileUploadOptions("BackgroundUrl.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });
            base.OnEditing(controlForm);
        }

        protected override SlideModel ConvertToModel(Slide entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(SlideModel model, Slide entity)
        {
            entity.Id = model.Id;
            entity.SliderId = new Guid(RouteData.GetRequiredString("sliderId"));
            entity.Position = model.Position;
            entity.BackgroundUrl = model.BackgroundUrl;
            entity.Title = model.Title;
            entity.SlideDirection = model.SlideDirection;
            entity.SlideDelay = model.SlideDelay;
            entity.Transition2D = model.Transition2D;
            entity.Transition3D = model.Transition3D;
            entity.OnClick = model.OnClick;
        }

        [Themed]
        [Url("{DashboardBaseUrl}/sliders/sublayers/{slideId}")]
        public ActionResult Sublayers(Guid slideId)
        {
            var slide = Service.GetById(slideId);

            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            var writer = new HtmlTextWriter(textWriter);

            writer.AddAttribute("method", "post");
            writer.AddAttribute("action", Url.Action("SaveSublayers"));
            writer.RenderBeginTag(HtmlTextWriterTag.Form);

            writer.AddAttribute(HtmlTextWriterAttribute.Name, "SlideId");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, slideId.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();// input

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "hdfSublayers");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "Sublayers");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();// input

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-toolbar");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-group");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-primary");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "Save");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "var html = ''; $('#sublayers-container .ls-s').each(function(){ html+=this.outerHTML; }); $('#hdfSublayers').val(html);");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write("Save");
            writer.RenderEndTag(); // button

            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-group");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            CreateButton(writer, "img");
            CreateButton(writer, "div");
            CreateButton(writer, "p");
            CreateButton(writer, "span");
            CreateButton(writer, "h1");
            CreateButton(writer, "h2");
            CreateButton(writer, "h3");
            CreateButton(writer, "h4");
            CreateButton(writer, "h5");
            CreateButton(writer, "h6");

            writer.RenderEndTag(); // div

            writer.RenderEndTag(); // div

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "sublayers-container");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "auto");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "400px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "10px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (!string.IsNullOrEmpty(slide.BackgroundUrl))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, Url.Content(slide.BackgroundUrl));
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ls-bg");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }

            writer.Write(slide.Sublayers);

            writer.RenderEndTag();
            writer.RenderEndTag(); // form

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write("$(document).ready(function(){");

            if (!string.IsNullOrEmpty(slide.BackgroundUrl))
            {
                writer.Write("var image = new Image(); image.onload = function(){{ var img = $('#sublayers-container .ls-bg'); img.css('width', image.naturalWidth + 'px'); img.css('max-width', image.naturalWidth + 'px'); img.css('height', image.naturalHeight + 'px'); }}; image.src = '{0}';", slide.BackgroundUrl);
            }

            if (!string.IsNullOrEmpty(slide.Sublayers))
            {
                writer.Write("$('#sublayers-container .ls-s').draggable({cursor:'pointer'});");
            }

            writer.Write("$('#sublayers-container').on('dblclick', '.ls-s', function(){{ $.fancybox.open({{ href: '{0}?tag=' + this.tagName.toLowerCase() + '&id=' + this.id, type: 'iframe', modal: true, padding: 0, width: 600 }}); }});", Url.Action("AddOrEditSublayer"));

            writer.Write("});");
            writer.RenderEndTag();

            WorkContext.Breadcrumbs.Add(T("Sliders"), Url.Action("Index", "Slider"));
            WorkContext.Breadcrumbs.Add(T("Slides"), Url.Action("Index", "Slide", new { sliderId = slide.SliderId }));

            var result = new ControlContentResult(sb.ToString())
                         {
                             Title = T("Slide Sublayers"),
                             AdditionResources = () => new[] { ResourceType.JQueryUI }
                         };
            return result;
        }

        [HttpPost]
        [ValidateInput(false), FormButton("Sublayers")]
        [Url("{DashboardBaseUrl}/sliders/sublayers/save", 10)]
        public ActionResult SaveSublayers(Guid slideId, string sublayers)
        {
            var slide = Service.GetById(slideId);
            slide.Sublayers = sublayers;
            Service.Save(slide);

            return RedirectToAction("Index", new { sliderId = slide.SliderId });
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/sliders/sublayers/addoredit", 10)]
        public ActionResult AddOrEditSublayer(string tag, string id)
        {
            var model = new SublayerModel { Id = id, Tag = tag };

            var result = new ControlFormResult<SublayerModel>(model)
                         {
                             Title = T("Sublayer"),
                             UpdateActionName = "UpdateSublayer"
                         };

            if (string.Equals(tag, "img", StringComparison.InvariantCultureIgnoreCase))
            {
                result.ExcludeProperty(x => x.HtmlContent);
                result.ExcludeProperty(x => x.FontFamily);
                result.ExcludeProperty(x => x.FontSize);
                result.ExcludeProperty(x => x.Color);
            }
            else
            {
                result.ExcludeProperty(x => x.ImageSource);
            }

            if (!string.IsNullOrEmpty(id))
            {
                var script = new StringBuilder();

                script.AppendLine(@"function colorToHex(color) {
                    if(!color) return '';
                    if (color.substr(0, 1) === '#') {
                        return color;
                    }
                    var digits = /(.*?)rgb\((\d+), (\d+), (\d+)\)/.exec(color); if(!digits) return color;

                    var red = parseInt(digits[2]);
                    var green = parseInt(digits[3]);
                    var blue = parseInt(digits[4]);

                    var rgb = blue | (green << 8) | (red << 16);
                    return digits[1] + '#' + rgb.toString(16);
                };");
                script.AppendLine(@"function getStyleValue(style, name){
                    var split = style.split(';');
                    for(var i = 0; i < split.length; i++){
                        if(split[i].indexOf(name + ':') == 0){
                            return $.trim(split[i].replace(name + ':', ''));
                        }
                    }
                    return '';
                }");

                script.AppendFormat("var element = $('#{0}', window.parent.document);", id);
                script.Append("var elementStyle = element.prop('style');");
                script.Append("var style = element.attr('style');");
                script.Append("$('#Top').val(element.css('top'));");
                script.Append("$('#Left').val(element.css('left'));");
                script.Append("$('#BackgroundColor').val(colorToHex(elementStyle[$.camelCase('backround-color')]));");
                script.Append("$('#SlideDirection').val(getStyleValue(style, 'slidedirection'));");
                script.Append("$('#SlideOutDirection').val(getStyleValue(style, 'slideoutdirection'));");
                script.Append("$('#DurationIn').val(getStyleValue(style, 'durationin'));");
                script.Append("$('#DurationOut').val(getStyleValue(style, 'durationout'));");
                script.Append("$('#EasingIn').val(getStyleValue(style, 'easingin'));");
                script.Append("$('#EasingOut').val(getStyleValue(style, 'easingout'));");
                script.Append("$('#RotateIn').val(getStyleValue(style, 'rotatein'));");
                script.Append("$('#RotateOut').val(getStyleValue(style, 'rotateout'));");

                if (string.Equals(tag, "img", StringComparison.InvariantCultureIgnoreCase))
                {
                    script.Append("$('#ImageSource').val(element.attr('src'));");
                    script.Append("$('#ImageSource_UploadList').html(element.attr('src'));");
                }
                else
                {
                    script.Append("$('#HtmlContent').val(element.html());");
                    script.Append("$('#FontFamily').val(elementStyle[$.camelCase('font-family')]);");
                    script.Append("$('#FontSize').val(elementStyle[$.camelCase('font-size')]);");
                    script.Append("$('#Color').val(colorToHex(elementStyle[$.camelCase('color')]));");
                }
                var scriptRegister = new ScriptRegister(WorkContext);
                scriptRegister.IncludeInline(script.ToString());

                result.AddAction(true)
                    .HasName("Delete")
                    .HasButtonStyle(ButtonStyle.Danger)
                    .HasText(T("Delete"))
                    .HasIconCssClass("cx-icon cx-icon-remove")
                    .HasValue(id);
            }

            result.RegisterExternalDataSource(x => x.SlideDirection, "left", "right", "top", "bottom", "fade");
            result.RegisterExternalDataSource(x => x.SlideOutDirection, "left", "right", "top", "bottom", "fade");

            var easings = new[] { "linear", "swing", "easeInQuad", "easeOutQuad", "easeInOutQuad", "easeInCubic", "easeOutCubic", "easeInOutCubic", "easeInQuart", "easeOutQuart", "easeInOutQuart", "easeInQuint", "easeOutQuint", "easeInOutQuint", "easeInExpo", "easeOutExpo", "easeInOutExpo", "easeInSine", "easeOutSine", "easeInOutSine", "easeInCirc", "easeOutCirc", "easeInOutCirc", "easeInElastic", "easeOutElastic", "easeInOutElastic", "easeInBack", "easeOutBack", "easeInOutBack", "easeInBounce", "easeOutBounce", "easeInOutBounce" };
            result.RegisterExternalDataSource(x => x.EasingIn, easings);
            result.RegisterExternalDataSource(x => x.EasingOut, easings);

            return result;
        }

        [HttpPost]
        [ValidateInput(false), FormButton("Save")]
        public ActionResult AddOrEditSublayer(SublayerModel model)
        {
            var isImage = false;
            if (string.Equals(model.Tag, "img", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(model.ImageSource))
                {
                    return new AjaxResult().CloseModalDialog();
                }
                isImage = true;
            }

            var sb = new StringBuilder();
            var textWriter = new StringWriter(sb);
            var writer = new HtmlTextWriter(textWriter, string.Empty) { Indent = 0, NewLine = string.Empty };

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "sublayer-" + Guid.NewGuid());
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ls-s ls-s-1");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "pointer");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Top, model.Top);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Left, model.Left);

            if (!string.IsNullOrEmpty(model.BackgroundColor))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, model.BackgroundColor);
            }

            if (!string.IsNullOrEmpty(model.SlideDirection))
            {
                writer.AddStyleAttribute("slidedirection", model.SlideDirection);
            }

            if (!string.IsNullOrEmpty(model.SlideOutDirection))
            {
                writer.AddStyleAttribute("slideoutdirection", model.SlideOutDirection);
            }

            if (model.DurationIn.HasValue)
            {
                writer.AddStyleAttribute("durationin", model.DurationIn.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (model.DurationOut.HasValue)
            {
                writer.AddStyleAttribute("durationout", model.DurationOut.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(model.EasingIn))
            {
                writer.AddStyleAttribute("easingin", model.EasingIn);
            }

            if (!string.IsNullOrEmpty(model.EasingOut))
            {
                writer.AddStyleAttribute("easingout", model.EasingOut);
            }

            if (model.RotateIn.HasValue)
            {
                writer.AddStyleAttribute("rotatein", model.RotateIn.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (model.RotateOut.HasValue)
            {
                writer.AddStyleAttribute("rotateout", model.RotateOut.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (isImage)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, model.ImageSource);
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag(); // img
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FontFamily))
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, model.FontFamily);
                }

                if (!string.IsNullOrEmpty(model.FontSize))
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, model.FontSize);
                }

                if (!string.IsNullOrEmpty(model.Color))
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Color, model.Color);
                }

                writer.RenderBeginTag(model.Tag);
                writer.Write(Server.HtmlEncode(model.HtmlContent));
                writer.RenderEndTag();
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                return new AjaxResult()
                    .ExecuteScript(string.Format("$('#sublayers-container', window.parent.document).append('{0}'); parent.jQuery('#sublayers-container .ls-s', window.parent.document).draggable({{cursor:'pointer'}});", sb))
                    .CloseModalDialog();
            }

            return new AjaxResult()
                .ExecuteScript(string.Format("$('#sublayers-container #{1}', window.parent.document).replaceWith('{0}'); parent.jQuery('#sublayers-container .ls-s', window.parent.document).draggable({{cursor:'pointer'}});", sb, model.Id))
                .CloseModalDialog();
        }

        [HttpPost, ActionName("AddOrEditSublayer")]
        [ValidateInput(false), FormButton("Delete")]
        public ActionResult DeleteSublayer(string id)
        {
            return new AjaxResult()
                .ExecuteScript(string.Format("$('#sublayers-container #{0}', window.parent.document).remove();", id))
                .CloseModalDialog();
        }

        private void CreateButton(HtmlTextWriter writer, string tag)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-default");
            writer.AddAttribute("data-toggle", "fancybox");
            writer.AddAttribute("data-fancybox-type", "iframe");
            writer.AddAttribute("data-fancybox-width", "600");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, Url.Action("AddOrEditSublayer", new { tag }));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(tag.ToUpperInvariant());
            writer.RenderEndTag();
        }
    }
}