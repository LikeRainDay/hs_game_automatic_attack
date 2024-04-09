using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// 道具基类
/// </summary>
public class ItemBase
{
    #region 字段
    /// <summary>
    /// 这个道具的数据
    /// </summary>
    public RowCfgItem ItemData;
    /// <summary>
    /// 存储这个道具的容器
    /// </summary>
    public ItemContainer ItemContainer;
    /// <summary>
    /// 存储触发事件冷却状态的数组，某个索引的触发事件触发了就会将标识修改为false，等待一个延时任务之后会将标识修改回true
    /// </summary>
    private List<bool> m_Cooldown;
    /// <summary>
    /// 存储触发效果的数组，一开始记录一下，后续就不需要每次使用的时候去分割表格里获取到的数据了
    /// </summary>
    private string[] m_EffectGroups;
    /// <summary>
    /// 延时任务的ID，对象销毁的时候需要移除一下延时任务，避免出错
    /// </summary>
    private List<int> m_DelayTaskIDs = new List<int>();

    /// <summary>
    /// 能触发的次数
    /// </summary>
    public int CanTriggerTimes;
    /// <summary>
    /// 成长效果层数
    /// </summary>
    public int GrowUpEffectCount;
    private int m_CurTimes;
    /// <summary>
    /// 当前经验
    /// </summary>
    public int CurTimes
    {
        get=>m_CurTimes;
        set
        {
            m_CurTimes = value;
            while (m_CurTimes>LevelUpNeedTimes)
            {
                m_CurTimes -= LevelUpNeedTimes;
                GrowUpEffectCount++;
                ItemContainer.NeedRebuild = true;
            }
        }
    }
    /// <summary>
    /// 升级所需经验
    /// </summary>
    public int LevelUpNeedTimes;
    /// <summary>
    /// 计数
    /// </summary>
    public int Count;
    #endregion

    #region 添加道具 移除道具
    /// <summary>
    /// 道具添加
    /// </summary>
    /// <param name="container">存放和管理这个道具的容器</param>
    /// <param name="itemData">这个道具的配置数据</param>
    public virtual void OnAdd(ItemContainer container, RowCfgItem itemData)
    {
        //绑定数据     
        ItemData = itemData;
        ItemContainer = container;
        ItemContainer.NeedRebuild = true;
        CanTriggerTimes = ItemData.canTriggerTimes == 0 ? 99999999 : ItemData.canTriggerTimes;
        LevelUpNeedTimes = ItemData.levelUpNeedTimes;
        //触发添加瞬间的效果
        if (!string.IsNullOrEmpty(ItemData.gainEffect))
        {
            //分割成子事件
            string[] events= ItemData.gainEffect.Split('。');
            events.ForEach(a => 
            {
                //分割获取到这个事件的参数
                string[] strs = a.Split(a.Contains("，") ? "，" : ",");
                //如果是事件 就执行事件  如果不是事件，就对对应属性就行操作
                if (strs[0] == StringDef.Event || strs[0] == StringDef.事件)
                {
                    ToolFun.ExecuteEvents(a);
                }
                else
                {
                    switch (strs[0])
                    {
                        case "当前生命值":
                            break;
                        case "最大生命值上限":
                            break;
                        default:
                            break;
                    }
                }
            });
            
        }

        //如果有触发类效果 那么就监听一下触发事件
        if (!string.IsNullOrEmpty(ItemData.trigger))
        {
            //一开始，全部事件都是冷却完毕可触发的
            m_Cooldown = new List<bool>();
            itemData.triggerCD.ForEach(a => m_Cooldown.Add(true));
            //分割触发效果组 和触发时机组  【XX】。【XX】|【XX】|【XX】。【XX】。【XX】
            m_EffectGroups = ItemData.triggerEffect.Split('|');
            string[] triggerGroups = ItemData.trigger.Split('|');

            //获取触发效果组数量，每次注册事件后数量--，剩余数量小于等于0了就结束
            int count = m_EffectGroups.Length;
            //分割字符串，获取事件名数组，然后挨个绑定事件
            int index = m_EffectGroups.Length - count;//count越来越小 index就越来越大，相当于从前往后绑定事件
            string[] eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.RegisterEvent(item, OnEvent1);
            }
            count--;
            if (count <= 0) return;

            index = m_EffectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.RegisterEvent(item, OnEvent2);
            }
            count--;
            if (count <= 0) return;

            index = m_EffectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.RegisterEvent(item, OnEvent3);
            }
            count--;
            if (count <= 0) return;

            index = m_EffectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.RegisterEvent(item, OnEvent4);
            }
            count--;
            if (count <= 0) return;

            index = m_EffectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.RegisterEvent(item, OnEvent5);
            }
        }

    }

    /// <summary>
    /// 道具移除
    /// </summary>
    public virtual void OnRemove()
    {
        //添加和移除都重建一下堆叠字典
        ItemContainer.NeedRebuild = true;

        //如果有触发类效果 那么就移除一下触发事件
        if (!string.IsNullOrEmpty(ItemData.trigger))
        {
            //分割触发效果组 和触发时机组  【XX】。【XX】|【XX】|【XX】。【XX】。【XX】
            string[] effectGroups = ItemData.triggerEffect.Split('|');
            string[] triggerGroups = ItemData.trigger.Split('|');

            //获取触发效果组数量，每次注册事件后数量--，剩余数量小于等于0了就结束
            int count = effectGroups.Length;
            //分割字符串，获取事件名数组，然后挨个绑定事件
            int index = effectGroups.Length - count;//count越来越小 index就越来越大，相当于从前往后绑定事件
            string[] eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.UnRegisterEvent(item, OnEvent1);
            }
            count--;
            if (count <= 0) return;

            index = effectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.UnRegisterEvent(item, OnEvent2);
            }
            count--;
            if (count <= 0) return;

            index = effectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.UnRegisterEvent(item, OnEvent3);
            }
            count--;
            if (count <= 0) return;

            index = effectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.UnRegisterEvent(item, OnEvent4);
            }
            count--;
            if (count <= 0) return;

            index = effectGroups.Length - count;
            eventNames = triggerGroups[index].Split(triggerGroups[index].Contains("，") ? '，' : ',');
            foreach (var item in eventNames)
            {
                EventMgr.UnRegisterEvent(item, OnEvent5);
            }
        }
        m_DelayTaskIDs.ForEach(a => TimeMgr.Timer.DeleteTimeTask(a));
    }
    #endregion

    #region 事件
    private object OnEvent1(object[] arg) { OnEvent(0, arg); return null; }
    private object OnEvent2(object[] arg) { OnEvent(1, arg); return null; }
    private object OnEvent3(object[] arg) { OnEvent(2, arg); return null; }
    private object OnEvent4(object[] arg) { OnEvent(3, arg); return null; }
    private object OnEvent5(object[] arg) { OnEvent(4, arg); return null; }

    /// <summary>
    /// 通用事件函数
    /// </summary>
    /// <param name="index">事件数据索引</param>
    /// <param name="arg">普通参数数组</param>
    /// <param name="arg1">特殊参数数组(一般为空)</param>
    /// <returns></returns>
    private object OnEvent(int index, object[] arg)
    {
        if (!m_Cooldown[index]) return null;//如果此索引事件还未冷却完毕就返回
        //如果事件的的接收者不是自己就无视，是自己就调整一下字符串内容，方便后续计算
        if (!ItemContainer.Role.Replace(arg)) return null;
        if (CanTriggerTimes <= 0) return null;
        //可触发次数--
        CanTriggerTimes--;

        //获取到对应的触发效果
        string effect = m_EffectGroups[index];
        //生效效果
        ItemContainer.DoEffect(effect,this);
        //等待冷却时长，重置标识
        m_DelayTaskIDs.Add(TimeMgr.Timer.AddTimeTask(a => m_Cooldown[index] = true, ItemData.triggerCD[index] * 1000));
        return null;
    }
    #endregion

}

