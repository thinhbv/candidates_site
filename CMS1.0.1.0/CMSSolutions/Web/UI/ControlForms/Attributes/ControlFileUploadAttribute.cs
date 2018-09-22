using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlFileUploadAttribute : ControlFormAttribute
    {
        public bool EnableFineUploader { get; set; }

        public string AllowedExtensions { get; set; }

        public bool ShowThumbnail { get; set; }

        public string UploadFolder { get; set; }

        public bool AllowBrowseOnServer { get; set; }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var id = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name);

            if (!EnableFineUploader)
            {
                return string.Format(Required
                    ? "<input type=\"file\" name=\"{0}\" class=\"{1}\" id=\"{2}\" data-val=\"true\" data-val-required=\"{3}\" /><span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>" 
                    : "<input type=\"file\" name=\"{0}\" class=\"{1}\" id=\"{2}\" />", Name, CssClass, id, Constants.Messages.Validation.Required);
            }

            var options = controlForm.GetFileUploadOptions(Name);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var uploadUrl = options.UploadUrl ?? urlHelper.Action("UploadFiles", "UploadFiles", new { area = Constants.Areas.Media });
            var uploadFolder = options.UploadFolder;
            if (string.IsNullOrEmpty(uploadFolder))
            {
                uploadFolder = UploadFolder;
            }

            string browseButton = null;
            if (AllowBrowseOnServer)
            {
                browseButton = string.Format("<button class=\"btn btn-default qq-browse-button\" onclick=\"$.fancybox.open({{ href: '{0}', type: 'iframe', modal: true, padding: 0, width: 500, height: 200, autoSize: false, minHeight: 250, afterClose: function(){{ if(window.fancyboxResult){{ $('#{1}, #{1}_Validator').val(window.fancyboxResult); $('#{1}_UploadList').html(window.fancyboxResult); }} }} }}); return false;\"><i class=\"cx-icon cx-icon-folder-open\"></i></button>", urlHelper.Action("Browse", "Media", new { area = Constants.Areas.Media }), id);
            }

            var clientOptions = new JObject
            {
                new JProperty("multiple", false),
                new JProperty("paramsInBody", true),
                new JProperty("validation", new JObject(
                    new JProperty("allowedExtensions", GetAllowedExtensions(options.AllowedExtensions ?? AllowedExtensions)),
                    new JProperty("sizeLimit", options.SizeLimit))),
                    new JProperty("request", new JObject(new JProperty("endpoint", uploadUrl), new JProperty("params", new JObject(new JProperty("folder", uploadFolder ?? ""))))),
                    new JProperty("text", new JObject(new JProperty("uploadButton", "<i class=\"fa fa-lg fa-upload\"></i>"))),
                new JProperty("template", string.Format("<div class=\"input-group\"><input type=\"text\" class=\"form-control\" name=\"{0}\" id=\"{1}\" value=\"{2}\" autocomplete=\"off\" data-val=\"{4}\" data-val-required=\"{5}\" /><div class=\"input-group-btn\"><div class=\"qq-upload-drop-area\"><span>{{dragZoneText}}</span></div><div class=\"btn btn-default qq-upload-button\">{{uploadButtonText}}</div>{3}</div></div><div class=\"qq-drop-processing\"><span>{{dropProcessingText}}</span><span class=\"qq-drop-processing-spinner\"></span></div><div class=\"qq-upload-list\"></div>", Name, id, Value, browseButton, Required.ToString().ToLowerInvariant(), Constants.Messages.Validation.Required)),
                new JProperty("fileTemplate", "<div><div class=\"qq-progress-bar hide\"></div><span class=\"qq-upload-spinner\"></span><span class=\"qq-upload-file hide\"></span><span class=\"qq-upload-size\"></span><a class=\"qq-upload-cancel\" href=\"#\">{cancelButtonText}</a><span class=\"qq-upload-status-text\">{statusText}</span></div>"),
            };

            var sb = new StringBuilder();
            
            sb.AppendFormat("<div class=\"{1}\" id=\"{0}_Container\"></div>", id, CssClass);

            if (Required)
            {
                sb.AppendFormat("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name);
            }

            if (ShowThumbnail)
            {
                if (Value != null)
                {
                    sb.AppendFormat("<a href=\"{0}\" target=\"_blank\" title=\"Click to view larger image\"><img src=\"{0}\" data-src=\"holder.js/128x128\" style=\"max-width: 128px; max-height: 128px; margin-top: 5px;\" /></a>", Value);
                }
            }

            var scriptRegister = new ScriptRegister(workContext);
            scriptRegister.IncludeInline(string.Format("$('#{0}_Container').fineUploader({1}).on('upload', function(){{ var f = document.getElementById('{0}').form; var o ={{}};var a = $(f).serializeArray(); $.each(a, function(){{ if(o[this.name] !== undefined){{ if(!o[this.name].push){{ o[this.name]=[o[this.name]]; }} o[this.name].push(this.value || ''); }}else{{ o[this.name] = this.value || '';}} }}); $(this).fineUploader('setParams', o); }}).on('complete', function(event, id, name, responseJSON){{ if(responseJSON.success){{ $('#{0}').val(responseJSON.mediaUrl); }} else {{ $('#{0}').val(''); }} }}).on('complete', function(){{ $('#{0}_Container .qq-upload-list').hide(); }});",
                id, clientOptions.ToString(Formatting.None)));
            
            return sb.ToString();
        }

        private static JArray GetAllowedExtensions(string allowedExtensions)
        {
            var result = new JArray();
            if (!string.IsNullOrEmpty(allowedExtensions))
            {
                var split = allowedExtensions.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var str in split)
                {
                    result.Add(str);
                }
            }

            return result;
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            if (EnableFineUploader)
            {
                yield return ResourceType.FineUploader;
            }

            if (AllowBrowseOnServer)
            {
                yield return ResourceType.FancyBox;
            }
        }
    }
}