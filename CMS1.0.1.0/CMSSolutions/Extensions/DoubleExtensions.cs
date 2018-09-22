namespace CMSSolutions.Extensions
{
    public static class DoubleExtensions
    {
        public static bool IsNullOrDefault(this double? value)
        {
            return value == null || value == default(double);
        }
    }
}