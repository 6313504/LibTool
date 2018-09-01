using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace DaiChong.Lib.Util
{
    /// <summary>
    /// 字符串处理类
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// 获取表单中所有的input
        /// </summary>
        /// <param name="html"></param>
        public static Dictionary<string, string> GetData(string text, char[] split)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            StringBuilder sb = new StringBuilder();
            foreach (char c in split)
            {
                sb.Append(c.ToString());
            }

            MatchCollection matches = Regex.Matches(text, @"(?<key>[^=" + sb.ToString() + "]+)=(?<value>[^" + sb.ToString() + "]*)", RegexOptions.IgnoreCase);
            foreach (Match ma in matches)
            {
                string key = ma.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(key))
                {
                    string _value2 = ma.Groups[2].Value.Trim();
                    if (dict.ContainsKey(key))
                    {
                        if (dict[key] != _value2 && string.IsNullOrEmpty(dict[key]) && !string.IsNullOrEmpty(_value2))
                        {
                            dict[key] = _value2;
                        }
                    }
                    else
                    {
                        dict.Add(key, _value2);
                    }
                }
            }
            return dict;
        }

        public static string GetForm(string url, Dictionary<string, string> data, string method, string formId, bool isAutoSubmit)
        {
            StringBuilder formHtml = new StringBuilder();
            formHtml.AppendFormat("<form id=\"{0}\"method=\"{1}\" action=\"{2}\">", formId, method, url);
            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    formHtml.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", key, data[key]);
                }
            }
            formHtml.Append("</form>");

            if (isAutoSubmit)
            {
                formHtml.Append("<script>document.forms[0].submit();</script> ");
            }
            return formHtml.ToString();
        }

        public static string GetForm(string url, Encoding encoding, String method, IDictionary<string, string> _data = null)
        {
            StringBuilder formHtml = new StringBuilder();
            String[] _url = url.Split('?');
            if (_url.Length == 2)
            {
                formHtml.AppendFormat("<form id=\"autoSubmit\" name=\"autoSubmit\"  method=\"{1}\" action=\"{0}\">", _url[0], method.ToUpper());
            }
            else
            {
                formHtml.AppendFormat("<form id=\"autoSubmit\" name=\"autoSubmit\"  method=\"{1}\" action=\"{0}\">", url, method.ToUpper());
            }

            if (_url.Length == 2)
            {
                Dictionary<string, string> data = GetData(_url[1], new char[] { '&' });
                if (data != null)
                {
                    foreach (string key in data.Keys)
                    {
                        formHtml.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", key, HttpUtility.UrlDecode(data[key], encoding));
                    }
                }
            }

            if (_data != null && _data.Count > 0)
            {
                foreach (string key in _data.Keys)
                {
                    formHtml.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", key, _data[key]);
                }
            }

            formHtml.Append("<input type='submit' value='submit' style='display:none;'>");
            formHtml.Append("</form><script>document.forms[\"autoSubmit\"].submit();</script> ");
            return formHtml.ToString();
        }

        public static string EncodeUrl(string url, Encoding enc)
        {
            int indexOf = url.IndexOf('?');
            string domian = url.Substring(0, indexOf);
            string para = url.Substring(indexOf+1, url.Length - indexOf-1);
            Dictionary<string, string> dic = StringUtils.GetData(para, new[] { '&'});
            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            foreach (var key in dic.Keys)
            {
               dic2.Add(key, HttpUtility.UrlEncode(dic[key], enc));
            }
            para = StringUtils.GetString(dic2);
            return $"{domian}?{para}";
        }

        public static string GetString(IDictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in data.Keys)
            {
                sb.AppendFormat("{0}={1}&", key, data[key]);
            }
            return sb.ToString().TrimEnd('&');
        }


        public static string GetString(Dictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in data.Keys)
            {
                sb.AppendFormat("{0}={1}&", key, data[key]);
            }
            return sb.ToString().TrimEnd('&');
        }

        public static string GetString(System.Collections.Specialized.NameValueCollection formvalue)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in formvalue.Keys)
            {
                sb.AppendFormat("{0}={1}&", key, formvalue[key]);
            }
            return sb.ToString().TrimEnd('&');
        }

        public static string GetString(SortedDictionary<int, string> data, string format)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int key in data.Keys)
            {
                if (string.IsNullOrEmpty(format))
                    sb.AppendFormat("{0}", data[key]);
                else
                    sb.AppendFormat("{0}{1}", data[key], format);
            }

            //return sb.ToString().TrimEnd('&');
            string str = sb.ToString();
            if (!string.IsNullOrEmpty(format))
                str = str.Substring(0, str.LastIndexOf(format));
            return str;
        }

        public static string GetFromBase64Str(string str, Encoding enc)
        {
            string rstr = "";
            try
            {
                byte[] inputBuffer = Convert.FromBase64String(str);
                rstr = enc.GetString(inputBuffer);
            }
            catch
            {

            }
            return rstr;
        }

      


        public static String GetString(SortedDictionary<int, string[]> dic)
        {
            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            foreach (string[] item in dic.Values)
            {
                dic2.Add(item[0], item[1]);
            }
            return GetString(dic2);
        }

        /// <summary>
        /// 1.直连 2.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format">1."" 2. "^" 3. ";"</param>
        /// <returns></returns>
        public static string GetString(IDictionary<string, string> data, string format)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in data.Keys)
            {
                if (string.IsNullOrEmpty(format))
                    sb.AppendFormat("{0}", data[key]);
                else
                    sb.AppendFormat("{0}{1}", data[key], format);
            }

            //return sb.ToString().TrimEnd('&');
            string str = sb.ToString();
            if (!string.IsNullOrEmpty(format))
                str = str.Substring(0, str.LastIndexOf(format));
            return str;
        }

        public static string GetGB2312String(string context)
        {
            string temp = string.Empty;
            byte[] buf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(context);
            temp = System.Text.Encoding.GetEncoding("gb2312").GetString(buf, 0, buf.Length);
            return temp;
        }
        /**
       * 生成随机串，随机串包含字母或数字
       * @return 随机串
       */
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }


        public static string BuildString(params string[] strings)
        {
            //return string.Join("", strings);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {
                sb.Append(strings[i]);
            }

            return sb.ToString();
        }



        public static string BoolToDBStr(bool flag)
        {
            if (flag)
            {
                return "1";
            }
            return "0";
        }

        public static string BoolToDBStr(object objFlag)
        {
            bool flag = false;
            try
            {
                flag = bool.Parse(objFlag.ToString());
            }
            catch
            {
                flag = false;
            }
            return BoolToDBStr(flag);
        }

        public static bool CheckString(string str, bool nullYN, string strCharList)
        {
            if (nullYN && ((str == null) || (str == "")))
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (strCharList.IndexOf(str.Substring(i, 1)) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static string CleanNonWord(string text)
        {
            return Regex.Replace(text, @"\W", string.Empty);
        }

        public static string FillStr(int iLen, string strTemp, char fillChar, bool bAtBefore)
        {
            string str = string.Empty;
            int length = strTemp.Length;
            if (length >= iLen)
            {
                return strTemp;
            }
            length = iLen - length;
            for (int i = 0; i < length; i++)
            {
                str = str + fillChar.ToString();
            }
            if (bAtBefore)
            {
                return (str + strTemp);
            }
            return (strTemp + str);
        }

        public static string FormatCurrency(object amt)
        {
            return FormatCurrency(amt, true, 2);
        }

        public static string FormatCurrency(object amt, bool thousandSep)
        {
            return FormatCurrency(amt, thousandSep, 2);
        }

        public static string FormatCurrency(object amt, bool thousandSep, int precision)
        {
            if (amt == null)
            {
                return "0";
            }
            try
            {
                return decimal.Parse(amt.ToString()).ToString("F");
            }
            catch
            {
                return "0";
            }
        }

        public static string FormatIP(string inputIP)
        {
            string str2 = "";
            string str3 = "000.000.000.000";
            string[] strArray = inputIP.Split(new char[] { '.' });
            if (strArray.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    int num = ToInt(strArray[i], -1);
                    if ((num < 0) || (num > 0xff))
                    {
                        return str3;
                    }
                    string str = "00" + num.ToString();
                    str2 = str2 + str.Substring(str.Length - 3, 3) + ".";
                }
                return str2.Substring(0, str2.Length - 1);
            }
            return str3;
        }

        public static string InputText(string text, int maxLength)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }
            text = Regex.Replace(text, @"[\s]{2,}", " ");
            text = Regex.Replace(text, @"(<[b|B][r|R]/*>)+|(<[p|P](.|\n)*?>)", "\n");
            text = Regex.Replace(text, @"(\s*&[n|N][b|B][s|S][p|P];\s*)+", " ");
            text = Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
            text = text.Replace("'", "''");
            return text;
        }

        /// <summary>
        /// 获得英文、数字_组成的串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetEngligSring(string text)
        {
            return Regex.Replace(text, "[^a-z^A-Z^0-9^-_ ]", string.Empty, RegexOptions.None);
        }

        /// <summary>
        /// 验证长度为count数字组成的串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool GetNumberSring(string text, int count)
        {
            string rep = @"\d{" + count + "}";
            return Regex.IsMatch(text, rep);
        }


        /// <summary>
        /// 验证英文、数字组成的串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ValidateEngligOrNumberSring(string text)
        {
            bool ret = false;
            ret = !Regex.IsMatch(text, "[^a-z^A-Z^0-9^]");
            return ret;
        }

        public static bool IsEmailFormat(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static bool IsFloat(string strInputNumber, bool boolNull)
        {
            if (strInputNumber.IndexOf("-") >= 0)
            {
                return false;
            }
            string[] strArray = strInputNumber.Split(new char[] { '.' });
            if (strArray.Length > 2)
            {
                return false;
            }
            if (strArray.Length == 2)
            {
                return (IsNumber(strArray[0], boolNull) && IsNumber(strArray[1], boolNull));
            }
            return IsNumber(strArray[0], boolNull);
        }

        public static bool IsNumber(string strInputNumber, bool boolNull)
        {
            if (strInputNumber.LastIndexOf('-') > 0)
            {
                return false;
            }
            return CheckString(strInputNumber, boolNull, "-1234567890");
        }

        public static string NotNullStr(object canNullStr)
        {
            return NotNullStr(canNullStr, string.Empty);
        }

        public static string NotNullStr(object canNullStr, string defaultStr)
        {
            try
            {
                if (canNullStr == null)
                {
                    if (defaultStr != null)
                    {
                        return defaultStr;
                    }
                    return "";
                }
                return Convert.ToString(canNullStr);
            }
            catch
            {
                return defaultStr;
            }
        }

        public static string QuotedToDBStr(object str)
        {
            return QuotedToDBStr(str, "");
        }

        public static string QuotedToDBStr(string str)
        {
            if (str == null)
            {
                return "null";
            }
            return ("'" + str.Replace("'", "''") + "'");
        }

        public static string QuotedToDBStr(object str, string defaultStr)
        {
            if ((str == null) || (str is DBNull))
            {
                if (defaultStr == null)
                {
                    return "null";
                }
                return QuotedToDBStr(str.ToString());
            }
            return QuotedToDBStr(str.ToString());
        }

        public static string RepeatChar(char c, int count)
        {
            string str = "";
            for (int i = 0; i < count; i++)
            {
                str = str + c.ToString();
            }
            return str;
        }

        public static string RepeatChar(string c, int count)
        {
            string str = "";
            for (int i = 0; i < count; i++)
            {
                str = str + c;
            }
            return str;
        }

        public static string SimpleIP(string inputIP)
        {
            string str = "";
            string str2 = "0.0.0.0";
            string[] strArray = inputIP.Split(new char[] { '.' });
            if (strArray.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    int num = ToInt(strArray[i], -1);
                    if ((num < 0) || (num > 0xff))
                    {
                        return str2;
                    }
                    str = str + num.ToString() + ".";
                }
                return str.Substring(0, str.Length - 1);
            }
            return str2;
        }

        public static bool ToBoolean(object objBool)
        {
            return ToBoolean(objBool, false);
        }

        public static bool ToBoolean(object objBool, bool defaultValue)
        {
            if (objBool == null)
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToBoolean(objBool);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string ToDBStr(object str)
        {
            return ToDBStr(str, string.Empty);
        }

        public static string ToDBStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return str.Replace("'", "''");
        }

        public static string ToDBStr(object str, string defaultStr)
        {
            if ((str == null) || (str is DBNull))
            {
                if (defaultStr == null)
                {
                    return string.Empty;
                }
                return ToDBStr(str.ToString());
            }
            return ToDBStr(str.ToString());
        }

        public static string ToDBStr(object str, int strLength)
        {
            if ((str == null) || (str is DBNull))
            {
                return string.Empty;
            }

            string temp = GetSubstring(str, strLength);
            return ToDBStr(temp);
        }

        public static decimal ToDecimal(object objdecimal)
        {
            return ToDecimal(objdecimal, 0M);
        }

        public static decimal ToDecimal(object objdecimal, decimal defaultValue)
        {
            if (objdecimal == null)
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToDecimal(objdecimal);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float ToFloat(object objFloat)
        {
            return ToFloat(objFloat, 0f);
        }

        public static float ToFloat(object objFloat, float defaultValue)
        {
            if (objFloat == null)
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToSingle(objFloat);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt(object objInt)
        {
            return ToInt(objInt, 0);
        }

        public static int ToInt(object objInt, int defaultValue)
        {
            int num;
            try
            {
                if (objInt == null)
                {
                    num = defaultValue;
                }
                else
                {
                    try
                    {
                        num = Convert.ToInt32(objInt);
                    }
                    catch
                    {
                        num = defaultValue;
                    }
                }
            }
            catch
            {
                num = defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 将各格式为yyyyMMddHh的字符串转换为时间对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string str, string format)
        {
            string strTemp = string.Empty;
            // 当前要查询的时段
            strTemp = string.Format("{0}-{1}-{2} {3}:00:00"
                , str.Substring(0, 4)
                , str.Substring(4, 2)
                , str.Substring(6, 2)
                , str.Substring(8, 2));
            return DateTime.Parse(strTemp);
        }

        public static DateTime ToDateTime(string str)
        {
            return DateTime.Parse(str);
        }


        public static string[] GetStrings(string str, string sign)
        {
            return str.Split(new string[] { sign }, StringSplitOptions.None);
        }

        public static bool VaildIP(string inputIP)
        {
            if (!string.IsNullOrEmpty(inputIP))
            {
                string[] strArray = inputIP.Split(new char[] { '.' });
                if (strArray.Length == 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int num = ToInt(strArray[i], -1);
                        if ((num < 0) || (num > 0xff))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static string GetSubstring(object objString, int subLength)
        {
            string temp = NotNullStr(objString).Trim();
            if (temp.Length > subLength)
                return temp.Substring(0, subLength);
            else
                return temp;

        }


        /// <summary>
        /// 过滤HTML代码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ParseHtmlALL(string html)
        {
            //html = ParseHtml(html);
            html = Regex.Replace(html, "<[^>]*>", "");
            return html;
        }

        /// <summary>
        /// 过滤HTML代码后,截取字符串
        /// </summary>
        /// <param name="html"></param>
        /// <param name="subLength">过滤后截取长度</param>
        /// <returns></returns>
        public static string ParseHtmlALL(string html, int subLength)
        {
            //html = ParseHtml(html);
            html = ParseHtmlALL(html);

            return GetSubstring(html, subLength);
        }

        //public static string ConvertHtml(string html, System.Web.UI.Page page)
        //{
        //    return page.Server.HtmlEncode(html);
        //}



        /// <summary>
        /// 格式化字符串，不足长度补零
        /// 如：sourceStr = 12
        ///     strLength = 6
        ///     结果=000012
        /// </summary>
        /// <param name="strLength"></param>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string FormatRandomString(int strLength, int sourceStr)
        {
            StringBuilder sb = new StringBuilder();
            int zc = strLength - sourceStr.ToString().Length;
            for (int i = 0; i < zc; i++)
            {
                sb.Append("0");
            }
            sb.Append(sourceStr.ToString());
            return sb.ToString();

        }

        /// <summary>
        /// 判断是否可以处理订单
        /// </summary>
        /// <param name="minute">相差时间（分）</param>
        /// <param name="d">某某时间</param>
        /// <returns></returns>
        public static bool IsActionOrder(int minute, DateTime d)
        {
            DateTime date = DateTime.Now;
            TimeSpan ts = d - date;//获得一个时间和当前的时间差
            double a = ts.TotalMinutes;//获得时间差的分钟
            int b = Math.Abs(Convert.ToInt32(a));//转换绝对值
            if (b > minute)//判断是否超过某某时间
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///   取随机数 
        /// </summary>
        /// <param name="length">取多少位</param>
        /// <returns>返回长度为length的字符串（整数）</returns>
        public static string BuildRandomStr(int length)
        {
            Random rand = new Random();

            int num = rand.Next();

            string str = num.ToString();

            if (str.Length > length)
            {
                str = str.Substring(0, length);
            }
            else if (str.Length < length)
            {
                int n = length - str.Length;
                while (n > 0)
                {
                    str.Insert(0, "0");
                    n--;
                }
            }

            return str;
        }

        public static string GetUrlEnCode(string context, Encoding encode)
        {
            return HttpUtility.UrlEncode(context, encode);
        }

        private static Encoding DefaultEncoding = System.Text.Encoding.GetEncoding("gb2312");
        public static string GetUrlEnCode(string context)
        {
            return HttpUtility.UrlEncode(context, DefaultEncoding);
        }

        public static string ConvertXmlString(string str)
        {
            Regex r = new Regex("[\u0000-\u0008\u000B\u000C\u000E-\u001F\uD800-\uDFFF\uFFFE\uFFFF]", RegexOptions.Compiled);
            return r.Replace(str, "");
        }

        /// <summary>
        /// 判断两个域名是否是同一个顶级域名
        /// </summary>
        /// <param name="loclurl"></param>
        /// <param name="getUrl"></param>
        /// <returns></returns>
        public static bool GetUrlIsSameUrls(string loclurl, string getUrl)
        {
            loclurl = loclurl.Replace("http://", "");
            string[] str1 = loclurl.Split('.');
            string[] str2 = getUrl.Split('.');

            if ((str2[str2.Length - 1] != str1[str1.Length - 1]) || (str1[str1.Length - 2] != str2[str2.Length - 2]))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 得到商户提交的顶级域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrl(string url)
        {
            url.Replace("http://", "");
            string[] temp = url.Split('.');
            string strTemp = string.Empty;
            for (int i = 0; i < temp.Length; i++)
            {

                if (i == temp.Length - 2)
                {
                    strTemp += temp[i] + ".";
                }
                if (i == temp.Length - 1)
                {
                    strTemp += temp[i];
                }
            }
            return strTemp;
        }

        #region 将字母，数字由全角转化为半角
        /// <summary>
        /// 将字母，数字由全角转化为半角
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>半角字符串</returns>
        public static string ChangeStrToDBC(string str)
        {
            string s = str;
            char[] c = s.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            //半角  
            string news = new string(c);
            return news;
        }
        #endregion

        public static XmlDocument GetXmlDocument(string xmlContext)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xmlContext);
            }
            catch
            {
                return null;
            }

            return doc;
        }

        public static string GetXmlDocumentNodeAttributesValue(XmlNode rootNode, string attributesName, string attributesNameValue, string attributesValueName)
        {
            string ret = string.Empty;

            try
            {
                XmlNode node = rootNode;

                int count = node.ChildNodes.Count;
                for (int i = 0; i < count; i++)
                {
                    XmlNode Activenode = node.ChildNodes.Item(i);

                    XmlAttribute xaName = Activenode.Attributes[attributesName];
                    if (xaName.Value.Trim().ToLower() == attributesNameValue.Trim().ToLower())
                    {
                        XmlAttribute xaValue = Activenode.Attributes[attributesValueName];
                        ret = xaValue.Value;
                        break;
                    }
                }
            }
            catch
            {

                ret = string.Empty;
            }


            return ret;
        }
        public static string GetXmlDocumentNodeValue(XmlDocument doc, string searchNodeName)
        {
            string ret = string.Empty;
            try
            {

                XmlNode node = doc.DocumentElement;
                int count = node.ChildNodes.Count;
                for (int i = 0; i < count; i++)
                {
                    XmlNode Activenode = node.ChildNodes.Item(i);
                    if (Activenode.Name.Trim().ToLower() == searchNodeName.Trim().ToLower())
                    {
                        ret = Activenode.InnerText.Trim();
                        break;
                    }
                }
            }
            catch
            {
                return "";
            }

            return ret;
        }

        public static CookieContainer GetCookieContainerByResponse(HttpWebResponse response, string domain)
        {
            string cookieStr = response.Headers["Set-Cookie"];

            CookieContainer cookie = new CookieContainer();

            MatchCollection cs = Regex.Matches(cookieStr, @"([^=;,\s]+)=([^;]+);(\s+(path|expires)=([^;]+);?)*\s+(HttpOnly)?");
            foreach (Match ma in cs)
            {

                MatchCollection cs2 = Regex.Matches(ma.Groups[0].Value, @"([^=;,\s]+)=([^;]+)");
                string key = cs2[0].Groups[1].Value;
                string value = cs2[0].Groups[2].Value;
                Cookie ck = new Cookie(key, value);
                ck.Domain = domain;
                if (ma.Groups[0].Value.Contains("HttpOnly"))
                {
                    ck.HttpOnly = true;
                }
                foreach (Match ma2 in cs2)
                {
                    string _key = ma2.Groups[1].Value;
                    if (_key == "path")
                    {
                        ck.Path = ma2.Groups[2].Value;
                    }
                    if (_key == "expires")
                    {
                        DateTime expires = DateTime.Now.AddYears(1);

                        if (DateTime.TryParse(ma2.Groups[2].Value, out expires))
                        {
                            ck.Expires = expires;
                        }
                    }
                }
                if (ck.Value.Length < 1000)
                {
                    cookie.Add(ck);
                }

            }
            return cookie;
        }

        public static CookieContainer ToCookieContainer(string cookieStr, string domain)
        {
            cookieStr = cookieStr.Replace(",", "%2C");
            CookieContainer cookie = new CookieContainer();

            string[] cookstr = cookieStr.Split(';');
            foreach (string str in cookstr)
            {
                string[] nameValue = str.Split('=');

                if (nameValue.Length == 1)
                {
                    Cookie ck = new Cookie(nameValue[0].Trim().ToString(), string.Empty);
                    ck.Domain = domain;
                    cookie.Add(ck);
                }
                else
                {
                    Cookie ck = new Cookie(nameValue[0].Trim().ToString(), nameValue[1].Trim().ToString());
                    ck.Domain = domain;
                    cookie.Add(ck);
                }
            }

            return cookie;
        }


        /// <summary>
        ///   取随机数 
        /// </summary>
        /// <param name="length">取多少位</param>
        /// <returns>返回长度为length的字符串（整数）</returns>
        public static string RandomString(int length, string _str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
        {
            long tick = DateTime.Now.Ticks;
            Random r = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(_str[r.Next(_str.Length)]);
            }
            return sb.ToString();
        }

        public static string RandomString(int st, int end)
        {

            long tick = DateTime.Now.Ticks;
            Random r = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            return r.Next(st, end).ToString();
        }

        /// <summary>
        /// 汉字转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(string str)
        {
            byte[] bts = Encoding.Unicode.GetBytes(str);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2) r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            return r;
        }

        /// <summary>
        /// 将Unicode编码转换为汉字字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        public static string ToGB2312(string str)
        {
            string r = "";
            MatchCollection mc = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            byte[] bts = new byte[2];
            foreach (Match m in mc)
            {
                bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                r += Encoding.Unicode.GetString(bts);
            }
            return r;
        }

        public static string Unicode2Gb(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            MatchCollection mc = Regex.Matches(text, "\\\\u([\\w]{4})");
            foreach (Match item in mc)
            {
                string _a = item.Groups[0].Value;
                try
                {
                    string _b = Convert.ToChar(int.Parse(_a.Substring(2, 4), NumberStyles.HexNumber)).ToString();
                    text = text.Replace(_a, _b);
                }
                catch { }
            }
            return text;
        }
    }
}
