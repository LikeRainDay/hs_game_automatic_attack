using System;
using System.Collections.Generic;
/// <summary>
/// 有限状态机接口
/// </summary>
public interface IFsm
{
    /// <summary>
    /// 上一个状态
    /// </summary>
    IFsmState PreviousState { get; }

    /// <summary>
    /// 当前状态
    /// </summary>
    IFsmState CurrentState { get; }

    /// <summary>
    /// 状态机处于哪个运行时状态
    /// </summary>
    FsmStatus Status { get; }

    /// <summary>
    /// 此状态在Update里要执行的操作
    /// </summary>
    void OnUpdate();

    /// <summary>
    /// 此状态在LateUpdate里要执行的操作
    /// </summary>
    void OnLateUpdate();

    /// <summary>
    /// 此状态在FixedUpdate里要执行的操作
    /// </summary>
    void OnFixedUpdate();

    /// <summary>
    /// 暂停状态帧
    /// </summary>
    void Pause();

    /// <summary>
    /// 恢复状态机
    /// </summary>
    void Resume();

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">进入状态时所需的参数</param>
    void ChangeState<T>(bool canRepeatEntry = false, params object[] args) where T : IFsmState;

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="fsmType">要进入的状态</param>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">进入状态时可能用到的参数</param>
    void ChangeState(Type fsmType, bool canRepeatEntry = false, params object[] args);

    /// <summary>
    /// 销毁状态机
    /// </summary>
    void Destroy();

}