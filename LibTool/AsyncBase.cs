using System;
using System.ComponentModel;
using System.Threading;

namespace DaiChong.Lib.Util
{

    public class MessageEventArgs : EventArgs { public string Message { get; set; } }

    public class ExceptionEventArgs : EventArgs { public Exception Exception { get; set; } }

    /// <summary>
    /// 消息委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="message">消息</param>
    public delegate void MessageHandler(object sender, MessageEventArgs e);

    /// <summary>
    /// 异常错误委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="exception">异常</param>
    public delegate void ErrorHandler(object sender, ExceptionEventArgs e);

    /// <summary>
    /// 完成委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="returnValue">处理结果</param>
    public delegate void CompleteHandler(object sender, EventArgs e);


    /// <summary>
    /// 异步基类
    /// </summary>
    public abstract class AsyncBase : IDisposable
    {

        /// <summary>
        /// 消息事件
        /// </summary>
        public event MessageHandler Message;

        /// <summary>
        /// 完成事件
        /// </summary>
        public event CompleteHandler Complete;

        /// <summary>
        /// 错误事件
        /// </summary>
        public event ErrorHandler Error;

        /// <summary>
        /// 执行中
        /// </summary>
        private bool m_IsRunning;

        /// <summary>
        /// 获取当前线程是否在执行中
        /// </summary>
        public bool GetIsRunning
        {
            get { return m_IsRunning; }
        }

        /// <summary>
        /// 是否释放
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        /// 执行委托的对象
        /// </summary>
        private ISynchronizeInvoke m_Target;

        /// <summary>
        /// 线程
        /// </summary>
        private Thread m_Thread;

        /// <summary>
        /// 异步对象ID
        /// </summary>
        public string AsyncObjectID;

        /// <summary>
        /// 设置执行委托的对象
        /// </summary>
        public ISynchronizeInvoke SetTarget
        {
            set
            {
                this.m_Target = value;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 是否资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            this.m_Thread = null;
            if (!this.m_Disposed)
            {

            }
            this.m_Disposed = true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AsyncBase()
        {
            this.m_Disposed = false;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="Target"></param>
        public AsyncBase(ISynchronizeInvoke Target)
        {
            this.m_Disposed = false;
            this.m_Target = Target;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~AsyncBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 异步工作
        /// </summary>
        protected abstract void AsyncWork();

        /// <summary>
        /// 发送完成
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnComplete(object sender)
        {
            this.ReturnTarget(this.Complete, new object[] { sender});
        }

        /// <summary>
        /// 发送错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnError(object sender, System.Exception e)
        {
            this.ReturnTarget(this.Error, new object[] { sender, e });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMessage(object sender, string message)
        {
            this.ReturnTarget(this.Message, new object[] { sender, message });
        }

        /// <summary>
        /// 返回给对象数据
        /// </summary>
        /// <param name="dlg"></param>
        /// <param name="paras"></param>
        protected void ReturnTarget(Delegate dlg, params object[] paras)
        {
            if (this.m_Target != null && dlg != null)
            {
                this.m_Target.BeginInvoke(dlg, paras);
            }
        }

        /// <summary>
        /// 开始运行异步方法
        /// </summary>
        public virtual void StartAsync()
        {
            if (this.m_IsRunning)
            {
                this.OnMessage(this, "线程正在运行中，执行异步操作无效。");
            }
            else
            {
                this.m_Thread = null;
                this.m_Thread = new Thread(new ThreadStart(this.Work));
                this.m_Thread.Start();
            }
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        private void Work()
        {
            try
            {
                this.m_IsRunning = true;
                this.AsyncWork();
            }
            catch (System.Exception ex)
            {
                this.OnError(this, ex);
                return;
            }
            finally
            {
                this.m_IsRunning = false;
                this.OnComplete(this);
            }
        }

    }
}
