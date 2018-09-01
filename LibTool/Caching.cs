//using System;
//using System.Web;
//using System.Web.Caching;

//namespace DaiChong.Lib.Util
//{
//    public class Caching
//    {
//        /// <summary>
//        /// 插入chche
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="obj"></param>
//        /// <param name="fileFullName"></param>
//        /// <param name="absoluteMinute">缓存时间：单位=分钟</param>
//        public static void PutCacheDependencyFile(string key, object obj, string fileFullName, int absoluteMinute)
//        {
//            CacheDependency fileDepends = new CacheDependency(fileFullName);
//            HttpRuntime.Cache.Insert(key, obj, fileDepends, DateTime.Now.AddMinutes(absoluteMinute), TimeSpan.Zero);
//        }

//        /// <summary>
//        /// 插入cache
//        /// </summary>
//        /// <param name="key">cache的key</param>
//        /// <param name="obj">cache的value</param>
//        /// <param name="second">缓存时间【秒】</param>
//        public static void PutCacheDependencyTime(string key, object obj, int second)
//        {
//            HttpRuntime.Cache.Insert(key, obj, null, DateTime.Now.AddSeconds((double)second), TimeSpan.Zero);
//        }

//        /// <summary>
//        /// 移除cache
//        /// </summary>
//        /// <param name="key"></param>
//        public static void RemoveCache(string key)
//        {
//            HttpRuntime.Cache.Remove(key);
//        }

//        /// <summary>
//        /// 获得cache
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public static object GetCache(string key)
//        {
//            return HttpRuntime.Cache[key];
//        }
//    }
//}
