using System.Web.Mvc;
using CMSSolutions.Data;
using CMSSolutions.Extensions;
using CMSSolutions.Services;
using CMSSolutions.Web.Optimization;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Mvc
{
    [Themed]
    public abstract class BaseControlController<TKey, TEntity, TModel> : BaseController
        where TModel : BaseModel<TKey>, new()
        where TEntity : BaseEntity<TKey>, new() where TKey : struct
    {
        private readonly IGenericService<TEntity, TKey> service;
        private string name;
        private string pluralizeName;

        public string TableName { get; set; }

        protected BaseControlController(IWorkContextAccessor workContextAccessor, IGenericService<TEntity, TKey> service)
            : base(workContextAccessor)
        {
            this.service = service;
            TableName = "tblDefault";
        }

        protected virtual string Name
        {
            get { return name ?? (name = typeof(TEntity).Name.SpacePascal()); }
        }

        protected virtual string PluralizedName
        {
            get { return pluralizeName ?? (pluralizeName = Name.Pluralize()); }
        }

        protected virtual IGenericService<TEntity, TKey> Service { get { return service; } }

        protected virtual bool EnableCreate { get { return true; } }

        protected virtual bool EnableEdit { get { return true; } }

        protected virtual string EditText { get { return T("Edit"); } }

        protected virtual bool EnableDelete { get { return true; } }

        protected virtual bool ShowModalDialog { get { return true; } }

        protected virtual int DialogModalWidth { get { return 600; } }

        protected virtual int DialogModalHeight { get { return 600; } }

        protected virtual bool EnablePaginate { get { return true; } }

        protected virtual string CreateText { get { return T("Create"); } }

        [CompressFilter]
        [Url("{BaseUrl}")]
        public virtual ActionResult Index()
        {
            if (!CheckPermissions())
            {
                return new HttpUnauthorizedResult();
            }

            var result = new ControlGridFormResult<TEntity>
            {
                Title = T(PluralizedName),
                FetchAjaxSource = GetRecords,
                UpdateActionName = "Update",
                DefaultPageSize = WorkContext.DefaultPageSize,
                EnablePaginate = EnablePaginate,
                ActionsHeaderText = T("Actions"),
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            if (EnableCreate)
            {
                var createAction = result.AddAction()
                    .HasText(CreateText)
                    .HasIconCssClass("cx-icon cx-icon-add")
                    .HasUrl(Url.Action("Create", RouteData.Values))
                    .HasButtonStyle(ButtonStyle.Primary)
                    .HasParentClass(Constants.ContainerCssClassCol12);

                if (ShowModalDialog)
                {
                    createAction.ShowModalDialog(DialogModalWidth, DialogModalHeight);
                }

                OnCreateCreateButton(createAction);
            }

            if (EnableEdit)
            {
                var editAction = result.AddRowAction()
                    .HasText(EditText)
                    .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
                    .HasButtonSize(ButtonSize.ExtraSmall);

                if (ShowModalDialog)
                {
                    editAction.ShowModalDialog(DialogModalWidth, DialogModalHeight);
                }

                OnCreateEditButton(editAction);
            }

            if (EnableDelete)
            {
                var deleteAction = result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);
                OnCreateDeleteButton(deleteAction);
            }

            OnViewIndex(result);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            return OnIndexResult(result);
        }

        protected virtual ActionResult OnIndexResult(ControlGridFormResult<TEntity> result)
        {
            return result;
        }

        protected virtual void OnCreateCreateButton(ControlFormAction createButton)
        {
        }

        protected virtual void OnCreateEditButton(ControlGridFormRowAction<TEntity> editButton)
        {
        }

        protected virtual void OnCreateDeleteButton(ControlGridFormRowAction<TEntity> deleteButton)
        {
        }

        protected virtual bool CheckPermissions()
        {
            return true;
        }

        protected abstract void OnViewIndex(ControlGridFormResult<TEntity> controlGrid);

        protected virtual ControlGridAjaxData<TEntity> GetRecords(ControlGridFormRequest options)
        {
            int totals;
            var records = Service.GetRecords(options, out totals);
            return new ControlGridAjaxData<TEntity>(records, totals);
        }

        [Themed(false)]
        [Url("{BaseUrl}/create")]
        public virtual ActionResult Create()
        {
            if (!CheckPermissions())
            {
                return new HttpUnauthorizedResult();
            }

            var model = new TModel();

            var result = new ControlFormResult<TModel>(model)
            {
                Title = T("Create " + Name),
                UpdateActionName = "Update",
                ShowCloseButton = true,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml  
            };

            OnCreating(result);

            return result;
        }

        protected virtual void OnCreating(ControlFormResult<TModel> controlForm)
        {
        }

        [Themed(false)]
        [Url("{BaseUrl}/edit/{id}")]
        public virtual ActionResult Edit(TKey id)
        {
            if (!CheckPermissions())
            {
                return new HttpUnauthorizedResult();
            }

            var model = ConvertToModel(Service.GetById(id));

            var result = new ControlFormResult<TModel>(model)
            {
                Title = T("Edit " + Name).Text,
                UpdateActionName = "Update",
                ShowCloseButton = true ,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml  
            };

            OnEditing(result);

            return result;
        }

        protected virtual void OnEditing(ControlFormResult<TModel> controlForm)
        {
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public virtual ActionResult Delete(TKey id)
        {
            if (!CheckPermissions())
            {
                return new HttpUnauthorizedResult();
            }

            var entity = Service.GetById(id);

            OnDeleting(entity);

            Service.Delete(entity);
            return OnDeleteSuccess(entity);
        }

        protected virtual void OnDeleting(TEntity entity)
        {
        }

        protected virtual ActionResult OnDeleteSuccess(TEntity entity)
        {
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{BaseUrl}/update"), Transaction]
        public virtual ActionResult Update(TModel model)
        {
            if (!CheckPermissions())
            {
                return new HttpUnauthorizedResult();
            }

            var entity = model.Id.Equals(default(TKey))
                ? new TEntity()
                : GetEntityForUpdate(model.Id);
            ConvertFromModel(model, entity);
            Service.Save(entity);

            return OnUpdateSuccess(entity);
        }

        protected virtual TEntity GetEntityForUpdate(TKey id)
        {
            return Service.GetById(id);
        }

        protected virtual ActionResult OnUpdateSuccess(TEntity entity)
        {
            if (ShowModalDialog)
            {
                return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
            }

            return new AjaxResult().Redirect(Url.Action("Index"), true);
        }

        protected abstract TModel ConvertToModel(TEntity entity);

        protected abstract void ConvertFromModel(TModel model, TEntity entity);
    }
}