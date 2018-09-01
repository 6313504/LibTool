using System;
using System.Net;
using System.Reactive.Linq;

namespace LibTool.Http
{
    public abstract class HttpGetMethodBase<T>
    {
        public virtual IObservable<T> Get(string Url)//设置虚拟方法是为了多态 但是这里不用设置应该也可以
        {
            //多态既是为了用子类的方法
            //其实我这里不需要用子类的方法
            //写了应该也可以
            //只要注意子类的Override
            var func = Observable.FromAsyncPattern<HttpWebRequest, T>(Webrequest, WebResponse);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            return func(request);
        }
        private IAsyncResult Webrequest(HttpWebRequest request, AsyncCallback callbcak, object ob)
        {
            return request.BeginGetResponse(callbcak, request);
        }

        //发的请求用的是父类的get,WebResponse用的是子类的
        protected abstract T WebResponse(IAsyncResult result);
    }
}
