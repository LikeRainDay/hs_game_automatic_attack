using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 简化版多语言方案用的多语言工具
/// </summary>
public class LanguageTextTool : MonoBehaviour
{
    [FieldName("多语言文本ID")]
    public int TextId;

    private Text m_Text;
    private void Start()
    {
        //注册语言切换事件
        EventMgr.RegisterEvent(EventName.LanguageChanged, ChangeLanguage);
        m_Text = GetComponent<Text>();
    }

    private async void OnEnable()
    {
        //等待数据初始化完毕
        while (GameManager.Instance == null || GameManager.Instance.TextDic.Count == 0)
        {
            await Task.Delay(10);
        }
        ChangeLanguage(null);
    }

    private object ChangeLanguage(object[] arg)
    {
        m_Text.text = GameManager.Instance.TextDic[TextId][GameManager.Instance.CurLanguage.GetHashCode()];
        return null;
    }

    private void OnDestroy()
    {
        //注销语言切换事件
        EventMgr.UnRegisterEvent(EventName.LanguageChanged, ChangeLanguage);
    }

    /// <summary>
    /// 更新文本ID
    /// </summary>
    /// <param name="textID">文本ID</param>
    public void UpdateTextID(int textID)
    {
        TextId = textID;
        ChangeLanguage(null);
    }
}
