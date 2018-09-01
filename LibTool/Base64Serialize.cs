using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DaiChong.Lib.Util
{
    public class Base64Serialize<T>
    {
        public string Serialize(T obj)
        {
            string s = string.Empty;
            using (Stream fs = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
                byte[] dt = new byte[fs.Length];
                fs.Read(dt, 0, (int)fs.Length);
                s = Convert.ToBase64String(dt);
            }
            return s;
        }

        public T DeSerialize(string base64)
        {
            byte[] dt = Convert.FromBase64String(base64);
            T t;
            using (Stream fs = new MemoryStream(dt))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                t = (T)formatter.Deserialize(fs);
            }
            return t;
        }
    }
}
