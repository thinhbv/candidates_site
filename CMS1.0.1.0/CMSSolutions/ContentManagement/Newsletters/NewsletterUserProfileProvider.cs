using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Web.Security;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Newsletters
{
    public class NewsletterUserProfileProvider : IUserProfileProvider
    {
        public const string ReceiveNewsletters = "ReceiveNewsletters";

        #region IUserProfileProvider Members

        public string Category
        {
            get { return "Newsletters"; }
        }

        public IEnumerable<string> GetFieldNames()
        {
            return new[]
            {
                ReceiveNewsletters
            };
        }

        public IEnumerable<ControlFormAttribute> GetFields(WorkContext workContext, int userId, bool onlyPublicProperties)
        {
            if (onlyPublicProperties)
            {
                // if there are many properties and some of them are public, then we can return only the public properties,
                //  but for this provider there is ONLY 1 property and it is not public, so we just return empty an result
                return Enumerable.Empty<ControlFormAttribute>();
            }

            var membershipService = workContext.Resolve<IMembershipService>();

            string receiveNewsletters = membershipService.GetProfileEntry(userId, ReceiveNewsletters);

            return new[]
            {
                new ControlChoiceAttribute(ControlChoice.CheckBox)
                {
                    Name = ReceiveNewsletters,
                    AppendText = "Receive Newsletters",
                    LabelText = "",
                    Value = !string.IsNullOrEmpty(receiveNewsletters)
                        ? bool.Parse(receiveNewsletters)
                        : false,
                    PropertyType = typeof(bool),
                }
            };
        }

        #endregion IUserProfileProvider Members
    }
}