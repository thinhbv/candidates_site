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
	using CMSSolutions.Extensions;
	using CMSSolutions.Websites.Extensions;
    
    
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

		[Themed(false)]
        [Url("admin/levelcandidates/edit/{id}")]
        public ActionResult Edit(int id, int candidateId)
        {
			var title = T("Create Level");
            var model = new LevelCandidatesModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
				 title = T("Edit Level");
            }
			model.candidate_id = candidateId;

			var result = new ControlFormResult<LevelCandidatesModel>(model)
			{
				Title = title,
				FormMethod = FormMethod.Post,
				UpdateActionName = "Update",
				SubmitButtonText = T("Save"),
				CancelButtonText = T("Close"),
				ShowBoxHeader = false,
				FormWrapperStartHtml = CMSSolutions.Constants.Form.FormWrapperStartHtml,
				FormWrapperEndHtml = CMSSolutions.Constants.Form.FormWrapperEndHtml
			};

			result.RegisterExternalDataSource(x => x.language_id, y => BindLanguages(model.language_id));
			result.RegisterExternalDataSource(x => x.level_dev, y => BindLevels(model.level_dev));
			result.RegisterExternalDataSource(x => x.main_skill, y => EnumExtensions.GetListItems<LevelType>());
			
            return result;
        }

		private IEnumerable<SelectListItem> BindLanguages(int languageId)
		{
			var service = WorkContext.Resolve<ILanguagesService>();
			var items = service.GetRecords();
			var result = new List<SelectListItem>();
			if (items != null)
			{
				result.AddRange(items.Select(item => new SelectListItem
				{
					Text = item.name,
					Value = item.Id.ToString(),
					Selected = item.Id == languageId
				}));
			}

			return result;
		}

		private IEnumerable<SelectListItem> BindLevels(int levelId)
		{
			var service = WorkContext.Resolve<ILevelsService>();
			var items = service.GetRecords();
			var result = new List<SelectListItem>();
			if (items != null)
			{
				result.AddRange(items.Select(item => new SelectListItem
				{
					Text = item.name,
					Value = item.Id.ToString(),
					Selected = item.Id == levelId
				}));
			}

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
				return new AjaxResult().Alert(T(CMSSolutions.Constants.Messages.InvalidModel));
            }
			var text = "Create success.";
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
				text = "Update success.";
            }
			item.candidate_id = model.candidate_id;
            item.language_id = model.language_id;
            item.level_dev = model.level_dev;
			item.main_skill = model.main_skill;
			item.month = model.month;
			item.notes = model.notes;
            service.Save(item);

			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(text)
				.CloseModalDialog().ExecuteScript("parent.$('#tblCandidates').jqGrid().trigger('reloadGrid');");
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var model = service.GetById(id);
            service.Delete(model);
			return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert("Delete success.");
        }
    }
}
