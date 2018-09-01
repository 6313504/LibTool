using System;
using System.Text;

namespace LibTool
{
    public class Base64Util
    {
        public static string EncodeBase64(Encoding encode, string source)
        {
            byte[] bytes = encode.GetBytes(source);
            String resutl = "";
            try
            {
                resutl = Convert.ToBase64String(bytes);
            }
            catch
            {
                resutl = source;
            }
            return resutl;
        }

        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }
}
