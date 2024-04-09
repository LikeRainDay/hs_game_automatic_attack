using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// 演出事件基类
/// </summary>
public abstract class BaseAction
{
    private int m_StepIndex;//用于判断当前事件是否需要继续进行，  场景切换等逻辑之后会++，让之前还未执行完的事件中断掉
    private string m_ConditionList;//条件列表
    private readonly float m_StartDelay;//启动延时
    private readonly float m_Duration;//持续时长
    protected List<string> m_CustomParams;//自定义参数
    public RowCfgPerformances RowCfgPerformances;//此Action的配置数据

    public BaseAction(RowCfgPerformances rowCfgPerformances)
    {
        //打印一下事件名 方便了解执行到什么环节了
        LogUtil.Debug(rowCfgPerformances.actionType + ":" + Time.time);
        //记录一下当前事件的数据
        m_ConditionList = rowCfgPerformances.conditionList;
        m_StartDelay = rowCfgPerformances.delay;
        m_Duration = rowCfgPerformances.duration;
        m_CustomParams = rowCfgPerformances.customParams;
        RowCfgPerformances = rowCfgPerformances;
    }

    /// <summary>
    /// 节点执行，外部调用
    /// </summary>
    public void Start()
    {
        //记录标识 事件结束的时候 如果标识不一致就不继续后续事件了  切换场景 切换演出的时候会修改标识
        m_StepIndex = GameManager.Instance.StepIndex;
        //开始本条演出
        GameManager.Instance.StartCoroutine(StartCo());
    }

    /// <summary>
    /// 开始执行事件
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCo()
    {
        //具体事件逻辑
        GameManager.Instance.StartCoroutine(OnTractStart());
        //持续时间为-1的类型需要等待别处来调用事件结束函数 比如对话结束的时候通知管理器进行下一条演出
        if (m_Duration == -1) yield break;
        //等待事件持续时间  执行事件结束函数
        yield return new WaitForSeconds(m_Duration);
        //执行结束事件
        Finish();
    }

    /// <summary>
    /// 轨道启动
    /// </summary>
    private IEnumerator OnTractStart()
    {
        //等待事件开启延时，真正执行事件
        yield return new WaitForSeconds(m_StartDelay);
        //执行具体的事件逻辑 如果标识没匹配上 就直接返回
        if (GameManager.Instance.StepIndex != m_StepIndex) yield break;
        OnStart();
    }

    /// <summary>
    /// 结束
    /// </summary>
    public void Finish()
    {
        //如果标识没匹配上 就直接返回  说明这段演出是旧的演出了  不需要继续执行了
        if (GameManager.Instance.StepIndex != m_StepIndex) return;

        //执行具体的结束逻辑
        OnFinish();

        //如果存在下一条事件
        if (RowCfgPerformances.nextID != 0)
        {
            //获取下一条演出配置数据
            RowCfgPerformances = ConfigManager.Instance.cfgPerformances[RowCfgPerformances.nextID];
            //如果下一条事件有条件 并且条件没有满足  就选择下一条配置
            while (!string.IsNullOrEmpty(RowCfgPerformances.conditionList) && !ToolFun.ExecuteJudgeEvents(RowCfgPerformances.conditionList))
            {
                RowCfgPerformances = ConfigManager.Instance.cfgPerformances[RowCfgPerformances.id + 1];
            }
            //创建演出action 并且执行
            ActionMgr.CurBaseAction = ActionFactory.Create(RowCfgPerformances);
            ActionMgr.CurBaseAction.Start();
            return;
        }
        //如果没有下一条事件了 就执行结束函数
        ActionMgr.ActionQueueExecuteFinish();

    }

    /// <summary>
    /// 供子类重写的事件开始函数
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// 供子类重写的事件结束函数
    /// </summary>
    protected virtual void OnFinish() { }

}