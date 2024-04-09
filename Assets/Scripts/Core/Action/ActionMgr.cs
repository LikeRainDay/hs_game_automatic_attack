using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 演出管理器
/// </summary>
public class ActionMgr
{
    public static BaseAction CurBaseAction;//当前正在执行的演出事件    
    private static Action m_OnExecuteAllComplete;//全部事件完成后的回调

    //注册一下进入下一条演出的事件
    static ActionMgr()
    {
        EventMgr.RegisterEvent(EventName.NextAction, (args) =>
        {
            //调用当前事件的退出函数
            if (CurBaseAction != null)
            {
                CurBaseAction?.Finish();
            }
            return null;

        });
    }

    /// <summary>
    /// 尝试执行事件
    /// </summary>
    /// <param name="actionId">入口事件id</param>
    /// <param name="onExecuteAllComplete">所有事件完成的回调</param>
    public static void TryExecuteAction(int actionId, Action onExecuteAllComplete = null)
    {
        //记录演出结束后的回调
        m_OnExecuteAllComplete = onExecuteAllComplete;

        //拿到入口演出事件的数据
        RowCfgPerformances _rowCfgPerformances = ConfigManager.Instance.cfgPerformances[actionId];
        //如果当前这个事件有条件，并且条件没有满足 就执行id+1的那条事件
        while (!string.IsNullOrEmpty(_rowCfgPerformances.conditionList) && !ToolFun.ExecuteJudgeEvents(_rowCfgPerformances.conditionList))
        {
            _rowCfgPerformances = ConfigManager.Instance.cfgPerformances[_rowCfgPerformances.id + 1];
        }

        //创建以及执行目标事件
        CurBaseAction = ActionFactory.Create(_rowCfgPerformances);
        CurBaseAction.Start();
    }

    /// <summary>
    /// 事件队列执行完毕
    /// </summary>
    public static void ActionQueueExecuteFinish()
    {
        //重置数据和状态 以及执行事件队列结束的回调函数
        CurBaseAction = null;
        m_OnExecuteAllComplete?.Invoke();
    }

}