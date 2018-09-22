namespace CMSSolutions.Websites.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using CMSSolutions;
    using CMSSolutions.Web.Mvc;
    using CMSSolutions.Web.Themes;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Websites.Models;
    using CMSSolutions.Websites.Services;
    using CMSSolutions.Web;
    using CMSSolutions.Web.UI.Navigation;
    using CMSSolutions.Web.Routing;
	using CMSSolutions.Web.Security.Services;
    
    
    [Authorize()]
    [Themed(IsDashboard=true)]
	public class ScheduleInterviewController : BaseAdminController
    {
        
        private readonly IScheduleInterviewService service;
        
        public ScheduleInterviewController(IWorkContextAccessor workContextAccessor, IScheduleInterviewService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblScheduleInterview";
        }
        
        [Url("admin/scheduleinterviews")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Schedule Interviews"), Url = "#" });

			ViewBag.Title = T("Schedule Interviews");

            return View();
        }
        
        private ControlGridAjaxData<ScheduleInterview> GetModule_ScheduleInterview(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<ScheduleInterview>(items, totals);
            return result;
        }
        
        [Url("admin/scheduleinterviews/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("ScheduleInterview"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Schedule Interviews"), Url = Url.Action("Index") });
            var model = new ScheduleInterviewModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
            var result = new ControlFormResult<ScheduleInterviewModel>(model);
            result.Title = this.T("Edit ScheduleInterview");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;
            result.AddAction().HasText(this.T("Clear")).HasUrl(this.Url.Action("Edit", RouteData.Values.Merge(new { id = 0 }))).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
            return result;
        }
        
        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/scheduleinterviews/update")]
        public ActionResult Update(ScheduleInterviewModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            ScheduleInterview item;
            if (model.Id == 0)
            {
                item = new ScheduleInterview();
            }
            else
            {
                item = service.GetById(model.Id);
            }
            item.pos_id = model.pos_id;
            item.candidate_id = model.candidate_id;
            item.interview_date = model.interview_date;
            item.created_date = model.created_date;
            item.updated_date = model.updated_date;
            service.Save(item);
			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var model = service.GetById(id);
            service.Delete(model);
			return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

		[HttpPost, ValidateInput(false)]
		[Url("admin/save-schedule")]
		public ActionResult Save(int[] interviewid, int candidateid, int levelid, string startdate, string enddate, int langid, string title)
		{
			var service = WorkContext.Resolve<IQuestionsService>();
			var serviceSchedule = WorkContext.Resolve<IScheduleInterviewService>();
			var memberSV = WorkContext.Resolve<IMembershipService>();
			var serviceCandidate = WorkContext.Resolve<ICandidatesService>();
			var list = service.GetRecords(x => x.language_id == langid && x.level_id == levelid);
			string listQuestionId = string.Empty;

			for (int i = 0; i < list.Count; i++)
			{
				listQuestionId = listQuestionId + list[i] + ",";
			}
			if (!string.IsNullOrEmpty(listQuestionId))
			{
				listQuestionId = listQuestionId.Substring(0, listQuestionId.Length - 1);
			}
			startdate = startdate.Substring(0, startdate.IndexOf(" GMT"));
			enddate = enddate.Substring(0, enddate.IndexOf(" GMT"));
			ScheduleInterview item = new ScheduleInterview();
			item.name = title;
			item.pos_id = levelid;
			item.candidate_id = candidateid;
			item.interviewers_id = string.Join(",", interviewid);
			item.interview_date = DateTime.Now;
			item.start_date = DateTime.Parse(startdate);
			item.end_date = DateTime.Parse(enddate);
			item.list_questions = listQuestionId;
			item.created_date = DateTime.Now;
			item.updated_date = DateTime.Now;
			serviceSchedule.Insert(item);
			var model = new DataViewModel();
			model.Status = true;
			MailTemplates mailTemp = new MailTemplates();
			mailTemp.name = "【Lịch phỏng vấn】" + title;
			mailTemp.url_template = "/Media/Default/UploadFiles/JobTemplate.html";
			var body = System.IO.File.ReadAllText(Server.MapPath(string.Format("~{0}", mailTemp.url_template)));
			var candidate = serviceCandidate.GetById(candidateid);
			var reviewer = memberSV.GetRecords(x => interviewid.Contains(x.Id));
			string mailTo = string.Empty;
			if (candidate != null)
			{
				 mailTo = candidate.mail_address;
			}
			for (int i = 0; i < reviewer.Count; i++)
			{
				mailTo = "," + mailTo + reviewer[i].Email; 
			}
			if (!string.IsNullOrEmpty(mailTo))
			{
				SendEmail(mailTemp.name, body, mailTo, GetListToBCC());
			}
			return Json(model.Status);
		}
    }
}
