using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Mvc
{
    public abstract class BaseModel<TKey>
    {
        [ControlHidden]
        public TKey Id { get; set; }
    }
}