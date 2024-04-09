using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机的状态，运行中，暂停中
/// </summary>
public enum FsmStatus
{
    /// <summary>
    /// 运行中
    /// </summary>
    Running,
    /// <summary>
    /// 暂停中
    /// </summary>
    Pause,
}

/// <summary>
/// 有限状态机
/// </summary>
public class Fsm : IFsm
{
    /// <summary>
    /// 状态机名称，有些环节可能需要用到，这里记录一下
    /// </summary>
    public string FsmName;

    /// <summary>
    /// 状态字典，键是状态类型，值是状态
    /// </summary>
    public readonly Dictionary<Type, IFsmState> StateDic = new Dictionary<Type, IFsmState>();

    /// <summary>
    /// 上一个状态
    /// </summary>
    public IFsmState PreviousState { get; private set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public IFsmState CurrentState { get; private set; }

    /// <summary>
    /// 状态机处于哪个运行时状态
    /// </summary>
    public FsmStatus Status { get; set; }

    /// <summary>
    /// 有参构造，传递一组想要的状态创建一个状态机
    /// </summary>
    /// <param name="name">状态机名称</param>
    /// <param name="fsmStateTypeList">状态列表</param>
    /// <param name="userData">这个状态机包含的状态</param>
    protected Fsm(string fsmName, IEnumerable<Type> fsmStateTypeList, params object[] userData)
    {
        FsmName = fsmName;
        foreach (var type in fsmStateTypeList)
        {
            var fsmState = Activator.CreateInstance(type) as IFsmState;
            fsmState.OnInit(this, userData);
            StateDic.Add(type, fsmState);
        }
        Status = FsmStatus.Running;
    }

    /// <summary>
    /// 此状态在Update里要执行的操作
    /// </summary>
    public void OnUpdate()
    {
        if (Status == FsmStatus.Running)
        {
            CurrentState?.OnUpdate();
        }
    }

    /// <summary>
    /// 此状态在LateUpdate里要执行的操作
    /// </summary>
    public void OnLateUpdate()
    {
        if (Status == FsmStatus.Running)
        {
            CurrentState?.OnLateUpdate();
        }
    }

    /// <summary>
    /// 此状态在FixedUpdate里要执行的操作
    /// </summary>
    public void OnFixedUpdate()
    {
        if (Status == FsmStatus.Running)
        {
            CurrentState?.OnFixedUpdate();
        }
    }

    /// <summary>
    /// 暂停状态机更新
    /// </summary>
    public void Pause()
    {
        Status = FsmStatus.Pause;
        CurrentState?.OnPause();
    }

    /// <summary>
    /// 恢复状态机更新
    /// </summary>
    public void Resume()
    {
        Status = FsmStatus.Running;
        CurrentState?.OnResume();
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">进入状态时所需的参数</param>
    public void ChangeState<T>(bool canRepeatEntry = false, params object[] args) where T : IFsmState
    {
        if (Status == FsmStatus.Pause) return;

        if (!StateDic.ContainsKey(typeof(T)))
        {
            LogUtil.Error($"状态机{FsmName}不存在状态:{typeof(T)}");
            return;
        }

        //如果是不允许重复进入的状态 且当前状态和目标状态一致 则无视
        if (!canRepeatEntry && CurrentState == StateDic[typeof(T)]) return;

        CurrentState?.OnExit();
        PreviousState = CurrentState;
        CurrentState = StateDic[typeof(T)];
        CurrentState.OnEnter(args);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="fsmType">要进入的状态</param>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">进入状态时可能用到的参数</param>
    public void ChangeState(Type fsmType, bool canRepeatEntry = false, params object[] args)
    {
        if (Status == FsmStatus.Pause) return;

        if (!StateDic.ContainsKey(fsmType))
        {
            LogUtil.Error($"状态机{FsmName}不存在状态:{fsmType}");
            return;
        }

        //如果是不允许重复进入的状态 且当前状态和目标状态一致 则无视
        if (!canRepeatEntry && CurrentState == StateDic[fsmType]) return;

        CurrentState?.OnExit();
        PreviousState = CurrentState;
        CurrentState = StateDic[fsmType];
        CurrentState.OnEnter(args);
    }

    /// <summary>
    /// 销毁状态机
    /// </summary>
    public void Destroy()
    {
        CurrentState?.OnExit();
        PreviousState = null;
        CurrentState = null;
        StateDic.Clear();
    }

}