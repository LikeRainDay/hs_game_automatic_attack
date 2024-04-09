using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 战场管理器
/// </summary>
public class BattleManager : MonoSingleton<BattleManager>
{
    /// <summary>
    /// 战斗中
    /// </summary>
    [HideInInspector] public bool Battle;
    /// <summary>
    /// 当前战场内，全部敌人的数组
    /// </summary>
    public List<Enemy> Enemys = new List<Enemy>();

    private void Start()
    {
        //注册波次结束事件和下一波事件
        EventMgr.RegisterEvent(EventName.WaveCompleted, WaveCompleted);
        EventMgr.RegisterEvent(EventName.NextWave, NextWave);
    }

    private void Update()
    {
        //如果战斗结束了，就隐藏一下可能存在的血量预警特效
        if (GameManager.Instance.Warning.activeInHierarchy && !Battle)
        {
            GameManager.Instance.Warning.Hide();
        }
    }

    private object WaveCompleted(object[] arg)
    {
        if (!Battle) return null;
        Battle = false;
        EventMgr.ExecuteEvent(EventName.BattleFinish, new object[] { OwnerType.主角 });

        //销毁敌人
        ClearAllEnemys();
        //增加收获属性给的材料
        Player.Instance.MaterialChange(Player.Instance.Harvesting);
        //稍微等待一会儿 显示战后的道具拾取 或 buff选择界面
        TimeMgr.Timer.AddTimeTask(a =>
        {
            //如果拾取的道具数量大于0，就显示道具选择界面
            if (Player.Instance.RewardIds.Count > 0)
            {
                ItemSelectedPage.Instance.Show();
            }
            //如果升级了，就显示Buff选择界面，如果没有就显示商店界面
            else if (Player.Instance.LevelUpTimes >= 0)
            {
                BuffSelectedPage.Instance.Show();
            }
            else
            {
                //隐藏战斗界面，显示商店界面
                UIMgr.Instance.ShowPage(new List<GameObject>() { BattlePage.Instance.gameObject }, ShopPage.Instance.gameObject, null, 0.5f, 0.25f, 0.5f);
            }
        }, 2000);

        return null;
    }

    private object NextWave(object[] arg)
    {
        //刷新血量
        Player.Instance.CurHP_Base = Player.Instance.MaxHP;
        //调用敌袭开始事件
        EventMgr.ExecuteEvent(EventName.BattleStart, new object[] { OwnerType.主角 });
        //波次++ 重随次数刷新
        Player.Instance.CurWave++;
        Player.Instance.CurInjuryAvoidanceTimes = Player.Instance.InjuryAvoidanceTimes;
        //重置数据
        Player.Instance.LevelUpTimes = 0;
        Player.Instance.RewardIds.Clear();
        Player.Instance.transform.position = Vector3.zero;
        BattlePage.Instance.UpdateTip();
        //销毁之前的场景
        if (SceneChangeTool.Instance != null)
        {
            Destroy(SceneChangeTool.Instance.gameObject);
        }
        //隐藏商店界面，显示战斗界面      
        UIMgr.Instance.ShowPage(new List<GameObject>() { ShopPage.Instance.gameObject }, BattlePage.Instance.gameObject, () =>
        {
            //修改标识 创建新的场景
            Battle = true;
            Instantiate(GameManager.Instance.Scene);
        }, 0.5f, 0.25f, 0.5f);
        return null;
    }

    /// <summary>
    /// 添加敌人
    /// </summary>
    public void AddEnemy(Enemy enemy)
    {
        Enemys.AddWithoutRepetition(enemy);
    }

    /// <summary>
    /// 移除敌人
    /// </summary>
    public void RemoveEnemy(Enemy enemy)
    {
        Enemys.RemoveSafe(enemy);
    }

    /// <summary>
    /// 清除战场里的全部敌人
    /// </summary>
    public void ClearAllEnemys()
    {
        Enemys.ForEach(a =>
        {
            if (a != null)
            {
                a.Die();
            }
        });
        //清空数组 
        Enemys.Clear();
    }

}
