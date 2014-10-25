using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace gzsw.util
{

    /// <summary>
    /// 加密/解密辅助类
    /// </summary>
    public static class CryptTools
    {
        //默认密钥向量
        private static byte[] IV64 = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        //Url密钥向量
        private static Byte[] key;
        //Url密钥向量
        private static Byte[] IV = new Byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// Id 加密 （对称加密）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isThrowError">不能解密是否报错 默认为False 否则使用原来的参数</param>
        /// <returns></returns>
        public static string IdEncrypt(long id, bool isThrowError =false)
        {
            
            var idEncryptKey = ConfigurationManager.AppSettings["IdEncryptKey"];
    
            return UrlEncrypt(id.ToString(), idEncryptKey);
        }

        /// <summary>
        /// Id 解密 （对称解密）
        /// </summary>
        /// <param name="idStr"></param>
        /// <param name="isThrowError">不能解密是否报错 默认为False 否则使用原来的参数</param>
        /// <returns></returns>
        public static long IdDecrypt(string idStr, bool isThrowError = false)
        {
            var idEncryptKey = ConfigurationManager.AppSettings["IdEncryptKey"];
            return long.Parse(UrlDecrypt(idStr, idEncryptKey));
        }

        /// <summary>
        /// 分页页码加密 
        /// 页码加密规则如下：
        /// 1.加密/使用对称加密 密钥是web.config的appSetting Key 为PagerEncryptKey的值
        /// 2.混淆/随机生成一个MD5 对使用对称加密后的字符串/2 取中间位置 将MD5插入中间
        /// 3.在首位插入2位随机字母或数字 最后插入3位随机字母或数字
        /// </summary>
        /// <returns></returns>
        public static string PagerNumEncrypt(long num, bool isThrowError = false)
        {
            // 分页加密密钥
            var pagerEncryptKey = ConfigurationManager.AppSettings["PagerEncryptKey"];
            // 对称加密
            var str = CryptTools.Encrypt(num.ToString(CultureInfo.InvariantCulture), pagerEncryptKey, isThrowError);
            // 获取混淆位置
            var startIndex = str.Length / 2;
            // 混淆加密
            var garble = Guid.NewGuid().ToString("N");
            // 加入混淆
            var value = str.Insert(startIndex, garble);
            return GenerateCheckCode(2)
                .ToString(CultureInfo.InvariantCulture) + value +
                GenerateCheckCode(3);
        }


        /// <summary>
        /// 分页页码解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static int PagerNumDecrypt(string content, bool isThrowError = false)
        {
            if (string.IsNullOrEmpty(content))
                return 1;
            /*  try
              {*/
            int result;
            // 分页解密
            var pagerEncryptKey = ConfigurationManager.AppSettings["PagerEncryptKey"];
            var removeValue = content.Remove(0, 2);
            removeValue = removeValue.Substring(0, removeValue.Length - 3);
            var tagerRemoveValue = removeValue.Remove(((removeValue.Length - 32) / 2), 32);
            int.TryParse(CryptTools.Decrypt(tagerRemoveValue, pagerEncryptKey, isThrowError), out result);
            return result;
            /*}
            catch (Exception)
            {
                return 1;
            }
            */
        }

        /// <summary>
        /// TripleDES加密
        /// </summary>
        /// <param name="content">需要加密的明文内容</param>
        /// <param name="secret">加密密钥</param>
        /// <param name="isThrowError">不能解密是否报错 默认为False 否则使用原来的参数</param>
        /// <returns>返回加密后密文字符串</returns>
        public static string Encrypt(string content, string secret, bool isThrowError = false)
        { 
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(secret.Length > 8 ? secret.Substring(0, 8) : secret);
                 
                byte[] rgbIV = IV64;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(content);
                var dCSP = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV),
                    CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch(Exception e)
            {
                if (isThrowError)
                {
                    throw e;
                }
                else
                {

                    return content;
                }
            }
        }


        /// <summary>
        /// Url加密
        /// </summary>
        /// <param name="strToEncrypt">要加密的字符串</param>
        /// <param name="strEncryptKey">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string UrlEncrypt(string strToEncrypt, string strEncryptKey="#!12^0#@")
        {
            var provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(strEncryptKey.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(strEncryptKey.Substring(0, 8));
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(strToEncrypt);
            var stream = new MemoryStream();
            var stream2 = new CryptoStream(stream,
                provider.CreateEncryptor(),
                CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            var builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            stream.Close();
            return builder.ToString();
        }
       
      
        /// <summary>
        /// Url解密
        /// </summary>
        /// <param name="strToDecrypt">要解密的字符串</param>
        /// <param name="strEncryptKey">密钥，必须与加密的密钥相同</param>
        /// <returns>解密后的字符串</returns>
        public static string UrlDecrypt(string strToDecrypt, string strEncryptKey="#!12^0#@")
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(strEncryptKey.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(strEncryptKey.Substring(0, 8));
            byte[] buffer = new byte[strToDecrypt.Length / 2];
            for (int i = 0; i < (strToDecrypt.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(strToDecrypt.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            stream.Close();
            return Encoding.GetEncoding("GB2312").GetString(stream.ToArray());
        } 

        /// <summary>
        /// TripleDES解密 
        /// </summary>
        /// <param name="content">需要解密的密文内容</param>
        /// <param name="secret">解密密钥</param>
        /// <returns>返回解密后明文字符串</returns>
        public static string Decrypt(string content, string secret, bool isThrowError =false)
        { 
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(secret);
                byte[] rgbIV = IV64;
                byte[] inputByteArray = Convert.FromBase64String(content);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch(Exception ex)
            {
                if (isThrowError)
                {
                    throw ex;
                }
                return content;
            }
        }

        /// <summary>
        /// TripleDES加密
        /// </summary>
        /// <param name="source">需要加密的密文内容</param>
        /// <param name="key">加密密钥</param>
        /// <returns>返回加密后密文字节</returns>
        public static byte[] Crypt(byte[] source, byte[] key)
        {
            if ((source.Length == 0) || (source == null) || (key == null) || (key.Length == 0))
            {
                throw new ArgumentException("Invalid Argument");
            }

            TripleDESCryptoServiceProvider dsp = new TripleDESCryptoServiceProvider();
            dsp.Mode = CipherMode.ECB;

            ICryptoTransform des = dsp.CreateEncryptor(key, null);

            return des.TransformFinalBlock(source, 0, source.Length);
        }


        /// <summary>
        /// TripleDES解密
        /// </summary>
        /// <param name="source">需要解密的密文内容</param>
        /// <param name="key">解密密钥</param>
        /// <returns>返回解密后密文字节</returns>
        public static byte[] Decrypt(byte[] source, byte[] key)
        {
            if ((source.Length == 0) || (source == null) || (key == null) || (key.Length == 0))
            {
                throw new ArgumentNullException("Invalid Argument");
            }

            TripleDESCryptoServiceProvider dsp = new TripleDESCryptoServiceProvider();
            dsp.Mode = CipherMode.ECB;

            ICryptoTransform des = dsp.CreateDecryptor(key, null);

            byte[] ret = new byte[source.Length + 8];

            int num = des.TransformBlock(source, 0, source.Length, ret, 0);

            ret = des.TransformFinalBlock(source, 0, source.Length);
            ret = des.TransformFinalBlock(source, 0, source.Length);
            num = ret.Length;

            byte[] realByte = new byte[num];
            Array.Copy(ret, realByte, num);
            ret = realByte;
            return ret;
        }

        //原始base64编码
        public static byte[] Base64Encode(byte[] source)
        {
            if ((source == null) || (source.Length == 0))
                throw new ArgumentException("source is not valid");

            ToBase64Transform tb64 = new ToBase64Transform();
            MemoryStream stm = new MemoryStream();
            int pos = 0;
            byte[] buff;

            while (pos + 3 < source.Length)
            {
                buff = tb64.TransformFinalBlock(source, pos, 3);
                stm.Write(buff, 0, buff.Length);
                pos += 3;
            }

            buff = tb64.TransformFinalBlock(source, pos, source.Length - pos);
            stm.Write(buff, 0, buff.Length);

            return stm.ToArray();

        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5(string source)
        {
            // ReSharper disable PossibleNullReferenceException
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5").ToLower();
            // ReSharper restore PossibleNullReferenceException
        }

        //原始base64解码
        public static byte[] Base64Decode(byte[] source)
        {
            if ((source == null) || (source.Length == 0))
                throw new ArgumentException("source is not valid");

            FromBase64Transform fb64 = new FromBase64Transform();
            MemoryStream stm = new MemoryStream();
            int pos = 0;
            byte[] buff;

            while (pos + 4 < source.Length)
            {
                buff = fb64.TransformFinalBlock(source, pos, 4);
                stm.Write(buff, 0, buff.Length);
                pos += 4;
            }

            buff = fb64.TransformFinalBlock(source, pos, source.Length - pos);
            stm.Write(buff, 0, buff.Length);
            return stm.ToArray();

        }


        /// <summary>
        /// 类型转换（String -> byte[]）
        /// </summary>
        /// <param name="secret">字符串</param>
        /// <returns>返回byte[]类型</returns>
        public static byte[] GetKey(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("Secret is not valid");

            ASCIIEncoding ae = new ASCIIEncoding();
            byte[] temp = Hash(ae.GetBytes(secret));

            byte[] ret = new byte[Constants.Operation.KeySize];

            if (temp.Length < Constants.Operation.KeySize)
            {
                System.Array.Copy(temp, 0, ret, 0, temp.Length);
                int i;
                for (i = temp.Length; i < Constants.Operation.KeySize; i++)
                {
                    ret[i] = 0;
                }
            }
            else
                System.Array.Copy(temp, 0, ret, 0, Constants.Operation.KeySize);

            return ret;
        }

        /// <summary>
        /// 比较两个byte数组是否相同
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] source, byte[] dest)
        {
            if ((source == null) || (dest == null))
                throw new ArgumentException("source or dest is not valid");

            if (source.Length != dest.Length)
                return false;
            else
                if (source.Length == 0)
                    return true;

            return !source.Where((t, i) => t != dest[i]).Any();
        }

        /// <summary>
        /// 使用md5计算散列
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] Hash(byte[] source)
        {
            if ((source == null) || (source.Length == 0))
                throw new ArgumentException("source is not valid");

            MD5 m = MD5.Create();
            return m.ComputeHash(source);
        }

        /// <summary>
        /// 对传入的明文密码进行Hash加密,密码不能为中文
        /// </summary>
        /// <param name="oriPassword">需要加密的明文密码</param>
        /// <returns>经过Hash加密的密码</returns>
        public static string HashPassword(string oriPassword)
        {
            if (string.IsNullOrEmpty(oriPassword))
                throw new ArgumentException("oriPassword is valid");

            ASCIIEncoding acii = new ASCIIEncoding();
            byte[] hashedBytes = Hash(acii.GetBytes(oriPassword));

            StringBuilder sb = new StringBuilder(30);
            foreach (byte b in hashedBytes)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString().ToLower();
        }

        #region Helper

        private static int rep = 0;
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成的字母字符串</returns>
        private static string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            System.Random random = new System.Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        #endregion
    }

    /// <summary>
    /// 类名称   ：Constants
    /// 类说明   ：加解密算法常量.
    /// 作者     ：
    /// 完成日期 ：
    /// </summary>
    public class Constants
    {
        public struct Operation
        {
            public static readonly int KeySize = 24;
            public static readonly byte[] UnicodeOrderPrefix = new byte[2] { 0xFF, 0xFE };
            public static readonly byte[] UnicodeReversePrefix = new byte[2] { 0xFE, 0xFF };
        }
    }
}