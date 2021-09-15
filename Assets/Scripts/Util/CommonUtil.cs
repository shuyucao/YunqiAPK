using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Util
{
    class CommonUtil
    {
        /**
         *  @param deviceIdSrc 设备唯一ID
         *  @return 加密编码后的设备ID
         */
        public static string GenerateDeviceId(String deviceIdSrc)
        {
            String md5salt = EncryptMD5_Salt(deviceIdSrc);
            String encrypted_device_id = EncryptBASE64(md5salt);
            String device_type = "a";
            String validate_char = encrypted_device_id.Substring(2, 1);
            String device_id = validate_char + device_type + deviceIdSrc;
            Debug.Print("md5salt:" + md5salt);
            Debug.Print("encrypted_device_id:" + encrypted_device_id);
            Debug.Print("validate_char:" + validate_char);
            Debug.Print("device_id:" + device_id);
            return device_id;
        }

        public static String EncryptMD5_Salt(String content)
        {

            String resultString = "";
            String appkey = "fdjf,jkgfkl";

            byte[] a = Encoding.UTF8.GetBytes(appkey);
            byte[] datSource = Encoding.UTF8.GetBytes(content);
            byte[] b = new byte[a.Length + 4 + datSource.Length];

            int i;
            for (i = 0; i < datSource.Length; i++)
            {
                b[i] = datSource[i];
            }

            b[i++] = (byte)163;
            b[i++] = (byte)172;
            b[i++] = (byte)161;
            b[i++] = (byte)163;

            for (int k = 0; k < a.Length; k++)
            {
                b[i] = a[k];
                i++;
            }

            try
            {
                // MessageDigest md5 = MessageDigest.getInstance(KEY_MD5);
                // md5.update(b);
                // 将得到的字节数组变成字符串返回
                // resultString = new HexBinaryAdapter().marshal(md5.digest());//转为十六进制的字符串
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] c = MD5.Create().ComputeHash(b);
                // 转为十六进制的字符串
                resultString = ByteToHexStr(c);
            }
            catch (Exception e)
            {
                Debug.Print("encryptMD5 fail", e);
            }

            return resultString.ToLower();
        }


        /**
         *  转为十六进制的字符串
         *  @param bytes 输入
         *  @return 十六进制的字符串
         */
        private static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /**
         * Base64解码
         * @param key
         * @return
         * @throws Exception
         */
        private static String EncryptBASE64(String key)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
        }

        public static String EncryptDes(String data, String key)
        {
            byte[] bt = EncryptDes(Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(key));
            String strs = Convert.ToBase64String(bt);
            return strs;
         }

        private static byte[] EncryptDes(byte[] src, byte[] key)
        {
            try
            {
                byte[] keyBytes = key;
                byte[] keyIV = keyBytes;
                byte[] inputByteArray = src;

                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);

                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();
                return memStream.ToArray();

            }
            catch
            {
                return src;
            }
        }
        public static String DecryptDes(String data, String key)
        {
            if (data == null)
            {
                return null;
            }
            byte[] buf = Convert.FromBase64String(data);
            byte[] bt = DecryptDes(buf, Encoding.UTF8.GetBytes(key));
            string str = Encoding.UTF8.GetString(bt);
            return str;
        }
        private static byte[] DecryptDes(byte[] src, byte[] key)
        {
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            desProvider.Mode = CipherMode.ECB;
            desProvider.Key = key;

            MemoryStream memStream = new MemoryStream();
            CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(key, key), CryptoStreamMode.Write);
            crypStream.Write(src, 0, src.Length);
            crypStream.FlushFinalBlock();
            crypStream.Close();
            return memStream.ToArray();
        }
    }
}
