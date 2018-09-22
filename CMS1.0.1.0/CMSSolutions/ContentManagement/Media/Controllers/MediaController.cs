using System.IO;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Media.Connectors;
using CMSSolutions.ContentManagement.Media.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.FileSystems.Media;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Media.Controllers
{
    [Authorize]
    [Feature(Constants.Areas.Media)]
    [Themed(IsDashboard = true)]
    public class MediaController : BaseController
    {
        private readonly IStorageProvider storageProvider;
        private readonly IMediaService mediaService;
        private readonly IMimeTypeProvider mimeTypeProvider;

        public MediaController(IWorkContextAccessor workContextAccessor, IMediaService mediaService, IMimeTypeProvider mimeTypeProvider, IStorageProvider storageProvider)
            : base(workContextAccessor)
        {
            this.mediaService = mediaService;
            this.mimeTypeProvider = mimeTypeProvider;
            this.storageProvider = storageProvider;
        }

        [Url("{DashboardBaseUrl}/media-library")]
        public ActionResult Index()
        {
            if (!CheckPermission(MediaPermissions.ManageMedia))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Media"));

            ViewBag.Title = T("Manage Media");

            var sb = new StringBuilder();
            sb.Append("<article>");
            sb.Append("<div class='jarviswidget' data-widget-editbutton='false' data-widget-deletebutton='false'>");
            sb.Append("<header>");
            sb.Append("<span class='widget-icon'>");
            sb.Append("<i class='fa fa-lg fa-fw fa-picture-o'></i>");
            sb.Append("</span>");
            sb.Append("<h2>Gallery</h2>");
            sb.Append("</header>");
            sb.Append("<div>");
            sb.Append("<div class=\"widget-body\">");
            sb.Append("<div id=\"elfinder\"></div>");
            sb.Append("<div>");
            sb.Append("<div>");
            sb.Append("<div>");
            sb.Append("</article>");  

            sb.AppendFormat(@"<script type=""text/javascript"">
	            $(document).ready(function() {{
	                var myCommands = elFinder.prototype._options.commands;
	                var disabled = ['extract', 'archive', 'resize', 'help', 'select'];
	                $.each(disabled, function (i, cmd) {{
	                    var idx;
	                    (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
	                }});
	                var options = {{
	                    url: '{0}',
	                    requestType: 'post',
	                    commands: myCommands,
	                    lang: 'en',
	                    ui: ['toolbar'],
	                    rememberLastDir: false,
	                    height: 500,
	                    resizable: false,
                        defaultView: 'list',
	                    uiOptions: {{
	                        toolbar: [
                                ['home', 'up'],
                                ['mkdir', 'upload'],
                                ['info'],
                                ['quicklook'],
                                ['cut', 'paste'],
                                ['rm'],
                                ['view', 'sort']
	                        ],
	                        tree: {{
	                            openRootOnLoad: false,
	                        }},
	                        cwd: {{
	                            oldSchool: true
	                        }}
	                    }},
	                    contextmenu: {{
	                        navbar: ['open', '|', 'cut', 'paste', '|', 'rm', '|', 'info'],

	                        cwd: ['reload', 'back', '|', 'upload', 'paste', '|', 'info'],

	                        files: [
                                'getfile', '|', 'open', 'quicklook', '|', 'download', '|', 'cut', 'paste', '|',
                                'rm', '|', 'edit', 'rename', 'resize', '|', 'archive', 'extract', '|', 'info'
	                        ]
	                    }},
	                    handlers: {{
	                        upload: function (event, instance) {{
	                            var uploadedFiles = event.data.added;
	                            var archives = ['application/x-gzip', 'application/x-tar', 'application/x-bzip2'];
	                            for (i in uploadedFiles) {{
	                                var file = uploadedFiles[i];
	                                if (jQuery.inArray(file.mime, archives) >= 0) {{
	                                    instance.exec('extract', file.hash);
	                                }}
	                            }}
	                        }},
	                    }},
	                    dialog: {{ width: 900, modal: true, title: ""Files"" }}, // open in dialog window
	                    commandsOptions: {{
	                        getfile: {{
	                            onlyURL: true,
	                            multiple: false,
	                            folders: false,
	                            oncomplete: ''
	                        }},
	                    }}
	                }};
	                $('#elfinder').elfinder(options).elfinder('instance');
	            }});
            </script>", @Url.Action("Connector", "Media", RouteData.Values));

            var result = new ControlContentResult(sb.ToString())
                             {
                                 AdditionResources = () => new[] {ResourceType.JQueryUI, ResourceType.ElFinder}
                             };

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/media-library/browse")]
        public ActionResult Browse()
        {
            WorkContext.Breadcrumbs.Add(T("Media"), Url.Action("Index", new { area = Constants.Areas.Media }));
            WorkContext.Breadcrumbs.Add(T("Browse"));

            var sb = new StringBuilder();
            sb.Append("<div class=\"box\">");
            sb.Append("<div class=\"box-header\"><i class=\"cx-icon cx-icon-close\" onclick=\"window.parent.fancyboxResult = null; parent.jQuery.fancybox.close();\"></i><h2 style=\"cursor:pointer\">File Browse</h2></div>");
            sb.Append("<div class=\"box-content nopadding\" style=\"border-bottom: none;\">");
            sb.Append("<div id=\"fileContainer\" style=\"height: 190px; padding-left: 5px;\"></div>");
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("<script type=\"text/javascript\">");
            sb.Append("$(document).ready( function() {");
            sb.AppendFormat("$('#fileContainer').fileTree({{ root: '/', script: '{0}', multiFolder: false }}, function(file){{ window.parent.fancyboxResult = file; parent.jQuery.fancybox.close(); }});",
                Url.Action("FileTreeConnector", "Media", new { area = Constants.Areas.Media }));
            sb.Append("});");
            sb.Append("</script>");

            var result = new ControlContentResult(sb.ToString())
            {
                AdditionResources = () => new[] { ResourceType.JQueryFileTree }
            };

            return result;
        }

        [HttpPost]
        [Url("{DashboardBaseUrl}/media-library/connector")]
        public ActionResult Connector()
        {
            var driver = new MediaSystemDriver(mediaService, mimeTypeProvider, storageProvider);
            var connector = new Connector(driver);
            return connector.Process(HttpContext.Request);
        }

        #region File Tree Connector

        [HttpPost]
        [Url("{DashboardBaseUrl}/media-library/file-tree-connector")]
        public ActionResult FileTreeConnector(string dir)
        {
            var path = string.IsNullOrEmpty(dir) ? null : Server.UrlDecode(dir.Trim('/'));

            var folders = mediaService.GetMediaFolders(path);
            var files = mediaService.GetMediaFiles(path);

            var sb = new StringBuilder();

            sb.Append("<ul class=\"jqueryFileTree\" style=\"display: none;\">");

            foreach (var folder in folders)
            {
                if (string.IsNullOrEmpty(dir) && folder.Name == "UploadFiles")
                {
                    continue;
                }
                sb.AppendFormat("<li class=\"directory collapsed\"><a href=\"#\" rel=\"{0}/\">{1}</a></li>", folder.MediaPath.Replace('\\', '/'), folder.Name);
            }

            foreach (var file in files)
            {
                sb.AppendFormat("<li class=\"file ext_{0}\"><a href=\"#\" rel=\"{1}\">{2}</a></li>", Path.GetExtension(file.Name), file.MediaPath, file.Name);
            }

            sb.Append("</ul>");

            return Content(sb.ToString());
        }

        [HttpPost]
        [Url("{DashboardBaseUrl}/media-library/file-tree-connector/new-folder")]
        public ActionResult NewFolder(string folder)
        {
            var path = string.IsNullOrEmpty(folder) ? null : folder.Trim('/');
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var parentPath = Path.GetDirectoryName(path);
            var folderName = Path.GetFileName(path);
            mediaService.CreateFolder(parentPath, folderName);

            return Content(folder + "/");
        }

        #endregion File Tree Connector
    }
}