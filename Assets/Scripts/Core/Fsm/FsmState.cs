/// <summary>
/// 状态基类
/// </summary>
public abstract class FsmState : IFsmState
{
    /// <summary>
    /// 持有这个状态的状态机
    /// </summary>
    private IFsm m_FsmController;

    #region 实现接口
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="fsm"> 状态机控制类 </param>
    /// <param name="userData"> 用户数据 </param>
    public virtual void OnInit(IFsm fsm, params object[] userData)
    {
        m_FsmController = fsm;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="args"> 切换状态时传入的参数 </param>
    public virtual void OnEnter(params object[] args) { }

    /// <summary>
    /// 退出状态
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// 此状态在Update里要执行的操作
    /// </summary>
    public void OnUpdate() { }

    /// <summary>
    /// 此状态在LateUpdate里要执行的操作
    /// </summary>
    public void OnLateUpdate() { }

    /// <summary>
    /// 此状态在FixedUpdate里要执行的操作
    /// </summary>
    public void OnFixedUpdate() { }

    /// <summary>
    /// 暂停状态机更新
    /// </summary>
    public virtual void OnPause() { }

    /// <summary>
    /// 恢复状态机更新
    /// </summary>
    public virtual void OnResume() { }
    #endregion

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <typeparam name="T">下一状态</typeparam>
    /// <param name="canRepeatEntry">目标状态能否重复进入，不能重复进入且当前状态与目标状态一致则不会调用目标状态的enter函数</param>
    /// <param name="args">传给下一状态的参数</param>
    public void ChangeState<T>(bool canRepeatEntry = false, params object[] args) where T : IFsmState
    {
        m_FsmController.ChangeState<T>(canRepeatEntry, args);
    }

}