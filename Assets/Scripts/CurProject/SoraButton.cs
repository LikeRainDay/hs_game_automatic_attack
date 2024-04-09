using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 有一些基础反馈效果的按钮工具脚本
/// </summary>
public class SoraButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int m_Rank;
    /// <summary>
    /// 品质等级，不同品质用不同的颜色
    /// </summary>
    public int Rank
    {
        get => m_Rank;
        set
        {
            m_Rank = value;
            if (value != 0)
            {
                //将按钮普通状态下的颜色设置成数组里对应索引的颜色
                BgNormal = BGNormal[value - 1];
            }
        }
    }

    [FieldName("背景颜色常态")]
    public Color BgNormal;
    [HideInInspector] public List<Color> BGNormal;
    [FieldName("背景颜色选中")]
    public Color BgSelected;
    [FieldName("背景颜色禁用常态")]
    public Color BgUnInteractableNormal;
    [FieldName("背景颜色禁用选中")]
    public Color BgUnInteractableSelected;
    [FieldName("文本颜色常态")]
    public Color TextNormal;
    [FieldName("文本颜色选中")]
    public Color TextSelected;
    [FieldName("文本颜色禁用常态")]
    public Color TextUnInteractableNormal;
    [FieldName("文本颜色禁用选中")]
    public Color TextUnInteractableSelected;
    [FieldName("背景图片")]
    public Image Bg;
    [FieldName("文本组件")]
    public TextMeshProUGUI Text;
    private bool m_Enter;//是否选中
    private Tween m_Tweem;

    private void OnDisable()
    {
        //显示的时候，规整一下状态
        TimeMgr.Timer.AddFrameTask(a =>
        {
            m_Enter = false;
            Interactable = true;
            Bg.color = BgNormal;
            Text.color = TextNormal;
            transform.localScale = Vector3.one;
        }, 2);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Enter = true;
        Bg.color = m_Interactable ? BgSelected : BgUnInteractableSelected;
        Text.color = m_Interactable ? TextSelected : TextUnInteractableSelected;
        m_Tweem.Kill();
        m_Tweem = transform.DOScale(Vector3.one * 1.1f, 0.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Enter = false;
        Bg.color = m_Interactable ? BgNormal : BgUnInteractableNormal;
        Text.color = m_Interactable ? TextNormal : TextUnInteractableNormal;
        m_Tweem.Kill();
        m_Tweem = transform.DOScale(Vector3.one * 1, 0.1f);
    }

    private bool m_Interactable = true;
    public bool Interactable
    {
        get => m_Interactable;
        set
        {
            m_Interactable = value;
            GetComponent<Button>().interactable = value;
            if (!m_Interactable)
            {
                Bg.color = m_Enter ? BgUnInteractableSelected : BgUnInteractableNormal;
                Text.color = m_Enter ? TextUnInteractableSelected : TextUnInteractableNormal;
            }
            else
            {
                Bg.color = m_Enter ? BgSelected : BgNormal;
                Text.color = m_Enter ? TextSelected : TextNormal;
            }
        }
    }
}
