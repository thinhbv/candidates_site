using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Domain;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Newsletters.Controllers
{
    [Themed(IsDashboard = false)]
    [Feature(Constants.Areas.Newsletters)]
    public class SubscriptionController : BaseController
    {
        public SubscriptionController(IWorkContextAccessor workContextAccessor)
            : base(workContextAccessor)
        {
        }

        private static readonly Regex rgxEmail = new Regex("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        [HttpPost]
        [Url("newsletter-subscriptions/subscribe")]
        public JsonResult Subscribe(string email)
        {
            // First check if valid email address
            if (!rgxEmail.IsMatch(email))
            {
                return Json(new { Successful = false, Message = T("That is not a valid e-mail address. Please check your input and try again.").Text });
            }

            var membershipService = WorkContext.Resolve<IMembershipService>();
            var existingUser = membershipService.GetUserByEmail(email);

            // Check if a user exists with that email..
            if (existingUser != null)
            {
                // if user is logged in already and is the same user with that email address
                if (WorkContext.CurrentUser != null && WorkContext.CurrentUser.Id == existingUser.Id)
                {
                    //auto set "ReceiveNewsletters" in profile to true
                    membershipService.SaveProfileEntry(WorkContext.CurrentUser.Id, NewsletterUserProfileProvider.ReceiveNewsletters, bool.TrueString);

                    var eventBus = WorkContext.Resolve<IEventBus>();
                    eventBus.Notify<INewsletterEventHandler>(x => x.Subscribed(email));

                    return Json(new { Successful = true, Message = T("You have successfully signed up for newsletters.").Text });
                }
                else
                {
                    //else just tell user to login and set "ReceiveNewsletters" in profile to true
                    return Json(new { Successful = false, Message = T("A user with that e-mail address already exists. If you are the owner of that e-mail address, please login and try again.").Text });
                }
            }
            else
            {
                //create a user and email details to him/her with random password
                var user = new User { UserName = email, Email = email };
                membershipService.CreateUserAndLocalAccount(user, Guid.NewGuid().ToString().Substring(0, 8), false);

                // and sign up for newsletter, as requested.
                membershipService.SaveProfileEntry(user.Id, NewsletterUserProfileProvider.ReceiveNewsletters, bool.TrueString);

                var eventBus = WorkContext.Resolve<IEventBus>();
                eventBus.Notify<INewsletterEventHandler>(x => x.Subscribed(email));

                return Json(new { Successful = true, Message = T("You have successfully signed up for newsletters.").Text });
            }
        }

        [Url("newsletter-subscriptions/unsubscribe")]
        public ActionResult Unsubscribe()
        {
            WorkContext.Breadcrumbs.Add(T("Newsletters"));
            WorkContext.Breadcrumbs.Add(T("Unsubscribe"));

            var result = new ControlFormResult()
            {
                Title = T("Unsubscribe").Text,
                UpdateActionName = "UnsubscribePost",
                SubmitButtonText = T("Submit")
            };

            result.AddProperty("EmailAddress", new ControlTextAttribute
            {
                Name = "EmailAddress",
                LabelText = "Email Address",
                ContainerCssClass = "col-sm-6 col-md-6",
                PropertyType = typeof(string)
            });

            return result;
        }

        [HttpPost]
        [Url("newsletter-subscriptions/unsubscribe-post")]
        public ActionResult UnsubscribePost()
        {
            string email = Request.Form["EmailAddress"];

            if (string.IsNullOrEmpty(email))
            {
                return new AjaxResult().Redirect(Request.ApplicationPath);
            }

            var membershipService = WorkContext.Resolve<IMembershipService>();
            var user = membershipService.GetUserByEmail(email);

            if (user != null)
            {
                membershipService.SaveProfileEntry(user.Id, NewsletterUserProfileProvider.ReceiveNewsletters, bool.FalseString);

                var eventBus = WorkContext.Resolve<IEventBus>();
                eventBus.Notify<INewsletterEventHandler>(x => x.Unsubscribed(email));

                return new AjaxResult().Alert(T("You have been unsubscribed.")).Redirect(Request.ApplicationPath);
            }

            return new AjaxResult().Alert(T("We could not locate your email address in our database."));
        }
    }
}