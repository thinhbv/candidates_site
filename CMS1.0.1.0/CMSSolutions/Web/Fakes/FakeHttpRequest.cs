using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace CMSSolutions.Web.Fakes
{
    public class FakeHttpRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection cookies;
        private readonly NameValueCollection formParams;
        private readonly string httpMethod;
        private readonly NameValueCollection queryStringParams;
        private readonly string relativeUrl;
        private readonly NameValueCollection serverVariables;
        private readonly Uri url;
        private readonly Uri urlReferrer;

        public FakeHttpRequest(string relativeUrl, string method,
                               NameValueCollection formParams, NameValueCollection queryStringParams,
                               HttpCookieCollection cookies, NameValueCollection serverVariables)
        {
            httpMethod = method;
            this.relativeUrl = relativeUrl;
            this.formParams = formParams;
            this.queryStringParams = queryStringParams;
            this.cookies = cookies;
            this.serverVariables = serverVariables;

            //ensure collections are not null
            if (this.formParams == null)
                this.formParams = new NameValueCollection();
            if (this.queryStringParams == null)
                this.queryStringParams = new NameValueCollection();
            if (this.cookies == null)
                this.cookies = new HttpCookieCollection();
            if (this.serverVariables == null)
                this.serverVariables = new NameValueCollection();
        }

        public FakeHttpRequest(string relativeUrl, string method, Uri url, Uri urlReferrer,
                               NameValueCollection formParams, NameValueCollection queryStringParams,
                               HttpCookieCollection cookies, NameValueCollection serverVariables)
            : this(relativeUrl, method, formParams, queryStringParams, cookies, serverVariables)
        {
            this.url = url;
            this.urlReferrer = urlReferrer;
        }

        public FakeHttpRequest(string relativeUrl, Uri url, Uri urlReferrer)
            : this(relativeUrl, HttpVerbs.Get.ToString("g"), url, urlReferrer, null, null, null, null)
        {
        }

        public override NameValueCollection ServerVariables
        {
            get { return serverVariables; }
        }

        public override NameValueCollection Form
        {
            get { return formParams; }
        }

        public override NameValueCollection QueryString
        {
            get { return queryStringParams; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return cookies; }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return relativeUrl; }
        }

        public override Uri Url
        {
            get { return url; }
        }

        public override Uri UrlReferrer
        {
            get { return urlReferrer; }
        }

        public override string PathInfo
        {
            get { return ""; }
        }

        public override string ApplicationPath
        {
            get
            {
                //we know that relative paths always start with ~/
                //ApplicationPath should start with /
                if (relativeUrl != null && relativeUrl.StartsWith("~/"))
                    return relativeUrl.Remove(0, 1);
                return null;
            }
        }

        public override string HttpMethod
        {
            get { return httpMethod; }
        }

        public override string UserHostAddress
        {
            get { return null; }
        }

        public override string RawUrl
        {
            get { return null; }
        }

        public override bool IsSecureConnection
        {
            get { return false; }
        }

        public override bool IsAuthenticated
        {
            get { return false; }
        }
    }
}