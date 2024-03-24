using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsPage : MonoBehaviour
{
    [FieldName("主要属性")]
    public GameObject Main;
    [FieldName("主要属性左侧文本")]
    public List<TextMeshProUGUI> L1;
    [FieldName("主要属性右侧数值")]
    public List<TextMeshProUGUI> R1;
    [FieldName("次要属性")]
    public GameObject Minor;
    [FieldName("次要属性左侧文本")]
    public List<TextMeshProUGUI> L2;
    [FieldName("次要属性右侧数值")]
    public List<TextMeshProUGUI> R2;
    private bool m_IsMain;
    /// <summary>
    /// 切换主次要属性
    /// </summary>
    public bool IsMain
    {
        get => m_IsMain;
        set
        {
            m_IsMain = value;
            Main.SetActive(value);
            Minor.SetActive(!value);
        }
    }

    private void Start()
    {
        //每次道具变更之后，都刷新一下属性
        EventMgr.RegisterEvent(EventName.ItemChange, Refresh);
    }

    private void OnEnable()
    {
        //每次显示的时候都默认显示主要属性，隐藏次要属性
        Main.SetActive(true);
        Minor.SetActive(false);
        Refresh(null);
    }

    private object Refresh(object[] arg)
    {
        //将界面上的内容 与玩家数据一一对应起来
        #region 主要属性
        R1[0].text = Player.Instance.CurLevel.ToString();//当前等级

        R1[1].text = Player.Instance.MaxHP.ToString();//最大生命值
        L1[1].color = Player.Instance.MaxHP == 15 ? Color.white : Player.Instance.MaxHP > 15 ? Color.green : Color.red;
        R1[1].color = Player.Instance.MaxHP == 15 ? Color.white : Player.Instance.MaxHP > 15 ? Color.green : Color.red;

        R1[2].text = Player.Instance.HPRegeneration.ToString();//生命再生
        L1[2].color = Player.Instance.HPRegeneration == 0 ? Color.white : Player.Instance.HPRegeneration > 0 ? Color.green : Color.red;
        R1[2].color = Player.Instance.HPRegeneration == 0 ? Color.white : Player.Instance.HPRegeneration > 0 ? Color.green : Color.red;

        R1[3].text = (Player.Instance.LifeSteal * 100).ToString("F0");//生命窃取
        L1[3].color = Player.Instance.LifeSteal == 0 ? Color.white : Player.Instance.LifeSteal > 0 ? Color.green : Color.red;
        R1[3].color = Player.Instance.LifeSteal == 0 ? Color.white : Player.Instance.LifeSteal > 0 ? Color.green : Color.red;

        R1[4].text = ((Player.Instance.Damage - 1) * 100).ToString("F0");//伤害
        L1[4].color = Player.Instance.Damage == 1 ? Color.white : Player.Instance.Damage > 0 ? Color.green : Color.red;
        R1[4].color = Player.Instance.Damage == 1 ? Color.white : Player.Instance.Damage > 0 ? Color.green : Color.red;

        R1[5].text = Player.Instance.MeleeDamage.ToString();//近战伤害
        L1[5].color = Player.Instance.MeleeDamage == 0 ? Color.white : Player.Instance.MeleeDamage > 0 ? Color.green : Color.red;
        R1[5].color = Player.Instance.MeleeDamage == 0 ? Color.white : Player.Instance.MeleeDamage > 0 ? Color.green : Color.red;

        R1[6].text = Player.Instance.RangedDamage.ToString();//远程伤害
        L1[6].color = Player.Instance.RangedDamage == 0 ? Color.white : Player.Instance.RangedDamage > 0 ? Color.green : Color.red;
        R1[6].color = Player.Instance.RangedDamage == 0 ? Color.white : Player.Instance.RangedDamage > 0 ? Color.green : Color.red;

        R1[7].text = Player.Instance.ElementalDamage.ToString();//元素伤害
        L1[7].color = Player.Instance.ElementalDamage == 0 ? Color.white : Player.Instance.ElementalDamage > 0 ? Color.green : Color.red;
        R1[7].color = Player.Instance.ElementalDamage == 0 ? Color.white : Player.Instance.ElementalDamage > 0 ? Color.green : Color.red;

        R1[8].text = ((Player.Instance.AttackSpeed - 1) * 100).ToString("F0");//攻击速度
        L1[8].color = Player.Instance.AttackSpeed == 1 ? Color.white : Player.Instance.AttackSpeed > 0 ? Color.green : Color.red;
        R1[8].color = Player.Instance.AttackSpeed == 1 ? Color.white : Player.Instance.AttackSpeed > 0 ? Color.green : Color.red;

        R1[9].text = (Player.Instance.CritChance * 100).ToString("F0");//暴击率
        L1[9].color = Player.Instance.CritChance == 0 ? Color.white : Player.Instance.CritChance > 0 ? Color.green : Color.red;
        R1[9].color = Player.Instance.CritChance == 0 ? Color.white : Player.Instance.CritChance > 0 ? Color.green : Color.red;

        R1[10].text = Player.Instance.Engineering.ToString();//工程学
        L1[10].color = Player.Instance.Engineering == 0 ? Color.white : Player.Instance.Engineering > 0 ? Color.green : Color.red;
        R1[10].color = Player.Instance.Engineering == 0 ? Color.white : Player.Instance.Engineering > 0 ? Color.green : Color.red;

        R1[11].text = Player.Instance.Range.ToString();//范围
        L1[11].color = Player.Instance.Range == 0 ? Color.white : Player.Instance.Range > 0 ? Color.green : Color.red;
        R1[11].color = Player.Instance.Range == 0 ? Color.white : Player.Instance.Range > 0 ? Color.green : Color.red;

        R1[12].text = Player.Instance.Armor.ToString();//护甲
        L1[12].color = Player.Instance.Armor == 0 ? Color.white : Player.Instance.Armor > 0 ? Color.green : Color.red;
        R1[12].color = Player.Instance.Armor == 0 ? Color.white : Player.Instance.Armor > 0 ? Color.green : Color.red;

        R1[13].text = (Player.Instance.DodgeRate * 100).ToString("F0");//闪避率
        L1[13].color = Player.Instance.DodgeRate == 0 ? Color.white : Player.Instance.DodgeRate > 0 ? Color.green : Color.red;
        R1[13].color = Player.Instance.DodgeRate == 0 ? Color.white : Player.Instance.DodgeRate > 0 ? Color.green : Color.red;

        R1[14].text = ((Player.Instance.Speed - 1) * 100).ToString("F0");//移速
        L1[14].color = Player.Instance.Speed == 1 ? Color.white : Player.Instance.Speed > 0 ? Color.green : Color.red;
        R1[14].color = Player.Instance.Speed == 1 ? Color.white : Player.Instance.Speed > 0 ? Color.green : Color.red;

        R1[15].text = Player.Instance.Luck.ToString();//幸运
        L1[15].color = Player.Instance.Luck == 0 ? Color.white : Player.Instance.Luck > 0 ? Color.green : Color.red;
        R1[15].color = Player.Instance.Luck == 0 ? Color.white : Player.Instance.Luck > 0 ? Color.green : Color.red;

        R1[16].text = Player.Instance.Harvesting.ToString();//收获
        L1[16].color = Player.Instance.Harvesting == 0 ? Color.white : Player.Instance.Harvesting > 0 ? Color.green : Color.red;
        R1[16].color = Player.Instance.Harvesting == 0 ? Color.white : Player.Instance.Harvesting > 0 ? Color.green : Color.red;
        #endregion

        #region 次要属性
        R2[0].text = (Player.Instance.ConsumableHeal - 1).ToString();//消耗品恢复
        L2[0].color = Player.Instance.ConsumableHeal == 1 ? Color.white : Player.Instance.ConsumableHeal > 0 ? Color.green : Color.red;
        R2[0].color = Player.Instance.ConsumableHeal == 1 ? Color.white : Player.Instance.ConsumableHeal > 0 ? Color.green : Color.red;

        R2[1].text = "0";//忘了
        L2[1].color = Color.white;
        R2[1].color = Color.white;

        R2[2].text = ((Player.Instance.XPGain - 1) * 100).ToString("F0");//经验获得
        L2[2].color = Player.Instance.XPGain == 1 ? Color.white : Player.Instance.XPGain > 0 ? Color.green : Color.red;
        R2[2].color = Player.Instance.XPGain == 1 ? Color.white : Player.Instance.XPGain > 0 ? Color.green : Color.red;

        R2[3].text = ((Player.Instance.PickupRange - 1) * 100).ToString("F0");//拾取范围
        L2[3].color = Player.Instance.PickupRange == 1 ? Color.white : Player.Instance.PickupRange > 0 ? Color.green : Color.red;
        R2[3].color = Player.Instance.PickupRange == 1 ? Color.white : Player.Instance.PickupRange > 0 ? Color.green : Color.red;

        R2[4].text = ((Player.Instance.ItemsPrice - 1) * 100).ToString("F0");//道具价格系数
        L2[4].color = Player.Instance.ItemsPrice == 1 ? Color.white : Player.Instance.ItemsPrice > 0 ? Color.green : Color.red;
        R2[4].color = Player.Instance.ItemsPrice == 1 ? Color.white : Player.Instance.ItemsPrice > 0 ? Color.green : Color.red;

        R2[5].text = ((Player.Instance.ExplosionDamage - 1) * 100).ToString("F0");//爆炸伤害
        L2[5].color = Player.Instance.ExplosionDamage == 1 ? Color.white : Player.Instance.ExplosionDamage > 0 ? Color.green : Color.red;
        R2[5].color = Player.Instance.ExplosionDamage == 1 ? Color.white : Player.Instance.ExplosionDamage > 0 ? Color.green : Color.red;

        R2[6].text = (Player.Instance.ExplosionSize * 100).ToString("F0");//爆炸范围
        L2[6].color = Player.Instance.ExplosionSize == 0 ? Color.white : Player.Instance.ExplosionSize > 0 ? Color.green : Color.red;
        R2[6].color = Player.Instance.ExplosionSize == 0 ? Color.white : Player.Instance.ExplosionSize > 0 ? Color.green : Color.red;

        R2[7].text = Player.Instance.Bounces.ToString();//反弹
        L2[7].color = Player.Instance.Bounces == 0 ? Color.white : Player.Instance.Bounces > 0 ? Color.green : Color.red;
        R2[7].color = Player.Instance.Bounces == 0 ? Color.white : Player.Instance.Bounces > 0 ? Color.green : Color.red;

        R2[8].text = Player.Instance.Piercing.ToString();//贯穿
        L2[8].color = Player.Instance.Piercing == 0 ? Color.white : Player.Instance.Piercing > 0 ? Color.green : Color.red;
        R2[8].color = Player.Instance.Piercing == 0 ? Color.white : Player.Instance.Piercing > 0 ? Color.green : Color.red;

        R2[9].text = (Player.Instance.PiercingDamage * 100).ToString("F0");//贯穿伤害
        L2[9].color = Player.Instance.PiercingDamage == 0 ? Color.white : Player.Instance.PiercingDamage > 0 ? Color.green : Color.red;
        R2[9].color = Player.Instance.PiercingDamage == 0 ? Color.white : Player.Instance.PiercingDamage > 0 ? Color.green : Color.red;

        R2[10].text = ((Player.Instance.DamageAgainstBosses - 1) * 100).ToString("F0");//对精英的伤害系数
        L2[10].color = Player.Instance.DamageAgainstBosses == 1 ? Color.white : Player.Instance.DamageAgainstBosses > 0 ? Color.green : Color.red;
        R2[10].color = Player.Instance.DamageAgainstBosses == 1 ? Color.white : Player.Instance.DamageAgainstBosses > 0 ? Color.green : Color.red;

        R2[11].text = (Player.Instance.BurningSpeed * 100).ToString("F0");//燃烧速度
        L2[11].color = Player.Instance.BurningSpeed == 0 ? Color.white : Player.Instance.BurningSpeed > 0 ? Color.green : Color.red;
        R2[11].color = Player.Instance.BurningSpeed == 0 ? Color.white : Player.Instance.BurningSpeed > 0 ? Color.green : Color.red;

        R2[12].text = ((Player.Instance.BurningSpeedRate - 1) * 100).ToString("F0");//燃烧速率
        L2[12].color = Player.Instance.BurningSpeedRate == 1 ? Color.white : Player.Instance.BurningSpeedRate > 0 ? Color.green : Color.red;
        R2[12].color = Player.Instance.BurningSpeedRate == 1 ? Color.white : Player.Instance.BurningSpeedRate > 0 ? Color.green : Color.red;

        R2[13].text = Player.Instance.Knockback.ToString();//击退
        L2[13].color = Player.Instance.Knockback == 0 ? Color.white : Player.Instance.Knockback > 0 ? Color.green : Color.red;
        R2[13].color = Player.Instance.Knockback == 0 ? Color.white : Player.Instance.Knockback > 0 ? Color.green : Color.red;

        R2[14].text = (Player.Instance.DoubleMaterialChance * 100).ToString("F0");//双倍材料概率
        L2[14].color = Player.Instance.DoubleMaterialChance == 0 ? Color.white : Player.Instance.DoubleMaterialChance > 0 ? Color.green : Color.red;
        R2[14].color = Player.Instance.DoubleMaterialChance == 0 ? Color.white : Player.Instance.DoubleMaterialChance > 0 ? Color.green : Color.red;

        R2[15].text = "0";//忘了
        L2[15].color = Color.white;
        R2[15].color = Color.white;

        R2[16].text = Player.Instance.FreeRerolls.ToString();//免费重随次数
        L2[16].color = Player.Instance.FreeRerolls == 0 ? Color.white : Player.Instance.FreeRerolls > 0 ? Color.green : Color.red;
        R2[16].color = Player.Instance.FreeRerolls == 0 ? Color.white : Player.Instance.FreeRerolls > 0 ? Color.green : Color.red;

        R2[17].text = Player.Instance.Trees.ToString();//树木
        L2[17].color = Player.Instance.Trees == 0 ? Color.white : Player.Instance.Trees > 0 ? Color.green : Color.red;
        R2[17].color = Player.Instance.Trees == 0 ? Color.white : Player.Instance.Trees > 0 ? Color.green : Color.red;

        R2[18].text = ((Player.Instance.Enemies - 1) * 100).ToString("F0");//敌人数量
        L2[18].color = Player.Instance.Enemies == 1 ? Color.white : Player.Instance.Enemies > 0 ? Color.green : Color.red;
        R2[18].color = Player.Instance.Enemies == 1 ? Color.white : Player.Instance.Enemies > 0 ? Color.green : Color.red;

        R2[19].text = (Player.Instance.EnemySpeed * 100).ToString("F0");//敌人速度
        L2[19].color = Player.Instance.EnemySpeed == 0 ? Color.white : Player.Instance.EnemySpeed > 0 ? Color.green : Color.red;
        R2[19].color = Player.Instance.EnemySpeed == 0 ? Color.white : Player.Instance.EnemySpeed > 0 ? Color.green : Color.red;
        #endregion
        return null;
    }
}
