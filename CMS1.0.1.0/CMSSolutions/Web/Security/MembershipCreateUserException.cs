using System;

namespace CMSSolutions.Web.Security
{
    [Serializable]
    public class MembershipCreateUserException : Exception
    {
        public MembershipCreateStatus StatusCode { get; set; }
    }
}