using System;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace CMSSolutions.Copyrights
{
    [WebServiceBinding(Name = "ProductServices", Namespace = Constants.NamespaceSite)]
    public class ProductServices : SoapHttpClientProtocol
    {
        [DebuggerStepThroughAttribute]
        public ProductServices()
        {
            Url = Constants.UrlProductServices;
        }

        [DebuggerStepThrough]
        [SoapDocumentMethodAttribute(Constants.NamespaceSite + "CheckCopyright",
            RequestNamespace = Constants.NamespaceSite,
            ResponseNamespace = Constants.NamespaceSite,
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CheckCopyright(string domain)
        {
            object[] results = Invoke("CheckCopyright", new object[] { domain });
            return (string)results[0];
        }

        [DebuggerStepThrough]
        [SoapDocumentMethodAttribute(Constants.NamespaceSite + "GetTokenKey",
            RequestNamespace = Constants.NamespaceSite,
            ResponseNamespace = Constants.NamespaceSite,
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetTokenKey(string domain)
        {
            object[] results = Invoke("GetTokenKey", new object[] { domain });
            return (string)results[0];
        }

        [DebuggerStepThrough]
        [SoapDocumentMethodAttribute(Constants.NamespaceSite + "GetErrorMessage",
            RequestNamespace = Constants.NamespaceSite,
            ResponseNamespace = Constants.NamespaceSite,
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetErrorMessage(int errorCode, string domain, DateTime startDate, DateTime endDate)
        {
            object[] results = Invoke("GetErrorMessage", new object[] { errorCode, domain, startDate, endDate });
            return (string)results[0];
        }
    }
}
