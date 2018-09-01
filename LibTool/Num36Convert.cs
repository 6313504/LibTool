using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTool
{
    public  static class Num36Convert
    {
        private const string X36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //10进制转换成36进制
        public static string ConvertTo36(int val,int length)
        {
            string result = "";
            while (val >= 36)
            {
                result = X36[val % 36] + result;
                val /= 36;
            }
            if (val >= 0) result = X36[val] + result;
            if (result.Length < length) {
                result = result.PadLeft(length).Replace(' ', '0');
            }
            return result;
        }

        //36进制转换成10进制
        public static int ConvertTo10(string str)
        {
            str = str.ToUpper();
            int result = 0;
            int len = str.Length;
            for (int i = len; i > 0; i--)
            {
                result += X36.IndexOf(str[i - 1]) * Convert.ToInt32(Math.Pow(36, len - i));
            }
            return result;
        }

    }
}
