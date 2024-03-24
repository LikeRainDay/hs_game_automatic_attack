using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
/// <summary>
/// 鼠标指针进入 退出 按下 和点击的时候的 按钮反馈动画效果工具脚本(简单加强按钮的反馈感)
/// </summary>
public class ButtonAnimTool : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    [Tooltip("动画效果时长")]
    public float AnimTime = 0.25f;
    [Tooltip("普通状态下按钮比例")]
    public Vector3 NormalScale = new Vector3(1, 1, 1);
    [Tooltip("指针进入时按钮比例")]
    public Vector3 PointEnterScale = new Vector3(1.12f, 1.12f, 1.12f);
    [Tooltip("指针按下时按钮比例")]
    public Vector3 PointDownScale = new Vector3(0.9f, 0.9f, 0.9f);

    [Header("点击抖动效果")]
    [Tooltip("开启点击抖动效果")]
    public bool ClickShake = true;
    [Tooltip("抖动时长")]
    public float ShakeTime = 0.5f;
    [Tooltip("抖动强度")]
    public int ShakeLevel = 2;
    [FieldName("抖动频率")]
    public int Vibrato = 15;
    [FieldName("抖动随机度")]
    public int Randomness = 90;
    [Tooltip("动画作用目标，不填写直接用当前脚本所在物体的")]
    public Transform TargetTrans;
    private Tween m_Tween;//缩放动画的Tween

    public virtual void OnEnable()
    {
        //如果没有主动赋值目标transform组件，就使用当前的
        if (TargetTrans == null)
        {
            TargetTrans = transform;
        }
        //显示的时候 规整一下缩放
        TargetTrans.localScale = NormalScale;
    }

    public virtual void OnDisable()
    {
        //隐藏的时候 关闭一下tween动画
        m_Tween.Kill();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        m_Tween.Kill();
        m_Tween = TargetTrans.DOScale(PointEnterScale, AnimTime);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        m_Tween.Kill();
        m_Tween = TargetTrans.DOScale(NormalScale, AnimTime);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        m_Tween.Kill();
        m_Tween = TargetTrans.DOScale(PointDownScale, AnimTime);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (ClickShake) return;//如果开启了抖动，那么就没必要在加这段表现了
        m_Tween.Kill();
        m_Tween = TargetTrans.DOScale(NormalScale, AnimTime / 2);//抬起复原速度快一些可能会舒服点
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (ClickShake)
        {
            m_Tween.Kill();
            TargetTrans.localScale = NormalScale;
            m_Tween = TargetTrans.DOShakeScale(ShakeTime, new Vector3(0.03f * ShakeLevel, 0.03f * ShakeLevel, 0.03f * ShakeLevel), Vibrato, Randomness).SetLoops(1, LoopType.Yoyo);
        }
    }

}

