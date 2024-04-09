using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据管理器，可以在这里将一些配置表里的数据转换成更方便使用的形式，比如字典的形式    游戏运行时的一些数据也可以汇总到这里，方便使用和统一管理
/// </summary>
public class DataMgr
{
    private static Dictionary<string, string> m_AssetPathDic;
    /// <summary>
    /// 资源路径字典，键是资源在表格里的名称，值是资源的路径
    /// </summary>
    public static Dictionary<string, string> AssetPathDic
    {
        get
        {
            if (m_AssetPathDic == null || m_AssetPathDic.Count == 0)
            {
                //根据项目实际情况 将各类资源都放到这个字典里，方便外部使用
                m_AssetPathDic = new Dictionary<string, string>();

                //精灵图片
                ConfigManager.Instance.cfgSprite.AllConfigs.ForEach(a =>
                {
                    Add(a.annotate, a.path);
                });
                //预制体
                ConfigManager.Instance.cfgPrefab.AllConfigs.ForEach(a =>
                {
                    Add(a.annotate, a.path);
                });
                //场景
                ConfigManager.Instance.cfgScene.AllConfigs.ForEach(a =>
                {
                    Add(a.annotate, a.path);
                });
                //音频
                ConfigManager.Instance.cfgAudioClip.AllConfigs.ForEach(a =>
                {
                    Add(a.annotate, a.path);
                });
                //字体
                ConfigManager.Instance.cfgFont.AllConfigs.ForEach(a =>
                {
                    Add(a.annotate, a.path);
                });

            }
            return m_AssetPathDic;
        }
    }

    private static Dictionary<EnumAudioClip, string> m_AudioClipPathDic;
    /// <summary>
    /// 音频枚举与路径对应字典，键是音频枚举，值是此枚举对应的路径
    /// </summary>
    public static Dictionary<EnumAudioClip, string> AudioClipPathDic
    {
        get
        {
            if (m_AudioClipPathDic == null || m_AudioClipPathDic.Count == 0)
            {
                m_AudioClipPathDic = new Dictionary<EnumAudioClip, string>();
                ConfigManager.Instance.cfgAudioClip.AllConfigs.ForEach(a =>
                {
                    m_AudioClipPathDic[(EnumAudioClip)Enum.Parse(typeof(EnumAudioClip), a.enumName)] = AssetPathDic[a.annotate];
                });
            }
            return m_AudioClipPathDic;
        }
    }

    private static Dictionary<EnumAudioClip, RowCfgAudioClip> m_AudioClipPathDic1;
    /// <summary>
    /// 音频枚举与音频配置数据字典，键是音频枚举，值是此枚举对应配置数据
    /// </summary>
    public static Dictionary<EnumAudioClip, RowCfgAudioClip> AudioClipPathDic1
    {
        get
        {
            if (m_AudioClipPathDic1 == null || m_AudioClipPathDic1.Count == 0)
            {
                m_AudioClipPathDic1 = new Dictionary<EnumAudioClip, RowCfgAudioClip>();
                ConfigManager.Instance.cfgAudioClip.AllConfigs.ForEach(a =>
                {
                    m_AudioClipPathDic1[(EnumAudioClip)Enum.Parse(typeof(EnumAudioClip), a.enumName)] = a;
                });
            }
            return m_AudioClipPathDic1;
        }
    }

    private static Dictionary<int, RowCfgItem> m_ItemDic;
    /// <summary>
    /// 道具数据字典，键是道具ID，值是道具的配置数据
    /// </summary>
    public static Dictionary<int, RowCfgItem> ItemDic
    {
        get
        {
            if (m_ItemDic == null || m_ItemDic.Count == 0)
            {
                m_ItemDic = new Dictionary<int, RowCfgItem>();

                ConfigManager.Instance.cfgItem.AllConfigs.ForEach(a =>
                {
                    if (a.id != 0)
                    {
                        m_ItemDic[a.id] = a;
                    }
                });

            }
            return m_ItemDic;
        }
    }

    private static Dictionary<int, RowCfgWeapon> m_WeaponInfoDataDic;
    /// <summary>
    /// 武器数据字典，键是武器ID，值是这个武器的配置数据
    /// </summary>
    public static Dictionary<int, RowCfgWeapon> WeaponInfoDataDic
    {
        get
        {
            if (m_WeaponInfoDataDic == null || m_WeaponInfoDataDic.Count == 0)
            {
                m_WeaponInfoDataDic = new Dictionary<int, RowCfgWeapon>();
                ConfigManager.Instance.cfgWeapon.AllConfigs.ForEach(a =>
                {
                    m_WeaponInfoDataDic[a.id] = a;
                });
            }
            return m_WeaponInfoDataDic;
        }
    }

    /// <summary>
    /// 如果资源路径字典里不包含此键，就加入字典，如果包含就输出日志提示一下
    /// </summary>
    /// <param name="annotate"></param>
    /// <param name="path"></param>
    public static void Add(string annotate, string path)
    {
        //如果字典里不包含这个键就加入字典 
        if (!m_AssetPathDic.ContainsKey(annotate))
        {
            m_AssetPathDic.Add(annotate, path);
        }
        else
        {
            Debug.LogWarning("存在同名资源:" + annotate);
        }
    }

    /// <summary>
    /// 获取当前重新随机消耗，Buff选择界面用(临时用公式)
    /// </summary>
    /// <returns></returns>
    public static int GetRerollCost()
    {
        //每次随机 增加已重随次数*(波数/2)
        return Player.Instance.CurWave + Player.Instance.CurRerollTimes * ((Player.Instance.CurWave / 2) + 1);
    }

    /// <summary>
    /// 获取当前重新随机消耗，商店界面用(临时用公式)
    /// </summary>
    /// <returns></returns>
    public static int GetRerollCost1()
    {
        //如果有重随次数存在消耗就为0
        if (Player.Instance.CurRerollTimes1 < 0) return 0;
        //每次随机 增加已重随次数*((波数/2)+1)
        return Player.Instance.CurWave + Player.Instance.CurRerollTimes1 * ((Player.Instance.CurWave / 2) + 1);
    }

    /// <summary>
    /// 随一个道具
    /// </summary>
    /// <param name="withWeapon">包含武器</param>
    /// <returns></returns>
    public static int RandomGetItemID(bool withWeapon = true)
    {
        int id = -1;
        //从池子里 随机获取一个道具id
        float totalWeight = 0;
        List<int> pool = withWeapon ? ItemPool.Instance.ItemAndWeaponPool : ItemPool.Instance.ItemsPool;
        pool.ForEach(a => totalWeight += ConfigManager.Instance.cfgItem[a].weight);
        float temp = 0;
        float random = totalWeight.GetRandom();
        foreach (var item in pool)
        {
            temp += ConfigManager.Instance.cfgItem[item].weight;
            if (temp > random)
            {
                id = item;
                break;
            }
        }

        return id;
    }

    /// <summary>
    /// 随一个Buff
    /// </summary>
    /// <returns></returns>
    public static int RandomGetBuffID()
    {
        int id = -1;
        //从池子里 随机获取一个道具id
        float totalWeight = 0;
        ItemPool.Instance.BuffsPool.ForEach(a => totalWeight += ConfigManager.Instance.cfgItem[a].weight);
        float temp = 0;
        float random = totalWeight.GetRandom();
        foreach (var item in ItemPool.Instance.BuffsPool)
        {
            temp += ConfigManager.Instance.cfgItem[item].weight;
            if (temp > random)
            {
                id = item;
                break;
            }
        }
        return id;
    }


}
