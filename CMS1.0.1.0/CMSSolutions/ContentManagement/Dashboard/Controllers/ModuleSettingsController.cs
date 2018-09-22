using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CMSSolutions.Configuration;
using CMSSolutions.ContentManagement.Dashboard.Models;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Descriptor;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.IO;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;
using Ionic.Zip;

namespace CMSSolutions.ContentManagement.Dashboard.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Dashboard)]
    public class ModuleSettingsController : BaseController
    {
        public ModuleSettingsController(IWorkContextAccessor workContextAccessor)
            : base(workContextAccessor)
        {

        }

        [Url("{DashboardBaseUrl}/module-settings")]
        public ActionResult Index()
        {
            if (!CheckPermission(DashboardPermissions.ManageModuleSettings))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Extension Modules"));

            ViewBag.Title = T("Extension Modules");

            var result = new ControlGridFormResult<ShellFeature>
            {
                Title = T("Extension Modules").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetData,
                IsAjaxSupported = true,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml,
                ActionsColumnWidth = 150
            };

            result.AddColumn(x => x.Name);

            result.AddAction()
                .HasText(T("Create"))
                .HasUrl(Url.Action("Create", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary)
                .HasBoxButton(false)
                .HasCssClass(Constants.RowLeft)
                .ShowModalDialog();

            result.AddAction()
                .HasText(T("Installer Module"))
                .HasUrl(Url.Action("InstallerModuleFileZip"))
                .HasButtonStyle(ButtonStyle.Success)
                .HasBoxButton(false)
                .HasRow(false)
                .HasCssClass(Constants.RowLeft);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { name = x.Name })))
                .HasButtonStyle(ButtonStyle.Default)
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog();

            result.AddRowAction(true)
               .HasText(T("Delete"))
               .HasName("Delete")
               .HasUrl(x => Url.Action("Delete", RouteData.Values.Merge(new { name = x.Name })))
               .HasButtonStyle(ButtonStyle.Danger)
               .HasButtonSize(ButtonSize.ExtraSmall)
               .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            return result;
        }

        private ControlGridAjaxData<ShellFeature> GetData(ControlGridFormRequest options)
        {
            var service = WorkContext.Resolve<IShellDescriptorManager>();
            var setting = service.GetShellDescriptor();
            var records = setting.Features;
            return new ControlGridAjaxData<ShellFeature>(records);
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/module-settings/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(DashboardPermissions.ManageModuleSettings))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Extension Modules"), Url.Action("Index", new { area = Constants.Areas.Dashboard }));
            WorkContext.Breadcrumbs.Add(T("Module Informations"));

            var model = new ShellFeatureModel();
            var result = new ControlFormResult<ShellFeatureModel>(model)
            {
                Title = T("Module Informations").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-page",
                ShowSubmitButton = true,
                ShowCloseButton = true,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/module-settings/edit")]
        public ActionResult Edit(string name)
        {
            var model = new ShellFeatureModel {Name = name};
            var result = new ControlFormResult<ShellFeatureModel>(model)
                {
                    Title = T("Module Informations"),
                    UpdateActionName = "Update",
                    FormMethod = FormMethod.Post,
                    SubmitButtonText = T("Save"),
                    ShowCancelButton = true,
                    ShowBoxHeader = false,
                    FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                    FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
                };

            return result;
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/module-settings/update")]
        public ActionResult Update(ShellFeatureModel model)
        {
            var service = WorkContext.Resolve<IShellDescriptorManager>();
            var setting = service.GetShellDescriptor();
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var sections = ((CMSConfigurationSection)configuration.GetSection("solutions")).Modules;

            var element = new ModuleProviderConfigurationElement
            {
                Id = model.Name,
                Name = model.Name,
                Category = model.Category
            };

            if (setting.Features.Any(item => item.Name == model.Name))
            {
                sections.Remove(element.Id);
                sections.Add(element);
                configuration.Save(ConfigurationSaveMode.Modified);

                return new AjaxResult().Alert(T("Existing the name."));
            }

            setting.Features.Add(new ShellFeature {Name = model.Name});
            service.UpdateShellDescriptor(0, setting.Features);

            var serviceFile = WorkContext.Resolve<IShellDescriptorCache>();
            var st = WorkContext.Resolve<ShellSettings>();
            serviceFile.Store(st.Name, setting);

            sections.Remove(element.Id);
            sections.Add(element); 
            configuration.Save(ConfigurationSaveMode.Modified);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Save success."));
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/module-settings/delete")]
        public ActionResult Delete(string name)
        {
            var service = WorkContext.Resolve<IShellDescriptorManager>();
            var setting = service.GetShellDescriptor();
            int index = -1;
            for (int i = 0; i < setting.Features.Count; i++)
            {
                if (setting.Features[i].Name == name)
                {
                    index = i;
                    break;
                }
            } 
            if (index > -1)
            {
               setting.Features.RemoveAt(index); 
            }

            service.UpdateShellDescriptor(0, setting.Features);
            var serviceFile = WorkContext.Resolve<IShellDescriptorCache>();
            var st = WorkContext.Resolve<ShellSettings>();
            serviceFile.Store(st.Name, setting);

            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var sections = ((CMSConfigurationSection)configuration.GetSection("solutions")).Modules;
            sections.Remove(name);
            configuration.Save(ConfigurationSaveMode.Modified);

            WorkContext.Resolve<IPermissionService>().DeleteByName(name);

            return Redirect(Url.Action("Index", "ModuleSettings"));
        }

        [Url("{DashboardBaseUrl}/module-settings/installer-module-zip")]
        public ActionResult InstallerModuleFileZip()
        {
            var result = new ControlFormResult
            {
                Title = T("Installer module").Text,
                FormActionUrl =Url.Action("SaveModule"),
                CssClass = "form-edit-page",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml,
                FormMethod = FormMethod.Post,
                IsAjaxSupported = false
            };

            
            result.AddProperty("AssemblyName", new ControlTextAttribute
            {
                Required = true,
                MaxLength = 255,
                LabelText = "Assembly Name",
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 0
            }, string.Empty);

            result.AddProperty("UploadedFile", new ControlFileUploadAttribute
            {
                EnableFineUploader = false,
                LabelText = T("Hãy nén module định dạng .zip"),
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 2,
                AllowedExtensions = "*.zip",
            }, string.Empty);

            return result;
        }

        [HttpPost]
        [Url("{DashboardBaseUrl}/module-settings/upload-module")]
        public ActionResult SaveModule()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if (file != null)
                {
                    #region Save module
                    var assemblyName = Request.Form["AssemblyName"];
                    var category = "Content Management";

                    var service = WorkContext.Resolve<IShellDescriptorManager>();
                    var setting = service.GetShellDescriptor();
                    var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                    var sections = ((CMSConfigurationSection)configuration.GetSection("solutions")).Modules;
                    var element = new ModuleProviderConfigurationElement
                    {
                        Id = assemblyName,
                        Name = assemblyName,
                        Category = category
                    };

                    if (setting.Features.Any(item => item.Name == assemblyName))
                    {
                        sections.Remove(element.Id);
                    }
                    else
                    {
                        var serviceFile = WorkContext.Resolve<IShellDescriptorCache>();
                        var st = WorkContext.Resolve<ShellSettings>();
                        serviceFile.Store(st.Name, setting);
                    }
                    setting.Features.Add(new ShellFeature { Name = assemblyName });
                    service.UpdateShellDescriptor(0, setting.Features);

                    sections.Add(element);
                    configuration.Save(ConfigurationSaveMode.Modified);
                    #endregion

                    #region Upload Module
                    var folderSource = Server.MapPath("~/Media/Temp/");
                    var fullUrl = Path.Combine(folderSource, file.FileName);
                    file.SaveAs(fullUrl);

                    var folderRoot = Path.GetFileNameWithoutExtension(fullUrl);
                    string lastFolderName = folderSource + folderRoot;
                    Directory.CreateDirectory(lastFolderName);
                    using (ZipFile zip = ZipFile.Read(fullUrl))
                    {
                        zip.ExtractAll(lastFolderName, ExtractExistingFileAction.OverwriteSilently);
                    }

                    foreach (var path in Directory.GetDirectories(lastFolderName))
                    {
                        var folderName = new DirectoryInfo(path).Name;
                        if (folderName == "bin")
                        {
                            string[] files = Directory.GetFiles(path);
                            foreach (var filePath in files)
                            {
                                System.IO.File.Copy(filePath, Server.MapPath("~/bin/" + Path.GetFileName(filePath)), true);
                            }
                        }
                        else
                        {
                            var sourceFolder = lastFolderName + "\\" + folderName;
                            var destinationFolder = Server.MapPath(string.Format("~/{0}", folderName));
                            CopyDirectories(sourceFolder, destinationFolder);
                        }
                    }

                    if (System.IO.File.Exists(fullUrl))
                    {
                        System.IO.File.Delete(fullUrl);
                    }

                    if (Directory.Exists(lastFolderName))
                    {
                        Directory.Delete(lastFolderName, true);
                    }

                    #endregion
                }
                else
                {
                    var model = new MessageErrorModel
                    {
                        ExceptionError = new Exception(T("Cannot find file upload.")),
                        TitleForm = T("Messages"),
                        Messages = T("Cannot find file upload."),
                        GoBackText = T("Back")
                    };

                    return View("ErrorPage", model);
                }
            }
            catch (Exception ex)
            {
                var model = new MessageErrorModel
                {
                    ExceptionError = ex,
                    TitleForm = T("Messages"),
                    Messages = ex.Message,
                    GoBackText = T("Back")
                };

                return View("ErrorPage", model);
            }

            return Redirect(Url.Action("Index"));
        }

        private void CopyDirectories(string sourcePath, string destinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                System.IO.File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }
    }
}
