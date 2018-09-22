using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class FormButtonAttribute : ActionMethodSelectorAttribute
    {
        public FormButtonAttribute(string buttonName)
        {
            ButtonName = buttonName;
            BindValue = "id";
        }

        #region Overrides of ActionMethodSelectorAttribute

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var valueProviderResult = controllerContext.Controller.ValueProvider.GetValue(ButtonName);
            bool hasValue = (valueProviderResult != null);

            if (hasValue && !string.IsNullOrWhiteSpace(BindValue))
            {
                ((IList)controllerContext.Controller.ValueProvider).Add(new ButtonValueProvider(
                    valueProviderResult.RawValue,
                    valueProviderResult.AttemptedValue,
                    BindValue));
            }

            return hasValue;
        }

        #endregion Overrides of ActionMethodSelectorAttribute

        public string ButtonName { get; set; }

        public string BindValue { get; set; }


        private class ButtonValueProvider : IValueProvider
        {
            private readonly object rawValue;
            private readonly string attemptedValue;
            private readonly string bindValue;

            public ButtonValueProvider(object rawValue, string attemptedValue, string bindValue)
            {
                this.rawValue = rawValue;
                this.attemptedValue = attemptedValue;
                this.bindValue = bindValue;
            }

            public bool ContainsPrefix(string prefix)
            {
                return prefix == bindValue;
            }

            public ValueProviderResult GetValue(string key)
            {
                if (key == bindValue)
                {
                    return new ValueProviderResult(rawValue, attemptedValue, CultureInfo.InvariantCulture);
                }

                return null;
            }
        }
    }
}