using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class OpenResponse : OpenResponseBase
    {
        public OpenResponse(DtoBase currentWorkingDirectory)
            : base(currentWorkingDirectory)
        {
            Options = new Options();
            AddResponse(currentWorkingDirectory);
        }
    }
}