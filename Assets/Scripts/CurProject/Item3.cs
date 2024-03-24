using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 商店界面道具元素 和武器元素
/// </summary>
public class Item3 : MonoBehaviour
{
    [HideInInspector] public ItemBase ItemBase;
    [FieldName("是武器")]
    public bool Weapon;
    [FieldName("图标")]
    public Image Icon;
    [FieldName("数量")]
    public TransMeshPro Count;
    private int m_ID;
    public int ID
    {
        get => m_ID;
        set
        {
            m_ID = value;
            //设置图标
            Icon.SetSpriteAsync(ConfigManager.Instance.cfgItem[value].icon);
            //武器不用显示数量
            if (Weapon)
            {
                Count.text = "";
            }
            else
            {
                //道具数量为1的时候也不显示数量
                Count.text = Player.Instance.ItemContainer.m_ItemDic[value].Count == 1 ? "" : Player.Instance.ItemContainer.m_ItemDic[value].Count.ToString();
            }
            //根据品质来设置颜色
            GetComponent<ColorTool>().Rank = ConfigManager.Instance.cfgItem[value].rank;
        }
    }

    /// <summary>
    /// 点击武器，显示详细信息
    /// </summary>
    public void ShowDetail()
    {
        //没有元素的就无视
        if (ID == 0) return;
        if (!Weapon) return;
        //显示详情页面
        ShopPage.Instance.Detail.ID = ID;
        ShopPage.Instance.m_CurItem = this;
        ShopPage.Instance.Detail.gameObject.SetActive(true);
        ShopPage.Instance.Detail.transform.position = transform.position;
        //显示一个遮罩
        ShopPage.Instance.Mask.Show();
    }
}
