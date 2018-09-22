using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
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
    [Url(BaseUrl = "{DashboardBaseUrl}/roles")]
    public class RoleController : BaseControlController<int, Role, RoleModel>
    {
        private readonly IRoleService roleService;
        private readonly IPermissionService permissionService;
        private readonly IMembershipService membershipService;
        private readonly Lazy<IEnumerable<IPermissionProvider>> permissionProviders;

        public RoleController(IWorkContextAccessor workContextAccessor,
            IRoleService roleService,
            IPermissionService permissionService,
            Lazy<IEnumerable<IPermissionProvider>> permissionProviders, IMembershipService membershipService)
            : base(workContextAccessor, roleService)
        {
            this.roleService = roleService;
            this.permissionService = permissionService;
            this.permissionProviders = permissionProviders;
            this.membershipService = membershipService;
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override void OnViewIndex(ControlGridFormResult<Role> controlGrid)
        {
            controlGrid.ActionsColumnWidth = 200;
			controlGrid.AddColumn(x => x.Id, T("ID")).Width = 60;
            controlGrid.AddColumn(x => x.Name, T("Role Name"));

            controlGrid.AddRowAction()
                .HasText(T("Permissions"))
                .HasUrl(x => Url.Action("EditPermissions", new { roleId = x.Id }))
                .HasButtonSize(ButtonSize.ExtraSmall);
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Membership"));
            WorkContext.Breadcrumbs.Add(T("Roles"));
            return base.Index();
        }

        [Url("{BaseUrl}/edit-permissions/{roleId}")]
        public ActionResult EditPermissions(int roleId)
        {
            var permissions = permissionProviders.Value.SelectMany(x => x.GetPermissions()).ToList();
            var allPermissions = permissionService.GetRecords();

            foreach (var permission in permissions)
            {
                if (allPermissions.All(x => x.Name != permission.Name))
                {
                    var newPermission = new Domain.Permission
                    {
                        Name = permission.Name,
                        Category = string.IsNullOrEmpty(permission.Category) ? "Miscellaneous" : permission.Category,
                        Description = permission.Description
                    };
                    permissionService.Insert(newPermission);
                    allPermissions.Add(newPermission);
                }
            }

            var role = roleService.GetById(roleId);
            var rolePermissions = membershipService.GetPermissionsForRole(role.Id);

            var model = new UpdatePermissionsModel
            {
                RoleId = roleId,
                Permissions = rolePermissions.Select(x => x.Id).ToArray()
            };

            var retult = new ControlFormResult<UpdatePermissionsModel>(model)
            {
                Title = T("Edit Role Permissions") ,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml 
            };

            var selectListItems = new List<ExtendedSelectListItem>();
            foreach (var categoryGroup in allPermissions.OrderBy(x => x.Category, new PermissionComparer(StringComparer.InvariantCultureIgnoreCase)).GroupBy(x => x.Category))
            {
                selectListItems.AddRange(categoryGroup.OrderBy(x => x.Description)
                    .Select(permission => new ExtendedSelectListItem
                    {
                        Category = permission.Category,
                        Text = permission.Description,
                        Value = permission.Id.ToString()
                    }));
            }

            retult.RegisterExternalDataSource(x => x.Permissions, selectListItems);
            return retult;
        }

        private class PermissionComparer : IComparer<string>
        {
            private readonly IComparer<string> baseComparer;

            public PermissionComparer(IComparer<string> baseComparer)
            {
                this.baseComparer = baseComparer;
            }

            public int Compare(string x, string y)
            {
                var value = String.Compare(x, y, StringComparison.Ordinal);

                if (value == 0)
                {
                    return 0;
                }

                if (baseComparer.Compare(x, "System") == 0)
                {
                    return -1;
                }

                if (baseComparer.Compare(y, "System") == 0)
                {
                    return 1;
                }

                return value;
            }
        }

        [HttpPost]
        [FormButton("Save")]
        public ActionResult EditPermissions(UpdatePermissionsModel model)
        {
            membershipService.AssignPermissionsToRole(model.RoleId, model.Permissions);

            return new AjaxResult().Alert(T("Cập nhật phân quyền thành công!"));
        }

        protected override RoleModel ConvertToModel(Role entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(RoleModel model, Role entity)
        {
            entity.Name = model.Name;
        }
    }
}