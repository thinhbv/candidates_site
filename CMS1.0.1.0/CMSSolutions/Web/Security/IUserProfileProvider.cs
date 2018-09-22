using System;
using System.Collections.Generic;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security
{
    public interface IUserProfileProvider : IDependency
    {
        string Category { get; }

        IEnumerable<string> GetFieldNames();

        IEnumerable<ControlFormAttribute> GetFields(WorkContext workContext, int userId, bool onlyPublicProperties);
    }
}