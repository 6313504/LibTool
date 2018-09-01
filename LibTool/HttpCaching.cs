//using System;
//using System.Collections.Generic;
//using System.Web;
//using System.Web.Caching;

//namespace DaiChong.Lib.Util
//{
//    public class HttpCaching
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
//            if (HttpContext.Current != null)
//            {
//                CacheDependency fileDepends = new CacheDependency(fileFullName);
//                HttpContext.Current.Cache.Insert(key, obj, fileDepends, DateTime.Now.AddMinutes(absoluteMinute), TimeSpan.Zero);
//            }
//            else
//            {
//                if (Caches == null)
//                {
//                    Caches = new Dictionary<string, CacheTimeObject>();
//                }
//                if (Caches.ContainsKey(key))
//                {
//                    Caches[key] = new CacheTimeObject { InsertTime = DateTime.Now, CacheMinute = (double)absoluteMinute, CacheObject = obj };
//                }
//                else 
//                {
//                    Caches.Add(key, new CacheTimeObject { InsertTime = DateTime.Now, CacheMinute = (double)absoluteMinute, CacheObject = obj });
//                }
//            }
//        }

//        /// <summary>
//        /// 插入cache
//        /// </summary>
//        /// <param name="key">cache的key</param>
//        /// <param name="obj">cache的value</param>
//        /// <param name="second">缓存时间【秒】</param>
//        public static void PutCacheDependencyTime(string key, object obj, int second)
//        {
//            if (HttpContext.Current != null)
//            {
//                HttpContext.Current.Cache.Insert(key, obj, null, DateTime.Now.AddSeconds((double)second), TimeSpan.Zero);
//            }
//            else
//            {
//                if (Caches == null)
//                {
//                    Caches = new Dictionary<string, CacheTimeObject>();
//                }
//                if (Caches.ContainsKey(key))
//                {
//                    Caches[key] = new CacheTimeObject { InsertTime = DateTime.Now, CacheMinute = (double)second, CacheObject = obj };
//                }
//                else 
//                {
//                    Caches.Add(key, new CacheTimeObject { InsertTime = DateTime.Now, CacheMinute = (double)second, CacheObject = obj });
//                }
//            }
//        }

//        /// <summary>
//        /// 移除cache
//        /// </summary>
//        /// <param name="key"></param>
//        public static void RemoveCache(string key)
//        {
//            if (HttpContext.Current != null)
//            {
//                HttpContext.Current.Cache.Remove(key);
//            }
//            else
//            {
//                if (Caches != null && Caches.ContainsKey(key))
//                {
//                    Caches.Remove(key);
//                }
//            }
//        }

//        /// <summary>
//        /// 获得cache
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public static object GetCache(string key)
//        {
//            if (HttpContext.Current != null)
//            {
//                return HttpContext.Current.Cache[key];
//            }
//            else
//            {
//                if (Caches == null)
//                    return null;
//                if (!Caches.ContainsKey(key))
//                    return null;
//                var obj = Caches[key] as CacheTimeObject;

//                if ((obj.InsertTime.AddMinutes(obj.CacheMinute) - DateTime.Now).TotalSeconds > 0)
//                {
//                    return obj.CacheObject;
//                }
//                else
//                {
//                    Caches.Remove(key);
//                    return null;
//                }
//            }
//        }

//        public static Dictionary<string, CacheTimeObject> Caches { get; set; }

//    }

//    public class CacheTimeObject
//    {
//        public DateTime InsertTime { get; set; }

//        public double CacheMinute { get; set; }

//        public object CacheObject { get; set; }
//    }
//}
