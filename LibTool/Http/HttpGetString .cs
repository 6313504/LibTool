using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LibTool.Http
{
    public class HttpGetString : HttpGetMethodBase<string>
    {
        protected override string WebResponse(IAsyncResult result)
        {
            try
            {
                var request = result.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                #region ignore
                if (response.Cookies != null)
                {
                    foreach (Cookie cookie in response.Cookies)
                    {
                        Debug.WriteLine(cookie.Value);
                    }
                }
                Debug.WriteLine(response.ContentType);
                Debug.WriteLine(response.StatusDescription);
                if (response.Headers["Set-Cookie"] != null)
                {
                    //setting may write
                    Debug.WriteLine(response.Headers["Set-Cookie"]);
                }
                #endregion
                Stream stream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
            catch
            {
                Debug.WriteLine("WEBERROR");
                return null;
            }
        }
    }
}
