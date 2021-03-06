﻿using System.Configuration;
namespace CMSSolutions.Websites.Extensions
{
    public class Constants
    {
		public const string PageIndex = "PageIndex";
		public const string CandidateId = "CandidateId";
		public const string Template = "template";
		public const string MailBody = "mail_body";
		public const string LanguageId = "language_id";
		public const string LevelId = "level_id";
		public const string Status = "status";
		public const string FromDate = "from_date";
		public const string ToDate = "to_date";
		public const string Keyword = "keyword";
		public const string ReturnUrl = "returnUrl";

		public const string IsNull = "null";
		public const string IsUndefined = "undefined";

		public const string DateTimeFomat = "MM/dd/yyyy";
		public const string DateTimeMin = "01/01/1900";

        public const string CssControlCustom = "form-control-custom";
        public const string CssThumbsSize = "thumbs-size";

        public const string SeoTitle = "SeoTitle";
        public const string SeoDescription = "SeoDescription";
        public const string SeoKeywords = "SeoKeywords";

		public const string DateCustomCss = "date-control-cust";

		public static string HRRoleId = ConfigurationManager.AppSettings["HR_ROLE_ID"];
		public static string InterviewRoleId = ConfigurationManager.AppSettings["INTERVIEW_ROLE_ID"];
		public static string ManagerRoleId = ConfigurationManager.AppSettings["MANAGER_ROLE_ID"];
    }
}