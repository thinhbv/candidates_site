using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace CMSSolutions.Web
{
    public class RecaptchaValidator
    {
        public string Challenge { get; set; }

        public string Response { get; set; }

        public string PrivateKey { get; set; }

        public string RemoteIP { get; set; }

        public RecaptchaResponse Validate()
        {
            string[] strArray;
            CheckNotNull(PrivateKey, "PrivateKey");
            CheckNotNull(RemoteIP, "RemoteIp");
            CheckNotNull(Challenge, "Challenge");
            CheckNotNull(Response, "Response");
            if ((Challenge == string.Empty) || (RemoteIP == string.Empty))
            {
                return RecaptchaResponse.InvalidSolution;
            }

            var request = (HttpWebRequest)WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Timeout = 30000;
            request.Method = "POST";
            request.UserAgent = "reCAPTCHA/ASP.NET";
            request.ContentType = "application/x-www-form-urlencoded";
            string s = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                                     new object[]
                                         {
                                             HttpUtility.UrlEncode(PrivateKey), HttpUtility.UrlEncode(RemoteIP),
                                             HttpUtility.UrlEncode(Challenge), HttpUtility.UrlEncode(Response)
                                         });
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var response = request.GetResponse())
                {
                    using (TextReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        strArray = reader.ReadToEnd().Split(new[] { "\n", @"\n" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }
            catch (WebException)
            {
                return RecaptchaResponse.RecaptchaNotReachable;
            }

            switch (strArray[0])
            {
                case "true":
                    return RecaptchaResponse.Valid;

                case "false":
                    return new RecaptchaResponse(false, strArray[1].Trim(new[] { '\'' }));
            }
            throw new InvalidProgramException("Unknown status response.");
        }

        private void CheckNotNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }

    public class RecaptchaResponse
    {
        public static readonly RecaptchaResponse InvalidChallenge = new RecaptchaResponse(false, "Invalid reCAPTCHA request. Missing challenge value.");

        public static readonly RecaptchaResponse InvalidResponse = new RecaptchaResponse(false, "Invalid reCAPTCHA request. Missing response value.");

        public static readonly RecaptchaResponse InvalidSolution = new RecaptchaResponse(false, "The verification words are incorrect.");

        public static readonly RecaptchaResponse RecaptchaNotReachable = new RecaptchaResponse(false, "The reCAPTCHA server is unavailable.");

        public static readonly RecaptchaResponse Valid = new RecaptchaResponse(true, string.Empty);

        internal RecaptchaResponse(bool isValid, string errorMessage)
        {
            RecaptchaResponse valid = null;
            if (IsValid)
            {
                valid = Valid;
            }
            else
            {
                switch (errorMessage)
                {
                    case null:
                        throw new ArgumentNullException("errorMessage");

                    case "incorrect-captcha-sol":
                        valid = InvalidSolution;
                        break;
                }
            }

            if (valid != null)
            {
                IsValid = valid.IsValid;
                ErrorMessage = valid.ErrorMessage;
            }
            else
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }
        }

        public string ErrorMessage { get; private set; }

        public bool IsValid { get; private set; }
    }
}