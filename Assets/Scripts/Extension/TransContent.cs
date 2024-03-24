using System.Text;
using UnityEngine;
/// <summary>
/// 多语言内容数据
/// </summary>
public struct TransContent
{
    public int TransId; //翻译id
    public object[] Args; //参数数组 用于替换掉标记符
    private StringBuilder m_PreArg; //前缀
    private StringBuilder m_EndArg; //后缀
    private StringBuilder m_ResultBuilder; //用于拼接最终结果

    public static TransContent Default
    {
        get
        {
            TransContent transContent;
            transContent.TransId = 0;
            transContent.Args = null;
            transContent.m_PreArg = null;
            transContent.m_EndArg = null;
            transContent.m_ResultBuilder = null;
            return transContent;
        }
    }

    /// <summary>
    /// 添加前缀
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reset">修改前缀为当前给的值</param>
    public void AddPreArg(string value, bool reset = false)
    {
        if (m_PreArg == null)
        {
            m_PreArg = new StringBuilder();
        }
        if (reset)
        {
            m_PreArg.Clear();
        }
        m_PreArg.Append(value);
    }

    /// <summary>
    /// 添加后缀
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reset">修改后缀为当前给的值</param>
    public void AddEndArg(string value, bool reset = false)
    {
        if (m_EndArg == null)
        {
            m_EndArg = new StringBuilder();
        }
        if (reset)
        {
            m_PreArg.Clear();
        }
        m_EndArg.Append(value);
    }

    public override string ToString()
    {
        if (m_ResultBuilder == null)
        {
            m_ResultBuilder = new StringBuilder();
        }
        else
        {
            m_ResultBuilder.Clear();
        }

        //添加前缀
        if (m_PreArg != null)
        {
            m_ResultBuilder.Append(m_PreArg);
        }
        //获取当前语言的文本内容
        var text = LanguageUtil.GetTransStr(TransId);
        //参数数组args替换掉标记符{0}{1}...可以得到一个string结构的文本  new object[] {8,1.5 };  小王今年{0}岁了，身高{1}米
        if (Args != null && Args.Length > 0)
        {
            
            //替换动态参数
            for (var i = 0; i < Args.Length; i++)
            {
                //当前占位符
                var placeholder = $"{{{i}}}";
                text = text.Replace(placeholder, Args[i].ToString());
            }
           
        }
        m_ResultBuilder.Append(text);

        //添加后缀
        if (m_EndArg != null)
        {
            m_ResultBuilder.Append(m_EndArg);
        }
        return m_ResultBuilder.ToString();
    }

}