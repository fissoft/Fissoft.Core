using System;
namespace Fissoft.MedicalDispatch
{
    public class GetDayOrTime
    {
        /// <summary>  
        /// 获取指定日期的月份第一天  
        /// </summary>  
        /// <param name="dateTime"></param>  
        /// <returns></returns>  
        public static DateTime GetDateTimeMonthFirstDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
        /// <summary>  
        /// 获取指定月份最后一天  
        /// </summary>  
        /// <param name="dateTime"></param>  
        /// <returns></returns>  
        public static DateTime GetDateTimeMonthLastDay(DateTime dateTime)
        {
            var day = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, day);
        }
    }
}
