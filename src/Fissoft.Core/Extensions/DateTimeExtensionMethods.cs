using System;
using System.Text;

namespace Fissoft.Framework.Systems
{
    public static class DateTimeExtensionMethods
    {
        public static string ToW3CDate(this DateTime dt)
        {
            return dt.ToUniversalTime().ToString("s") + "Z";
        }

        /// <summary>
        /// Returns a nicely formatted duration
        /// eg. 3 seconds ago, 4 hours ago etc.
        /// </summary>
        /// <param name="dateTime">The datetime value</param>
        /// <returns>A nicely formatted duration</returns>
        /// <see cref="http://samscode.com/index.php/2009/12/timespan-or-datetime-to-friendly-duration-text-e-g-3-days-ago/"/>
        public static string TimeAgoString(this DateTime dateTime)
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan timespan = DateTime.Now - dateTime;

            // A year or more?  Do "[Y] years and [M] months ago"
            if ((int)timespan.TotalDays >= 365)
            {
                // Years
                int nYears = (int)timespan.TotalDays / 365;
                sb.Append(nYears);
                if (nYears > 1)
                    sb.Append(" 年");
                else
                    sb.Append(" 年");

                // Months
                int remainingDays = (int)timespan.TotalDays - (nYears * 365);
                int nMonths = remainingDays / 30;
                if (nMonths == 1)
                    sb.Append(" ").Append(nMonths).Append(" 个月");
                else if (nMonths > 1)
                    sb.Append(" ").Append(nMonths).Append(" 个月");
            }
            // More than 60 days? (appx 2 months or 8 weeks)
            else if ((int)timespan.TotalDays >= 60)
            {
                // Do months
                int nMonths = (int)timespan.TotalDays / 30;
                sb.Append(nMonths).Append(" 个月");
            }
            // Weeks? (7 days or more)
            else if ((int)timespan.TotalDays >= 7)
            {
                int nWeeks = (int)timespan.TotalDays / 7;
                sb.Append(nWeeks);
                if (nWeeks == 1)
                    sb.Append(" 周");
                else
                    sb.Append(" 周");
            }
            // Days? (1 or more)
            else if ((int)timespan.TotalDays >= 1)
            {
                int nDays = (int)timespan.TotalDays;
                sb.Append(nDays);
                if (nDays == 1)
                    sb.Append(" 天");
                else
                    sb.Append(" 天");
            }
            // Hours?
            else if ((int)timespan.TotalHours >= 1)
            {
                int nHours = (int)timespan.TotalHours;
                sb.Append(nHours);
                if (nHours == 1)
                    sb.Append(" 小时");
                else
                    sb.Append(" 小时");
            }
            // Minutes?
            else if ((int)timespan.TotalMinutes >= 1)
            {
                int nMinutes = (int)timespan.TotalMinutes;
                sb.Append(nMinutes);
                if (nMinutes == 1)
                    sb.Append(" 分钟");
                else
                    sb.Append(" 分钟");
            }
            // Seconds?
            else if ((int)timespan.TotalSeconds >= 1)
            {
                int nSeconds = (int)timespan.TotalSeconds;
                sb.Append(nSeconds);
                if (nSeconds == 1)
                    sb.Append(" 秒");
                else
                    sb.Append(" 秒");
            }
            // Just say "1 second" as the smallest unit of time
            else
            {
                sb.Append("1 秒");
            }

            sb.Append(" 前");

            // For anything more than 6 months back, put " ([Month] [Year])" at the end, for better reference
            if ((int)timespan.TotalDays >= 30 * 6)
            {
                sb.Append(" (" + dateTime.ToString("MMMM") + " " + dateTime.Year + ")");
            }

            return sb.ToString();
        }

        ///   <summary>   
        ///   得指定日期的指定格式间隔
        ///   </summary>   
        ///   <param   name="dt1">日期1</param>   
        ///   <param   name="dt2">日期2</param>   
        ///   <param   name="dateformat">间隔格式: y:年 M:月 d:天 h:小时 m:分钟 s:秒 fff:毫秒 ffffff:微妙 fffffff:100毫微妙</param>   
        ///   <returns>间隔   long型</returns>   
        public static long GetInterval(this DateTime dt1, DateTime dt2, string dateformat)
        {
            try
            {
                long interval = dt1.Ticks - dt2.Ticks;
                DateTime dt11;
                DateTime dt22;

                switch (dateformat)
                {
                    case "fffffff"://100毫微妙   
                        break;
                    case "ffffff"://微妙   
                        interval /= 10;
                        break;
                    case "fff"://毫秒   
                        interval /= 10000;
                        break;
                    case "s"://秒   
                        interval /= 10000000;
                        break;
                    case "m"://分钟   
                        interval /= 600000000;
                        break;
                    case "h"://小时   
                        interval /= 36000000000;
                        break;
                    case "d"://天   
                        interval /= 864000000000;
                        break;
                    case "M"://月   
                        dt11 = (dt1.CompareTo(dt2) >= 0) ? dt2 : dt1;
                        dt22 = (dt1.CompareTo(dt2) >= 0) ? dt1 : dt2;
                        interval = -1;
                        while (dt22.CompareTo(dt11) >= 0)
                        {
                            interval++;
                            dt11 = dt11.AddMonths(1);
                        }
                        break;
                    case "y"://年   
                        dt11 = (dt1.CompareTo(dt2) >= 0) ? dt2 : dt1;
                        dt22 = (dt1.CompareTo(dt2) >= 0) ? dt1 : dt2;
                        interval = -1;
                        while (dt22.CompareTo(dt11) >= 0)
                        {
                            interval++;
                            dt11 = dt11.AddMonths(1);
                        }
                        interval /= 12;
                        break;
                }
                return interval;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return 0;
            }
        }

    }
}