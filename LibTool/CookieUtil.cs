using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DaiChong.Lib.Util
{
    public class CookieUtil
    {
        public static string GetCookieValue(CookieContainer cookies, string name)
        {
            List<Cookie> lst = GetAllCookies(cookies);
            foreach (Cookie cookie in lst)
            {
                if (cookie.Name == name)
                    return cookie.Value;
            }
            return "";
        }

        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) 
                        lstCookies.Add(c);
            }

            return lstCookies;
        }

        public static void RemoveDomain(CookieContainer cc,string domain)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            object _key = null;
            foreach (object key in table.Keys)
            {
                if (key.ToString() == domain)
                {
                    _key = key;
                    break;
                }
            }
            if (_key != null)
            {
                table.Remove(_key);
            }
        }

        public static string GetAllCookieString(CookieContainer cc)
        {
            var cooklist = GetAllCookies(cc);
            StringBuilder sbc = new StringBuilder();

            foreach (Cookie cookie in cooklist)
            {
                sbc.AppendFormat("{0};{1};{2};{3};{4};{5}\r\n",
                    cookie.Domain, cookie.Name, cookie.Path, cookie.Port,
                    cookie.Secure.ToString(), cookie.Value);
            }
            return sbc.ToString();
        }

        public static CookieContainer GetCookieContainer(string strCookie)
        {
            CookieContainer cookieContainer = new CookieContainer();
            string[] cookies = strCookie.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string c in cookies)
            {
                string[] cc = c.Split(";".ToCharArray());
                Cookie ck = new Cookie(); ;
                ck.Discard = false;
                ck.Domain = cc[0];
                ck.Expired = true;
                ck.HttpOnly = true;
                ck.Name = cc[1];
                ck.Path = cc[2];
                if (cc.Length > 3)
                {
                    ck.Port = cc[3];
                }
                if (cc.Length > 4)
                {
                    ck.Secure = bool.Parse(cc[4]);
                }
                if (cc.Length > 5)
                {
                    ck.Value = cc[5];
                }
                cookieContainer.Add(ck);
            }
            return cookieContainer;
        }

        private readonly object lockobject = new object();

        public void SaveCookie(string filename, CookieContainer cookie)
        {
            lock (lockobject)
            { 
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookie);
 
                }
                catch (Exception)
                {
                   
                }
            }
            }
        }

        public CookieContainer ReadCookieByFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }
            lock (lockobject)
            {
                try
                {
                    using (Stream stream = File.Open(filename, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        return (CookieContainer)formatter.Deserialize(stream);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
  
        public  void DeleteCookie(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    lock (lockobject)
                    {
                        File.Delete(filename);
                    }
                }
            }
            catch (Exception)
            {
            }  
        } 
    }
}
