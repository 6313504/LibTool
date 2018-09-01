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
                URL = "https://www.baidu.com",
                Encode = Encoding.GetEncoding("gb2312") 
            };

              web.Post();

        }

       
    }
}
