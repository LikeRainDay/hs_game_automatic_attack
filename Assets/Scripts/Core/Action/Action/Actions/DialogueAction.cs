using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 打开对话的事件 【打开对话，气泡，10101】 【打开对话，普通(立绘对话)，10101】
/// </summary>
public class DialogueAction : BaseAction
{
    public DialogueAction(RowCfgPerformances rowCfgPerformances) : base(rowCfgPerformances) { }

    protected override void OnStart()
    {
        base.OnStart();
        //todo
        //DialogueMgr.Instance.InitDialogue(int.Parse(customParams[1]), customParams[0] == "普通" ? false : true);
    }

    protected override void OnFinish()
    {
        base.OnFinish();
        //todo
        //隐藏对话界面
        //DialoguePanel.Instance.Hide();
    }

}
