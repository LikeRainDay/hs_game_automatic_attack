using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System;
using UnityEditor;
/// <summary>
/// 类与xml 二进制转换的工具
/// </summary>
public class SerializeTool
{
    /// <summary>
    /// 类转 xml
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <param name="savePath"></param>
    public static void Class2Xml(Type type, object obj, string savePath)
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        XmlSerializer serializer = new XmlSerializer(type);
        using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            serializer.Serialize(fs, obj);
        }
    }

    /// <summary>
    /// xml转 类
    /// </summary>
    /// <param name="type"></param>
    /// <param name="savePath"></param>
    /// <returns></returns>
    public static object Xml2Class(Type type, string savePath)
    {
        XmlSerializer serializer = new XmlSerializer(type);
        using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            return serializer.Deserialize(fs);
        }
    }


    /// <summary>
    /// 类转 二进制
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="savePath"></param>
    public static void Class2Binary(object obj, string savePath)
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            formatter.Serialize(fs, obj);
        }
    }

    /// <summary>
    /// 二进制转 类
    /// </summary>
    /// <param name="savePath"></param>
    /// <returns></returns>
    public static object Binary2Class(string savePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            return formatter.Deserialize(fs);
        }

    }

}