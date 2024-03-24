using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
/// <summary>
/// 控制主角移动的事件，可以设置起点终点，或者只设置终点，按照给定速度或时长位移到终点，最后可以填一个曲线参数。eg.【起点X，起点Y，2秒，Linear】 【起点X，起点Y，终点X，终点Y，2秒，Linear】
/// </summary>
public class PlayerMoveAction : BaseAction
{
    public PlayerMoveAction(RowCfgPerformances rowCfgPerformances) : base(rowCfgPerformances) { }

    protected override void OnStart()
    {
        base.OnStart();
        //Todo
    }

    protected override void OnFinish()
    {
        base.OnFinish();
        //Todo
    }
}
