using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Pages;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Security.Domain;
using CMSSolutions.Web.Security.Models;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true), Authorize]
    [Feature(Constants.Areas.Security)]
    public class AccountController : BaseController
    {
        private readonly IMembershipService service;
        private readonly IEventBus eventBus;
        private readonly MembershipSettings membershipSettings;
        private readonly Lazy<IEnumerable<IUserProfileProvider>> userProfileProviders;

        protected virtual string DefaultRedirectUrl
        {
            get { return Url.Content("~/"); }
        }

        public AccountController(
            IWorkContextAccessor workContextAccessor,
            IMembershipService service,
            IEventBus eventBus,
            MembershipSettings membershipSettings,
            Lazy<IEnumerable<IUserProfileProvider>> userProfileProviders)
            : base(workContextAccessor)
        {
            this.service = service;
            this.eventBus = eventBus;
            this.membershipSettings = membershipSettings;
            this.userProfileProviders = userProfileProviders;
        }

        #region Login & LogOff

        [AllowAnonymous]
        [Url("account/login")]
        public virtual ActionResult Login()
        {
            var model = new LoginModel
            {
                ReturnUrl = Request.QueryString["ReturnUrl"]
            };

            ActionResult viewResult;
            if (FindView("Login", model, out viewResult))
            {
                return viewResult;
            }

            var result = new ControlFormResult<LoginModel>(model)
            {
                Title = T("Login to your account").Text,
                UpdateActionName = "Login",
                ShowCancelButton = false,
                SubmitButtonText = T("Login").Text,
                CssClass = "login-form col-md-4",
                ClientId = "fLogin",
                ShowValidationSummary = true,
                ShowBoxHeader = false,
                IconHeader = "fa fa-lg fa-fw fa-user",
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml,
                IsFormCenter = true,
                CssFormCenter = "col-md-4"
            };

            //if (membershipSettings.AllowRegisterUser)
            //{
            //    result.AddAction(addToTop: false).HasText(T("Register")).HasUrl(Url.Action("Register")).HasButtonStyle(ButtonStyle.Default).HasClientId("btnRegister");
            //}

            //if (membershipSettings.AllowForgotPassword)
            //{
            //    result.AddAction(addToTop: false).HasText(T("Forgot password?")).HasUrl(Url.Action("Recovery")).HasButtonStyle(ButtonStyle.Default).HasClientId("btnFindPassword");
            //}

            var providers = WorkContext.Resolve<IEnumerable<IAuthenticationClientProvider>>();

            var clients = (from provider in providers
                           let resolvedProvider = (IAuthenticationClientProvider)WorkContext.Resolve(provider.GetType())
                           where resolvedProvider.IsValid()
                           select resolvedProvider).ToList();

            if (clients.Count > 0)
            {
                var action = result.AddAction();
                action.HasHtmlBuilder(() =>
                {
                    var sb = new StringBuilder();
                    sb.Append("<fieldset class=\"oauth-login-form\">");
                    sb.AppendFormat("<legend>{0}</legend>", T("Sign In using"));

                    foreach (var client in clients)
                    {
                        sb.AppendFormat("<a title=\"{0}\" class=\"btn-oauth btn-oauth-{1}\" href=\"{2}\">{0}</a>", client.Name, client.ProviderName, client.GetLoginUrl(Url, model.ReturnUrl));
                    }

                    sb.Append("</fieldset>");
                    return sb.ToString();
                });
            }

            ViewBag.BodyCssClass = "login-page";

            return result;
        }

        [HttpPost, AllowAnonymous, FormButton("Save")]
        public virtual ActionResult Login(LoginModel model)
        {
            try
            {
                service.Login(model.UserName, model.Password, model.RememberMe);
                var returnUrl = model.ReturnUrl;
                string redirectUrl;

                if (Url.IsLocalUrl(returnUrl) &&
                    returnUrl.Length > 1 &&
                    returnUrl.StartsWith("/") &&
                    !returnUrl.StartsWith("//") &&
                    !returnUrl.StartsWith("/\\") &&
                    !returnUrl.EndsWith("account/register"))
                {
                    redirectUrl = returnUrl;
                }
                else
                {
                    redirectUrl = DefaultRedirectUrl;
                }

                return new AjaxResult().Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                return new AjaxResult().ExecuteScript(string.Format("$('#fLogin .validation-summary').addClass('alert alert-danger').html('<span>{0}</span>').show('slow');setTimeout(function(){{ $('#fLogin .validation-summary').hide('slow'); }}, 5000);", ex.Message));
            }
        }

        [Themed(IsDashboard = true)]
        [Url("account")]
        public ActionResult Manage()
        {
            WorkContext.Breadcrumbs.Add(T("Account"));
            WorkContext.Breadcrumbs.Add(T("Manage"));

            var user = WorkContext.CurrentUser;
            var model = new UserModel
            {
                UserName = user.UserName,
                Email = user.Email
            };
            var accountInfo = new ControlFormResult<UserModel>(model)
            {
                Title = T("Account Info"),
                ReadOnly = true,
                ShowCancelButton = false,
                ShowBoxHeader = false,
                IconHeader = "fa fa-lg fa-fw fa-user",
                CssClass = "col-md-4", 
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };
            accountInfo.ExcludeProperty(x => x.SuperUser);
            accountInfo.ExcludeProperty(x => x.IsLockedOut);

            accountInfo.AddAction(addToTop: false).HasText(T("Change Password")).HasUrl(Url.Action("ChangePassword")).HasButtonStyle(ButtonStyle.Primary);

            return accountInfo;
        }

        [Url("account/log-off")]
        public virtual ActionResult LogOff()
        {
            service.Logout();
            return Redirect(DefaultRedirectUrl);
        }

        #endregion Login & LogOff

        #region Register

        [AllowAnonymous]
        [Url("account/register")]
        public virtual ActionResult Register()
        {
            if (!membershipSettings.AllowRegisterUser)
            {
                return new HttpUnauthorizedResult();
            }

            var model = new RegisterModel();

            ActionResult viewResult;
            if (FindView("Register", model, out viewResult))
            {
                return viewResult;
            }

            var result = new ControlFormResult<RegisterModel>(model)
            {
                Title = T("Sign Up").Text,
                UpdateActionName = "Register",
                ShowCancelButton = false,
                SubmitButtonText = T("Sign Up").Text,
                CssClass = "register-form col-md-4",
                ShowBoxHeader = false,
                //IconHeader = "fa fa-lg fa-fw fa-user",
                //FormWrapperStartHtml = CMSConstants.Form.FormWrapperStartHtml,
                //FormWrapperEndHtml = CMSConstants.Form.FormWrapperEndHtml,
                IsFormCenter = true,
                CssFormCenter = "col-md-4"
            };

            result.AddAction(addToTop: false).HasText(T("Login")).HasUrl(Url.Action("Login")).HasButtonStyle(ButtonStyle.Default);
            
            if (membershipSettings.AllowForgotPassword)
            {
                result.AddAction(addToTop: false).HasText(T("Forgot password?")).HasUrl(Url.Action("Recovery")).HasButtonStyle(ButtonStyle.Default);
            }

            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Register(User user, string password)
        {
            try
            {
                service.CreateUserAndLocalAccount(user, password, membershipSettings.RequireConfirmation);
                return new AjaxResult().Alert(T(SecurityConstants.SuccessRegister)).Redirect(Url.Action("Login"));
            }
            catch (MembershipCreateUserException e)
            {
                return new AjaxResult().Alert(T(ErrorCodeToString(e.StatusCode)));
            }
        }

        [AllowAnonymous]
        [Url("account/activate/{token}")]
        public virtual ActionResult Activate(string token)
        {
            var confirm = service.ConfirmAccount(token);
            if (confirm)
            {
                return RedirectToAction("Login");
            }

            return new HttpNotFoundResult();
        }

        #endregion Register

        #region ChangePassword

        [Themed(IsDashboard = true)]
        [Url("account/change-password")]
        public virtual ActionResult ChangePassword()
        {
            WorkContext.Breadcrumbs.Add(T("Account"));
            WorkContext.Breadcrumbs.Add(T("Change Password"));

            var model = new ChangePasswordModel
            {
                Email = WorkContext.CurrentUser.Email,
                UserName = WorkContext.CurrentUser.UserName
            };

            var result = new ControlFormResult<ChangePasswordModel>(model)
            {
                Title = T("Change Password").Text,
                UpdateActionName = "ChangePassword",
                SubmitButtonText = T("Change Password"),
                CancelButtonUrl = Url.Action("Manage")
            };

            return result;
        }

        [HttpPost]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    service.ChangePassword(User.Identity.Name, model.OldPassword, model.Password);
                    eventBus.Notify<IMembershipEventHandler>(x => x.PasswordChanged(User.Identity.Name));

                    return new AjaxResult().Alert(T("Your password has been successfully updated."));
                }
                catch (Exception)
                {
                    return new AjaxResult().Alert(T(SecurityConstants.ErrorChangePassword));
                }
            }

            throw new InvalidModelStateException(ModelState);
        }

        #endregion ChangePassword

        #region Password Recovery

        [AllowAnonymous]
        [Url("account/recovery")]
        public virtual ActionResult Recovery()
        {
            if (!membershipSettings.AllowForgotPassword)
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Account"));
            WorkContext.Breadcrumbs.Add(T("Recovery"));

            var model = new PasswordRecoveryModel();
            var result = new ControlFormResult<PasswordRecoveryModel>(model)
            {
                Title = T("Find Your Password"),
                Description = T("Please enter your email address below. You will receive a link to reset your password."),
                UpdateActionName = "Recovery",
                ShowCancelButton = false,
                SubmitButtonText = T("Recovery"),
                CssClass = "forgot-password-form col-md-4",
                ShowBoxHeader = false,
                //IconHeader = "fa fa-lg fa-fw fa-user",
                //FormWrapperStartHtml = CMSConstants.Form.FormWrapperStartHtml,
                //FormWrapperEndHtml = CMSConstants.Form.FormWrapperEndHtml,
                IsFormCenter = true,
                CssFormCenter = "col-md-4"
            };

            result.AddAction(addToTop: false).HasText(T("Login")).HasUrl(Url.Action("Login")).HasButtonStyle(ButtonStyle.Default);

            if (membershipSettings.AllowRegisterUser)
            {
                result.AddAction(addToTop: false).HasText(T("Register")).HasUrl(Url.Action("Register")).HasButtonStyle(ButtonStyle.Default);
            }

            return result;
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult Recovery(PasswordRecoveryModel model)
        {
            try
            {
                User user = null;

                if (!string.IsNullOrEmpty(model.Email))
                {
                    user = service.GetUserByEmail(model.Email);
                }

                if (user == null)
                {
                    return new AjaxResult().Alert(T("Email not found. Please try again."));
                }

                var passwordVerificationToken = service.GeneratePasswordResetToken(user, 1440);

                eventBus.Notify<IMembershipEventHandler>(x => x.PasswordReset(user, passwordVerificationToken));

                return new AjaxResult()
                    .Alert(T("Email with instructions has been sent to you."))
                    .Redirect(Url.Action("ResetPassword"));
            }
            catch (Exception)
            {
                return new AjaxResult().Alert(T(SecurityConstants.ResetPasswordError));
            }
        }

        [AllowAnonymous]
        [Url("account/reset-password/{resetToken?}")]
        public virtual ActionResult ResetPassword(string resetToken)
        {
            if (membershipSettings.EnablePasswordReset)
            {
                return new HttpUnauthorizedResult();
            }

            if (string.IsNullOrEmpty(resetToken))
            {
                return RedirectToAction("Login");
            }

            var localAccount = service.GetLocalAccount(resetToken);
            if (localAccount == null)
            {
                return RedirectToAction("Login");
            }

            if (!localAccount.PasswordVerificationTokenExpirationDate.HasValue || DateTime.UtcNow > localAccount.PasswordVerificationTokenExpirationDate.Value)
            {
                return RedirectToAction("Login");
            }

            WorkContext.Breadcrumbs.Add(T("Account"));
            WorkContext.Breadcrumbs.Add(T("Reset Password"));

            var model = new ResetPasswordModel { ResetToken = resetToken };

            var result = new ControlFormResult<ResetPasswordModel>(model)
            {
                Title = T("Reset Password"),
                UpdateActionName = "ResetPassword",
                ShowCancelButton = false,
                ShowBoxHeader = false,
                //IconHeader = "fa fa-lg fa-fw fa-user",
                //CssClass = "col-md-4", 
                //FormWrapperStartHtml = CMSConstants.Form.FormWrapperStartHtml,
                //FormWrapperEndHtml = CMSConstants.Form.FormWrapperEndHtml
                IsFormCenter = true,
                CssFormCenter = "col-md-4"
            };

            result.AddAction(addToTop: false).HasText(T("Login")).HasUrl(Url.Action("Login")).HasButtonStyle(ButtonStyle.Default);
            if (membershipSettings.AllowForgotPassword)
            {
                result.AddAction(addToTop: false).HasText(T("Forgot Password")).HasUrl(Url.Action("Recovery")).HasButtonStyle(ButtonStyle.Default);
            }

            return result;
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual ActionResult ResetPassword(ResetPasswordModel model)
        {
            var user = service.ResetPasswordWithToken(model.ResetToken, model.Password);
            eventBus.Notify<IMembershipEventHandler>(x => x.PasswordChanged(user.UserName));

            return new AjaxResult().Alert(T(SecurityConstants.SuccessResetPassword)).Redirect(Url.Action("Login"));
        }

        #endregion Password Recovery

        #region Helpers

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : Redirect(DefaultRedirectUrl);
        }

        protected static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion Helpers

        #region User Profile

        [Themed(IsDashboard = true)]
        [Url("account/profile/{userId}")]
        public virtual ActionResult ViewProfile(int userId)
        {
            return UserProfile(userId);
        }

        [Themed(IsDashboard = true)]
        [Url("account/my-profile")]
        public virtual ActionResult ViewMyProfile()
        {
            return UserProfile(WorkContext.CurrentUser.Id);
        }

        [Themed(IsDashboard = true)]
        [Url("account/profile/edit/{userId}/")]
        public virtual ActionResult EditProfile(int userId)
        {
            return UserProfile(userId, true);
        }

        [Themed(IsDashboard = true)]
        [Url("account/my-profile/edit")]
        public virtual ActionResult EditMyProfile()
        {
            return UserProfile(WorkContext.CurrentUser.Id, true);
        }

        private ActionResult UserProfile(int userId, bool editMode = false)
        {
            var result = new ControlFormResult
            {
                UpdateActionName = "UpdateProfile",
                SubmitButtonText = T("Save").Text,
                ReadOnly = !editMode
            };

            string title;
            bool onlyPublicProperties = false;

            if (editMode)
            {
                if (userId == WorkContext.CurrentUser.Id)
                {
                    title = T("Edit My Profile");
                }
                else if (WorkContext.CurrentUser.SuperUser || CheckPermission(StandardPermissions.FullAccess))
                {
                    title = T("Edit Profile");
                }
                else
                {
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                if (userId == WorkContext.CurrentUser.Id)
                {
                    title = T("My Profile");

                    result.AddAction(addToTop: false)
                        .HasText(T("Edit"))
                        .HasUrl(Url.Action("EditMyProfile"))
                        .HasButtonStyle(ButtonStyle.Primary);
                }
                else if (WorkContext.CurrentUser.SuperUser || CheckPermission(StandardPermissions.FullAccess))
                {
                    var user = service.GetUser(userId);
                    title = string.Format(T("Profile for '{0}'", user.UserName));

                    result.AddAction(addToTop: false)
                        .HasText(T("Edit"))
                        .HasUrl(Url.Action("EditProfile", RouteData.Values.Merge(new {userId })))
                        .HasButtonStyle(ButtonStyle.Primary);
                }
                else
                {
                    var user = service.GetUser(userId);
                    title = string.Format(T("Profile for '{0}'", user.UserName));
                    onlyPublicProperties = true;
                }

                result.CancelButtonText = T("Close");
            }

            result.Title = title;

            result.AddHiddenValue("UserId", userId.ToString());

            bool hasProperties = false;
            foreach (var provider in userProfileProviders.Value)
            {
                var newGroup = result.AddGroupedLayout(provider.Category);

                foreach (var field in provider.GetFields(WorkContext, userId, onlyPublicProperties))
                {
                    hasProperties = true;

                    result.AddProperty(field.Name, field, field.Value);
                    newGroup.Add(field.Name);
                }
            }

            if (!hasProperties)
            {
                return new ContentViewResult
                {
                    Title = title,
                    BodyContent = T("There is no profile available to view.")
                };
            }

            return result;
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost]
        [Url("account/update-profile")]
        public virtual ActionResult UpdateProfile()
        {
            var userId = Convert.ToInt32(Request.Form["UserId"]); // Guid.Parse(Request.Form["UserId"]);

            var newProfile = new Dictionary<string, string>();

            foreach (var provider in userProfileProviders.Value)
            {
                foreach (var fieldName in provider.GetFieldNames())
                {
                    string value = Request.Form[fieldName];

                    if (value == "true,false")
                    {
                        value = "true";
                    }

                    newProfile.Add(fieldName, value);
                }
            }

            service.UpdateProfile(userId, newProfile);

            if (userId == WorkContext.CurrentUser.Id)
            {
                return new AjaxResult().Redirect(Url.Action("ViewMyProfile"));
            }
            return new AjaxResult().Redirect(Url.Action("ViewProfile", RouteData.Values.Merge(new {userId })));
        }

        #endregion User Profile
    }
}