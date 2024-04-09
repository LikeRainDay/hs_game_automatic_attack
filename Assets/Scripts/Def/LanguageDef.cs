using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 语言枚举
/// </summary>
public enum Language
{
    /// <summary>
    /// 中文
    /// </summary>
    [EnumName("中文")]
    Chinese = 0,
    /// <summary>
    /// 英文
    /// </summary>
    [EnumName("英文")]
    English = 1,
    /// <summary>
    /// 日文
    /// </summary>
    [EnumName("日文")]
    Japanese = 2,

}

/// <summary>
/// 语言相关的一些定义
/// </summary>
public static class LanguageDef
{
    /// <summary>
    /// 如果文本组件上没有设置字体配置ID，就根据字体名字来这个字典查找ID，如果设置了字体配置ID就直接使用对应ID
    /// </summary>
    private static readonly Dictionary<string, int> FontName2FontCfgId = new Dictionary<string, int>()
    {
        //eg.如果当前字体名字是 LiberationSans ，且字体组件上没有设置字体配置ID，那么就使用 1这个字体配置ID
        {"LiberationSans", 1},
        {"SourceHanSansBoldCn", 2},
        {"ComicMeshPro", 3},
        {"沐瑶随心手写体",4 },
        {"荆南麦圆体",5 },
        {"占位子体",6},
        {"中文像素字体",7},
    };

    /// <summary>
    /// 根据传入的字体配置ID获取对应语言的字体，如果没有传入字体配置ID(默认0)，那么就根据传入的当前字体的名字来获取对应的字体
    /// </summary>
    /// <param name="fontCfgId">字体配置ID</param>
    /// <param name="fontName">当前字体的名字</param>
    /// <returns></returns>
    public static async Task<T> GetFont<T>(int fontCfgId, string fontName) where T : Object
    {
        //如果没有设置字体配置ID 那么就根据当前字体的名字去字典里查找
        if (fontCfgId == 0)
        {
            fontCfgId = FontName2FontCfgId[fontName];
        }

        //获取到当前语言的字体ID
        int fontId = ConfigManager.Instance.cfgFontMap[fontCfgId].fontIds[GameManager.Instance.CurLanguage.GetHashCode()];
        //根据字体ID来获取对应的路径 来加载字体
        var fontPath = ConfigManager.Instance.cfgFont[fontId].path;
        return await AssetMgr.LoadAssetAsync<T>(fontPath);
    }

}