using System;
/// <summary>
/// 多语言工具
/// </summary>
public static class LanguageUtil
{
    /// <summary>
    /// 创建实例
    /// </summary>
    /// <param name="transId"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static TransContent CreateTransContent(int transId, params object[] args)
    {
        var transContent = TransContent.Default;
        transContent.TransId = transId;
        transContent.Args = args;
        return transContent;
    }

    /// <summary>
    /// 获取给定ID当前语言的文本
    /// </summary>
    /// <param name="transTextId">文本ID</param>
    /// <returns></returns>
    public static string GetTransStr(int transTextId)
    {
        try
        {
            //                                                               PS:因为#作为Excel表格的分隔符了，所以16进制颜色的#符号在表格里用~代替，这里将其替换回来
            return ConfigManager.Instance.cfgText[transTextId].contents[(int)GameManager.Instance.CurLanguage].Replace("~", "#");
        }
        catch (Exception e)
        {
            LogUtil.Error($"翻译缺失: {e.ToString()} 翻译ID:{transTextId}");
            return "missing translation";
        }
    }

}