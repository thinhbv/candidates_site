using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using CMSSolutions.Extensions;
using CMSSolutions.Websites.Entities;

namespace CMSSolutions.Websites.Extensions
{
    public class Utilities
    {
		public static bool IsNumeric(string val, NumberStyles numberStyle)
		{
			Double result;
			return Double.TryParse(val, numberStyle, CultureInfo.CurrentCulture, out result);
		}

		public static string SerializeXml<T>(T dataToSerialize)
		{
			var stringwriter = new StringWriter();
			var serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(stringwriter, dataToSerialize);
			return stringwriter.ToString();
		}

		public static T DeserializeXml<T>(string xmlText)
		{
			var stringReader = new StringReader(xmlText);
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(stringReader);
		}

        public static T ConvertJsonToObject<T>(string input)
        {
            var json = new JavaScriptSerializer();
            return json.Deserialize<T>(input.Trim());
        }

        public static string ConvertObjectToJson<T>(T input)
        {
            var json = new JavaScriptSerializer();
            return json.Serialize(input);
        }

		public static string RemoveInjection(string query)
		{
			query = query.Replace("'", "''");
			return query;
		}

		public static string GenerateUniqueNumber()
		{
			var bytes = new byte[4];
			var rng = RandomNumberGenerator.Create();
			rng.GetBytes(bytes);
			uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
			return String.Format("{0:D8}", random);
		}

		public static int[] ParseListInt(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new int[0];
			}

			return value.Split(',').Select(Int32.Parse).ToArray();
		}

		public static string ParseString(int[] list)
		{
			if (list != null && list.Length > 0)
			{
				return string.Join(", ", list);
			}

			return string.Empty;
		}

		public static bool IsDirectory(string path)
		{
			FileAttributes attr = File.GetAttributes(path);
			if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
			{
				return true;
			}

			return false;
		}

		public static bool IsPathExists(string path)
		{
			if (Directory.Exists(path))
			{
				return true;
			}

			if (File.Exists(path))
			{
				return true;
			}

			return false;
		}

		public static string DateString(DateTime? dateTime)
		{
			if (dateTime == null)
			{
				return Constants.DateTimeMin;
			}

			return dateTime.Value.ToString(Constants.DateTimeFomat);
		}

		public static DateTime DateNull()
		{
			return new DateTime(1900, 01, 01);
		}

		public static bool IsNotNull(object value)
		{
			if (value == null)
			{
				return false;
			}

			if (value.Equals(Constants.IsNull) || value.Equals(""))
			{
				return false;
			}

			if (value.Equals(Constants.IsUndefined))
			{
				return false;
			}

			return true;
		}

		private static string DomainMapper(Match match)
		{
			var idn = new IdnMapping();
			string domainName = match.Groups[2].Value;
			try
			{
				domainName = idn.GetAscii(domainName);
			}
			catch (ArgumentException ex)
			{
				throw ex;
			}

			return match.Groups[1].Value + domainName;
		}

		public static bool IsEmailValid(string emailaddress)
		{
			if (String.IsNullOrEmpty(emailaddress))
			{
				return false;
			}

			emailaddress = Regex.Replace(emailaddress, @"(@)(.+)$", DomainMapper);
			return Regex.IsMatch(emailaddress,
				   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
				   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
				   RegexOptions.IgnoreCase);
		}

		public static string GetCharUnsigned(string values)
		{
			if (string.IsNullOrEmpty(values))
			{
				return string.Empty;
			}

			var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
			string temp = values.Normalize(NormalizationForm.FormD);
			var converttext = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

			converttext = Regex.Replace(converttext, "[^a-zA-Z0-9_.]+", " ", RegexOptions.Compiled);

			var list = new char[] { ' ', '/', ',', '&', '\"', '?', '|', ':', '"', '`', '\\', ';', '~', '!', '@', '#', '$', '%', '^', '*', '(', ')', '\'', '_', '=', '+', '{', '}', '[', ']', '.', '>', '<' };
			converttext = list.Aggregate(converttext, (current, schar) => current.Replace(schar, ' '));

			converttext = converttext.Replace("--", " ").Trim('.').TrimEnd(' ').TrimStart(' ');

			return converttext.ToLower();
		}

		public static List<SelectListItem> GetListDay()
		{
			var list = new List<SelectListItem>();
			for (int i = 1; i <= 31; i++)
			{
				var text = (i <= 9 ? "0" + i : i.ToString());
				list.Add(new SelectListItem
				{
					Value = text,
					Text = text,
				});
			}

			return list;
		}

		public static List<SelectListItem> GetListMonth()
		{
			var list = new List<SelectListItem>();
			for (int i = 1; i <= 12; i++)
			{
				var text = (i <= 9 ? "0" + i : i.ToString());
				list.Add(new SelectListItem
				{
					Value = text,
					Text = "Month " + text,
				});
			}

			return list;
		}

		public static List<SelectListItem> GetListYear()
		{
			var year = DateTime.Now.Year;
			var list = new List<SelectListItem>();
			for (int i = 1900; i <= year; i++)
			{
				var text = i.ToString();
				list.Add(new SelectListItem
				{
					Value = text,
					Text = text,
				});
			}

			return list.OrderByDescending(x => x.Value).ToList();
		}

        public static void WriteEventLog(string messages)
        {
            try
            {
                var eventLogName = "ApplicationErrors";
                if (!EventLog.SourceExists(eventLogName))
                {
                    EventLog.CreateEventSource(eventLogName, "Application Errors");
                }

                var log = new EventLog { Source = eventLogName };
                log.WriteEntry(messages, EventLogEntryType.Error);
            }
            catch (Exception)
            {

            }
        }
    }
}