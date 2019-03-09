using DaiChong.Lib.Type;
using DaiChong.Lib.Util;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace DaiChong.Lib.Http
{

    /// <summary>
    /// 发送HTTP请求的类
    /// </summary>
    public class Submit
    {
        //private Proxy ProxyHttp { get; set; }

        //public void SetProxy(Proxy proxy)
        //{
        //    if (proxy != null && !string.IsNullOrEmpty(proxy.ProxyIp))
        //    {
        //        ReqProxy = true;
        //        this.ProxyHttp = proxy;
        //        ProxyAddress = proxy.ProxyIp;
        //        UseDefaultCredentialsOnly = proxy.HasPwd;
        //    }
        //}

        #region - 变量 -

        WebProxy proxy = null;

        private string proxyString = string.Empty;

        private string forWord = string.Empty;

        private int timeOut = 25;

        private int readWriteTimeout = 20;

        private int delay = 5;

        protected Uri uri;

        protected string url;

        #endregion

        #region - 属性 -

        private bool allowAutoRedirect = true;
        public bool AllowAutoRedirect
        {
            get { return allowAutoRedirect; }
            set { allowAutoRedirect = value; }
        }

        public static UserAgentType UserAgentType { get; set; }

        public string Accept { get; set; }

        public string Accept_Charset { get; set; }

        public string Accept_Encoding { get; set; }

        public string Accept_Language { get; set; }

        public string User_Agent { get; set; }

        public string ContentType { get; set; }

        public static string ProxyUserName = "";

        public static string ProxyPassword = "";

        public static bool IsGetDomainIp = false;

        public static bool IsGetExceptionResponse = false;

        public static bool UseDefaultCredentials = false;

        public static bool Expect100Continue = true;

        public bool UseDefaultCredentialsOnly = false;

        public System.Version HttpVersion { get; set; }

        /// <summary>
        /// 请求返回的HTML
        /// </summary>
        public string Html { get; set; }

        public PostFormat PostFormat { get; set; }

        /// <summary>
        /// 代理地址
        /// </summary>
        public string ProxyAddress
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    proxy = new WebProxy(value);
                    if (IsForWord)
                    {
                        forWord = value;
                    }
                }
                else
                {
                    proxy = null;
                }
                proxyString = value;
            }
            get
            {
                return proxyString;
            }
        }

        /// <summary>
        /// 是否需要代理
        /// </summary>
        public bool ReqProxy { get; set; }

        /// <summary>
        /// 是否是伪装代理
        /// </summary>
        public bool IsForWord { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public int TimeOut
        {
            set
            {
                timeOut = value;
            }
            get
            {
                return timeOut;
            }
        }

        public int ReadWriteTimeout
        {
            set
            {
                readWriteTimeout = value;
            }
            get
            {
                return readWriteTimeout;
            }
        }

        public int Delay
        {
            set
            {
                delay = value;
            }
            get
            {
                return delay;
            }
        }


        /// <summary>
        /// 设置要请求的URL
        /// </summary>
        public string URL
        {
            set { url = value; uri = new Uri(value); }
            get { return url; }
        }

        public string Referer
        {
            set;
            get;
        }

        private Encoding encode = Encoding.Default;
        public Encoding Encode
        {
            set { encode = value; }
            get { return encode; }
        }

        public CookieContainer Cookies
        {
            get;
            set;
        }

        public HttpWebRequest Request
        {
            get;
            set;
        }

        public IDictionary<string, string> GetParameters
        {
            get;
            set;
        }

        public IDictionary<string, string> PostParameters
        {
            get;
            set;
        }

        #endregion

        #region - 方法 -

        void SetProxy()
        {
            #region 伪造代理
            if (IsForWord)
            {
                if (!string.IsNullOrEmpty(forWord))
                {
                    Request.Headers.Add("X_FORWARDED_FOR", forWord);
                    // Request.Headers.Add("HTTP_X_FORWARDED_FOR", forWord);
                }
            }
            #endregion

            #region 设置代理
            else
            {
                //代理
                if (proxy != null && ReqProxy)
                {
                    if (!string.IsNullOrEmpty(ProxyUserName) && !string.IsNullOrEmpty(ProxyPassword))
                    {
                        proxy.Credentials = new NetworkCredential(ProxyUserName, ProxyPassword);
                    }
                    Request.Proxy = proxy;
                    Request.ServicePoint.Expect100Continue = false;
                }
            }
            #endregion

            if (Submit.UseDefaultCredentials || UseDefaultCredentialsOnly)
            {
                Request.UseDefaultCredentials = true;
            }

        }

        private String GetUrl()
        {
            if (GetParameters != null && GetParameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + WebUtils.BuildPostData(GetParameters, Encode);
                }
                else
                {
                    url = url + "?" + WebUtils.BuildPostData(GetParameters, Encode);
                }
            }
            if (url.EndsWith("?"))
            {
                url = url.TrimEnd('?');
            }
            return url;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="method">请求方式(GET/POST)</param>
        private HttpWebResponse SetResponse()
        {
            try
            {
                Response = (HttpWebResponse)Request.GetResponse();
            }
            catch (WebException ex)
            {
                if (IsGetExceptionResponse)
                {
                    if (ex.Response == null)
                    {
                        throw;
                    }
                    return (HttpWebResponse)ex.Response;
                }

                if (ex.Message == "操作超时" || ex.Message == "The operation has timed out")
                {
                    Ping p1 = new Ping();
                    string ip = string.Empty;
                    try
                    {
                        ip = p1.Send(Request.RequestUri.Host).Address.ToString();
                    }
                    catch
                    {
                        throw new WebException(ex.Message + "ping不到IP");
                    }
                    throw new WebException(ex.Message + "ip为：" + ip);
                }
                throw;
            }
            catch
            {
                throw;
            }

            return Response;
        }

        ///// <summary>
        ///// 发送请求
        ///// </summary>
        ///// <param name="method">请求方式(GET/POST)</param>
        //private async Task<WebResponse> SetResponseAsync()
        //{
        //    try
        //    {

        //        //Response = (await Task.Factory.FromAsync(Request.BeginGetResponse, Request.EndGetResponse, Request)) as HttpWebResponse;
        //    }
        //    catch (WebException ex)
        //    {
        //        if (IsGetExceptionResponse)
        //        {
        //            if (ex.Response == null)
        //            {
        //                throw ex;
        //            }
        //            Response = (HttpWebResponse)ex.Response;
        //        }

        //        if (ex.Message == "操作超时" || ex.Message == "The operation has timed out")
        //        {
        //            Ping p1 = new Ping();
        //            string ip = string.Empty;
        //            try
        //            {
        //                ip = (await p1.SendPingAsync(Request.RequestUri.Host)).Address.ToString();
        //            }
        //            catch
        //            {
        //                throw new WebException(ex.Message + "ping不到IP");
        //            }
        //            throw new WebException(ex.Message + "ip为：" + ip);
        //        }
        //        throw ex;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return Response;
        //}

        //protected virtual async Task SetRequestAsync(HttpMethodType method)
        //{
        //    BeforeSetRequest(method);

        //    #region POST
        //    if (method == HttpMethodType.POST)
        //    {
        //        PostParameters = PostParameters ?? new Dictionary<string, string>();
        //        byte[] postData = null;

        //        if (PostFormat == PostFormat.Common)
        //        {
        //            postData = Encode.GetBytes(GetPostString());
        //        }
        //        else
        //        {
        //            if (PostParameters != null && PostParameters.ContainsKey("Content") && !String.IsNullOrEmpty(PostParameters["Content"]))
        //            {
        //                postData = Encode.GetBytes(PostParameters["Content"]);
        //            }
        //            if (!String.IsNullOrEmpty(PostContent))
        //            {
        //                postData = Encode.GetBytes(PostContent);
        //            }
        //        }

        //        Request.Method = "POST";
        //        Request.ContentLength = postData.Length;

        //        Task<Stream> requestStream = Task<Stream>.Factory.FromAsync(Request.BeginGetRequestStream, Request.EndGetRequestStream, Request);
        //        await requestStream.Result.WriteAsync(postData, 0, postData.Length);
        //    }
        //    #endregion
        //}

        protected void SetRequest(HttpMethodType method)
        {
            BeforeSetRequest(method);

            #region POST
            if (method == HttpMethodType.POST)
            {
                PostParameters = PostParameters ?? new Dictionary<string, string>();
                byte[] postData = null;

                if (PostFormat == PostFormat.Common)
                {
                    postData = Encode.GetBytes(GetPostString());
                }
                else
                {
                    if (PostParameters != null && PostParameters.ContainsKey("Content") && !String.IsNullOrEmpty(PostParameters["Content"]))
                    {
                        postData = Encode.GetBytes(PostParameters["Content"]);
                    }
                    if (!String.IsNullOrEmpty(PostContent))
                    {
                        postData = Encode.GetBytes(PostContent);
                    }
                }

                Request.Method = "POST";
                Request.ContentLength = postData.Length;

                Stream newStream = Request.GetRequestStream();
                newStream.Write(postData, 0, postData.Length);
                newStream.Close();
                //newStream.Dispose();
            }
            #endregion
        }

        private void BeforeSetRequest(HttpMethodType method)
        {

            #region 设置URL
            URL = GetUrl();
            #endregion

            #region 超时等待时间
            Request = (HttpWebRequest)WebRequest.Create(uri);
            Request.Timeout = TimeOut * 1000;
            //超时等待时间
            Request.ReadWriteTimeout = ReadWriteTimeout * 1000;
            #endregion

            #region Https
            if (uri.ToString().ToLower().StartsWith("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                Request.ProtocolVersion = System.Net.HttpVersion.Version11;
            }
            #endregion

            #region 设置代理
            SetProxy();
            #endregion

            if (!string.IsNullOrEmpty(Referer))
            {
                Request.Referer = Referer;
            }
            Request.CookieContainer = Cookies;
            Request.KeepAlive = true;
            Request.AllowAutoRedirect = AllowAutoRedirect;
            Request.ServicePoint.Expect100Continue = Expect100Continue;
            if (HttpVersion != null)
            {
                Request.ProtocolVersion = HttpVersion;
            }

            Thread.Sleep(delay);

            SetHead(method);
        }

        public static bool IsUpper { get; set; }

        private string GetPostString()
        {
            if (Request.ContentType.ToLower().Contains("multipart/form-data"))
            {
                string sp = "--" + Regex.Match(Request.ContentType, @"boundary=(.*)").Groups[1].Value;
                StringBuilder sb = new StringBuilder();

                IEnumerator<KeyValuePair<string, string>> dem = this.PostParameters.GetEnumerator();
                while (dem.MoveNext())
                {
                    string name = dem.Current.Key;
                    string value = dem.Current.Value;
                    // 忽略参数名或参数值为空的参数
                    if (!string.IsNullOrEmpty(name))
                    {
                        sb.AppendLine(sp);
                        sb.AppendLine(string.Format(@"Content-Disposition: form-data; name=""{0}""", name));
                        sb.AppendLine("");
                        sb.AppendLine(value);

                    }
                }
                sb.AppendLine(sp + "--");
                return sb.ToString();
            }

            return WebUtils.BuildPostData(PostParameters, Encode, IsUpper);
        }

        public HttpWebResponse Response { get; set; }

        protected void SetHead(HttpMethodType method)
        {

            if (method == HttpMethodType.POST)
            {
                Request.ContentType = "application/x-www-form-urlencoded";
            }
            if (!string.IsNullOrEmpty(ContentType))
            {
                Request.ContentType = ContentType;
            }

            if (!string.IsNullOrEmpty(Accept))
            {
                Request.Accept = Accept;
            }
            if (!string.IsNullOrEmpty(Accept_Charset))
            {
                Request.Headers.Add("Accept-Charset", Accept_Charset);
            }

            if (!string.IsNullOrEmpty(Accept_Encoding))
            {
                Request.Headers.Add("Accept-Encoding", Accept_Encoding);
            }
            else
            {
                if (method == HttpMethodType.POST)
                {
                    Request.Headers.Add("Accept-Encoding", "gzip,deflate");
                }
            }
            if (!string.IsNullOrEmpty(Accept_Language))
            {
                Request.Headers.Add("Accept-Language", Accept_Language);
            }
            else
            {
                if (method == HttpMethodType.POST)
                {
                    Request.Headers.Add("Accept-Language", "zh-cn");
                }
            }

            if (Headers != null)
            {
                if (Headers.Count > 0)
                {
                    foreach (string key in Headers.Keys)
                    {
                        Request.Headers.Add(key, Headers[key]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(CookieString))
            {
                Request.Headers.Add("Cookie", CookieString);
            }

            switch (UserAgentType)
            {
                case Type.UserAgentType.IE6:
                    Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                    break;
                case Type.UserAgentType.IE7:
                    Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
                    break;
                case Type.UserAgentType.FireFox5:
                    Request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:5.0.1) Gecko/20100101 Firefox/5.0.1";
                    break;
                case Type.UserAgentType.WebKit:
                    Request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.805.0 Safari/535.1";
                    break;
                case Type.UserAgentType.Ios5:
                    Request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3";
                    break;
                case Type.UserAgentType.IpadMini:
                    Request.UserAgent = "Mozilla/5.0 (iPad; CPU OS 4_3_5 like Mac OS X; en-us) AppleWebKit/533.17.9 (KHTML, like Gecko) Version/5.0.2 Mobile/8L1 Safari/6533.18.5";
                    break;
                case Type.UserAgentType.Ipad:
                    Request.UserAgent = "Mozilla/5.0 (iPad; CPU OS 7_0 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Version/7.0 Mobile/11A465 Safari/9537.53";
                    break;
                case Type.UserAgentType.Iphone6p:
                    Request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4";
                    break;
            }
            if (!string.IsNullOrEmpty(User_Agent))
            {
                Request.UserAgent = User_Agent;
            }
        }

        private bool HasKey(string[] keys, string key)
        {
            bool hasKey = false;
            foreach (var _key in keys)
            {
                if (_key == key)
                {
                    hasKey = true;
                    break;
                }
            }
            return hasKey;
        }

        //https
        protected bool CheckValidationResult(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors errors)
        { // Always accept
            return true;
        }

        public TimeSpan GetTime { get; set; }

      
        public void Get()
        {
            Html = string.Empty;
            DateTime now = DateTime.Now;
            SetRequest(HttpMethodType.GET);
            SetResponse();
            SetNewUrl();
            SetHtmlFromResponse();
            GetTime = DateTime.Now - now;
        }

        public TimeSpan PostTime { get; set; }

     
        public void Post()
        {
            Html = string.Empty;
            DateTime now = DateTime.Now;
            SetRequest(HttpMethodType.POST);
            SetResponse();
            SetNewUrl();
            SetHtmlFromResponse();
            PostTime = DateTime.Now - now;
        }

        //public Image GetImage()
        //{
        //    Process(HttpMethodType.GET);
        //    return GetImageFromResponse();
        //}

        //public Image PostImage()
        //{
        //    Process(HttpMethodType.POST);
        //    return GetImageFromResponse();
        //}

       

        public void GetFileFromResponse(HttpMethodType method, string path, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                if (Response.Headers["Content-Disposition"] != null)
                {
                    filename = Regex.Match(Response.Headers["Content-Disposition"], @"filename=(\S+)").Groups[1].Value;
                }
                else
                {
                    filename = Guid.NewGuid().ToString();
                }
            }

            using (Response)
            {
                using (Stream stream = Response.GetResponseStream())
                {
                    using (FileStream fileStream = File.Create(path + filename))
                    {
                        //建立字节组，并设置它的大小是多少字节
                        byte[] bytes = new byte[102400];
                        int n = 1;
                        while (n > 0)
                        {
                            //一次从流中读多少字节，并把值赋给Ｎ，当读完后，Ｎ为０,并退出循环
                            n = stream.Read(bytes, 0, 10240);
                            fileStream.Write(bytes, 0, n); //将指定字节的流信息写入文件流中
                        }
                    }
                }
            }

        }

        [Obsolete("DownloadMp3Stream方法已过时")]
        public Stream DownloadMp3Stream(HttpMethodType method)
        {
            using (Response)
            {
                if (Response.Headers["Content-Type"] == "audio/mp3")
                {
                    return Response.GetResponseStream();
                }
            }
            return null;
        }

        private void SetNewUrl()
        {
            if (!string.IsNullOrEmpty(Response.Headers["location"]))
            {
                if (Response.Headers["location"].ToLower().StartsWith("http"))
                {
                    this.URL = Response.Headers["location"];
                }
                else
                {
                    if (!string.IsNullOrEmpty(Domain) && !Response.Headers["location"].ToLower().StartsWith("http"))
                    {
                        this.URL = Domain + Response.Headers["location"];
                    }
                }
            }
        }

        public Submit Process(HttpMethodType method)
        {
            SetRequest(method);
            SetResponse();
            SetNewUrl();
            return this;
        }

        //public async Task<WebResponse> ProcessAsync(HttpMethodType method)
        //{
        //    await SetRequestAsync(method);
        //    var response = await Request.GetResponseAsync();
        //    SetNewUrl();
        //    return response;
        //}

        private void SetHtmlFromResponse()
        {
            Html = WebUtils.GetResponseAsString(Response, Encode);
        }

        //private async Task SetHtmlFromResponseAsync()
        //{
        //    Html = await WebUtils.GetResponseAsStringAsync(Response, Encode);
        //}

        public String GetHtmlFromResponse()
        {
            return WebUtils.GetResponseAsString(Response, Encode);
        }

        //public Task<String> GetHtmlFromResponseAsync(HttpWebResponse response)
        //{
        //    return WebUtils.GetResponseAsStringAsync(response, Encode);
        //}

        ///// <summary>
        ///// 把响应流转换为文本。
        ///// </summary>
        ///// <param name="rsp">响应流对象</param>
        ///// <param name="encoding">编码方式</param>
        ///// <returns>响应文本</returns>
        //public string GetResponseAsStringAsync(WebResponse rsp)
        //{
        //    StringBuilder result = new StringBuilder();

        //    Stream stream = rsp.GetResponseStream();
        //    if ((rsp as HttpWebResponse).ContentEncoding.ToLower().Contains("gzip"))
        //    {
        //        stream = new GZipStream(stream, CompressionMode.Decompress);
        //    }

        //    using (StreamReader reader = new StreamReader(stream, Encode))
        //    {
        //        if (rsp.ContentLength > 0)
        //        {
        //            // 每次读取不大于256个字符，并写入字符串
        //            char[] buffer = new char[256];
        //            int readBytes = 0;

        //            while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                result.Append(buffer, 0, readBytes);
        //            }
        //        }
        //        else //Chunked 编码
        //        {
        //            while (!reader.EndOfStream)
        //                result.Append((char)reader.Read());
        //        }
        //    }

        //    return result.ToString();
        //}

        public Stream GetStreamFromResponse()
        {
            return Response.GetResponseStream();
        }
 
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(url))
            {
                sb.AppendLine(url);
                sb.AppendLine();
            }

            if (GetParameters != null)
            {
                sb.AppendLine(WebUtils.BuildPostData(GetParameters, Encode));
                sb.AppendLine();
            }

            if (PostParameters != null)
            {
                sb.AppendLine();
                sb.AppendLine(WebUtils.BuildPostData(PostParameters, Encode));
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(Html))
            {
                sb.AppendLine(Html);
            }

            return sb.ToString();
        }

        public void Init()
        {
            url = string.Empty;
            uri = null;
            Referer = string.Empty;
            PostParameters = null;
            GetParameters = null;
        }

        public void Save(string filepath, bool islog)
        {
            using (FileStream fst = new FileStream(filepath, FileMode.Create))
            {
                StreamWriter swt = new StreamWriter(fst, this.Encode);

                if (islog)
                {
                    if (!string.IsNullOrEmpty(this.ProxyAddress) && this.ReqProxy)
                    {
                        swt.WriteLine("PROXY:" + this.ProxyAddress);
                        swt.WriteLine();
                    }
                    swt.WriteLine("URL:" + this.URL);
                    swt.WriteLine();
                    swt.WriteLine("REFER:" + this.Referer);
                    swt.WriteLine();

                    if (this.PostParameters != null)
                    {
                        foreach (string key in this.PostParameters.Keys)
                        {
                            swt.WriteLine(key + ":" + this.PostParameters[key]);
                        }
                    }
                    if (this.GetParameters != null)
                    {
                        foreach (string key in this.GetParameters.Keys)
                        {
                            swt.WriteLine(key + ":" + this.GetParameters[key]);
                        }
                    }
                    swt.WriteLine();
                }
                swt.WriteLine(this.Html);
                swt.Flush();
            }

        }
        #endregion

        public string CookieString { get; set; }

        public string Domain { get; set; }
        public string PostContent { get; set; }

        public static Submit GetFromStream(string stream)
        {
            var web = new Submit();

            //string step2 = @"POST /service/pay/multiRecharge.action HTTP/1.1
            //X-Requested-With: XMLHttpRequest
            //X-HttpWatch-RID: 7201-10075
            //Content-Type: application/x-www-form-urlencoded; charset=UTF-8
            //Accept: text/html, */*; q=0.01
            //Referer: http://bj.189.cn/service/pay/vcRechargeInit.action
            //Accept-Language: zh-CN
            //Accept-Encoding: gzip, deflate
            //User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko
            //Host: bj.189.cn
            //Content-Length: 264
            //Connection: Keep-Alive
            //Cookie: s_pers=%20s_fid%3D749744E1BCDBBB02-357D5D9DED141146%7C1498113289045%3B; nvid=1; JSESSIONID_bj=BQKpVHnKzL0cxHpRYWpN7TT1LGJwVK2DdL820XhyP2QLQ7t8fVgR!1273647049
            //webCardTemporary.logFlag=0&webCardTemporary.phoneNum=&webCardTemporary.directNumNew=&webCardTemporary.billNo=&webCardTemporary.directFlag=0&webCardTemporary.singlePW=&webCardTemporary.rechargedNbrType=&shijian=Mon Jun 22 2015 14:47:32 GMT+0800 (涓浗鏍囧噯鏃堕棿)";

            string host = Regex.Match(stream, @"Host:\s?(\S+)").Groups[1].Value.Trim();
            string url = Regex.Match(stream, @"(POST|GET)\s?(\S+)").Groups[2].Value.Trim();
            web.URL = string.Format("{0}{1}{2}", "http://", host, url);
            web.Referer = Regex.Match(stream, @"Referer:\s?(\S+)").Groups[1].Value.Trim();
            if (stream.StartsWith("POST"))
            {
                string[] lines = stream.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string lastLine = lines[lines.Length - 1];
                Dictionary<string, string> para = StringUtils.GetData(lastLine, new char[] { '&' });
                web.PostParameters = para;
            }
            return web;
        }
    }
}

namespace DaiChong.Lib.Type
{
    public enum HttpMethodType : int
    {
        GET = 0,
        POST = 1
    }

    public enum UserAgentType : int
    {
        IE6 = 6,
        IE7 = 0,
        FireFox5 = 15,
        WebKit = 30,
        Ios5 = 31,
        IpadMini,
        Ipad,
        Iphone6p
    }

    public enum PostFormat
    {
        /// <summary>
        /// 普通
        /// </summary>
        Common = 0,

        /// <summary>
        /// 字符
        /// </summary>
        String = 1
    }
}

namespace DaiChong.Lib.Util
{
    //public class ImageUtils
    //{
    //    public static byte[] ImageToBytes(Image img)
    //    {
    //        return ImageToBytes(img, ImageFormat.Jpeg);
    //    }

    //    public static byte[] ImageToBytes(Image img, ImageFormat format)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        img.Save(ms, format);
    //        byte[] by = new byte[ms.Length];
    //        ms.Position = 0;
    //        ms.Read(by, 0, Convert.ToInt32(ms.Length));
    //        ms.Close();
    //        return by;
    //    }

    //    public static string Image2Base64(Image img)
    //    {
    //        byte[] bytes = Image2Byte(img);
    //        string strBase64 = Convert.ToBase64String(bytes, 0, bytes.Length);

    //        return strBase64;
    //    }

    //    public static Image Base642Image(string strBase64)
    //    {
    //        byte[] bytes = Convert.FromBase64String(strBase64);
    //        Image img = Byte2Image(bytes);

    //        return img;
    //    }

    //    //将Image转换为byte[]
    //    public static byte[] Image2Byte(Image image)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        BinaryFormatter bf = new BinaryFormatter();
    //        bf.Serialize(ms, (object)image);
    //        ms.Close();
    //        return ms.ToArray();
    //    }

    //    //将byte[]转换为Image
    //    public static Image Byte2Image(byte[] bytes)
    //    {
    //        MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
    //        BinaryFormatter bf = new BinaryFormatter();
    //        object obj = bf.Deserialize(ms);
    //        ms.Close();
    //        return (Image)obj;
    //    }

    //    /// <summary>
    //    /// 裁剪图片
    //    /// </summary>
    //    /// <param name="point"></param>
    //    /// <param name="size"></param>
    //    /// <returns></returns>
    //    public static Image Cut(Image oldImage, Point point, Size size)
    //    {
    //        Image newimage = new Bitmap(size.Width, size.Height);
    //        Graphics g = Graphics.FromImage(newimage);
    //        Rectangle destRect = new Rectangle(0, 0, size.Width, size.Height);
    //        Rectangle srcRect = new Rectangle(point.X, point.Y, size.Width, size.Height);
    //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //        g.SmoothingMode = SmoothingMode.HighQuality;
    //        g.DrawImage(oldImage, destRect, srcRect, GraphicsUnit.Pixel);
    //        g.Dispose();
    //        return newimage;
    //    }

    //}

    /// <summary>
    /// 网络工具类。
    /// </summary>
    public abstract class WebUtils
    {

        /// <summary>
        /// 获取表单中所有的input
        /// </summary>
        /// <param name="html"></param>
        public static Dictionary<string, string> GetInitPostData(string html)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            Regex regKey = new Regex(@"\sname=\s*(""(?<key>[^""]+)""|'(?<key>[^']+)'|(?<key>[\S]+))", RegexOptions.IgnoreCase);
            Regex regValue = new Regex(@"\svalue=\s*(""(?<value>[^""]*)""|'(?<value>[^']*)'|(?<value>[\S]*))", RegexOptions.IgnoreCase);
            MatchCollection matches = Regex.Matches(html, @"<(input|select|postfield)[^>]+>", RegexOptions.IgnoreCase);
            foreach (Match ma in matches)
            {
                string input = ma.Groups[0].Value;
                string key = regKey.Match(input).Groups["key"].Value;
                if (!string.IsNullOrEmpty(key) && key != "\"\"")
                {
                    dict[key] = regValue.Match(input).Groups["value"].Value;
                }
            }

            //给select 设置默认值  
            MatchCollection matches2 = Regex.Matches(html, @"<select[^>]+>(\s*<option[^>]+>[^<]+</option>\s*)+</select>", RegexOptions.IgnoreCase);
            foreach (Match ma in matches2)
            {
                string input = ma.Groups[0].Value;
                string key = Regex.Match(input, @"name=(""|')([^^""']+)", RegexOptions.IgnoreCase).Groups[2].Value;
                if (!string.IsNullOrEmpty(key))
                {
                    MatchCollection matches3 = Regex.Matches(input, @"option([^>]+)", RegexOptions.IgnoreCase);
                    foreach (Match ma2 in matches3)
                    {
                        if (ma2.Groups[1].Value.ToLower().Contains("selected"))
                        {
                            dict[key] = Regex.Match(ma2.Groups[1].Value, @"value=(""|')([^""']+)", RegexOptions.IgnoreCase).Groups[2].Value;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(dict[key]) && matches3.Count == 1)
                    {
                        dict[key] = Regex.Match(matches3[0].Groups[1].Value, @"value=(""|')([^""']+)", RegexOptions.IgnoreCase).Groups[2].Value;
                    }
                }
            }

            return dict;
        }

        public static string GetSelectOptionName(string html, string name)
        {
            MatchCollection matches2 = Regex.Matches(html, @"<select[^>]+>(\s*<option[^>]+>[^<]+</option>\s*)+</select>", RegexOptions.IgnoreCase);
            foreach (Match ma in matches2)
            {
                string input = ma.Groups[0].Value;
                string key = Regex.Match(input, @"name=(""|')([^""']+)", RegexOptions.IgnoreCase).Groups[2].Value;
                if (key == name)
                {
                    MatchCollection matches3 = Regex.Matches(input, @"option([^>]+)>([^<]+)", RegexOptions.IgnoreCase);
                    foreach (Match ma2 in matches3)
                    {
                        if (ma2.Groups[1].Value.ToLower().Contains("selected"))
                        {
                            return ma2.Groups[2].Value;
                        }
                    }
                    if (matches3.Count == 1)
                    {
                        return matches3[0].Groups[2].Value;
                    }
                }
            }
            return string.Empty;
        }

        public static string GetSelectOptionName(string html, string name, string value)
        {
            MatchCollection matches2 = Regex.Matches(html, @"<select[^>]+>(\s*<option[^>]+>[^<]+</option>\s*)+</select>", RegexOptions.IgnoreCase);
            foreach (Match ma in matches2)
            {
                string input = ma.Groups[0].Value;
                string key = Regex.Match(input, @"name=(""|')([^""']+)", RegexOptions.IgnoreCase).Groups[2].Value;
                if (key == name)
                {
                    MatchCollection matches3 = Regex.Matches(input, @"option([^>]+)>([^<]+)", RegexOptions.IgnoreCase);
                    foreach (Match ma2 in matches3)
                    {
                        string _value = Regex.Match(ma2.Groups[1].Value, @"value=(""|')([^""']+)", RegexOptions.IgnoreCase).Groups[2].Value;
                        if (_value == value)
                        {
                            return ma2.Groups[2].Value;
                        }
                    }
                    if (matches3.Count == 1)
                    {
                        return matches3[0].Groups[2].Value;
                    }
                }
            }
            return string.Empty;
        }

        public static string GetFormAction(string html)
        {
            return Regex.Match(html, @"action=[""']?([^""'\s]+)[""']?", RegexOptions.IgnoreCase).Groups[1].Value;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();

            using (rsp)
            {
                Stream stream = rsp.GetResponseStream();
                if (rsp.ContentEncoding.ToLower().Contains("gzip"))
                {
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                }

                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    if (rsp.ContentLength > 0)
                    {
                        // 每次读取不大于256个字符，并写入字符串
                        char[] buffer = new char[256];
                        int readBytes = 0;
                        while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            result.Append(buffer, 0, readBytes);
                        }
                    }
                    else //Chunked 编码
                    {
                        while (!reader.EndOfStream)
                            result.Append((char)reader.Read());
                    }

                }
            }

            return result.ToString();
        }


        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildPostData(IDictionary<string, string> parameters, Encoding encoding, bool upper = false)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (value == null)
                    {
                        value = string.Empty;
                    }
                    string enc = string.Empty;
                    if (upper)
                    {
                        value = value.Replace("\\", "");
                        enc = UrlEncode(value, encoding);
                    }
                    else
                    {
                        enc = HttpUtility.UrlEncode(value, encoding);
                    }
                    postData.Append(enc);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        public static string UrlEncode(string str, Encoding encoding)
        {
            str = System.Web.HttpUtility.UrlEncode(str, encoding);
            byte[] buf = Encoding.ASCII.GetBytes(str);//这里是我个人的扩展方法，等同于Encoding.ASCII.GetBytes(str)
            for (int i = 0; i < buf.Length; i++)
                if (buf[i] == '%')
                {
                    if (buf[i + 1] >= 'a') buf[i + 1] -= 32;
                    if (buf[i + 2] >= 'a') buf[i + 2] -= 32;
                    i += 2;
                }
            return Encoding.ASCII.GetString(buf);//同上，等同于Encoding.ASCII.GetString(buf)
        }
    }

}