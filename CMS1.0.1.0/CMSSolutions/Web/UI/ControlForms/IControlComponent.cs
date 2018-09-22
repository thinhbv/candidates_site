using System.Text;

namespace CMSSolutions.Web.UI.ControlForms
{
    public interface IControlComponent
    {
        void Build(StringBuilder stringBuilder);
    }
}