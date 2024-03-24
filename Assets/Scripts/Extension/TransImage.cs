using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 多语言UI图片
/// </summary>
public class TransImage : Image, IInit
{
    [FieldName("资源名", HtmlColorDef.AliceBlue)]
    public string SpriteAssetName;
    [FieldName("资源加载类型")]
    public AssetLoadType LoadType = AssetLoadType.Permanent;
    [FieldName("语言设置界面元素，语言界面元素切换语言选中时就会切换资源，其他界面只有在确认选择某种语言后才切换资源")]
    public bool LanguagePanel;  
    private bool m_IsDirty = true; //脏标记
    private bool m_Init;//初始化完毕

    /// <summary>
    /// 更新翻译id
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="loadType">资源加载类型</param>
    public void UpdateTrans(string assetName, AssetLoadType loadType = AssetLoadType.Permanent)
    {
        m_IsDirty = true;
        LoadType = loadType;
        SpriteAssetName = assetName;
        UpdateImage();
    }

    private void Update()
    {
        UpdateImage();
    }

    private void UpdateImage()
    {
        if (!m_IsDirty) return;
        if (!Application.isPlaying) return;
        if (string.IsNullOrEmpty(SpriteAssetName)) return;
        m_IsDirty = false;
        this.SetSpriteAsync(SpriteAssetName, false, LoadType);
    }

    public void Init(params object[] objs)
    {
        m_Init = true;
        EventMgr.RegisterEvent(EventName.LanguageChanged, OnLanguageChanged);
    }
    protected override void OnDestroy()
    {
        if (m_Init) EventMgr.UnRegisterEvent(EventName.LanguageChanged, OnLanguageChanged);
    }

    private object OnLanguageChanged(object[] arg = null)
    {
        //切换语言选中时分发的事件(arg=new object[])，只有语言选择界面的元素需要响应
        //确认语言时分发的事件(arg=null)，大家都会响应
        if (arg != null && !LanguagePanel) return null;
        m_IsDirty = true;
        UpdateImage();
        return null;
    }

}