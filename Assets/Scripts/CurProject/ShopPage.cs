using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
/// <summary>
/// 商店页面
/// </summary>
public class ShopPage : PanelBase 
{
    [FieldName("左上角波次")]
    [SerializeField] private TransMeshPro m_CurWave;
    [FieldName("剩余材料值")]
    public TransMeshPro RemainMaterial;
    [FieldName("重随消耗")]
    [SerializeField] private TransMeshPro m_RerollCost;
    [FieldName("武器数量")]
    [SerializeField] private TransMeshPro m_WeaponCount;
    [FieldName("右下角波次")]
    [SerializeField] private TransMeshPro m_NextWave;
    [FieldName("重随按钮")]
    public SoraButton RerollBtn;
    [Header("商品元素")]
    public List<Item2> Items;
    [FieldName("道具父物体")]
    public GameObject ItemParent;
    [FieldName("武器父物体")]
    public GameObject WeaponParent;
    [FieldName("详情界面")]
    public Item2 Detail;
    [FieldName("遮挡用遮罩")]
    public GameObject Mask;
    [HideInInspector] public Item3 m_CurItem;

    public static ShopPage Instance;
    public override void Init(params object[] objs)
    {
        Instance = this;
        base.Init(objs);
    }

    private void OnEnable()
    {
        //刷新重随次数
        Player.Instance.CurRerollTimes1 = (int)Player.Instance.FreeRerolls;
        EventMgr.ExecuteEvent(EventName.EnterStore, new object[] { OwnerType.主角 });
        UpdateWave(Player.Instance.CurWave);
        UpdateWeaponCount();
        UpdateMaterial();
        //刷新武器和道具列表
        UpdateWeaponList();
        UpdateItemList();
        UpdateWeaponCount();
        //Player.Instance.CurRerollTimes = 0;
        //Player.Instance.CurRerollTimes1 = 0;
        Reroll();
        Detail.gameObject.Hide();
        Mask.Hide();
    }
    protected override void DelayShow(Action hideEvent = null)
    {
        base.DelayShow(hideEvent);
        FadeIn();       
    }

    protected override void DelayHide()
    {
        base.DelayHide();
        FadeOut();
    }

    /// <summary>
    /// 刷新波数
    /// </summary>
    /// <param name="curWave">当前波次</param>
    public void UpdateWave(int curWave)
    {
        m_CurWave.UpdateTrans(m_CurWave.TextId, curWave);
        m_NextWave.UpdateTrans(m_NextWave.TextId, curWave + 1);
    }

    /// <summary>
    /// 更新武器数量
    /// </summary>
    public int UpdateWeaponCount()
    {
        int count = 0;
        Player.Instance.ItemContainer.m_ItemDic.ForEach(a => 
        {
            if (a.Key>=2000)
            {
                count += a.Value.Count;
            }
        });

        m_WeaponCount.UpdateTrans(m_WeaponCount.TextId, count);
        return count;
    }

    /// <summary>
    /// 刷新材料数量
    /// </summary>
    public void UpdateMaterial()
    {
        //剩余材料
        RemainMaterial.text =Mathf.FloorToInt(Player.Instance.Material).ToString();
        //重随消耗
        m_RerollCost.UpdateTrans(m_RerollCost.TextId, DataMgr.GetRerollCost1());
        m_RerollCost.GetComponentInParent<SoraButton>().Interactable = Player.Instance.Material > DataMgr.GetRerollCost1();
        //刷新商品按钮的状态
        Items.ForEach(a => a.UpdateBtnState());
    }

    /// <summary>
    /// 刷新商品
    /// </summary>
    public void Reroll()
    {
        //如果当前材料值小于重新随机消耗 就无视
        if (!(Player.Instance.Material > DataMgr.GetRerollCost1())) return;

        Player.Instance.CurRerollTimes1++;
        //扣除刷新所需材料
        Player.Instance.MaterialChange(-DataMgr.GetRerollCost1());
        //刷新商品
        for (int i = 0; i < 4; i++)
        {
            Items[i].ID = DataMgr.RandomGetItemID();
            Items[i].transform.localScale = Vector3.one;
        }
        //更新价钱
        UpdateMaterial();
    }

    /// <summary>
    /// 购买
    /// </summary>
    /// <param name="index">位置索引</param>
    public void Buy(int index)
    {
        if (Player.Instance.Material < Items[index].Price.text.ToFloat()) return;
        if (Items[index].ID >= 2000 && Player.Instance.GetWeaponCount() >= 6) return;
        Items[index].transform.localScale = Vector3.zero;
        Items[index].Lock = false;
        //扣除所需材料
        Player.Instance.MaterialChange(-Items[index].Price.text.ToFloat());
        //加入背包
        Player.Instance.ItemContainer.AddItem(Items[index].ID);
        //刷新武器和道具列表
        UpdateWeaponList();
        UpdateItemList();
        UpdateWeaponCount();
        //刷新其他UI
        UpdateMaterial();
    }

    /// <summary>
    /// 刷新武器队列
    /// </summary>
    public void UpdateWeaponList()
    {
        int index = 0;
        //找到武器 按顺序绑定
        Player.Instance.ItemContainer.m_ItemDic.ForEach(a =>
        {
            //过滤出武器
            if (a.Key >=2000)
            {               
                a.Value.ForEach(b =>
                {
                    WeaponParent.transform.GetChild(index).GetComponent<Item3>().ID = a.Key;
                    WeaponParent.transform.GetChild(index).GetComponent<Item3>().ItemBase = b;
                    index++;
                });
               
            }
        });
        //将绑定了数据的元素显示出来，没绑定的隐藏
        for (int i = 0; i < WeaponParent.transform.childCount; i++)
        {
            WeaponParent.transform.GetChild(i).gameObject.SetActive(i < index);
        }
    }

    /// <summary>
    /// 刷新道具队列
    /// </summary>
    public void UpdateItemList()
    {
        int index = 0;
        //找到道具 按顺序绑定
        Player.Instance.ItemContainer.m_ItemDic.ForEach(a =>
        {
            if (a.Key<1000&&a.Value.Count>0)
            {
                ItemParent.transform.GetChild(index).GetComponent<Item3>().ID = a.Key;
                index++;
            }
        });
        //将绑定了数据的元素显示出来，没绑定的隐藏
        for (int i = 0; i < ItemParent.transform.childCount; i++)
        {
            ItemParent.transform.GetChild(i).gameObject.SetActive(i < index);
        }
    }

    /// <summary>
    /// 出售
    /// </summary>
    public void Sell()
    {
        Player.Instance.MaterialChange(m_CurItem.ItemBase.ItemData.price);
        Player.Instance.ItemContainer.RemoveItem(m_CurItem.ItemBase);
        m_CurItem.gameObject.Hide();
        Back();
    }

    /// <summary>
    /// 合成
    /// </summary>
    public void Merge()
    {
        //将一个与当前ID一致的武器合成一个更高级的武器
        int id = Detail.ID;
        List<ItemBase> items = Player.Instance.ItemContainer.m_ItemDic[id].RandomFromList(2, true);
        ItemBase itemBase = Player.Instance.ItemContainer.AddItem(id + 1);
        itemBase.GrowUpEffectCount = items[0].GrowUpEffectCount + items[1].GrowUpEffectCount;
        itemBase.Count = items[0].Count + items[1].Count;
        items.ForEach(a =>
        {
            Player.Instance.ItemContainer.RemoveItem(a);
        });
        UpdateWeaponList();
        Back();
    }

    /// <summary>
    /// 取消
    /// </summary>
    public void Back()
    {
        //显示一个遮罩
        Mask.Hide();
        Detail.gameObject.Hide();
    }

    /// <summary>
    /// 下一波
    /// </summary>
    public void Go()
    {
        if (BattleManager.Instance.Battle) return;
        EventMgr.ExecuteEvent(EventName.NextWave, new object[] { OwnerType.主角 });
    }

}
