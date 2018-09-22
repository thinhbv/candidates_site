using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace CMSSolutions.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception x)
        {
            return x is CMSException ||
                x is StackOverflowException ||
                x is OutOfMemoryException ||
                x is AccessViolationException ||
                x is AppDomainUnloadedException ||
                x is ThreadAbortException ||
                x is SecurityException ||
                x is SEHException;
        }

        public static int GetLineNumber(this Exception x)
        {
            return new StackTrace(x).GetFrame(0).GetFileLineNumber();
        }

        public static string GetMessageStack(this Exception x)
        {
            if (x == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.AppendLine(x.Message);

            while (x.InnerException != null)
            {
                x = x.InnerException;
                sb.Append("--> ");
                sb.AppendLine(x.Message);
            }

            return sb.ToString();
        }
    }
}