using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 状态机管理器
/// </summary>
public class FsmMgr : Singleton<FsmMgr>
{
    /// <summary>
    /// 状态机字典  key：某个状态机的持有类 value：某个状态机
    /// </summary>
    private static readonly Dictionary<object, IFsm> m_FsmDict = new Dictionary<object, IFsm>();

    /// <summary>
    /// 获取状态机，具体游戏对象的状态机可以直接从游戏对象上获取，这里一般是获取流程相关的 系统相关的状态机 
    /// </summary>
    /// <param name="owner">状态机持有类</param>
    /// <typeparam name="T">状态机</typeparam>
    /// <returns></returns>
    public T GetFsm<T>(object owner) where T : Fsm, new()
    {
        if (m_FsmDict.ContainsKey(owner))
        {
            return m_FsmDict[owner] as T;
        }
        return null;
    }

    /// <summary>
    /// 添加某个状态机到管理器
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="fsm"></param>
    public static void AddFsm(object owner, IFsm fsm)
    {
        if (!m_FsmDict.ContainsKey(owner))
        {
            m_FsmDict.Add(owner, fsm);
        }
    }

    /// <summary>
    /// 从管理器移除某个状态机
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="fsm"></param>
    public static void RemoveFsm(object owner, IFsm fsm)
    {
        if (m_FsmDict.ContainsKey(owner))
        {
            m_FsmDict.Remove(owner);
        }
    }

    /// <summary>
    /// 切换某个状态机状态
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="fsmType">要进入的状态</param>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">进入状态时所需的参数</param>
    public void ChangeState(object owner, Type fsmType, bool canRepeatEntry = false, params object[] args)
    {
        if (!m_FsmDict.ContainsKey(owner))
        {
            Debug.LogError($"不存在状态机{owner.ToString()}");
            return;
        }
        m_FsmDict[owner].ChangeState(fsmType, canRepeatEntry, args);
    }

    /// <summary>
    ///  切换某个状态机状态
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">进入状态时所需的参数</param>
    public void ChangeState<T>(object owner, bool canRepeatEntry = false, params object[] args) where T : IFsmState
    {
        if (!m_FsmDict.ContainsKey(owner))
        {
            Debug.LogError($"不存在状态机{owner.ToString()}");
            return;
        }
        m_FsmDict[owner].ChangeState<T>(canRepeatEntry, args);
    }

    /// <summary>
    /// 销毁某个有限状态机
    /// </summary>
    /// <param name="owner"> 持有类 </param>
    /// <typeparam name="T"></typeparam>
    public void DestroyFsm<T>(T owner) where T : class
    {
        if (!m_FsmDict.ContainsKey(owner)) return;

        var fsm = m_FsmDict[owner];
        fsm.Destroy();
        m_FsmDict.Remove(owner);
    }

}