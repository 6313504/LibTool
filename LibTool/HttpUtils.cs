using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace DaiChong.Lib.Util
{
    public static class HttpUtils
    {
        /// <summary>
        /// 获得使用转发服务器下的用户真实ip
        /// </summary>
        /// <returns></returns>
        //public static string GetFormIpByTransmit(HttpContext context)
        //{
        //    string ret = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (!string.IsNullOrEmpty(ret))
        //    {
        //        return ret;
        //    }

        //    ret = context.Request.Headers["X-Client-Address"];
        //    if (string.IsNullOrEmpty(ret))
        //    {
        //        ret = context.Request.UserHostAddress;
        //    }
        //    else
        //    {
        //        // 如果客户端
        //        if (ret.IndexOf(",") >= 0)
        //        {
        //            string[] ss = ret.Split(new string[] { "," }, StringSplitOptions.None);
        //            ret = ss[ss.Length - 1];
        //        }
        //    }
        //    return ret;
        //}


        //public static string GetIP(HttpRequest request)
        //{
        //    string result = String.Empty;
        //    result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        //可能有代理 
        //        if (result.IndexOf(".") == -1)    //没有"."肯定是非IPv4格式 
        //            result = null;
        //        else
        //        {
        //            if (result.IndexOf(",") != -1)
        //            {
        //                //有","，估计多个代理。取第一个不是内网的IP。 
        //                result = result.Replace(" ", "").Replace("\"", "");
        //                string[] temparyip = result.Split(",;".ToCharArray());
        //                for (int i = 0; i < temparyip.Length; i++)
        //                {
        //                    if (IsIPAddress(temparyip[i])
        //                        && temparyip[i].Substring(0, 3) != "10."
        //                        && temparyip[i].Substring(0, 7) != "192.168"
        //                        && temparyip[i].Substring(0, 7) != "172.16.")
        //                    {
        //                        return temparyip[i];    //找到不是内网的地址 
        //                    }
        //                }
        //            }
        //            else if (IsIPAddress(result)) //代理即是IP格式 
        //                return result;
        //            else
        //                result = null;    //代理中的内容 非IP，取IP 
        //        }
        //    }

        //    if (string.IsNullOrEmpty(result) && request.ServerVariables["REMOTE_ADDR"] != null)
        //        result = request.ServerVariables["REMOTE_ADDR"].ToString();
        //    if (string.IsNullOrEmpty(result))
        //        result = request.UserHostAddress;
        //    return result;
        //}

        public static bool IsIPAddress(string ipStr)
        {
            if (ipStr == null || ipStr == string.Empty || ipStr.Length < 7 || ipStr.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(ipStr);
        }

    }
}
