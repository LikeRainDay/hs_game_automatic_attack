using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 升级Buff选择界面
/// </summary>
public class BuffSelectedPage : PanelBase
{
    [FieldName("重随消耗")]
    public TransMeshPro RerollCost;
    [Header("元素")]
    public List<ColorTool> ColorTools;
    private List<RowCfgItem> m_Datas = new List<RowCfgItem>();//存放当前能随机到的全部buff
    private List<RowCfgItem> m_CurDatas = new List<RowCfgItem>();//存放当前随到的buff
    public static BuffSelectedPage Instance;
    private float m_CD;//交互CD，选择一个Buff之后稍微等待下才能选择下一个Buff
    public override void Init(params object[] objs)
    {
        Instance = this;
        base.Init(objs);
    }

    protected override void DelayShow(Action hideEvent = null)
    {
        base.DelayShow(hideEvent);
        FadeIn();
        //规整元素缩放
        for (int i = 0; i < 4; i++)
        {
            Item1 item1 = ColorTools[i].GetComponent<Item1>();
            item1.transform.localScale = Vector3.one;
        }
        //刷新重随次数
        Player.Instance.CurRerollTimes = 0;
        //随机一波元素
        Reroll();
    }

    protected override void DelayHide()
    {
        base.DelayHide();
        FadeOut();
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public void Reroll()
    {
        m_Datas.Clear();
        //得到一批当前波次能解锁的Buff
        ConfigManager.Instance.cfgItem.AllConfigs.ForEach(a =>
        {
            if (a.id >= 1000 && a.id < 2000 && a.minWave <= Player.Instance.CurWave)
            {
                m_Datas.Add(a);
            }
        });
        //计算总权重 然后随机一组
        float totalWeight = 0;
        m_Datas.ForEach(a => totalWeight += a.weight);
        //存放随到元素的数组
        m_CurDatas.Clear();
        while (m_CurDatas.Count < 4)
        {
            float randon = totalWeight.GetRandom();
            float temp = 0;
            foreach (var item in m_Datas)
            {
                temp += item.weight;
                if (temp > randon)
                {
                    if (!m_CurDatas.Contains(item))
                    {
                        m_CurDatas.Add(item);
                    }
                    break;
                }
            }

        }
        //刷新元素
        for (int i = 0; i < 4; i++)
        {
            //设置品质
            ColorTools[i].Rank = m_CurDatas[i].rank;
            //设置内容
            Item1 item1 = ColorTools[i].GetComponent<Item1>();
            item1.ID = m_CurDatas[i].id;
            item1.transform.DOScaleX(0, 0.12f).onComplete += () =>
              {
                  item1.transform.DOScaleX(1, 0.08f);
              };
        }
        RerollCost.UpdateTrans(RerollCost.TextId, DataMgr.GetRerollCost());
        Player.Instance.CurRerollTimes++;
    }

    /// <summary>
    /// 选择某个加成
    /// </summary>
    public void Selected(int index)
    {
        if (m_CD > 0) return;
        m_CD = 0.3f;
        //玩家获得某个加成，如果还有次数就刷新，没有次数就到下一个环节
        Player.Instance.ItemContainer.AddItem(m_CurDatas[index].id);
        Player.Instance.LevelUpTimes--;
        if (Player.Instance.LevelUpTimes > 0)
        {
            Player.Instance.CurRerollTimes = 0;
            Reroll();
        }
        else
        {
            //隐藏当前界面和战斗界面 显示商店界面
            UIMgr.Instance.ShowPage(new List<GameObject>() { gameObject, BattlePage.Instance.gameObject }, ShopPage.Instance.gameObject, null, 0.5f, 0.25f, 0.5f);
        }

    }

    public override void Update()
    {
        base.Update();
        m_CD -= Time.deltaTime;
    }
}
