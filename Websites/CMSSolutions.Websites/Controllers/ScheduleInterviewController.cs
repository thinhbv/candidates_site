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
	using System.Text;
    
    
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
			StringBuilder strXML = new StringBuilder();
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Schedule Interviews"), Url = "#" });
			var serviceSchedule = WorkContext.Resolve<IScheduleInterviewService>();
			var list = serviceSchedule.GetRecords(x => x.end_date > DateTime.Now);
			strXML.AppendLine("{");
			strXML.AppendLine("  \"data\": [");
			for (int i = 0; i < list.Count; i++)
			{
				string scheduleXML = string.Empty;
				int[] arr = ConvertStringToArrayInt(list[i].list_questions);
				var service = WorkContext.Resolve<IQuestionsService>();
				StringBuilder strQuest = new StringBuilder();
				if (arr != null)
				{
					var model = service.GetRecords(x => arr.Contains(x.Id));

					for (int j = 0; j < model.Count; j++)
					{
						if (j == model.Count - 1)
						{
							strQuest.Append(model[j].content);
						}
						else
						{
							strQuest.Append(model[j].content + "\\n");
						}
					}
				}
				if (i == list.Count - 1)
				{
					scheduleXML = "      {\"id\":\"" + list[i].Id.ToString() + "\"," + "\"start_date\":\"" + list[i].start_date.ToString("yyyy:MM:dd HH:mm:ss") + "\"," + "\"end_date\":\"" + list[i].end_date.ToString("yyyy:MM:dd HH:mm:ss") + "\"," + "\"text\":\"" + list[i].name + "\"," + "\"level_id\":\"" + list[i].pos_id.ToString() + "\"," + "\"lang_id\":\"" + list[i].pos_id.ToString() + "\"," + "\"can_id\":\"" + list[i].candidate_id.ToString() + "\"," + "\"user_id\":\"" + list[i].interviewers_id + "\"," + "\"quest_id\":\"" + strQuest.ToString() + "\"}";
				}
				else
				{
					scheduleXML = "      {\"id\":\"" + list[i].Id.ToString() + "\"," + "\"start_date\":\"" + list[i].start_date.ToString("yyyy:MM:dd HH:mm:ss") + "\"," + "\"end_date\":\"" + list[i].end_date.ToString("yyyy:MM:dd HH:mm:ss") + "\"," + "\"text\":\"" + list[i].name + "\"," + "\"level_id\":\"" + list[i].pos_id.ToString() + "\"," + "\"lang_id\":\"" + list[i].pos_id.ToString() + "\"," + "\"can_id\":\"" + list[i].candidate_id.ToString() + "\"," + "\"user_id\":\"" + list[i].interviewers_id + "\"," + "\"quest_id\":\"" + strQuest.ToString() + "\"},";
				}
				strXML.AppendLine(scheduleXML);
			}
			strXML.AppendLine("    ]");
			strXML.AppendLine("}");
			System.IO.File.WriteAllBytes(Server.MapPath("/") + "/Media/common/events.json", new byte[0]);
			System.IO.File.WriteAllText(Server.MapPath("/") + "/Media/common/events.json", strXML.ToString(), System.Text.Encoding.UTF8);
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
		public ActionResult Save(long id, string interviewid, int candidateid, int levelid, string startdate, string enddate, string langid, string title)
		{
			var service = WorkContext.Resolve<IQuestionsService>();
			var serviceSchedule = WorkContext.Resolve<IScheduleInterviewService>();
			var memberSV = WorkContext.Resolve<IMembershipService>();
			var serviceCandidate = WorkContext.Resolve<ICandidatesService>();
			int[] arrLang = ConvertStringToArrayInt(langid);
			string listQuestionId = string.Empty;
			if (arrLang != null)
			{
				string tmp = levelid.ToString();
				var list = service.GetRecords(x => arrLang.Contains(x.language_id) && x.types.Contains(tmp));
				for (int i = 0; i < list.Count; i++)
				{
					listQuestionId = listQuestionId + list[i].Id.ToString() + ",";
				}
				if (!string.IsNullOrEmpty(listQuestionId))
				{
					listQuestionId = listQuestionId.Substring(0, listQuestionId.Length - 1);
				}
			}
			int[] arr = ConvertStringToArrayInt(interviewid);

			startdate = startdate.Substring(0, startdate.IndexOf(" GMT"));
			enddate = enddate.Substring(0, enddate.IndexOf(" GMT"));
			ScheduleInterview item = new ScheduleInterview();
			item = serviceSchedule.GetById(id);
			if (item == null)
			{
				item = new ScheduleInterview();
			}
			item.name = title;
			item.pos_id = levelid;
			item.lang_id = langid;
			item.candidate_id = candidateid;
			item.interviewers_id = interviewid;
			item.interview_date = DateTime.Now;
			item.start_date = DateTime.Parse(startdate);
			item.end_date = DateTime.Parse(enddate);
			item.list_questions = listQuestionId;
			item.created_date = DateTime.Now;
			item.updated_date = DateTime.Now;
			serviceSchedule.Save(item);
			MailTemplates mailTemp = new MailTemplates();
			mailTemp.name = "【Lịch phỏng vấn】" + title;
			mailTemp.url_template = "/Media/Default/UploadFiles/JobTemplate.html";
			var body = System.IO.File.ReadAllText(Server.MapPath(string.Format("~{0}", mailTemp.url_template)));
			var candidate = serviceCandidate.GetById(candidateid);

			string mailTo = string.Empty;
			if (candidate != null)
			{
				 mailTo = candidate.mail_address;
			}
			if (arr != null)
			{
				var reviewer = memberSV.GetRecords(x => arr.Contains(x.Id));

				for (int i = 0; i < reviewer.Count; i++)
				{
					mailTo = "," + mailTo + reviewer[i].Email;
				}
			}
			if (!string.IsNullOrEmpty(mailTo))
			{
				if (mailTo.StartsWith(","))
				{
					mailTo = mailTo.Substring(1);
				}
				SendEmail(mailTemp.name, body, mailTo, GetListToBCC());
			}
			var model = new DataViewModel();
			model.Status = true;
			return Json(model.Status);
		}

		[HttpPost, ValidateInput(false)]
		[Url("admin/delete-schedule")]
		public ActionResult Deleted(int scheduleid)
		{
			var serviceSchedule = WorkContext.Resolve<IScheduleInterviewService>();
			string listQuestionId = string.Empty;
			var item = serviceSchedule.GetById(scheduleid);
			serviceSchedule.Delete(item);
			var model = new DataViewModel();
			model.Status = true;
			return Json(model.Status);
		}
    }
}
