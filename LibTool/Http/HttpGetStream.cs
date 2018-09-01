using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace LibTool.Http
{
    public class HttpGetStream : HttpGetMethodBase<Stream>
    {
        protected override Stream WebResponse(IAsyncResult result)
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
                return response.GetResponseStream();
            }
            catch
            {
                Debug.WriteLine("WEBERROR");
                return null;
            }
        }
    }
}
