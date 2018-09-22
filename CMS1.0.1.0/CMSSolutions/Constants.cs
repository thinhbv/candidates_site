using System.ComponentModel.DataAnnotations;
using System.Web.Services;

namespace CMSSolutions
{
    public enum MessageType
    {
        [Display(Name = "Chưa đăng ký tên miền")]
        RegisterDomain = 1,

        [Display(Name = "Thông báo thời gian sử dụng")]
        EstimatedTime = 100,

        [Display(Name = "Thông báo gần hết hạn sử dụng")]
        FinishDateUsed = 200,

        [Display(Name = "Thông báo đã hết hạn sử dụng")]
        ExpiredDate = 300,

        [Display(Name = "Thông báo tài khoản đã bị khóa")]
        LockedAccount = 400
    }

    public static class Constants
    {
        public const WsiProfiles EnumWsiProfiles = WsiProfiles.BasicProfile1_1;
        public const string UrlProductServices = "http://laptrinhmvc.com/ProductServices.asmx";
        public const string NamespaceSite = "http://tempuri.org/";
        public const string DefaultStatus = "True";

        public const string RowLeft = "row-left";
        public const string RowRight = "row-right";

        public const string ContainerCssClassCol12 = "col-xs-12 col-md-12";
        public const string ContainerCssClassCol10 = "col-xs-12 col-md-10";
        public const string ContainerCssClassCol9 = "col-xs-12 col-md-9";
        public const string ContainerCssClassCol8 = "col-xs-12 col-md-8";
        public const string ContainerCssClassCol6 = "col-xs-12 col-md-6";
        public const string ContainerCssClassCol4 = "col-xs-12 col-md-4";
        public const string ContainerCssClassCol3 = "col-xs-12 col-md-3";
        public const string ContainerCssClassCol2 = "col-xs-12 col-md-2";

        public const string NotMapped = "NotMapped";
        public const string DefaultUserName = "admin";
        public const string DefaultFullName = "Administrator";
        public const string DefaultEmail = "admin@yourdomain.com";
        public const string DefaultPassword = "Admin@1234";
        public const string ThemeDashboard = "Dashboard";
        public const string ThemeDefault = "Default";
        public const string KeyIsEncrypt = "IsEncrypt";
        public const string KeyIsSecurityUsers = "IsSecurityUsers";
        public const string KeyIsLoginWithLocal = "IsLoginWithLocal";
        public const string KeyDomainLocalhostForSite = "DomainLocalhostForSite";
        public const string Localhost = "localhost";
        public const string LocalhostIP = "127.0.0.1";

        public static class CacheInstance
        {
            public const string InstanceName = "CoreCacheManager";
        }

        public static class Grid
        {
            public const string GridWrapperStartHtml = "<article><div class=\"jarviswidget\" data-widget-deletebutton=\"false\" data-widget-editbutton=\"false\" data-widget-custombutton=\"false\"><header><span class=\"widget-icon\"> <i class=\"fa {1}\"></i> </span><h2>{0} </h2></header><div><div class=\"jarviswidget-editbox\"></div><div class=\"widget-body\">";

            public const string GridWrapperEndHtml = "</div></div></article>";
        }

        public static class Form
        {
            public const string FormWrapperStartHtml = "<article><div class=\"jarviswidget\" data-widget-editbutton=\"false\" data-widget-deletebutton=\"false\"><header><span class=\"widget-icon\"> <i class=\"fa {1}\"></i> </span><h2>{0} </h2></header><div><div class=\"widget-body\">";

            public const string FormWrapperEndHtml = "</div></div></div></article>";
        }

        public static class Areas
        {
            public const string Core = "CMSSolutions";
            public const string Aliases = "CMSSolutions.Alias";
            public const string Dashboard = "CMSSolutions.Dashboard";
            public const string Homepage = "CMSSolutions.Homepage";
            public const string Lists = "CMSSolutions.Lists";
            public const string Localization = "CMSSolutions.Localization";
            public const string Media = "CMSSolutions.Media";
            public const string Menus = "CMSSolutions.Menus";
            public const string Messages = "CMSSolutions.Messages";
            public const string Newsletters = "CMSSolutions.Newsletters";
            public const string OAuth = "CMSSolutions.Security.OAuth";
            public const string Pages = "CMSSolutions.Pages";
            public const string ScheduledTasks = "CMSSolutions.ScheduleTasks";
            public const string Security = "CMSSolutions.Security";
            public const string Sliders = "CMSSolutions.Sliders";
            public const string Widgets = "CMSSolutions.Widgets";
            public const string Accounts = "CMSSolutions.Accounts";
            public const string Application = "CMSSolutions.Application";
        }

        public static class Messages
        {
            public const string CannotDeleteRecord = "This record cannot be deleted, as there are related records referencing it.";
            public const string ConfirmDeleteRecord = "Bạn có chắc chắn muốn xóa dữ liệu này?";
            public const string EnableRecord = "Are you sure that you want to enabled this record?";
            public const string GenericSaveSuccess = "Successfully saved changes.";
            public const string InvalidModel = "Vui lòng kiểm tra lại các trường dữ liệu. Có một số trường bạn chưa chọn hoặc đang để trống.";
            public const string ErrorDbConnection = "Can not found connection string.";
            public const string ErrorPublishKey = "Vui lòng đăng ký tên miền tại http://laptrinhmvc.com/download.html để có thể đăng nhập.";
            public const string ErrorMissingKeyIsEncrypt = "Hệ thống chưa cài đặt <add key=\"IsEncrypt\" value=\"False\" /> trong web.config";
            public const string ErrorMissingKeyIsSecurityUsers = "Hệ thống chưa cài đặt <add key=\"IsSecurityUsers\" value=\"True\" /> trong web.config";
            public const string ErrorMissingKeyIsLoginWithLocal = "Hệ thống chưa cài đặt <add key=\"IsLoginWithLocal\" value=\"False\" /> trong web.config";
            public const string ErrorMissingKeyDomainLocalhost = "Hệ thống chưa cài đặt <add key=\"DomainLocalhostForSite\" value=\"laptrinhmvc.com\" /> trong web.config";

            public static class Validation
            {
                public const string Date = "Please enter a valid date.";
                public const string Digits = "Please enter only digits.";
                public const string Email = "Please enter a valid email address.";
                public const string EqualTo = "Please enter the same value again.";
                public const string MaxLength = "Please enter no more than {0} characters.";
                public const string MinLength = "Please enter at least {0} characters.";
                public const string Number = "Please enter a valid number.";
                public const string Range = "Please enter a value between {0} and {1}.";
                public const string RangeLength = "Please enter a value between {0} and {1} characters long.";
                public const string RangeMax = "Please enter a value less than or equal to {0}.";
                public const string RangeMin = "Please enter a value greater than or equal to {0}.";
                public const string Required = "This field is required.";
                public const string Url = "Please enter a valid URL.";
            }
        }

        public static class Scripts
        {
            public const string JQueryFormExtension = "jQuery(function ($) { $.extend({ form: function (url, data, method) { if (method == null) method = 'POST'; if (data == null) data = {};  var form = $('<form>').attr({ method: method, action: url }).css({ display: 'none' });  var addData = function (name, data) { if ($.isArray(data)) { for (var i = 0; i < data.length; i++) { var value = data[i]; addData(name + '[]', value); } } else if (typeof data === 'object') { for (var key in data) { if (data.hasOwnProperty(key)) { addData(name + '[' + key + ']', data[key]); } } } else if (data != null) { form.append($('<input>').attr({ type: 'hidden', name: String(name), value: String(data) })); } };  for (var key in data) { if (data.hasOwnProperty(key)) { addData(key, data[key]); } }  return form.appendTo('body'); } }); });";
            public const string JQueryFormParams = @"(function(g){var i=/radio|checkbox/i,j=/[^\[\]]+/g,k=/^[\-+]?[0-9]*\.?[0-9]+([eE][\-+]?[0-9]+)?$/,l=function(b){if(typeof b==""number"")return true;if(typeof b!=""string"")return false;return b.match(k)};g.fn.extend({formParams:function(b,d){if(!!b===b){d=b;b=null}if(b)return this.setParams(b);else if(this[0].nodeName.toLowerCase()==""form""&&this[0].elements)return jQuery(jQuery.makeArray(this[0].elements)).getParams(d);return jQuery(""input[name], textarea[name], select[name]"",this[0]).getParams(d)},setParams:function(b){this.find(""[name]"").each(function(){var d=b[g(this).attr(""name"")],a;if(d!==undefined){a=g(this);if(a.is("":radio""))a.val()==d&&a.attr(""checked"",true);else if(a.is("":checkbox"")){d=g.isArray(d)?d:[d];g.inArray(a.val(),d)>-1&&a.attr(""checked"",true)}else a.val(d)}})},getParams:function(b){var d={},a;b=b===undefined?false:b;this.each(function(){var e=this;if(!((e.type&&e.type.toLowerCase())==""submit""||!e.name)){var c=e.name,f=g.data(e,""value"")||g.fn.val.call([e]),h=i.test(e.type);c=c.match(j);e=!h||!!e.checked;if(b){if(l(f))f=parseFloat(f);else if(f===""true"")f=true;else if(f===""false"")f=false;if(f==="""")f=undefined}a=d;for(h=0;h<c.length-1;h++){a[c[h]]||(a[c[h]]={});a=a[c[h]]}c=c[c.length-1];if(a[c]){g.isArray(a[c])||(a[c]=a[c]===undefined?[]:[a[c]]);e&&a[c].push(f)}else if(e||!a[c])a[c]=e?f:undefined}});return d}})})(jQuery);";
        }
    }
}