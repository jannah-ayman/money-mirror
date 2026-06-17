using System;

namespace MoneyMirror.Core.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo EgyptTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

        /// <summary>
        /// Gets the current date and time converted to Egypt Standard Time.
        /// </summary>
        public static DateTime EgyptNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);
    }
}
