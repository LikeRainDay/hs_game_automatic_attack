using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主界面
/// </summary>
public class MainMenuPage : PanelBase
{
    public static MainMenuPage Instance;
    public override void Init(params object[] objs)
    {
        base.Init(objs);
        Instance = this;
    }

    protected override void DelayShow(Action hideEvent = null)
    {
        base.DelayShow(hideEvent);
        //淡入界面 以及播放主界面音乐
        FadeIn();
        AudioMgr.PlayMusic(EnumAudioClip.A12);
    }

    protected override void DelayHide()
    {
        base.DelayHide();
        FadeOut();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        //关闭首页 显示战斗界面
        UIMgr.Instance.ShowPage(new List<GameObject>() { gameObject }, BattlePage.Instance.gameObject, () =>
        {
            //播放战斗音乐 创建场景
            AudioMgr.PlayMusic(EnumAudioClip.A10);
            Instantiate(GameManager.Instance.Player);
            TimeMgr.Timer.AddTimeTask(a => Instantiate(GameManager.Instance.Scene), 50);
            BattleManager.Instance.Battle = true;
        }, 0.75f, 0.25f, 0.5f);
    }

    /// <summary>
    /// 关闭游戏
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
