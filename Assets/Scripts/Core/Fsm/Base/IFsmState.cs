/// <summary>
/// 有限状态机状态接口
/// </summary>
public interface IFsmState
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="fsm"> 状态机控制类 </param>
    /// <param name="userData"> 用户数据 </param>
    void OnInit(IFsm fsm, params object[] userData);

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="args">切换状态时传入的参数</param>
    void OnEnter(params object[] args);

    /// <summary>
    /// 退出状态
    /// </summary>
    void OnExit();

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
    /// 暂停状态机更新
    /// </summary>
    void OnPause();

    /// <summary>
    /// 恢复状态机更新
    /// </summary>
    void OnResume();

}