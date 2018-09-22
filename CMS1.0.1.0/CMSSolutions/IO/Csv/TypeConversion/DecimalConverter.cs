﻿using System.Globalization;

namespace CMSSolutions.IO.Csv.TypeConversion
{
    /// <summary>
    /// Converts a Decimal to and from a string.
    /// </summary>
    public class DecimalConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="culture">The culture used when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(CultureInfo culture, string text)
        {
            decimal d;
            if (decimal.TryParse(text, NumberStyles.Float, culture, out d))
            {
                return d;
            }

            return base.ConvertFromString(culture, text);
        }

        /// <summary>
        /// Determines whether this instance [can convert from] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can convert from] the specified type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(System.Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }
}