using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 升级属性瓢字
/// </summary>
public class Tip1 : MonoBehaviour
{
    [FieldName("要跟随的坐标点")]
    public Vector3 Pos;
    private RectTransform m_Rect;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="pos">要跟随的世界坐标点</param>
    /// <param name="sp">要显示的图片</param>
    public void Init(Vector3 pos, Sprite sp)
    {
        Pos = pos;
        GetComponentInChildren<Image>().sprite = sp;
    }

    private void Start()
    {
        EventMgr.RegisterEvent(EventName.Clear, Clear);
        m_Rect = GetComponent<RectTransform>();
        Destroy(gameObject, 2);
    }

    private void Update()
    {
        //设置坐标
        Vector2 view = Camera.main.WorldToViewportPoint(Pos);
        m_Rect.anchoredPosition = new Vector2(view.x * 3840, view.y * 2160);
    }

    private void OnDestroy()
    {
        EventMgr.UnRegisterEvent(EventName.Clear, Clear);
    }
    private object Clear(object[] arg)
    {
        Destroy(gameObject);
        return null;
    }
}
