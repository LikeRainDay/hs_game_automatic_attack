using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using Steamworks;
using TMPro;

/// <summary>
/// UI管理器
/// </summary>
public class UIMgr : MonoBehaviour
{
    [Header("界面")]
    public List<PanelBase> Panels;
    [Tooltip("淡入淡出画面用的遮罩，层级应该最高，可以盖住全部界面")]
    public Image FadeMask;
    [FieldName("瓢字提示预制体")]
    public GameObject TipObject;
    [FieldName("属性提示预制体")]
    public GameObject TipObject1;
    [FieldName("提示UI的父物体")]
    public RectTransform TipParent;
    public TextMeshProUGUI Debug1;
    public TextMeshProUGUI Debug2;
    public TextMeshProUGUI Debug3;
    private Tween m_FadeMaskTween;//淡入淡出的tween动画，每次淡入淡出之前都需要先关闭一下之前可能存在的tween动画，防止新旧动画叠加
    //简单单例模式
    public static UIMgr Instance;
    private void Awake()
    {
        Instance = this;
        InputMgr.Instance.Raycaster = GetComponent<GraphicRaycaster>();
        //页面初始化 一般是注册一下这个页面的一些输入监听事件
        Panels.ForEach(a => a.Init());
    }
    private void Update()
    {
#if UNITY_EDITOR
        //编辑器下测试用代码
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug1.text =
                "材料:" + Player.Instance.Material + "\n" +
                "当前等级:" + Player.Instance.CurLevel + "\n" +
                "经验:" + Player.Instance.CurExp + "\n" +
                "当前生命值:" + Player.Instance.CurHP + "\n" +
                "最大生命值:" + Player.Instance.MaxHP + "\n" +
                "最大生命值上限:" + Player.Instance.MaxHPUL + "\n" +
                "生命再生:" + Player.Instance.HPRegeneration + "\n" +
                "生命窃取:" + Player.Instance.LifeSteal + "\n" +
                "伤害:" + Player.Instance.Damage + "\n" +
                "近战伤害:" + Player.Instance.MeleeDamage + "\n" +
                "远程伤害:" + Player.Instance.RangedDamage + "\n" +
                "元素伤害:" + Player.Instance.ElementalDamage + "\n" +
                "攻击速度:" + Player.Instance.AttackSpeed + "\n" +
                "暴击率:" + Player.Instance.CritChance + "\n" +
                "工程学:" + Player.Instance.Engineering + "\n";
            Debug2.text =
                "范围:" + Player.Instance.Range + "\n" +
                "护甲:" + Player.Instance.Armor + "\n" +
                "闪避率:" + Player.Instance.DodgeRate + "\n" +
                "速度:" + Player.Instance.Speed + "\n" +
                "幸运:" + Player.Instance.Luck + "\n" +
                "收获:" + Player.Instance.Harvesting + "\n" +
                "使用消耗品恢复:" + Player.Instance.ConsumableHeal + "\n" +
                "获得经验:" + Player.Instance.XPGain + "\n" +
                "拾取范围:" + Player.Instance.PickupRange + "\n" +
                "道具价格:" + Player.Instance.ItemsPrice + "\n" +
                "爆炸伤害:" + Player.Instance.ExplosionDamage + "\n" +
                "爆炸范围:" + Player.Instance.ExplosionSize + "\n" +
                "投射物反弹次数:" + Player.Instance.Bounces + "\n" +
                "投射物贯通个数:" + Player.Instance.Piercing + "\n" +
                "贯通伤害:" + Player.Instance.PiercingDamage + "\n";
            Debug3.text =
                "对头目和精英怪的伤害系数:" + Player.Instance.DamageAgainstBosses + "\n" +
                "燃烧速度:" + Player.Instance.BurningSpeed + "\n" +
                "燃烧速度比率:" + Player.Instance.BurningSpeedRate + "\n" +
                "燃烧蔓延至附近的一名敌人:" + Player.Instance.BurnOther + "\n" +
                "击退:" + Player.Instance.Knockback + "\n" +
                "材料翻倍概率:" + Player.Instance.DoubleMaterialChance + "\n" +
                "商店免费刷新次数:" + Player.Instance.FreeRerolls + "\n" +
                "树木:" + Player.Instance.Trees + "\n" +
                "树木能被一击毙命:" + Player.Instance.TreesKill + "\n" +
                "敌人:" + Player.Instance.Enemies + "\n" +
                "敌人速度:" + Player.Instance.EnemySpeed + "\n" +
                "敌人伤害:" + Player.Instance.EnemyDamageRate + "\n" +
                "特殊敌人:" + Player.Instance.SpecialEnemy + "\n" +
                "无法通过其他途径恢复生命值:" + Player.Instance.HealNegation + "\n" +
                "仇恨值:" + Player.Instance.Hatred + "\n" +
                "击中敌人时能使其降低10速度最高30:" + Player.Instance.SpeedCutMax30 + "\n" +
                "立即吸收掉落的材料:" + Player.Instance.ImmediatelyEatMaterialChance + "\n" +
                "回收道具价格系数:" + Player.Instance.ItemsRecyclePrice + "\n" +
                "免疫伤害次数:" + Player.Instance.InjuryAvoidanceTimes + "\n" +
                "随机数:" + Player.Instance.RandomNumber + "\n";
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug1.text = "";
            Debug2.text = "";
            Debug3.text = "";
        }
#endif

    }

    /// <summary>
    /// 用淡入淡出遮罩的方式，隐藏旧界面显示新界面
    /// </summary>
    /// <param name="hidePage">需要隐藏的界面，会在纯黑时隐藏</param>
    /// <param name="showPage">需要显示的界面，会在纯黑时显示</param>
    /// <param name="action">回调函数，会在纯黑时执行的回调</param>
    /// <param name="fadeInTime">淡入用时</param>
    /// <param name="stayTime">停留用时</param>
    /// <param name="fadeOutTime">淡出用时</param>
    /// <param name="maskColor">遮罩颜色，默认为纯黑</param>
    /// <param name="unscaled">不受时间缩放影响</param>
    public void ShowPage(List<GameObject> hidePage, GameObject showPage, Action action = null, float fadeInTime = 1, float stayTime = 0, float fadeOutTime = 1, Color maskColor = default, bool unscaled = false)
    {
        //关闭之前可能存在的tween动画 防止新旧动画叠加错乱
        m_FadeMaskTween.Kill();
        //规整颜色 遮罩淡入
        FadeMask.color = maskColor == default ? new Color(0, 0, 0, 0) : maskColor;
        FadeMask.gameObject.SetActive(true);
        m_FadeMaskTween = FadeMask.DOFade(1, fadeInTime).SetUpdate(unscaled);
        m_FadeMaskTween.onComplete += () =>
        {
            //遮罩淡入完毕后，隐藏需要隐藏的界面，显示需要显示的界面
            hidePage.ForEach(a => a.Hide());
            showPage?.SetActive(true);
            //执行回调函数
            action?.Invoke();
            //等待一个时长后淡出遮罩
            if (unscaled)
            {
                TimeMgr.Timer.AddUnscaledTimeTask(a =>
                {
                    m_FadeMaskTween = FadeMask.DOFade(0, fadeOutTime).SetUpdate(true);
                }, stayTime * 1000);
            }
            else
            {
                TimeMgr.Timer.AddTimeTask(a =>
                {
                    m_FadeMaskTween = FadeMask.DOFade(0, fadeOutTime).SetUpdate(false);
                }, stayTime * 1000);
            }
        };

    }

    /// <summary>
    /// 在游戏里打开Steam商店页面
    /// </summary>
    /// <param name="id">AppID</param>
    public void OpenSteamPage(int id)
    {
        if (SteamMgr.Initialized)
        {
            SteamFriends.ActivateGameOverlayToStore(new AppId_t((uint)id), EOverlayToStoreFlag.k_EOverlayToStoreFlag_AddToCart);
        }
    }

    /// <summary>
    /// 打开网页
    /// </summary>
    /// <param name="url"></param>
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="path">文件夹路径</param>
    public static void OpenDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        path = path.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// 显示瓢字
    /// </summary>
    /// <param name="pos">生成位置</param>
    /// <param name="content">内容</param>
    /// <param name="color">颜色</param>
    public void ShowNumbTip(Vector3 pos, string content, Color color)
    {
        GameObject tip = Instantiate(TipObject, TipParent);
        tip.GetComponent<Tip>().Init(pos, content, color);
    }

    /// <summary>
    /// 显示瓢字(属性增加)
    /// </summary>
    /// <param name="pos">生成位置</param>
    /// <param name="icons">需要显示的图标</param>
    public async void ShowNumbTip1(Vector3 pos, List<string> icons)
    {
        foreach (var item in icons)
        {
            var sp = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[item]);
            GameObject tip = Instantiate(TipObject1, TipParent);
            //波动一下位置以及每个之间等待一下 避免重叠
            tip.GetComponent<Tip1>().Init(pos + (Vector3)new Vector4(1, -1, -1, 1).GetRandomV2FromV4(), sp);
            await Task.Delay(500);
        }
    }

}
