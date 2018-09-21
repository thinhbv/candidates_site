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
    
    
    [Authorize()]
    [Themed(IsDashboard=true)]
	public class LevelCandidatesController : BaseAdminController
    {
        
        private readonly ILevelCandidatesService service;
        
        public LevelCandidatesController(IWorkContextAccessor workContextAccessor, ILevelCandidatesService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblLevelCandidates";
        }
        
        [Url("admin/levelcandidates/edit/{id}")]
        public ActionResult Edit(int id, int candidateId)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("LevelCandidates"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Level Candidates"), Url = Url.Action("Index") });
            var model = new LevelCandidatesModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
			model.candidate_id = candidateId;

            var result = new ControlFormResult<LevelCandidatesModel>(model);
            result.Title = this.T("Edit LevelCandidates");
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
        [Url("admin/levelcandidates/update")]
        public ActionResult Update(LevelCandidatesModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            LevelCandidates item;
            if (model.Id == 0)
            {
                item = new LevelCandidates();
				item.created_date = DateTime.Now;
            }
            else
            {
                item = service.GetById(model.Id);
				item.updated_date = DateTime.Now;
            }
			item.candidate_id = model.candidate_id;
            item.language_id = model.language_id;
            item.level_dev = model.level_dev;
			item.main_skill = model.main_skill;
			item.month = model.month;
			item.notes = model.notes;
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
    }
}
