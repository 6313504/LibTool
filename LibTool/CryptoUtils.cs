using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DaiChong.Lib.Util
{
    /// <summary>
    /// 加密相关的常用方法类
    /// </summary>
    public static class CryptoUtils
    {
        #region MD5 加密转换为小写的


        /// <summary>
        /// MD5 加密转换为小写的
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            return MD5(str,Encoding.ASCII).ToLower();
        }

        public static string MD5(string str, string encoding)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            string strMD5 = BitConverter.ToString(MD5.ComputeHash(Encoding.GetEncoding(encoding).GetBytes(str))).Replace("-", "");
            return strMD5.ToLower();
        }

        public static string MD5(string str, Encoding encoding)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            string strMD5 = BitConverter.ToString(MD5.ComputeHash(encoding.GetBytes(str))).Replace("-", "");
            return strMD5.ToLower();
        }



        public static string Ka123BossMd5(string s)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(s));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString().ToUpper();
        }

        //public static string kamenMd5(string s)
        //{
        //    return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5").ToLower();

        //    //byte[] result = Encoding.UTF8.GetBytes(s);
        //    //MD5 md5 = new MD5CryptoServiceProvider();
        //    //byte[] output = md5.ComputeHash(result);
        //    //return BitConverter.ToString(output).Replace("-", "");
        //}

        #endregion

        //public static string SHA1Encrypt(string str)
        //{
        //    string resultStr = string.Empty;
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        resultStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");
        //    }
        //    return resultStr;
        //}

        #region TripleDESEncrypt 天下一卡通加密
        public static string TripleDESEncrypt(string a_strString, string a_strKey)
        {
            // 计算24位key
            if (a_strKey.Length < 24)
            {
                string newKey = a_strKey;
                for (int i = 0; i < 24 / a_strKey.Length; i++)
                {
                    newKey += a_strKey;
                }
                a_strKey = newKey;
            }

            a_strKey = a_strKey.Substring(0, 24);

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey);
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            //byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(a_strString);
            //return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            byte[] Buffer = Encoding.UTF8.GetBytes(a_strString);
            return StringToHex(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        private static string StringToHex(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes)
            {
                result += b.ToString("X2");
            }
            return result;
        }

        #endregion

        #region TripleDESEncrypt 3DES加密24位填充类型为0字节
        /// <summary>
        /// 3DES加密24位填充类型为0字节
        /// </summary>
        /// <param name="encryptSource">加密的数据源</param>
        /// <param name="encryptKey">加密的密匙</param>
        /// <param name="encryptIV">加密的矢量</param>
        /// <returns>加密串</returns>
        public static string TripleDESEncrypt(string encryptSource, string encryptKey, string encryptIV)
        {
            return TripleDESEncrypt(encryptSource, encryptKey, encryptIV, System.Security.Cryptography.PaddingMode.Zeros, 24, "ToBase64");
        }
        #endregion

        #region TripleDESEncrypt 3DES加密24位自定填充类型
        /// <summary>
        /// 3DES加密24位自定填充类型
        /// </summary>
        /// <param name="encryptSource">加密的数据源</param>
        /// <param name="encryptKey">加密的密匙</param>
        /// <param name="encryptIV">加密的矢量</param>
        /// <param name="paddingMode">填充类型</param>
        /// <returns>加密串</returns>
        public static string TripleDESEncrypt(string encryptSource, string encryptKey, string encryptIV, System.Security.Cryptography.PaddingMode paddingMode)
        {
            return TripleDESEncrypt(encryptSource, encryptKey, encryptIV, paddingMode, 24, "ToBase64");
        }
        #endregion

        #region TripleDESEncrypt  3DES加密自定义字节位填充类型
        /// <summary>
        /// 3DES加密自定义字节位填充类型
        /// </summary>
        /// <param name="encryptSource">加密的数据源</param>
        /// <param name="encryptKey">加密的密匙</param>
        /// <param name="encryptIV">加密的矢量</param>
        /// <param name="paddingMode">自定义填充类型</param>
        /// <param name="byteNum">自定义字节数</param>
        /// <param name="outType">输出字节形式ToHex16,ToBase64</param>
        /// <returns>加密串</returns>
        public static string TripleDESEncrypt(string encryptSource, string encryptKey, string encryptIV, System.Security.Cryptography.PaddingMode paddingMode, int byteNum, string outType, System.Security.Cryptography.CipherMode mode = System.Security.Cryptography.CipherMode.ECB)
        {
            //构造一个对称算法
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            if (string.IsNullOrEmpty(outType))
            {
                return "输出配置不能空";
            }
            if ((encryptKey.Trim().Length) != byteNum)
            {
                return "加密字节";
            }
            byte[] Key = System.Text.Encoding.Default.GetBytes(encryptKey);
            mCSP.Key = Key;
            //默认矢量
            if (String.IsNullOrEmpty(encryptIV))
            {
                encryptIV = encryptKey.Substring(0, 8);
            }
            mCSP.IV = System.Text.Encoding.Default.GetBytes(encryptIV);

            //指定加密的运算模式
            mCSP.Mode = mode;
            //获取或设置加密算法的填充模式
            mCSP.Padding = paddingMode;

            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);

            byt = System.Text.Encoding.Default.GetBytes(encryptSource.Trim());

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();

            string mToString = string.Empty;
            //输出16进制字符
            if (outType == "ToHex16")
            {
                mToString = ToHexString(ms.ToArray());
            }
            //输出ToBase64字符
            if (outType == "ToBase64")
            {
                mToString = Convert.ToBase64String(ms.ToArray()).ToString().Replace("\0", "");
            }

            return mToString;
        }
        #endregion

        #region TripleDESDecrypt  3DES解密24位Zeros填充
        /// <summary>
        /// 3DES解密24位Zeros填充
        /// </summary>
        /// <param name="decryptSource">解密的数据源</param>
        /// <param name="decryptKey">解密的密匙</param>
        /// <param name="decryptIV">解密的矢量</param>
        /// <returns>解密串</returns>
        public static string TripleDESDecrypt(string decryptSource, string decryptKey, string decryptIV)
        {
            return TripleDESDecrypt(decryptSource, decryptKey, decryptIV, System.Security.Cryptography.PaddingMode.Zeros, 24, "ToBase64");
        }
        #endregion

        #region TripleDESDecrypt 3DES解密24位自定义填充类型
        /// <summary>
        /// 3DES解密24位自定义填充类型
        /// </summary>
        /// <param name="decryptSource">解密的数据源</param>
        /// <param name="decryptKey">解密的密匙</param>
        /// <param name="decryptIV">解密的矢量</param>
        /// <param name="paddingMode">填充类型</param>
        /// <returns>解密串</returns>
        public static string TripleDESDecrypt(string decryptSource, string decryptKey, string decryptIV, System.Security.Cryptography.PaddingMode paddingMode)
        {
            return TripleDESDecrypt(decryptSource, decryptKey, decryptIV, paddingMode, 24, "ToBase64");
        }
        #endregion

        #region TripleDESDecrypt 3DES解密
        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <param name="decryptSource">解密的数据源</param>
        /// <param name="decryptKey">解密的密匙</param>
        /// <param name="decryptIV">解密的矢量</param>
        /// <param name="paddingMode">填充模式</param>
        /// <param name="byteNum">字节</param>
        ///  <param name="outType">解密字节形式ToHex16,ToBase64</param>
        /// <returns>解密串</returns>
        public static string TripleDESDecrypt(string decryptSource, string decryptKey, string decryptIV, System.Security.Cryptography.PaddingMode paddingMode, int byteNum, string outType, System.Security.Cryptography.CipherMode mode = System.Security.Cryptography.CipherMode.ECB)
        {
            //构造一个对称算法
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt = new byte[0];

            if (string.IsNullOrEmpty(outType))
            {
                return "解密配置不能空";
            }
            if ((decryptKey.Trim().Length) != byteNum)
            {
                return "解密字节错误";
            }

            byte[] Key = System.Text.Encoding.Default.GetBytes(decryptKey.Trim());

            mCSP.Key = Key;
            //默认矢量
            if (String.IsNullOrEmpty(decryptIV))
            {
                decryptIV = decryptKey.Substring(0, 8);
            }
            mCSP.IV = System.Text.Encoding.Default.GetBytes(decryptIV);

            mCSP.Mode = mode;
            mCSP.Padding = paddingMode;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);

            //输出16进制字符
            if (outType == "ToHex16")
            {
                byt = ConvertHexToBytes(decryptSource);
            }
            //输出ToBase64字符
            if (outType == "ToBase64")
            {
                byt = Convert.FromBase64String(decryptSource);
            }
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();

            return Encoding.Default.GetString(ms.ToArray()).Replace("\0", "").Trim();
        }
        #endregion

        #region ConvertHexToBytes将16进制字符串转化为字节数组
        /// <summary>
        /// 将16进制字符串转化为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ConvertHexToBytes(string value)
        {
            int len = value.Length / 2;
            byte[] ret = new byte[len];
            for (int i = 0; i < len; i++)
                ret[i] = (byte)(Convert.ToInt32(value.Substring(i * 2, 2), 16));
            return ret;
        }
        #endregion

        #region ToHexString 将字节数组转换为字符串
        /// <summary>
        /// 字符
        /// </summary>
        public static char[] HexDigits = {'0', '1', '2', '3', '4', '5', '6', '7',
										'8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        /// <summary>
        /// 将字节数组转换为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = HexDigits[b >> 4];
                chars[i * 2 + 1] = HexDigits[b & 0xF];
            }
            return new string(chars);
        }
        #endregion

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="p_Data">要加密数据</param>
        /// <param name="p_Key">8位加密key</param>
        /// <returns></returns>
        public static string DES_Encrypt(string p_Data, string p_Key)
        {
            StringBuilder ret = new StringBuilder();
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(p_Data);

                des.Key = ASCIIEncoding.ASCII.GetBytes(p_Key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(p_Key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }

            }
            catch (Exception)
            {

            }

            return ret.ToString();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="p_Data">要解密数据</param>
        /// <param name="p_Key">8位加密key</param>
        /// <returns></returns>
        public static string DES_Decrypt(string p_Data, string p_Key)
        {
            string retValue = string.Empty;

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputByteArray = new byte[p_Data.Length / 2];
                for (int x = 0; x < p_Data.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(p_Data.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(p_Key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(p_Key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();

                retValue = System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            { }

            return retValue;

        }



        private static byte[] GetKey(string sKey)
        {
            byte[] arrBTmp = ASCIIEncoding.ASCII.GetBytes(sKey);
            byte[] arrB = new byte[8];

            // 将原始字节数组转换为8位
            for (int i = 0; i < arrBTmp.Length && i < arrB.Length; i++)
            {
                arrB[i] = arrBTmp[i];
            }
            return arrB;
        }

        /// <summary>
        /// 加密,8位密钥，16位加密结果
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt2(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = GetKey(sKey);
            des.Mode = CipherMode.ECB;
            des.IV = GetKey(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                string tempstr = "";
                tempstr = Convert.ToString(b, 16);
                if (tempstr.Length < 2)
                    tempstr = "0" + tempstr;
                ret.Append(tempstr);
                //ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString().ToLower();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt2(string pToDecrypt, string sKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0;
                 x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = GetKey(sKey);
                des.IV = GetKey(sKey);
                des.Mode = CipherMode.ECB;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 盛大Mobile5得到KEY
        /// </summary>
        /// <param name="randStr">随机数</param>
        /// <param name="certSerialNo">版本号</param>
        /// <param name="str">证书</param>
        /// <returns></returns>
        public static string GetKey(string randStr, out string certSerialNo, string certStr)
        {
            byte[] bytes = Convert.FromBase64String(certStr.Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", "").Trim());
            X509Certificate2 cert = new X509Certificate2(bytes);
            certSerialNo = cert.SerialNumber;
            RSACryptoServiceProvider rsa = cert.PublicKey.Key as RSACryptoServiceProvider;
            byte[] enc = rsa.Encrypt(Encoding.UTF8.GetBytes(randStr), false);
            return Convert.ToBase64String(enc);
        }

        /// <summary>
        /// 70卡解密专用
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptDES(string source, string key, string iv)
        {
            Encoding en = Encoding.ASCII;
            byte[] sourcebyte = Convert.FromBase64String(source);
            byte[] keybyte = en.GetBytes(key);
            byte[] ivbyte = en.GetBytes(iv);
            MemoryStream msc = new MemoryStream(sourcebyte);
            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            CryptoStream crstr = new CryptoStream(msc, desc.CreateDecryptor(keybyte, ivbyte), CryptoStreamMode.Read);
            byte[] decbyte = new byte[sourcebyte.Length];
            crstr.Read(decbyte, 0, decbyte.Length);
            return en.GetString(decbyte);
        }

        /// <summary>
        ///  70卡加密专用
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncrypotyDES(string source, string key, string iv)
        {
            Encoding en = Encoding.ASCII;
            byte[] sourcebyte = en.GetBytes(source);
            byte[] keybyte = en.GetBytes(key);
            byte[] ivbyte = en.GetBytes(iv);
            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream crstr = new CryptoStream(ms, desc.CreateEncryptor(keybyte, ivbyte), CryptoStreamMode.Write);
            crstr.Write(sourcebyte, 0, sourcebyte.Length);
            crstr.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 天宏骏网卡密加密
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string EncryptPassWord(string pToEncrypt, string sKey)
        {
            var des = new DESCryptoServiceProvider();  //把字符串放到byte数组中   
            des.Mode = CipherMode.ECB;
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);

            des.Key = Encoding.Default.GetBytes(sKey);  //建立加密对象的密钥和偏移量 
            des.IV = Encoding.Default.GetBytes(sKey);   //原文使用ASCIIEncoding.ASCII方法的GetBytes方法  
            var ms = new MemoryStream();     //使得输入密码必须输入英文文本 
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #region 开联通证书导入
        /// <summary>     
        /// 根据私钥证书得到证书实体，得到实体后可以根据其公钥和私钥进行加解密     
        /// 加解密函数使用DEncrypt的RSACryption类     
        /// </summary>     
        /// <param name="pfxFileName"></param>     
        /// <param name="password"></param>     
        /// <returns></returns>     
        public static X509Certificate2 GetCertificateFromPfxFile(string pfxFileName, string password)
        {
            try
            {
                return new X509Certificate2(pfxFileName, password, X509KeyStorageFlags.Exportable);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 数字签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>签名</returns>
        public static string HashAndSignString(string plaintext, string privateKey)
        {
            using (RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider())
            {
                RSAalg.FromXmlString(privateKey);
                //使用MD5进行摘要算法
                byte[] md5Bytes = MD5Encrypt(plaintext);
                //函数内部会再次计算哈希值，第2次哈希算法是SHA1
                byte[] encryptedData = RSAalg.SignData(md5Bytes, new SHA1CryptoServiceProvider());
                return Convert.ToBase64String(encryptedData);
            }
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="SignedData">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static bool VerifySigned(string plaintext, string SignedData, string publicKey)
        {
            using (RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider())
            {
                RSAalg.FromXmlString(publicKey);
                byte[] md5Bytes = MD5Encrypt(plaintext);
                byte[] signedDataBytes = Convert.FromBase64String(SignedData);
                return RSAalg.VerifyData(md5Bytes, new SHA1CryptoServiceProvider(), signedDataBytes);
            }
        }
        /// <summary>
        /// 加载密钥
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password"></param>
        /// <param name="keyFlag">true:加载私钥，false:加载公钥</param>
        /// <returns></returns>
        public static String loadKey(String path, String password, bool keyFlag)
        {
            X509Certificate2 c3 = GetCertificateFromPfxFile(path, password);
            string key = "";
            if (keyFlag)
            {
                key = c3.PrivateKey.ToXmlString(keyFlag);
            }
            else
            {
                key = c3.PublicKey.Key.ToXmlString(keyFlag);
            }
            return key;
        }

        ///   <summary>  
        ///   给一个字符串进行MD5加密  
        ///   </summary>  
        ///   <param   name="strText">待加密字符串</param>  
        ///   <returns>加密后的字符串</returns>  
        public static byte[] MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(strText));
            return result;
        }
        #endregion

    }
}
