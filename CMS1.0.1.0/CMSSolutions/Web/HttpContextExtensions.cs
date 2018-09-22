using System.Security;
using System.Web;

namespace CMSSolutions.Web
{
    public static class HttpContextExtensions
    {
        private static AspNetHostingPermissionLevel? trustLevel;
        public static AspNetHostingPermissionLevel GetAspNetHostingPermissionLevel(this HttpContext httpContext)
        {
            if (!trustLevel.HasValue)
            {
                trustLevel = AspNetHostingPermissionLevel.None;
                foreach (var level in new[] {
                    AspNetHostingPermissionLevel.Unrestricted,
                    AspNetHostingPermissionLevel.High,
                    AspNetHostingPermissionLevel.Medium,
                    AspNetHostingPermissionLevel.Low,
                    AspNetHostingPermissionLevel.Minimal
                })
                {
                    try
                    {
                        new AspNetHostingPermission(level).Demand();
                        trustLevel = level;
                        break; 
                    }
                    catch (SecurityException)
                    {
                    }
                }
            }
            return trustLevel.Value;
        }
    }
}