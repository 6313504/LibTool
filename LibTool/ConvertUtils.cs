using System;

namespace DaiChong.Lib.Util
{
    public class ConvertUtils
    {
        public static decimal ToDecimal(object obj)
        {
            decimal d = 0m;
            if (obj != null)
            {
                Decimal.TryParse(obj.ToString(), out d);
            }
            return d;
        }

        public static long ToInt64(object obj)
        {
            long d = 0;
            if (obj != null)
            {
                Int64.TryParse(obj.ToString(), out d);
            }
            return d;
        }

        public static int ToInt(object obj)
        {
            int d = 0;
            if (obj != null)
            {
                Int32.TryParse(obj.ToString(), out d);
            }
            return d;
        }

        public static string ToString(object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 将数字变成时间 (new Date).getTime()
        /// </summary>
        /// <param name="ltime"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(long ltime)
        {
            long Eticks = (long)(ltime * 10000) + 621355968000000000;
            DateTime dt = new DateTime(Eticks).ToLocalTime();
            return dt;
        }

        /// <summary>
        /// 将时间变成数字
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetLongFromTime(DateTime dt)
        {
            DateTime dt1 = dt.ToUniversalTime();
            return (dt1.Ticks - 621355968000000000) / 10000;
        }

        public static long GetLongFromTime()
        {
            return GetLongFromTime(DateTime.Now);
        }
 
        //public static int GetIntFromTime(DateTime time)
        //{
        //    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        //    return (int)(time - startTime).TotalSeconds;
        //}
    }
}
