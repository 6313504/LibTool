//using System;
//using System.Runtime.InteropServices;

//namespace DaiChong.Lib.Util
//{
 
//    public sealed class DllInvoke:IDisposable
//    {
//        [DllImport("kernel32.dll")]
//        private extern static IntPtr LoadLibrary(String path);

//        [DllImport("kernel32.dll")]
//        private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);

//        [DllImport("kernel32.dll")]
//        private extern static bool FreeLibrary(IntPtr lib);
//        private IntPtr hLib;

//        public DllInvoke(String DLLPath)
//        {
//            hLib = LoadLibrary(DLLPath);
//        }

//        ~DllInvoke()
//        {
//            FreeLibrary(hLib);
//        }

//        //将要执行的函数转换为委托
//        public Delegate Invoke(String APIName, System.Type t)
//        {
//            IntPtr api = GetProcAddress(hLib, APIName);
//            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
//        }

//        public void Dispose()
//        {
//            FreeLibrary(hLib);
//        }
//    }
//}