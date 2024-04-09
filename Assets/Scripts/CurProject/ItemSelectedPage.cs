using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 箱子道具选择界面
/// </summary>
public class ItemSelectedPage : PanelBase
{
    [FieldName("遮盖场景的遮罩")]
    public GameObject Mask;
    [FieldName("元素")]
    public Item4 Item;
    public static ItemSelectedPage Instance;
    public override void Init(params object[] objs)
    {
        Instance = this;
        base.Init(objs);
    }

    protected override void DelayShow(Action hideEvent = null)
    {
        base.DelayShow(hideEvent);
        FadeIn();
        //显示一下遮盖场景的遮罩
        Mask.SetActive(true);
        //刷新一下元素数据
        Item.ID = Player.Instance.RewardIds[0];
        //小动画
        Item.transform.DOScaleX(0, 0.12f).onComplete += () =>
        {
            Item.transform.DOScaleX(1, 0.08f);
        };
    }

    protected override void DelayHide()
    {
        base.DelayHide();
        FadeOut();
    }
}
