using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DaiChong.Lib.Util
{

    /// <summary>
    /// 系统配置文件实体类
    /// </summary>
    [Serializable()]
    public class ConfigInfo
    {
        /// <summary>
        /// 版本号key
        /// </summary>
        public const string MVersionNumberKey = "VersionNumber";
        /// <summary>
        /// 文本文件内容分割符号        |   {[,]}   |
        /// </summary>
        public const string MTxtCompart = "|   {[,]}   |";

        /// <summary>
        /// 版本号,每次更新系统配置的时间
        /// </summary>
        public DateTime VersionNumber = DefaultDateTime;

        public const string MSystemConfigInfoCryptoKey = "~07%0^1z";

        private Dictionary<string, string> mAppSettings;

        /// <summary>
        /// 系统配置项列表
        /// </summary>
        public Dictionary<string, string> AppSettings
        {
            get { if (mAppSettings == null) mAppSettings = new Dictionary<string, string>(); return mAppSettings; }
            set { this.mAppSettings = value; }
        }

        /// <summary>
        /// 根据key获得系统配置项value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            key = key.Trim().ToLower();
            if (AppSettings.ContainsKey(key))
            {
                return AppSettings[key];
            }

            return string.Empty;
            
        }

        public int GetAppSettingCount()
        {
            return AppSettings.Count;
        }

        /// <summary>
        /// 默认时间
        /// </summary>
        public static DateTime DefaultDateTime = DateTime.Parse("1000-01-01 00:00:00");

        /// <summary>
        /// 将当前实例转换为文本文件内容
        /// </summary>
        /// <returns></returns>
        public string ToTxtContext()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("{0}{1}{2}\r\n"
                ,MVersionNumberKey.Trim().ToLower()
                ,MTxtCompart
                , VersionNumber.ToString("yyyy-MM-dd HH:mm:ss")
                ));

            foreach (string k in AppSettings.Keys)
            {
                sb.Append(string.Format("{0}{1}{2}\r\n"
                , k.Trim().ToLower()
                , MTxtCompart
                , AppSettings[k]
                ));
            }

            string ret = CryptoUtils.Encrypt2(sb.ToString(), MSystemConfigInfoCryptoKey);
            return ret;
        }

        public static ConfigInfo GetConfigInfoByTxtFile(string file)
        {
            ConfigInfo ci = new ConfigInfo();
            string[] strs = null;
            string key = string.Empty;
            string value = string.Empty;
            StreamReader sr = null;

            try
            {
                sr = File.OpenText(file);
                string temp = sr.ReadToEnd();
                temp = CryptoUtils.Decrypt2(temp, MSystemConfigInfoCryptoKey);
                string[] list = temp.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach(string txt in list)
                {
                    strs = txt.Split(new string[] { MTxtCompart }, StringSplitOptions.None);
                    if (strs != null && strs.Length == 2)
                    {
                        key = strs[0].Trim().ToLower();
                        value = strs[1];
                        if (key.Trim().ToLower() == MVersionNumberKey.Trim().ToLower())
                        {
                            ci.VersionNumber = DateTime.Parse(value);
                        }
                        else
                        {
                            ci.AppSettings.Add(key, value);
                        }
                    }
                }
            }
            catch  
            {
                
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }

            return ci;
        }

    }
}
