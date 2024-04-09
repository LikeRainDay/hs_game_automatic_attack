using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
/// <summary>
/// 简单封装的Toggle按钮工具脚本  只支持鼠标点击
/// </summary>
public class SoraToggle : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("开启时背景图片颜色")]
    public Color OnColor;
    [Tooltip("关闭时背景图片颜色")]
    public Color OffColor;
    [Tooltip("小圆圈")]
    public Image Head;
    [Tooltip("背景图片")]
    public Image Bg;
    [FieldName("左右缩进")]
    public float Offset = 4;
    [FieldName("动画时长")]
    public float AnimTime = 0.25f;
    [Tooltip("开启事件")]
    public UnityEvent On;
    [Tooltip("关闭事件")]
    public UnityEvent Off;
    private Tween m_Tween;//滑块滑动动画Tween
    private bool m_IsOpen;
    public bool IsOpen
    {
        get => m_IsOpen;
        set
        {
            //设置状态 和 背景颜色
            m_IsOpen = value;
            Bg.color = value ? OnColor : OffColor;
            if (m_IsOpen)
            {
                On?.Invoke();
            }
            else
            {
                Off?.Invoke();
            }
            //拿到背景一半的长度
            float x = Bg.GetComponent<RectTransform>().sizeDelta.x / 2;
            //拿到头一半的长度
            float headRadius = Head.GetComponent<RectTransform>().sizeDelta.x / 2;
            //两边都各往回缩给定的长度 + 头一半的长度  
            float left = -x + Offset + headRadius;
            float right = x - Offset - headRadius;

            //如果显示着 就用动画位移一下 否则直接设置位置即可
            m_Tween.Kill();
            if (gameObject.activeInHierarchy)
            {
                m_Tween = Head.GetComponent<RectTransform>().DOAnchorPosX(value ? right : left, AnimTime);
            }
            else
            {
                Head.GetComponent<RectTransform>().anchoredPosition = new Vector2(value ? right : left, 0);
            }
        }
    }

    /// <summary>
    /// 初始化状态
    /// </summary>
    /// <param name="isOpen">启用</param>
    public void Init(bool isOpen)
    {
        //记录一下动画时长
        float time = AnimTime;
        //初始化不希望有动画，所以临时将时间修改为0
        AnimTime = 0;
        IsOpen = isOpen;
        //将动画时长修改回来
        AnimTime = time;
    }

    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //切换状态
        IsOpen = !IsOpen;
    }

    private void OnDisable()
    {
        m_Tween.Kill();
    }

}
