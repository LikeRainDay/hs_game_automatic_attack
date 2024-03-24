using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// 各种瓢字
/// </summary>
public class Tip : MonoBehaviour
{
    [FieldName("要跟随的坐标点")]
    public Vector3 Pos;
    private RectTransform m_Rect;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="pos">要跟随的世界坐标点</param>
    /// <param name="content">要显示的文本内容</param>
    /// <param name="color">文本颜色</param>
    public void Init(Vector3 pos, string content, Color color)
    {
        Pos = pos;
        GetComponent<TextMeshProUGUI>().text = content;
        GetComponent<TextMeshProUGUI>().color = color;
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
