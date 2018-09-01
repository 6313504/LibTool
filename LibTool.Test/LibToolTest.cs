using DaiChong.Lib.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Xunit;

namespace LibTool.Test
{
    public class LibToolTest
    {
        public LibToolTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void PayTest()
        {

            var web = new Submit
            {
                URL = "http://localhost/api/receive/AnYun",
                Encode = Encoding.GetEncoding("gb2312"),
                GetParameters = new Dictionary<string, string> {
                    { "P_ErrMsg","Ï£Íû"}
                }
            };

              web.Post();

        }

       
    }
}
