using System;

namespace CMSSolutions
{
    public class MessageErrorModel
    {
        public Exception ExceptionError { get; set; }

        public string HttpStatusCode { get; set; }

        public string Messages { get; set; }

        public string TitleForm { get; set; }

        public string GoBackText { get; set; }

        public bool Status { get; set; }
    }
}