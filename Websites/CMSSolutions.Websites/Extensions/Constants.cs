using System.Configuration;
namespace CMSSolutions.Websites.Extensions
{
    public class Constants
    {
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
    }
}