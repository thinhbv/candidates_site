using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CMSSolutions
{
    public static class Verify
    {
        public static bool IsPassword(this string input)
        {
            string pattern = @"^(?=.*[0-9])(?=.*[!@#$%^&*])[0-9a-zA-Z!@#$%^&*0-9]{8,}$";
            return Regex.IsMatch(input, pattern);
        }

        public static bool AreAllNullOrEmpty(IEnumerable<string> values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreAllNullOrEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreAnyNullOrEmpty(IEnumerable<string> values)
        {
            foreach (string value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AreAnyNullOrEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Any(params bool[] values)
        {
            foreach (bool value in values)
            {
                if (value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}