using System;

namespace BoxKite.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortTimeString(this DateTime date)
        {
            return string.Format("h:mm tt", date);
        }

        public static string ToFriendlyText(this DateTime pastTime, string friendlyDistance)
        {
            return ToFriendlyText(pastTime, DateTime.Now) + " " + friendlyDistance;
        }

        public static string ToFriendlyText(this DateTime pastTime)
        {
            return ToFriendlyText(pastTime, DateTime.Now);
        }

        public static string ToFriendlyText(this DateTime pastTime, DateTime currentTime)
        {
            var timeSince = currentTime - pastTime;
            if (timeSince > new TimeSpan(24, 0, 0))
            {
                if (timeSince < new TimeSpan(48, 0, 0))
                {
                    return "Yesterday, " + pastTime.ToShortTimeString();
                }

                return pastTime.ToString();
            }
            if (timeSince < new TimeSpan(0, 1, 0))
            {
                return "< 1 minute ago";
            }

            if (timeSince < new TimeSpan(1, 0, 0))
            {
                return timeSince > new TimeSpan(0, 1, 59)
                           ? timeSince.Minutes + " minutes ago"
                           : timeSince.Minutes + " minute ago";
            }

            if (timeSince < new TimeSpan(23, 0, 0))
            {
                return timeSince > new TimeSpan(1, 59, 59)
                           ? timeSince.Hours + " hours ago"
                           : timeSince.Hours + " hour ago";
            }
            return pastTime.ToShortTimeString();
        }

        public static string FormatDayOfMonth(this DateTime dateTime)
        {
            var dayOfMonth = dateTime.Day;

            if (dayOfMonth == 1 || dayOfMonth == 21 || dayOfMonth == 31)
                return string.Format("{0}st", dayOfMonth);

            if (dayOfMonth == 2 || dayOfMonth == 22)
                return string.Format("{0}nd", dayOfMonth);

            if (dayOfMonth == 3 || dayOfMonth == 23)
                return string.Format("{0}rd", dayOfMonth);

            return string.Format("{0}th", dayOfMonth);
        }
    }

}
