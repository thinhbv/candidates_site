using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Configuration
{
    public interface ISettings : IDependency
    {
        string Name { get; }

        bool Hidden { get; }

        void OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext);
    }
}