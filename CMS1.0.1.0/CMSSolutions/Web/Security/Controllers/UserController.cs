using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Security;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Domain;
using CMSSolutions.Web.Security.Models;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Security)]
    [Url(BaseUrl = "{DashboardBaseUrl}/users")]
    public class UserController : BaseControlController<int, User, UserModel>
    {
        private readonly IMembershipService membershipService;
        private readonly IRoleService roleService;

        public UserController(IWorkContextAccessor workContextAccessor, IMembershipService membershipService, IRoleService roleService)
            : base(workContextAccessor, membershipService)
        {
            this.membershipService = membershipService;
            this.roleService = roleService;
            TableName = "tblUsers";
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override void OnCreateCreateButton(ControlFormAction createButton)
        {
            createButton.CssClass = "btn btn-primary";

            base.OnCreateCreateButton(createButton);
        }

        protected override void OnViewIndex(ControlGridFormResult<User> controlGrid)
        {
            controlGrid.ClientId = TableName;
            controlGrid.ActionsColumnWidth = 250;

            controlGrid.AddCustomVar("RoleId", "$('#ddlRoleId').val();", true);
            controlGrid.AddCustomVar("SearchText", "$('#txtSearchText').val();", true);

			controlGrid.AddColumn(x => x.Id).HasHeaderText(T("ID")).Width = 80;
            controlGrid.AddColumn(x => x.UserName).HasHeaderText(T("User Name")).EnableFilter();
            controlGrid.AddColumn(x => x.FullName).HasHeaderText(T("Full Name")).EnableFilter();
            controlGrid.AddColumn(x => x.Email).EnableFilter();
            controlGrid.AddColumn(x => x.IsLockedOut)
                .HasHeaderText(T("Is Active"))
                .EnableFilter()
                .AlignCenter()
                .HasWidth(100)
                .RenderAsStatusImage(true);

            controlGrid.AddAction(new ControlFormHtmlAction(BuildRoleDropDown)).HasParentClass(Constants.ContainerCssClassCol3);
            controlGrid.AddAction(new ControlFormHtmlAction(BuildSearchText)).HasParentClass(Constants.ContainerCssClassCol3);  

            controlGrid.AddRowAction()
                .HasText(T("Roles"))
                .HasUrl(x => Url.Action("EditRoles", new { userId = x.Id }))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog();

            controlGrid.AddRowAction()
                .HasText(T("Password"))
                .HasUrl(x => Url.Action("ChangePassword", new { userId = x.Id }))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog();

            controlGrid.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            controlGrid.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
        }

        private string BuildRoleDropDown()
        {
            var roles = roleService.GetRecords();
            var sb = new StringBuilder();

            sb.AppendFormat(T("Role Name") + "<select id=\"ddlRoleId\" name=\"RoleId\" autocomplete=\"off\" class=\"uniform form-control\" onchange=\"$('#"+TableName+"').jqGrid().trigger('reloadGrid');\">");
            sb.AppendFormat("<option value=\"0\">{0}</option>", T("All Roles"));

            foreach (var role in roles)
            {
                sb.AppendFormat("<option value=\"{1}\">{0}</option>", role.Name, role.Id);
            }

            sb.Append("</select>");

            return sb.ToString();
        }

        private string BuildSearchText()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(T("Keyword") + " <input id=\"txtSearchText\" name=\"SearchText\" class=\"form-control\" onkeypress = \"return InputEnterEvent(event, '" + TableName + "');\" onblur=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\"></input>");

            return sb.ToString();
        }

        protected override ControlGridAjaxData<User> GetRecords(ControlGridFormRequest options)
        {
            int totals; 
            var roleId = 0;
            if (Request.Form["RoleId"] != null)
            {
                roleId = Convert.ToInt32(Request.Form["RoleId"]); 
            }

            var searchText = string.Empty;
            if (!string.IsNullOrEmpty(Request.Form["SearchText"]))
            {
                searchText = Request.Form["SearchText"];
            }

            if (roleId > 0)
            {
                var records = membershipService.SearchUserByRolePaged(searchText, roleId, options.PageIndex, options.PageSize, out totals);
                return new ControlGridAjaxData<User>(records, totals);
            }
            else
            {
                var records = membershipService.SearchUserPaged(searchText, options.PageIndex, options.PageSize, out totals);
                return new ControlGridAjaxData<User>(records, totals);
            }
        }

        protected override void OnCreating(ControlFormResult<UserModel> controlForm)
        {
            controlForm.AddProperty("Password", new ControlTextAttribute
            {
                Type = ControlText.Password,
                MaxLength = 128,
                Required = true,
                LabelText = "Password",
                ContainerCssClass = "col-xs-6 col-md-6",
                ContainerRowIndex = 1
            });
            controlForm.AddProperty("ConfirmPassword", new ControlTextAttribute
            {
                Type = ControlText.Password,
                MaxLength = 128,
                Required = true,
                EqualTo = "Password",
                LabelText = "Confirm Password",
                ContainerCssClass = "col-xs-6 col-md-6",
                ContainerRowIndex = 1
            });

            var callbackUrl = Request.QueryString["callbackUrl"];
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                controlForm.AddHiddenValue("CallbackUrl", callbackUrl);
            }

            controlForm.ShowBoxHeader = false;
            controlForm.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            controlForm.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Membership"));
            WorkContext.Breadcrumbs.Add(T("Users"));
            return base.Index();
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{BaseUrl}/update")]
        public override ActionResult Update(UserModel model)
        {
            User user; 
            if (model.Id == 0)
            {
                var password = Request.Form["Password"];
                if (password.Length <= 7)
                {
                    return new AjaxResult().Alert(T("Mật khẩu phải >= 8 ký tự !"));
                }

                if (!password.IsPassword())
                {
                    return new AjaxResult().Alert(T("Mật khẩu phải nhập ký tự Hoa, Thường, Số và ký tự đặc biệt !"));
                }

                user = membershipService.CreateUserAndLocalAccount(model.UserName, model.FullName, model.PhoneNumber, model.Email, password, false);
            }
            else
            {
                user = membershipService.GetUser(model.Id);
                ConvertFromModel(model, user);
                membershipService.Update(user);
            }

            var callbackUrl = Request.Form["CallbackUrl"];
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                return new AjaxResult().Redirect(callbackUrl + "?userId=" + user.Id, true);
            }

            return OnUpdateSuccess(null);
        }

        protected override UserModel ConvertToModel(User entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(UserModel model, User entity)
        {
            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.IsLockedOut = model.IsLockedOut;
            entity.SuperUser = model.SuperUser;
            entity.FullName = model.FullName;
            entity.PhoneNumber = model.PhoneNumber;
        }

        [Url("{BaseUrl}/edit-roles/{userId}")]
        [Themed(false)]
        public virtual ActionResult EditRoles(int userId)
        {
            var user = membershipService.GetUser(userId);
            var model = new UpdateRolesModel
            {
                UserId = userId,
                Roles = membershipService.GetRolesForUser(user.Id).Select(x => x.Id).ToArray()
            };

            var retult = new ControlFormResult<UpdateRolesModel>(model)
            {
                Title = T("Update User Roles").Text,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var roles = roleService.GetRecords().ToDictionary(k => k.Id, v => v.Name);
            retult.RegisterExternalDataSource(x => x.Roles, roles);

            return retult;
        }

        [HttpPost]
        [FormButton("Save")]
        public ActionResult EditRoles(UpdateRolesModel model)
        {
            membershipService.AssignUserToRoles(model.UserId, model.Roles);
            return new AjaxResult().CloseModalDialog();
        }

        [Url("{BaseUrl}/change-password/{userId}")]
        [Themed(false)]
        public virtual ActionResult ChangePassword(int userId)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                throw new NotAuthorizedException();
            }

            var user = membershipService.GetUser(userId);

            var model = new ChangePasswordModel
            {
                UserName = user.UserName,
                Email = user.Email
            };

            var retult = new ControlFormResult<ChangePasswordModel>(model)
            {
                Title = T("Change Password").Text,
                ShowBoxHeader = false,
                IconHeader = "fa fa-lg fa-fw fa-user",
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml  
            };

            retult.ExcludeProperty(x => x.OldPassword);

            return retult;
        }

        [HttpPost]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                throw new NotAuthorizedException();
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                return new AjaxResult().Alert(T("Vui lòng xác nhập lại mật khẩu!"));
            }

            if (model.Password.Length <= 7)
            {
                return new AjaxResult().Alert(T("Mật khẩu phải >= 8 ký tự !"));
            }

            if (!model.Password.IsPassword())
            {
                return new AjaxResult().Alert(T("Mật khẩu phải nhập ký tự Hoa, Thường, Số và ký tự đặc biệt !"));
            }

            var user = membershipService.GetUser(model.UserName);
            membershipService.SetPassword(user, model.Password);

            return new AjaxResult().CloseModalDialog();
        }
    }
}