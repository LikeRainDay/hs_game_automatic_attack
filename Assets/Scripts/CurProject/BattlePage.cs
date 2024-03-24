using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战斗界面
/// </summary>
public class BattlePage : PanelBase
{
    [FieldName("动画组件")]
    public Animator Anim;
    [FieldName("血条")]
    public Image HpImg;
    [FieldName("经验条")]
    public Image ExpImg;
    [FieldName("血量文本")]
    public TextMeshProUGUI HpText;
    [FieldName("等级文本")]
    public TextMeshProUGUI LevelText;
    [FieldName("材料文本")]
    public TextMeshProUGUI MaterialText;
    [FieldName("种子文本")]
    public TextMeshProUGUI SeedText;
    [FieldName("波次文本")]
    public TransMeshPro WaveText;

    [FieldName("倒计时文本")]
    public TextMeshProUGUI CountDownText;
    [EnumName("倒计时音效")]
    public EnumAudioClip CountDownClip;
    [EnumName("倒计时音效(预警期间)")]
    public EnumAudioClip CountDownClip1;
    [FieldName("倒计时预警线")]
    public int WarningTimeLine;
    [FieldName("倒计时预警颜色")]
    public Color WarningColor;

    [FieldName("升级提示图标父物体")]
    public Transform UpgradeTipParent;
    [FieldName("宝箱提示图标父物体")]
    public Transform RewardTipParent;//todo:扩一下不同稀有度的宝箱用不同的图标

    private bool m_StartCountDown;//开始倒计时
    private float m_TimeRemain;//剩余倒计时
    private float m_Time;//当前累计时长 累计满一秒就会倒计时-1
    public static BattlePage Instance;
    public override void Init(params object[] objs)
    {
        base.Init(objs);
        Instance = this;
    }

    protected override void DelayShow(Action hideEvent = null)
    {
        base.DelayShow(hideEvent);
        FadeIn();
    }

    protected override void DelayHide()
    {
        base.DelayHide();
        FadeOut();
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void Renew()
    {
        //刷新经验 血量 材料数量
        Player.Instance.ExpChange(0);
        Player.Instance.HpChange(0);
        Player.Instance.MaterialChange(0);
        //刷新波次和种子数量
        Player.Instance.CurWave = Player.Instance.CurWave;
        Player.Instance.CurSeed = Player.Instance.CurSeed;
        //隐藏右上角升级图标和宝箱图标
        UpdateTip();
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    /// <param name="time">倒计时时长</param>
    public void StartCountDown(float time)
    {
        m_StartCountDown = true;
        m_Time = 0;
        m_TimeRemain = time;
        CountDownText.color = Color.white;
        CountDownText.SetText(Mathf.RoundToInt(m_TimeRemain).ToString());
    }

    public override void Update()
    {
        base.Update();

        //倒计时相关逻辑
        if (m_StartCountDown)
        {
            m_Time += Time.deltaTime;//累计时长 时长累计满1秒刷新倒计时
            if (m_Time >= 1)
            {
                m_Time = 0;
                m_TimeRemain--;
                CountDownText.SetText(Mathf.RoundToInt(m_TimeRemain).ToString());
                //如果剩余时长比预警线低了，就修改倒计时颜色
                CountDownText.color = m_TimeRemain >= WarningTimeLine ? Color.white : WarningColor;
                AudioMgr.PlaySound(m_TimeRemain >= WarningTimeLine ? CountDownClip : CountDownClip1);
                //倒计时结束停止倒计时 且发送波次结束事件
                if (CountDownText.text == "0")
                {
                    m_StartCountDown = false;
                    print("波次结束");
                    EventMgr.ExecuteEvent(EventName.WaveCompleted, new object[] { OwnerType.主角 });
                }
            }
        }
    }

    /// <summary>
    /// 更新右上角提示图标
    /// </summary>
    public void UpdateTip()
    {
        //根据当前波次升级次数来设置图标数量
        for (int i = 0; i < UpgradeTipParent.childCount; i++)
        {
            UpgradeTipParent.GetChild(i).gameObject.SetActive(i < Player.Instance.LevelUpTimes);
        }
        for (int i = 0; i < RewardTipParent.childCount; i++)
        {
            if (i < Player.Instance.RewardIds.Count)
            {
                RewardTipParent.GetChild(i).gameObject.SetActive(true);
                //todo:后续需要根据奖励的品质来设置不同的图标
            }
            else
            {
                RewardTipParent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
