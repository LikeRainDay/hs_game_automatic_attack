using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 根据品质自动设置颜色的工具
/// </summary>
public class ColorTool : MonoBehaviour
{
    [FieldName("外框")]
    public Image Image1;
    [FieldName("内框")]
    public Image Image2;
    [FieldName("小框1")]
    public Image Image3;
    [FieldName("小框2")]
    public Image Image4;
    [FieldName("名字")]
    public TransMeshPro NameText;

    [Header("各品质外框颜色")]
    public List<Color> Colors1;
    [Header("各品质内框颜色")]
    public List<Color> Colors2;
    [Header("各品质小框颜色")]
    public List<Color> Colors3;
    [Header("各品质名字颜色")]
    public List<Color> Colors4;
    private int m_Rank;
    public int Rank
    {
        get => m_Rank;
        set
        {
            m_Rank = value;
            if (Image1) Image1.color = Colors1[Rank - 1];
            if (Image2) Image2.color = Colors2[Rank - 1];
            if (Image3) Image3.color = Colors3[Rank - 1];
            if (Image4) Image4.color = Colors3[Rank - 1];
            if (NameText) NameText.color = Colors4[Rank - 1];
        }
    }
}
