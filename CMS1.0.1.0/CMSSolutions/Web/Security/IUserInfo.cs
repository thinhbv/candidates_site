using System;

namespace CMSSolutions.Web.Security
{
    public interface IUserInfo
    {
        int Id { get; }

        string UserName { get; }

        string FullName { get; set; }

        string PhoneNumber { get; set; }

        string Email { get; }

        bool SuperUser { get; }

        DateTime CreateDate { get; }
    }
}