using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 多语言按钮
/// </summary>
public class TransButton : Button, IInit
{
    [FieldName("高亮图资源名", HtmlColorDef.AliceBlue)]
    public string HighlightedAssetName;
    [FieldName("按下图资源名", HtmlColorDef.AliceBlue)]
    public string PressedAssetName;
    [FieldName("选中图资源名", HtmlColorDef.AliceBlue)]
    public string SelectedAssetName;
    [FieldName("禁用图资源名", HtmlColorDef.AliceBlue)]
    public string DisabledAssetName;
    [FieldName("资源加载类型")]
    public AssetLoadType LoadType = AssetLoadType.Permanent;
    [FieldName("语言设置界面元素，语言界面元素切换语言选中时就会切换资源，其他界面只有在确认选择某种语言后才切换资源")]
    public bool LanguagePanel;
    private bool m_IsDirty = true;//脏标记
    private bool m_Init;//初始化完毕

    private void Update()
    {
        UpdateImage();
    }

    private void UpdateImage()
    {
        if (!m_IsDirty) return;
        if (!Application.isPlaying) return;
        m_IsDirty = false;
        this.UpdateSpriteState(LoadType);
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