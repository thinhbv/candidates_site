using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CMSSolutions.Security.Cryptography
{
    public static class EncryptionExtensions
    {
        public static string Encrypt(string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);
                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = keyArray, 
                    Mode = CipherMode.ECB, 
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string Decrypt(string key, string toDecrypt)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = keyArray, 
                    Mode = CipherMode.ECB, 
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        static public string ToHex(this byte[] ba)
        {
            if (ba == null || ba.Length == 0)
            {
                return "";
            }
            const string HexFormat = "{0:X2}";
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ba)
            {
                sb.Append(string.Format(HexFormat, b));
            }
            return sb.ToString();
        }

        static public byte[] FromHex(this string hexEncoded)
        {
            if (hexEncoded == null || hexEncoded.Length == 0)
            {
                return null;
            }
            try
            {
                int l = Convert.ToInt32(hexEncoded.Length / 2);
                byte[] b = new byte[l];
                for (int i = 0; i <= l - 1; i++)
                {
                    b[i] = Convert.ToByte(hexEncoded.Substring(i * 2, 2), 16);
                }
                return b;
            }
            catch (Exception ex)
            {
                throw new System.FormatException("The provided string does not appear to be Hex encoded:" + System.Environment.NewLine + hexEncoded + System.Environment.NewLine, ex);
            }
        }

        static public byte[] FromBase64(this string base64Encoded)
        {
            if (base64Encoded == null || base64Encoded.Length == 0)
            {
                return null;
            }
            try
            {
                return Convert.FromBase64String(base64Encoded);
            }
            catch (System.FormatException ex)
            {
                throw new System.FormatException("The provided string does not appear to be Base64 encoded:" + System.Environment.NewLine + base64Encoded + System.Environment.NewLine, ex);
            }
        }

        static public string ToBase64(this byte[] b)
        {
            if (b == null || b.Length == 0)
            {
                return "";
            }
            return Convert.ToBase64String(b);
        }

        static internal string GetXmlElement(string xml, string element)
        {
            Match m = default(Match);
            m = Regex.Match(xml, "<" + element + ">(?<Element>[^>]*)</" + element + ">", RegexOptions.IgnoreCase);
            if (m == null)
            {
                throw new Exception("Could not find <" + element + "></" + element + "> in provided Public Key XML.");
            }
            return m.Groups["Element"].ToString();
        }

        static internal string GetConfigString(string key)
        {
            return GetConfigString(key, false);
        }

        static internal string GetConfigString(string key, bool isRequired)
        {
            string s = (string)ConfigurationManager.AppSettings.Get(key);
            if (s == null)
            {
                if (isRequired)
                {
                    throw new ConfigurationErrorsException("key <" + key + "> is missing from .config file");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return s;
            }
        }

        static internal string WriteConfigKey(string key, string value)
        {
            string s = "<add key=\"{0}\" value=\"{1}\" />" + System.Environment.NewLine;
            return string.Format(s, key, value);
        }

        static internal string WriteXmlElement(string element, string value)
        {
            string s = "<{0}>{1}</{0}>" + System.Environment.NewLine;
            return string.Format(s, element, value);
        }

        static internal string WriteXmlNode(string element)
        {
            return WriteXmlNode(element, false);
        }

        static internal string WriteXmlNode(string element, bool isClosing)
        {
            string s = null;
            if (isClosing)
            {
                s = "</{0}>" + System.Environment.NewLine;
            }
            else
            {
                s = "<{0}>" + System.Environment.NewLine;
            }
            return string.Format(s, element);
        }
    }
}