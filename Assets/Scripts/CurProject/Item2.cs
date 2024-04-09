using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 商店商品元素(武器的详情页面也是用这个)
/// </summary>
public class Item2 : MonoBehaviour
{
    [FieldName("图标")]
    public Image Icon;
    [FieldName("名字")]
    public TransMeshPro Name;
    [FieldName("类型")]
    public TransMeshPro Type;
    [FieldName("效果")]
    public TransMeshPro Effect;
    [FieldName("价格")]
    public TransMeshPro Price;
    [FieldName("上锁")]
    public GameObject Locked;
    [FieldName("解锁")]
    public GameObject Unlocked;
    [FieldName("购买按钮")]
    public SoraButton Buy;
    private bool m_Lock;
    /// <summary>
    /// 是否锁住了
    /// </summary>
    public bool Lock
    {
        get => m_Lock;
        set
        {
            if (Locked == null) return;
            m_Lock = value;
            Locked.SetActive(value);
            Unlocked.SetActive(!value);
        }
    }
    private int m_ID;
    public int ID
    {
        get => m_ID;
        set
        {
            //刷新一下上锁和解锁按钮的显示隐藏
            Lock = Lock;
            //处于锁住状态时不会被刷新掉
            if (Lock) return;
            m_ID = value;

            //获取数据
            RowCfgItem row = ConfigManager.Instance.cfgItem[value];
            //根据品质 设置一下颜色
            GetComponent<ColorTool>().Rank = row.rank;
            //刷新内容
            Icon.SetSpriteAsync(row.icon);
            Name.UpdateTrans(row.name);
            Type.UpdateTrans(row.markTextID);
            string effect = ConfigManager.Instance.cfgText[row.effectIntroduce].contents[GameManager.Instance.CurLanguage.GetHashCode()];
            //如果是武器的话，需要先动态计算一堆数据，然后按照一套比较复杂的规则将内容显示出来
            if (value >= 2000)
            {
                Effect.text = Player.Instance.GetWeaponInfo(row.effectIntroduce);
            }
            //如果是道具，那么只需要将诸如  概率对随机1名敌人造成[@0.25*幸运]伤害 括号内的内容计算成一个具体数值，并且将括号去掉 以及将表格里用于代替#的~换回成#符号 以及恢复换行符
            else
            {
                Effect.text = Player.Instance.ItemContainer.TryCalcString(effect, true).Replace("[", "").Replace("]", "").Replace("~", "#").Replace("\\n", "\n");
            }

            if (Locked != null)
            {
                //临时逻辑: 商品价格=基础价格*玩家商品价格系数*(1+波次/10)
                Price.text = ((int)(row.price * Player.Instance.ItemsPrice * (1 + Player.Instance.CurWave / 10))).ToString();
                //将购买按钮的颜色设置成对应品质的
                Buy.BGNormal = GetComponent<ColorTool>().Colors3;
                Buy.Rank = row.rank;
            }
            else
            {
                GetComponentsInChildren<SoraButton>().ForEach(action => action.BGNormal = GetComponent<ColorTool>().Colors3);
                GetComponentsInChildren<SoraButton>().ForEach(action => action.Rank = row.rank);
            }  
        }
    }

    /// <summary>
    /// 刷新按钮状态
    /// </summary>
    public void UpdateBtnState()
    {
        //如果当前材料不够购买商品 就禁用购买按钮
        transform.Find("购买按钮").GetComponent<SoraButton>().Interactable = Player.Instance.Material >= Price.text.ToFloat();
    }

}
