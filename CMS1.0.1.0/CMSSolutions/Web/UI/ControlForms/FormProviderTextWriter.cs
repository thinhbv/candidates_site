using System.IO;
using System.Text;

namespace CMSSolutions.Web.UI.ControlForms
{
    internal class FormProviderTextWriter : TextWriter
    {
        private readonly ControlFormProvider formProvider;

        public FormProviderTextWriter(ControlFormProvider formProvider)
        {
            this.formProvider = formProvider;
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(string value)
        {
            formProvider.WriteToOutput(value);
        }
    }
}