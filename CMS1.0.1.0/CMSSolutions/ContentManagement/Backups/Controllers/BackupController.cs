using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Backups.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Backups)]
    [Url(BaseUrl = "{DashboardBaseUrl}/backups")]
    public class BackupController : BaseController
    {
        private readonly IBackupProvider backupProvider;
        private readonly IBackupStorageProvider backupStorageProvider;

        public BackupController(
            IWorkContextAccessor workContextAccessor,
            IBackupProvider backupProvider = null,
            IBackupStorageProvider backupStorageProvider = null)
            : base(workContextAccessor)
        {
            this.backupProvider = backupProvider;
            this.backupStorageProvider = backupStorageProvider;
        }

        [Url("{BaseUrl}")]
        public ActionResult Index()
        {
            if (!CheckPermission(BackupsPermissions.ManageBackups))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Backups"));

            if (backupProvider == null)
            {
                return new ControlContentResult("The current data provider does not support backup.")
                {
                    Title = "Backup/Restore Database"
                };
            }

            if (backupStorageProvider == null)
            {
                return new ControlContentResult("The current does not have any backup storage provider.")
                {
                    Title = "Backup/Restore Database"
                };
            }

            return null;
        }
    }
}