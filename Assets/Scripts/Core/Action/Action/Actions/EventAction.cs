using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 演出里调用通用事件 
/// </summary>
public class EventAction : BaseAction
{
    public string Type;//事件名
    public EventAction(RowCfgPerformances rowCfgPerformances, string type) : base(rowCfgPerformances)
    {
        Type = type; //记录一下事件名，拼接事件字符串的时候要用
    }

    protected override void OnStart()
    {
        base.OnStart();
        //把字符串还原回去  事件名 + 其余参数
        string str = Type + "，";
        for (int i = 0; i < m_CustomParams.Count; i++)
        {
            //如果当前元素不是最后一个元素 就加一个逗号
            str += i != m_CustomParams.Count - 1 ? m_CustomParams[i] + "，" : m_CustomParams[i];
        }
        //执行事件                       如果没有参数 那么直接用事件名即可
        ToolFun.ExecuteEvents(m_CustomParams.Count == 0 ? Type : str);
    }

}
