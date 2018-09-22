using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Collections;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security
{
    public class SecurityUserProfileProvider : IUserProfileProvider
    {
        public const string PreferredLanguage = "PreferredLanguage";

        #region IUserProfileProvider Members

        public string Category
        {
            get { return "General"; }
        }

        public IEnumerable<string> GetFieldNames()
        {
            return new[]
            {
                PreferredLanguage
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
            var languageService = workContext.Resolve<ILanguageService>();

            var languages = languageService.GetActiveLanguages();

            if (languages.Count() < 2)
            {
                return Enumerable.Empty<ControlFormAttribute>();
            }

            string preferredLanguageId = membershipService.GetProfileEntry(userId, PreferredLanguage);
            string selectedValue = null;

            if (!string.IsNullOrEmpty(preferredLanguageId))
            {
                var preferredLanguage = languages.FirstOrDefault(x => x.Id.ToString() == preferredLanguageId);

                if (preferredLanguage != null)
                {
                    selectedValue = preferredLanguageId;
                }
            }

            var selectList = languages.ToSelectList(value => value.Id.ToString(), text => text.Name);

            return new ControlFormAttribute[]
            {
                new ControlChoiceAttribute(ControlChoice.DropDownList)
                {
                    Name = PreferredLanguage,
                    LabelText = "Preferred Language",
                    SelectListItems = selectList,
                    PropertyType = typeof(string),
                    Value = selectedValue
                }
            };
        }

        #endregion IUserProfileProvider Members
    }
}