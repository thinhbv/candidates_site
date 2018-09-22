using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Messages.Services;
using CMSSolutions.ContentManagement.Newsletters.Domain;
using CMSSolutions.ContentManagement.Newsletters.Models;
using CMSSolutions.ContentManagement.Newsletters.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Newsletters.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Newsletters)]
    public class NewsletterController : BaseController
    {
        private readonly ILanguageManager languageManager;
        private readonly INewsletterService newsletterService;
        private readonly IMessageService messageService;

        public NewsletterController(
            IWorkContextAccessor workContextAccessor,
            INewsletterService newsletterService,
            ILanguageManager languageManager,
            IMessageService messageService)
            : base(workContextAccessor)
        {
            this.newsletterService = newsletterService;
            this.languageManager = languageManager;
            this.messageService = messageService;
        }

        [Url("{DashboardBaseUrl}/newsletters/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Newsletters"), Url.Action("Index", new { area = Constants.Areas.Newsletters }));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new NewsletterModel();

            var result = new ControlFormResult<NewsletterModel>(model)
            {
                Title = T("Create Newsletter").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            return result;
        }

        [ActionName("Update")]
        [FormButton("Delete")]
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            var page = newsletterService.GetById(id);
            newsletterService.Delete(page);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [Url("{DashboardBaseUrl}/newsletters/edit/{id}")]
        public ActionResult Edit(Guid id)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            NewsletterModel model = newsletterService.GetById(id);

            WorkContext.Breadcrumbs.Add(T("Newsletters"), Url.Action("Index", new { area = Constants.Areas.Newsletters }));
            WorkContext.Breadcrumbs.Add(model.Title);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var result = new ControlFormResult<NewsletterModel>(model)
            {
                Title = T("Edit Newsletter").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.AddAction(true, true, false).HasText(T("Save & Continue")).HasName("SaveAndContinue").HasButtonStyle(ButtonStyle.Info);

            return result;
        }

        [ActionName("Update")]
        [FormButton("Publish")]
        [HttpPost]
        public ActionResult Publish(Guid id)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            var newsletter = newsletterService.GetById(id);
            var subscribers = newsletterService.GetSubscribers();

            var message = new MailMessage();

            foreach (var triple in subscribers)
            {
                if (!string.IsNullOrEmpty(triple.Third))
                {
                    int languageId = int.Parse(triple.Third);

                    var language = WorkContext.Resolve<ILanguageService>().GetById(languageId);

                    if (language != null)
                    {
                        var translatedNewsletter = newsletterService.GetByLanguage(id, language.CultureCode);
                        if (translatedNewsletter != null)
                        {
                            message.Subject = translatedNewsletter.Title;
                            message.Body = translatedNewsletter.BodyContent;
                        }
                        else
                        {
                            message.Subject = newsletter.Title;
                            message.Body = newsletter.BodyContent;
                        }
                    }
                    else
                    {
                        message.Subject = newsletter.Title;
                        message.Body = newsletter.BodyContent;
                    }
                }
                else
                {
                    message.Subject = newsletter.Title;
                    message.Body = newsletter.BodyContent;
                }

                message.To.Add(new MailAddress(triple.Second, triple.First));
                messageService.SendEmailMessage(message);
            }

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [Url("{DashboardBaseUrl}/newsletters")]
        public ActionResult Index()
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Newsletters"));

            var result = new ControlGridFormResult<Newsletter>
            {
                Title = T("Manage Newsletters").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetNewsletters,
                EnablePaginate = true,
                EnableSearch = true,
                DefaultPageSize = WorkContext.DefaultPageSize,
                ActionsColumnWidth = 280,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Title);
            result.AddColumn(x => x.DateCreated, "Date Created").HasDisplayFormat("{0:g}");

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary).HasParentClass(Constants.ContainerCssClassCol12);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall);

            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            if (languages.Any())
            {
                result.AddRowAction()
                    .HasText(T("Translations"))
                    .HasUrl(x => Url.Action("Translations", RouteData.Values.Merge(new { id = x.Id })))
                    .HasButtonSize(ButtonSize.ExtraSmall)
                    .HasButtonStyle(ButtonStyle.Info)
                    .ShowModalDialog();
            }

            result.AddRowAction(true)
                .HasText(T("Publish"))
                .HasName("Publish")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Success)
                .HasConfirmMessage(T("This action will email the newsletter to all subscribers. Are you sure you want to do this?").Text);

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            return result;
        }

        [ValidateInput(false), FormButton("SaveAndContinue"), ActionName("Update")]
        public ActionResult SaveAndContinue(NewsletterModel model)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            Newsletter entity = null;

            if (model.Id != Guid.Empty)
            {
                entity = newsletterService.GetById(model.Id);
            }
            else
            {
                entity = new Newsletter { DateCreated = DateTime.UtcNow };
            }

            entity.Title = model.Title;
            entity.BodyContent = model.BodyContent;
            entity.CultureCode = model.CultureCode;
            entity.RefId = model.RefId;

            newsletterService.Save(entity);

            return null;
        }

        [Url("{DashboardBaseUrl}/newsletters/translate/{id}/{cultureCode}")]
        public ActionResult Translate(Guid id, string cultureCode)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            NewsletterModel model = newsletterService.GetByLanguage(id, cultureCode);

            WorkContext.Breadcrumbs.Add(T("Newsletters"), Url.Action("Index", new { area = Constants.Areas.Newsletters }));
            if (model != null)
            {
                WorkContext.Breadcrumbs.Add(model.Title);
            }

            WorkContext.Breadcrumbs.Add(T("Translate"));
            WorkContext.Breadcrumbs.Add(cultureCode);

            var showSaveAndContinue = false;

            if (model == null)
            {
                model = newsletterService.GetById(id);
                model.Id = Guid.Empty;
                model.CultureCode = cultureCode;
                model.RefId = id;
                ViewData.ModelState["Id"] = new ModelState { Value = new ValueProviderResult(Guid.Empty, Guid.Empty.ToString(), null) };
            }
            else
            {
                ViewData.ModelState["Id"] = new ModelState { Value = new ValueProviderResult(model.Id, model.Id.ToString(), null) };
                showSaveAndContinue = true;
            }

            var result = new ControlFormResult<NewsletterModel>(model)
            {
                Title = T("Translate Newsletter").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.AssignGridLayout(x => x.Id, 0, 0);
            result.AssignGridLayout(x => x.CultureCode, 0, 0);
            result.AssignGridLayout(x => x.RefId, 0, 0);
            result.AssignGridLayout(x => x.Title, 0, 0);
            result.AssignGridLayout(x => x.BodyContent, 0, 1, 2);

            if (showSaveAndContinue)
            {
                result.AddAction(true, true, false).HasText(T("Save & Continue")).HasName("SaveAndContinue").HasButtonStyle(ButtonStyle.Info);
            }

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/newsletters/translations/{id}")]
        public ActionResult Translations(Guid id)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            var model = new TranslationModel { Id = id };

            var result = new ControlFormResult<TranslationModel>(model)
            {
                Title = T("Select Language").Text,
                UpdateActionName = "Translations",
                SubmitButtonText = T("OK"),
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            result.RegisterExternalDataSource(x => x.CultureCode, languages.ToDictionary(k => k.CultureCode, v => v.Name));

            return result;
        }

        [HttpPost, FormButton("Save")]
        public ActionResult Translations(TranslationModel model)
        {
            if (!CheckPermission(NewsletterPermissions.ManageNewsletters))
            {
                return new HttpUnauthorizedResult();
            }

            return new AjaxResult()
                .Redirect(Url.Action("Translate", new { id = model.Id, cultureCode = model.CultureCode }), true);
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost]
        [Url("{DashboardBaseUrl}/newsletters/update")]
        public ActionResult Update(NewsletterModel model)
        {
            SaveAndContinue(model);

            return new AjaxResult().Redirect(Url.Action("Index"));
        }

        private ControlGridAjaxData<Newsletter> GetNewsletters(ControlGridFormRequest options)
        {
            int totals;
            var records = newsletterService.GetRecords(options, out totals, x => x.RefId == null);
            return new ControlGridAjaxData<Newsletter>(records, totals);
        }
    }
}