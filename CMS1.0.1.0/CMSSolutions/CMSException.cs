using System;
using System.Runtime.Serialization;

namespace CMSSolutions
{
    public class CMSException : ApplicationException
    {
        public CMSException()
        {
        }

        public CMSException(string message)
            : base(message)
        {
        }

        public CMSException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CMSException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}