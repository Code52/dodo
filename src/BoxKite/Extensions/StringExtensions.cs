using System;
using System.Globalization;

namespace BoxKite.Extensions
{
    public static class StringExtensions
    {
        public static DateTime ParseDateTime(this string date)
        {
            // Sun Apr 15 02:31:50 +0000 2012

            var tokenizer = date.Split(new[] { ' ' });

            var dayOfWeek = tokenizer[0];
            var month = tokenizer[1];
            var dayInMonth = tokenizer[2];
            var time = tokenizer[3];
            var offset = tokenizer[4];
            var year = tokenizer[5];

            string dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);

            DateTime ret = DateTime.Parse(dateTime);
            return ret;
        }

        // based on http://stackoverflow.com/a/4975344
        public static DateTimeOffset ToDateTimeOffset(this string date)
        {
            // Sun Apr 15 02:31:50 +0000 2012
            DateTimeOffset timestamp;
            if (DateTimeOffset.TryParseExact(date, "ddd MMM dd HH:mm:ss K yyyy", null, DateTimeStyles.None, out timestamp))
            {
                return timestamp;
            }

            // Sun, 15 Apr 2012 02:38:21 +0000
            if (DateTimeOffset.TryParseExact(date, "ddd, dd MMM yyyy HH:mm:ss K", null, DateTimeStyles.None, out timestamp))
            {
                return timestamp;
            }

            return timestamp;
        }
    }
}
