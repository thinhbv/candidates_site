using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Models;
using CMSSolutions.Localization.Services;
using CMSSolutions.Security;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Localization.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Localization)]
    public class LocalizationController : BaseController
    {
        private readonly ILocalizableStringService service;
        private readonly ILanguageService languageService;

        public LocalizationController(IWorkContextAccessor workContextAccessor, ILocalizableStringService service, ILanguageService languageService)
            : base(workContextAccessor)
        {
            this.service = service;
            this.languageService = languageService;
            TableName = "tblLocalizable";
        }

        [Url("{DashboardBaseUrl}/localizable-strings/{languageId}")]
        public ActionResult LocalizableStrings(int languageId)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            ViewBag.Title = T("Manage Localizable Strings");

            var language = languageService.GetById(languageId);

            WorkContext.Breadcrumbs.Add(T("Languages"), Url.Action("Index", "Language", new { area = Constants.Areas.Localization }));
            WorkContext.Breadcrumbs.Add(language.Name);
            WorkContext.Breadcrumbs.Add(T("Localizable Strings"));

            var result = new ControlGridFormResult<ComparitiveLocalizableString>
            {
                Title = "Manage Localizable Strings",
                CssClass = "table-excel",
                UpdateActionName = "Update",
                EnablePaginate = true,
                DefaultPageSize = 15,
                FetchAjaxSource = options => GetComparitiveTable(options, language.CultureCode),
                ActionsColumnWidth = 100,
                ClientId = TableName,
                EnableSearch = true
            };

            result.AddCustomVar("SearchText", "$('#txtSearchText').val();", true);   

            result.AddColumn(x => x.Key).RenderAsHtml(s => string.Format("{1}<input type=\"hidden\" name=\"KeyValue${0}\" value=\"{1}\" />", s.SequenceId, s.Key));
            result.AddColumn(x => x.InvariantValue).HasHeaderText("Invariant String");
            result.AddColumn(x => x.LocalizedValue).HasHeaderText("Localized String").RenderAsHtml(s => string.Format("<input type=\"text\" name=\"LocalizedValue${1}\" class=\"form-control input-sm\" value=\"{0}\">", s.LocalizedValue, s.SequenceId));

            result.AddAction(true)
                .HasText(T("Save"))
                .HasName("Save")
                .HasIconCssClass("cx-icon cx-icon-save")
                .HasValue(language.CultureCode)
                .HasButtonStyle(ButtonStyle.Primary).HasParentClass(Constants.ContainerCssClassCol12);

            result.AddAction(new ControlFormHtmlAction(BuildSearchText)).HasParentClass(Constants.ContainerCssClassCol3);

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Key)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord));

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");

            return result;
        }

        private string BuildSearchText()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(T("Keywords") + " <input id=\"txtSearchText\" name=\"SearchText\" class=\"form-control\" onkeypress = \"return InputEnterEvent(event, '" + TableName + "');\" onblur=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\"></input>");

            return sb.ToString();
        }

        private ControlGridAjaxData<ComparitiveLocalizableString> GetComparitiveTable(ControlGridFormRequest options, string cultureCode)
        {
            var searchText = string.Empty;
            if (Request.Form["SearchText"] != null)
            {
                searchText = Request.Form["SearchText"];
            }

            int total;
            var records = service.GetComparitiveTable(searchText, cultureCode, options.PageIndex, options.PageSize, out total);

            var index = 0;
            foreach (var record in records)
            {
                record.SequenceId = index++;
            }

            return new ControlGridAjaxData<ComparitiveLocalizableString>(records, total);
        }

        [ValidateInput(false)]
        [HttpPost, FormButton("Save")]
        [Url("{DashboardBaseUrl}/localizable-strings/update", Priority = 10)]
        public ActionResult Update(FormCollection formCollection, string id)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            var keys = formCollection.AllKeys.Where(x => x.StartsWith("LocalizedValue$"));
            var table = (from key in keys
                         let index = key.Replace("LocalizedValue$", "")
                         let keyValue = formCollection["KeyValue$" + index]
                         let localizedValue = formCollection[key]
                         where !string.IsNullOrEmpty(localizedValue)
                         select new ComparitiveLocalizableString
                        {
                            Key = keyValue,
                            LocalizedValue = localizedValue
                        }).ToList();

            service.Update(id, table);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Cập nhật thành công."));
        }

        [HttpPost, FormButton("Delete"), ActionName("Update")]
        public ActionResult Delete(string id)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                throw new NotAuthorizedException();
            }

            service.Delete(id);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã xóa thành công."));
        }
    }
}