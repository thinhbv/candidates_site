using System;
using System.Configuration;
using System.Web;
using CMSSolutions.Copyrights;

namespace CMSSolutions.Configuration
{
    public static class KeyConfiguration
    {
        public static string CurrentDoamin
        {
            get
            {
                return HttpContext.Current.Request.Url.Host;
            }
        }

        public static string PublishKey
        {
            get
            {
                try
                {
                    var domain = DomainSite;
                    var service = new ProductServices();
                    var message = service.CheckCopyright(domain);
                    if (message != Constants.DefaultStatus)
                    {
                        throw new Exception(message);
                    }

                    return service.GetTokenKey(domain);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static bool IsEncrypt
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings[Constants.KeyIsEncrypt]))
                {
                    return bool.Parse(ConfigurationSettings.AppSettings[Constants.KeyIsEncrypt]);
                }

                throw new Exception(Constants.Messages.ErrorMissingKeyIsEncrypt);
            }
        }

        public static bool IsSecurityUsers
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings[Constants.KeyIsSecurityUsers]))
                {
                    return bool.Parse(ConfigurationSettings.AppSettings[Constants.KeyIsSecurityUsers]);
                }

                throw new Exception(Constants.Messages.ErrorMissingKeyIsSecurityUsers);
            }
        }

        public static bool IsLoginWithLocal
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings[Constants.KeyIsLoginWithLocal]))
                {
                    return bool.Parse(ConfigurationSettings.AppSettings[Constants.KeyIsLoginWithLocal]);
                }

                throw new Exception(Constants.Messages.ErrorMissingKeyIsLoginWithLocal);
            }
        }

        public static string DomainLocalhost
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings[Constants.KeyDomainLocalhostForSite]))
                {
                    return Constants.Localhost + "." + ConfigurationSettings.AppSettings[Constants.KeyDomainLocalhostForSite].Trim();
                }

                throw new Exception(Constants.Messages.ErrorMissingKeyDomainLocalhost);
            }
        }

        public static string DomainSite
        {
            get
            {
                var domain = CurrentDoamin;
                if (string.IsNullOrEmpty(domain))
                {
                    throw new Exception(Constants.Messages.ErrorPublishKey);
                }

                if (domain.Equals(Constants.Localhost) || domain.Equals(Constants.LocalhostIP))
                {
                    if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings[Constants.KeyDomainLocalhostForSite]))
                    {
                        return Constants.Localhost + "." + ConfigurationSettings.AppSettings[Constants.KeyDomainLocalhostForSite].Trim();
                    }
                }

                return domain;
            }
        }
    }
}