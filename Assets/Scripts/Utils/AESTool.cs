using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
/// <summary>
/// 文件加密解密工具
/// </summary>
public class AESTool
{
    /// <summary>
    /// 会加在加密过的文件开头，用于加密解密时判断此文件是否加密过，避免重复加密解密造成数据错误
    /// </summary>
    private static string AESHead = "AESEncrypt";

    /// <summary>
    /// 文件加密
    /// </summary>
    /// <param name="path">要加密的文件的路径</param>
    /// <param name="key">秘钥</param>
    public static void FileEncrypt(string path, string key)
    {
        if (!File.Exists(path)) return;

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs != null)
                {
                    byte[] headBuff = new byte[10];//"AESEncrypt" 10个字节
                    //读取前10字节，将其转换成字符串与代码内定义的已加密文件标识字符串比对，如果一致则说明加密过就不在加密，如果不一致就执行加密逻辑
                    fs.Read(headBuff, 0, 10);
                    string headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AESHead)
                    {
                        Debug.Log(path + "已经加密过了！");
                        return;
                    }

                    //读取全部数据
                    fs.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);

                    //写入已加密文件的标识内容
                    byte[] headBuffer = Encoding.UTF8.GetBytes(AESHead);
                    fs.Write(headBuffer, 0, 10);

                    //加密前面读取到的正文部分数据 然后写入
                    byte[] EncBuffer = EncryptByte(buffer, key);
                    fs.Write(EncBuffer, 0, EncBuffer.Length);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"文件{path}加密出错:" + e.ToString());
        }
    }

    /// <summary>
    /// 文件解密
    /// </summary>
    /// <param name="path">要解密的文件的路径</param>
    /// <param name="key">秘钥</param>
    public static void FileDecrypt(string path, string key)
    {
        if (!File.Exists(path)) return;

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs != null)
                {
                    //读取前10个字节，看是否是标识文件加密过的字符串，如果不是则不解密，因为没有加密过
                    byte[] headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    string headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AESHead)
                    {
                        //读取正文部分
                        byte[] buffer = new byte[fs.Length - headBuff.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        //解密正文部分 然后写入保存
                        byte[] EncBuffer = DecryptByte(buffer, key);
                        fs.Write(EncBuffer, 0, EncBuffer.Length);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"文件{path}解密出错:" + e.ToString());
        }
    }

    /// <summary>
    /// 读取文件内容，返回byte数组。如果文件加密过，会将读取到的内容解密后再转换成byte数据返回
    /// </summary>
    /// <param name="path">要读取的文件的路径</param>
    /// <param name="key">秘钥</param>
    /// <returns></returns>
    public static byte[] ReadFileReturnByte(string path, string key)
    {
        if (!File.Exists(path)) return null;

        byte[] EncBuffer = null;
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs != null)
                {
                    //读取前10个字节，看是否是标识文件加密过的字符串，如果不是则不解密，因为没有加密过
                    byte[] headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    string headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AESHead)
                    {
                        //读取正文部分  然后解密获取到byte数组
                        byte[] buffer = new byte[fs.Length - headBuff.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                        EncBuffer = DecryptByte(buffer, key);
                    }
                    else
                    {
                        //读取正文部分  然后解密获取到byte数组
                        fs.Seek(0, SeekOrigin.Begin);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                        EncBuffer = buffer;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"文件{path}解密出错:" + e.ToString());
        }
        return EncBuffer;
    }

    /// <summary>
    /// 加密字符串，返回加密后的字符串
    /// </summary>
    /// <param name="encryptString">内容</param>
    /// <param name="encryptKey">密钥</param>
    public static string EncryptString(string encryptString, string encryptKey)
    {
        return Convert.ToBase64String(EncryptByte(Encoding.Default.GetBytes(encryptString), encryptKey));
    }

    /// <summary>
    /// 解密字符串，返回解密后的字符串
    /// </summary>
    /// <param name="encryptString">内容</param>
    /// <param name="encryptKey">密钥</param>
    public static string DecryptString(string encryptString, string encryptKey)
    {
        return Convert.ToBase64String(DecryptByte(Encoding.Default.GetBytes(encryptString), encryptKey));
    }

    /// <summary>
    /// 加密byte数组，返回加密后的byte数组
    /// </summary>
    /// <param name="encryptByte">待加密的byte数组</param>
    /// <param name="encryptKey">秘钥</param>
    /// <returns></returns>
    public static byte[] EncryptByte(byte[] encryptByte, string encryptKey)
    {
        return EncryptOrDecryptByte(encryptByte, encryptKey, true);
    }

    /// <summary>
    /// 解密byte数组，返回解密后的byte数组
    /// </summary>
    /// <param name="decryptByte">待解密的byte数组</param>
    /// <param name="decryptKey">秘钥</param>
    /// <returns></returns>
    public static byte[] DecryptByte(byte[] decryptByte, string decryptKey)
    {
        return EncryptOrDecryptByte(decryptByte, decryptKey, false);
    }

    /// <summary>
    /// 加密或解密byte数组，返回加密解密后的byte数组
    /// </summary>
    /// <param name="encryptOrDecryptByte">内容</param>
    /// <param name="key">密钥</param>
    /// <param name="encrypt"></param>
    /// <returns></returns>
    private static byte[] EncryptOrDecryptByte(byte[] encryptOrDecryptByte, string key, bool encrypt = true)
    {
        if (encryptOrDecryptByte.Length == 0)
        {
            Debug.LogError("加密解密内容不得为空");
        }
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("秘钥不得为空");
        }

        byte[] m_ContentByte;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, m_salt);
            ICryptoTransform transform = encrypt ? m_AESProvider.CreateEncryptor(pdb.GetBytes(32), m_btIV) : m_AESProvider.CreateDecryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(encryptOrDecryptByte, 0, encryptOrDecryptByte.Length);
            m_csstream.FlushFinalBlock();
            m_ContentByte = m_stream.ToArray();
            m_stream.Close();
            m_stream.Dispose();
            m_csstream.Close();
            m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_ContentByte;
    }

}

