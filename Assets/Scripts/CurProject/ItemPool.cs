using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 武器 道具 Buff池子
/// </summary>
public class ItemPool : MonoSingleton<ItemPool>
{
    private List<int> m_ItemAndWeaponPool = new List<int>();
    /// <summary>
    /// 武器、道具池子(商店用)
    /// </summary>
    public List<int> ItemAndWeaponPool
    {
        get
        {
            m_ItemAndWeaponPool.Clear();
            ConfigManager.Instance.cfgItem.AllConfigs.ForEach(a =>
            {
                //过滤掉Buff
                if (a.id < 1000 || a.id >= 2000)
                {
                    //如果当前波次比道具能刷出来的最小波次大 就加入池子
                    if (a.id != 0 && Player.Instance.CurWave >= a.minWave)
                    {
                        m_ItemAndWeaponPool.Add(a.id);
                    }
                }
            });
            return m_ItemAndWeaponPool;
        }
    }

    private List<int> m_ItemsPool = new List<int>();
    /// <summary>
    /// 道具池子(宝箱用)
    /// </summary>
    public List<int> ItemsPool
    {
        get
        {
            m_ItemsPool.Clear();
            ConfigManager.Instance.cfgItem.AllConfigs.ForEach(a =>
            {
                //过滤掉Buff
                if (a.id < 1000)
                {
                    //如果当前波次比道具能刷出来的最小波次大 就加入池子
                    if (a.id != 0 && Player.Instance.CurWave >= a.minWave)
                    {
                        m_ItemsPool.Add(a.id);
                    }
                }
            });
            return m_ItemsPool;
        }
    }


    private List<int> m_BuffsPool = new List<int>();
    /// <summary>
    /// 升级Buff池子(升级Buff用)
    /// </summary>
    public List<int> BuffsPool
    {
        get
        {
            m_BuffsPool.Clear();
            ConfigManager.Instance.cfgItem.AllConfigs.ForEach(a =>
            {
                //过滤出Buff
                if (a.id >= 1000 && a.id < 2000)
                {
                    //如果当前波次比道具能刷出来的最小波次大 就加入池子
                    if (Player.Instance.CurWave >= a.minWave)
                    {
                        m_BuffsPool.Add(a.id);
                    }
                }
            });
            return m_BuffsPool;
        }
    }

}
