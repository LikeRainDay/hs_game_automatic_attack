using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 多语言Text文本组件
/// </summary>
public class TransText : Text, IInit
{
    [FieldName("翻译id(为0不翻译)", HtmlColorDef.AliceBlue)]
    public int TextId;
    [FieldName("字体配置ID(为0则采用当前字体对应的配置ID)", HtmlColorDef.AliceBlue)]
    public int FontSettingId;
    [FieldName("文本组件设置id(为0采用默认)", HtmlColorDef.AliceBlue)]
    public int SettingId;
    [FieldName("语言设置界面元素，语言界面元素切换语言选中时就会切换资源，其他界面只有在确认选择某种语言后才切换资源")]
    public bool LanguagePanel;

    private bool m_IsDirty; //脏标记
    private int m_CacheFontSize; //初始字体大小
    private float m_CacheLineSpacing; //初始行间距
    private bool m_AlreadyRegisterEvent;//已经注册过事件，注册过事件才需要在销毁物体的时候移除事件
    private readonly List<TransContent> m_TransContentList = new List<TransContent>(4); //翻译文本列表
    private readonly StringBuilder m_ResultBuilder = new StringBuilder(); //用于拼接结果
    private bool m_AreadySet;
    protected override void Start()
    {
        //更新文本
        if (!m_AreadySet && TextId != 0)
        {
            UpdateTrans(TextId);
        }
    }

    protected void Update()
    {
        UpdateText();
    }

    /// <summary>
    /// 更新翻译id
    /// </summary>
    /// <param name="id">文本ID</param>
    public void UpdateTrans(int id)
    {
        TextId = id;
        m_TransContentList.Clear();
        m_TransContentList.Add(LanguageUtil.CreateTransContent(id));
        m_IsDirty = true;
        UpdateText();
    }

    /// <summary>
    /// 更新翻译id
    /// </summary>
    /// <param name="id">文本ID</param>
    /// <param name="paramArray">填充占位坑位的数据</param>
    public void UpdateTrans(int id, params object[] paramArray)
    {
        m_AreadySet = true;
        TextId = id;
        m_TransContentList.Clear();
        m_TransContentList.Add(LanguageUtil.CreateTransContent(id, paramArray));
        m_IsDirty = true;
        UpdateText();
    }

    /// <summary>
    /// 更新翻译id，可能用不到
    /// </summary>
    /// <param name="transContentList"></param>
    public void UpdateTrans(params TransContent[] transContentList)
    {
        m_TransContentList.Clear();
        m_TransContentList.AddRange(transContentList);
        m_IsDirty = true;
        UpdateText();
    }

    /// <summary>
    /// 更新文本
    /// </summary>
    private void UpdateText()
    {
        if (!m_IsDirty) return;
        if (!Application.isPlaying) return;

        m_IsDirty = false;
        m_ResultBuilder.Clear();
        if (m_TransContentList.Count <= 0) return;

        foreach (var transContent in m_TransContentList)
        {
            m_ResultBuilder.Append(transContent.ToString());
        }
        text = m_ResultBuilder.ToString().RecoverEnter();
    }

    /// <summary>
    /// 语言更变回调
    /// </summary>
    /// <param name="arg"></param>
    private object OnLanguageChanged(object[] arg = null)
    {
        //切换语言选中时分发的事件(arg=new object[])，只有语言选择界面的元素需要响应
        //确认语言时分发的事件(arg=null)，大家都会响应
        if (arg != null && !LanguagePanel) return null;

        //Text组件估计有编辑模式运行的特性 每次关掉都会走awake
        if (!Application.isPlaying) return null;

        LanguageChanged();
        return null;
    }

    /// <summary>
    /// 语言变更
    /// </summary>
    /// <param name="f"></param>
    private async void LanguageChanged()
    {
        //获取字体
        font = await LanguageDef.GetFont<Font>(FontSettingId, font.name);

        //如果缺少配置 则使用默认字体配置
        if (SettingId == 0) SettingId = 1;

        var rowCfgFontSetting = ConfigManager.Instance.cfgFontSetting[SettingId];
        var languageId = GameManager.Instance.CurLanguage.GetHashCode();
        //越界
        if (languageId >= rowCfgFontSetting.textSettings.Length)
        {
            Debug.LogError($"当前语言缺少字体配置:{GameManager.Instance.CurLanguage.ToString()}");
            return;
        }

        //调整新字体参数
        fontSize = (int)(m_CacheFontSize * rowCfgFontSetting.textSettings[languageId].fontSizeScale);
        lineSpacing = m_CacheLineSpacing * rowCfgFontSetting.textSettings[languageId].lineSpacingScale;
        //标记当前帧更新
        m_IsDirty = true;
        UpdateText();
    }

    public void Init(params object[] objs)
    {
        //初始化 添加翻译文本
        m_TransContentList.Clear();
        //无视0
        if (TextId != 0)
        {
            m_TransContentList.Add(LanguageUtil.CreateTransContent(TextId));
        }

        //缓存当前的字体大小 间距
        m_CacheFontSize = fontSize;
        m_CacheLineSpacing = lineSpacing;

        //注册事件
        m_AlreadyRegisterEvent = true;
        EventMgr.RegisterEvent(EventName.LanguageChanged, OnLanguageChanged);
    }

    protected override void OnDestroy()
    {
        if (m_AlreadyRegisterEvent) EventMgr.UnRegisterEvent(EventName.LanguageChanged, OnLanguageChanged);
    }

}