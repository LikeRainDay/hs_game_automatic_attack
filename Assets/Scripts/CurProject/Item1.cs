using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Buff选择界面元素
/// </summary>
public class Item1 : MonoBehaviour
{
    [FieldName("Buff图标")]
    public Image Icon;
    [FieldName("Buff名字文本")]
    public TransMeshPro Name;
    [FieldName("Buff效果文本")]
    public TransMeshPro Effect;
    private int m_ID;
    public int ID
    {
        get => m_ID;
        set
        {
            m_ID = value;
            //刷新内容
            RowCfgItem row = ConfigManager.Instance.cfgItem[value];
            Icon.SetSpriteAsync(row.icon);
            Name.UpdateTrans(row.name);
            Effect.UpdateTrans(row.effectIntroduce);
            //将按钮的颜色设置成对应品质的
            GetComponentInChildren<SoraButton>().BGNormal = GetComponent<ColorTool>().Colors3;
            GetComponentInChildren<SoraButton>().Rank = row.rank;

        }
    }
}
