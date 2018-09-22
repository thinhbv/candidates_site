using System.Collections.Generic;

namespace CMSSolutions.Web.UI.ControlForms
{
    public interface IControlFormProvider
    {
        IEnumerable<ControlFormAttribute> GetAttributes();
    }
}