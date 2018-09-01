using System;
using System.Collections.Generic;
using System.Text;

namespace DaiChong.Lib.Util
{
    public static class RandomUtil
    {
        private static long tick = DateTime.Now.Ticks;

        private static Random r = new Random((int)(DateTime.Now.Ticks & 0xffffffffL) | (int)(tick >> 32));

        public static int GetInt(int start, int end)
        {
            return r.Next(start, end);
        }

        public static double GetDouble()
        {
            return r.NextDouble();
        }
    }
}
