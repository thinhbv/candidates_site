using System;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Logging;
using CMSSolutions.Tasks.Domain;
using CMSSolutions.Tasks.Models;
using CMSSolutions.Tasks.Services;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Tasks.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.ScheduledTasks)]
    public class ScheduleTaskController : BaseController
    {
        private readonly IScheduleTaskService scheduleTaskService;
        private readonly IScheduleTaskManager scheduleTaskManager;

        public ScheduleTaskController(IWorkContextAccessor workContextAccessor, IScheduleTaskService scheduleTaskService, IScheduleTaskManager scheduleTaskManager)
            : base(workContextAccessor)
        {
            this.scheduleTaskService = scheduleTaskService;
            this.scheduleTaskManager = scheduleTaskManager;
        }

        [Url("{DashboardBaseUrl}/tasks")]
        public ActionResult Index()
        {
            if (!CheckPermission(TasksPermissions.ManageScheduleTasks))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Scheduled Tasks"));

            var result = new ControlGridFormResult<ScheduleTask>
            {
                Title = T("Schedule Tasks").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetScheduleTasks,
                ClientId = "tblScheduleTasks",
                ActionsColumnWidth = 200,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name);
            result.AddColumn(x => x.LastStartUtc).HasHeaderText(T("Last Start UTC"));
            result.AddColumn(x => x.LastSuccessUtc).HasHeaderText(T("Last Success UTC"));
            result.AddColumn(x => x.Enabled).AlignCenter().RenderAsStatusImage();

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create"))
                .HasButtonStyle(ButtonStyle.Primary)
                .HasBoxButton(false)
                .HasCssClass(Constants.RowLeft)
                .ShowModalDialog();

            result.AddAction(true)
                .HasText(T("Refresh"))
                .HasIconCssClass("cx-icon cx-icon-refresh")
                .OnClientClick("$('#tblScheduleTasks').jqGrid().trigger('reloadGrid'); return false;")
                .HasButtonStyle(ButtonStyle.Info)
                .HasBoxButton(false)
                .HasRow(false)
                .HasCssClass(Constants.RowLeft);

            result.AddRowAction(true)
                .HasText(T("Run Now"))
                .HasName("RunNow")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Success)
                .EnableWhen(x => x.Enabled);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog();

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord));

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            return result;
        }

        private ControlGridAjaxData<ScheduleTask> GetScheduleTasks(ControlGridFormRequest arg)
        {
            var tasks = scheduleTaskService.GetAllTasks();
            return new ControlGridAjaxData<ScheduleTask>(tasks);
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/tasks/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(TasksPermissions.ManageScheduleTasks))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Scheduled Tasks"), Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var result = new ControlFormResult<ScheduleTaskModel>(new ScheduleTaskModel { Enabled = true })
            {
                Title = T("Create Task").Text,
                UpdateActionName = "Update"
            };
            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/tasks/edit/{id}")]
        public ActionResult Edit(Guid id)
        {
            if (!CheckPermission(TasksPermissions.ManageScheduleTasks))
            {
                return new HttpUnauthorizedResult();
            }

            var model = scheduleTaskService.GetTaskById(id);

            WorkContext.Breadcrumbs.Add(T("Scheduled Tasks"), Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(model.Name);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            return new ControlFormResult<ScheduleTaskModel>(model)
            {
                Title = T("Edit Task").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };
        }

        [FormButton("Save")]
        [HttpPost, ValidateInput(false)]
        [Url("{DashboardBaseUrl}/tasks/update", Priority = 5)]
        public ActionResult Update(ScheduleTaskModel model)
        {
            if (!CheckPermission(TasksPermissions.ManageScheduleTasks))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException(ModelState);
            }

            ScheduleTask scheduleTask;
            if (model.Id == Guid.Empty)
            {
                scheduleTask = new ScheduleTask
                {
                    Name = model.Name,
                    Type = model.Type,
                    CronExpression = model.CronExpression,
                    Enabled = model.Enabled
                };
                scheduleTaskService.InsertTask(scheduleTask);
            }
            else
            {
                scheduleTask = scheduleTaskService.GetTaskById(model.Id);
                scheduleTask.Name = model.Name;
                scheduleTask.Type = model.Type;
                scheduleTask.CronExpression = model.CronExpression;
                scheduleTask.Enabled = model.Enabled;
                scheduleTaskService.UpdateTask(scheduleTask);
            }

            scheduleTaskManager.ScheduleJob(scheduleTask);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(Guid id)
        {
            if (!CheckPermission(TasksPermissions.ManageScheduleTasks))
            {
                return new HttpUnauthorizedResult();
            }

            var task = scheduleTaskService.GetTaskById(id);
            scheduleTaskService.DeleteTask(task);

            task.Enabled = false;
            scheduleTaskManager.ScheduleJob(task);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [FormButton("RunNow")]
        [HttpPost, ActionName("Update")]
        public ActionResult RunNow(Guid id)
        {
            if (!CheckPermission(TasksPermissions.ManageScheduleTasks))
            {
                return new HttpUnauthorizedResult();
            }

            Logger.Info("Start execute RunNow for scheduler task.");
            scheduleTaskManager.RunNow(id);
            return new AjaxResult().Alert("The scheduler task was execute done.").NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [FormButton("Refresh")]
        [HttpPost, ActionName("Update")]
        public ActionResult Refresh()
        {
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }
    }
}