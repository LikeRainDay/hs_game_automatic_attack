using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
/// <summary>
/// 角色基类，游戏内的物体基本除了角色就是静物
/// </summary>
public class Role : Thing, IItemOwner
{

    #region 字段
    [FieldName("动画组件")]
    public Animator Anim;
    [FieldName("受击时给予多少无敌时长")]
    public float HitInvincibleTime;
    [EnumName("受击音效")]
    public EnumAudioClip HitClip;
    [FieldName("受击特效")]
    public GameObject HitEffect;
    [FieldName("受击动画名")]
    public string HitAnimName;
    [HideInInspector] public float InvincibleTime;//当前无敌时长
    private float m_AlwaysCD;//调用"总是"事件的剩余CD   CD小于0了就会调用一次 总是 事件
    public Dictionary<string, float> LevelUp = new Dictionary<string, float>();
    #region BuffOwner接口 基础数据
    public int OwnerInstanceID { get => InstanceID; set => InstanceID = value; }//将BuffOwner里的实例ID和Thing里的关联起来
    public ItemContainer ItemContainer { get; set; }
    public MonoBehaviour Mono => this;
    public OwnerType Owner { get => OwnerType.主角; }
    #endregion   
    #endregion
    [HideInInspector] public List<int> CurProps;//当前拥有的道具
    [HideInInspector] public int CurRerollTimes;//当前已经重随次数
    [HideInInspector] public int CurRerollTimes1;//当前已经重随次数
    [HideInInspector] public int LevelUpTimes;//当前波次角色升级次数
    [HideInInspector] public List<int> RewardIds = new List<int>();//当前波次宝箱奖励
    private int m_CurWave = 1;//当前波次
    public int CurWave
    {
        get => m_CurWave;
        set
        {
            m_CurWave = value;
            //更新UI
            BattlePage.Instance.WaveText.UpdateTrans(BattlePage.Instance.WaveText.TextId, value);
        }
    }
    private float m_CurSeed;//当前种子数量
    public float CurSeed
    {
        get => m_CurSeed;
        set
        {
            m_CurSeed = value;
            //更新UI 如果没有种子 就隐藏对应UI
            BattlePage.Instance.SeedText.SetText(Mathf.RoundToInt(value).ToString());
            if (value < 1)
            {
                BattlePage.Instance.SeedText.transform.parent.gameObject.Hide();
            }
            else
            {
                BattlePage.Instance.SeedText.transform.parent.gameObject.Show();
            }
        }
    }

    #region 属性     
    #region 持有者位置
    /// <summary>
    /// 坐标
    /// </summary>
    public virtual Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    public virtual Vector3 Position_Base
    {
        get => transform.position;
        set => transform.position = value;
    }
    #endregion

    #region 材料
    protected float m_Material = 30;
    /// <summary>
    /// 材料
    /// </summary>
    public virtual float Material
    {
        get => m_Material;
        set => m_Material = value;
    }
    public virtual float Material_Base
    {
        get => m_Material;
        set => m_Material = value;
    }
    #endregion

    #region 当前等级
    protected int m_CurLevel;
    /// <summary>
    /// 当前等级
    /// </summary>
    public virtual int CurLevel
    {
        get => m_CurLevel;
        set => m_CurLevel = value;
    }
    public virtual int CurLevel_Base
    {
        get => m_CurLevel;
        set => m_CurLevel = value;
    }
    #endregion

    #region 当前经验
    protected float m_CurExp;
    /// <summary>
    /// 当前经验
    /// </summary>
    public virtual float CurExp
    {
        get => m_CurExp;
        set => m_CurExp = value;
    }
    public virtual float CurExp_Base
    {
        get => m_CurExp;
        set => m_CurExp = value;
    }
    #endregion

    #region 当前生命值
    protected float m_CurHP = 15;
    /// <summary>
    /// 当前生命值
    /// </summary>
    public virtual float CurHP
    {
        get => m_CurHP;
        set
        {
            m_CurHP = value;
            if (BattleManager.Instance.Battle)
            {
                GameManager.Instance.Warning?.SetActive(value / MaxHP < 0.2f);
            }

        }
    }
    public virtual float CurHP_Base
    {
        get => m_CurHP;
        set
        {
            m_CurHP = value;
            if (BattleManager.Instance.Battle)
            {
                GameManager.Instance.Warning?.SetActive(value / MaxHP < 0.2f);
            }
        }
    }
    #endregion

    #region 最大生命值
    protected float m_MaxHP = 15;
    /// <summary>
    /// 最大生命值
    /// </summary>
    public virtual float MaxHP
    {
        get
        {
            if (LevelUp.ContainsKey("最大生命值"))
            {
                return ItemContainer.GetFloatValue(EventName.MaxHP, m_MaxHP) + LevelUp["最大生命值"] + GetValue(EventName.MaxHP);
            }
            else
            {
                return ItemContainer.GetFloatValue(EventName.MaxHP, m_MaxHP) + GetValue(EventName.MaxHP);
            }

        }
    }
    public virtual float MaxHP_Base
    {
        get => m_MaxHP;
        set => m_MaxHP = value;
    }
    #endregion


    #region 最大生命值上限
    protected float m_MaxHPUL;
    /// <summary>
    /// 最大生命值上限
    /// </summary>
    public virtual float MaxHPUL
    {
        get => ItemContainer.GetFloatValue(EventName.MaxHPUL, m_MaxHPUL) + GetValue(EventName.MaxHPUL);
    }
    public virtual float MaxHPUL_Base
    {
        get => m_MaxHPUL;
        set => m_MaxHPUL = value;
    }
    #endregion

    #region 生命再生
    protected float m_HPRegeneration;
    /// <summary>
    /// 生命再生
    /// </summary>
    public virtual float HPRegeneration
    {
        get => ItemContainer.GetFloatValue(EventName.HPRegeneration, m_HPRegeneration) + GetValue(EventName.HPRegeneration);
    }
    public virtual float HPRegeneration_Base
    {
        get => m_HPRegeneration;
        set => m_HPRegeneration = value;
    }
    #endregion

    #region 生命窃取
    protected float m_LifeSteal;
    /// <summary>
    /// 生命窃取
    /// </summary>
    public virtual float LifeSteal
    {
        get => ItemContainer.GetFloatValue(EventName.LifeSteal, m_LifeSteal) + GetValue(EventName.LifeSteal);
    }
    public virtual float LifeSteal_Base
    {
        get => m_LifeSteal;
        set => m_LifeSteal = value;
    }
    #endregion

    #region 伤害
    protected float m_Damage = 1;
    /// <summary>
    /// 伤害
    /// </summary>
    public virtual float Damage
    {
        get => ItemContainer.GetFloatValue(EventName.Damage, m_Damage) + GetValue(EventName.Damage);
    }
    public virtual float Damage_Base
    {
        get => m_Damage;
        set => m_Damage = value;
    }
    #endregion

    #region 近战伤害
    protected float m_MeleeDamage;
    /// <summary>
    /// 近战伤害
    /// </summary>
    public virtual float MeleeDamage
    {
        get
        {
            if (LevelUp.ContainsKey("近战伤害"))
            {
                return ItemContainer.GetFloatValue(EventName.MeleeDamage, m_MeleeDamage) + LevelUp["近战伤害"] + GetValue(EventName.MeleeDamage);
            }
            else
            {
                return ItemContainer.GetFloatValue(EventName.MeleeDamage, m_MeleeDamage) + GetValue(EventName.MeleeDamage);
            }

        }
    }
    public virtual float MeleeDamage_Base
    {
        get => m_MeleeDamage;
        set => m_MeleeDamage = value;
    }
    #endregion


    #region 远程伤害
    protected float m_RangedDamage;
    /// <summary>
    /// 远程伤害
    /// </summary>
    public virtual float RangedDamage
    {
        get => ItemContainer.GetFloatValue(EventName.RangedDamage, m_RangedDamage) + GetValue(EventName.RangedDamage);
    }
    public virtual float RangedDamage_Base
    {
        get => m_RangedDamage;
        set => m_RangedDamage = value;
    }
    #endregion

    #region 属性伤害
    protected float m_ElementalDamage;
    /// <summary>
    /// 属性伤害
    /// </summary>
    public virtual float ElementalDamage
    {
        get => ItemContainer.GetFloatValue(EventName.ElementalDamage, m_ElementalDamage) + GetValue(EventName.ElementalDamage);
    }
    public virtual float ElementalDamage_Base
    {
        get => m_ElementalDamage;
        set => m_ElementalDamage = value;
    }
    #endregion

    #region 攻击速度
    protected float m_AttackSpeed = 1;
    /// <summary>
    /// 攻击速度
    /// </summary>
    public virtual float AttackSpeed
    {
        get => ItemContainer.GetFloatValue(EventName.AttackSpeed, m_AttackSpeed) + GetValue(EventName.AttackSpeed);
    }
    public virtual float AttackSpeed_Base
    {
        get => m_AttackSpeed;
        set => m_AttackSpeed = value;
    }
    #endregion

    #region 暴击率
    protected float m_CritChance;
    /// <summary>
    /// 暴击率
    /// </summary>
    public virtual float CritChance
    {
        get => ItemContainer.GetFloatValue(EventName.CritChance, m_CritChance) + GetValue(EventName.CritChance);
    }
    public virtual float CritChance_Base
    {
        get => m_CritChance;
        set => m_CritChance = value;
    }
    #endregion

    #region 工程学
    protected float m_Engineering;
    /// <summary>
    /// 工程学
    /// </summary>
    public virtual float Engineering
    {
        get => ItemContainer.GetFloatValue(EventName.Engineering, m_Engineering) + GetValue(EventName.Engineering);
    }
    public virtual float Engineering_Base
    {
        get => m_Engineering;
        set => m_Engineering = value;
    }
    #endregion


    #region 范围
    protected float m_Range;
    /// <summary>
    /// 范围
    /// </summary>
    public virtual float Range
    {
        get => ItemContainer.GetFloatValue(EventName.Range, m_Range) + GetValue(EventName.Range);
    }
    public virtual float Range_Base
    {
        get => m_Range;
        set => m_Range = value;
    }
    #endregion

    #region 护甲
    protected float m_Armor;
    /// <summary>
    /// 护甲
    /// </summary>
    public virtual float Armor
    {
        get => ItemContainer.GetFloatValue(EventName.Armor, m_Armor) + GetValue(EventName.Armor);
    }
    public virtual float Armor_Base
    {
        get => m_Armor;
        set => m_Armor = value;
    }
    #endregion

    #region 闪避
    protected float m_DodgeRate;
    /// <summary>
    /// 闪避
    /// </summary>
    public virtual float DodgeRate
    {
        get => ItemContainer.GetFloatValue(EventName.DodgeRate, m_DodgeRate) + GetValue(EventName.DodgeRate);
    }
    public virtual float DodgeRate_Base
    {
        get => m_DodgeRate;
        set => m_DodgeRate = value;
    }
    #endregion

    #region 速度
    [FieldName("移动速度")]
    public float m_MoveSpeed = 6;
    public float MoveSpeed
    {
        get => m_MoveSpeed * Speed;
    }
    protected float m_Speed = 1;
    /// <summary>
    /// 速度
    /// </summary>
    public virtual float Speed
    {
        get => ItemContainer.GetFloatValue(EventName.Speed, m_Speed) + GetValue(EventName.Speed);
    }
    public virtual float Speed_Base
    {
        get => m_Speed;
        set => m_Speed = value;
    }
    #endregion

    #region 幸运
    protected float m_Luck;
    /// <summary>
    /// 幸运
    /// </summary>
    public virtual float Luck
    {
        get => ItemContainer.GetFloatValue(EventName.Luck, m_Luck) + GetValue(EventName.Luck);
    }
    public virtual float Luck_Base
    {
        get => m_Luck;
        set => m_Luck = value;
    }
    #endregion


    #region 收获
    protected float m_Harvesting;
    /// <summary>
    /// 收获
    /// </summary>
    public virtual float Harvesting
    {
        get => ItemContainer.GetFloatValue(EventName.Harvesting, m_Harvesting) + GetValue(EventName.Harvesting);
    }
    public virtual float Harvesting_Base
    {
        get => m_Harvesting;
        set => m_Harvesting = value;
    }
    #endregion

    #region 使用消耗品恢复
    protected float m_ConsumableHeal = 1;
    /// <summary>
    /// 使用消耗品恢复
    /// </summary>
    public virtual float ConsumableHeal
    {
        get => ItemContainer.GetFloatValue(EventName.ConsumableHeal, m_ConsumableHeal) + GetValue(EventName.ConsumableHeal);
    }
    public virtual float ConsumableHeal_Base
    {
        get => m_ConsumableHeal;
        set => m_ConsumableHeal = value;
    }
    #endregion

    #region 获得经验
    protected float m_XPGain = 1;
    /// <summary>
    /// 获得经验
    /// </summary>
    public virtual float XPGain
    {
        get => ItemContainer.GetFloatValue(EventName.XPGain, m_XPGain) + GetValue(EventName.XPGain);
    }
    public virtual float XPGain_Base
    {
        get => m_XPGain;
        set => m_XPGain = value;
    }
    #endregion

    #region 拾取范围
    private float m_PickRange = 3;
    public float PickRange { get => m_PickRange * PickupRange; }
    protected float m_PickupRange = 1;
    /// <summary>
    /// 拾取范围
    /// </summary>
    public virtual float PickupRange
    {
        get => ItemContainer.GetFloatValue(EventName.PickupRange, m_PickupRange) + GetValue(EventName.PickupRange);
    }
    public virtual float PickupRange_Base
    {
        get => m_PickupRange;
        set => m_PickupRange = value;
    }
    #endregion

    #region 道具价格
    protected float m_ItemsPrice = 1;
    /// <summary>
    /// 道具价格
    /// </summary>
    public virtual float ItemsPrice
    {
        get => ItemContainer.GetFloatValue(EventName.ItemsPrice, m_ItemsPrice) + GetValue(EventName.ItemsPrice);
    }
    public virtual float ItemsPrice_Base
    {
        get => m_ItemsPrice;
        set => m_ItemsPrice = value;
    }
    #endregion


    #region 爆炸伤害
    protected float m_ExplosionDamage = 1;
    /// <summary>
    /// 爆炸伤害
    /// </summary>
    public virtual float ExplosionDamage
    {
        get => ItemContainer.GetFloatValue(EventName.ExplosionDamage, m_ExplosionDamage) + GetValue(EventName.ExplosionDamage);
    }
    public virtual float ExplosionDamage_Base
    {
        get => m_ExplosionDamage;
        set => m_ExplosionDamage = value;
    }
    #endregion

    #region 爆炸范围
    protected float m_ExplosionSize;
    /// <summary>
    /// 爆炸范围
    /// </summary>
    public virtual float ExplosionSize
    {
        get => ItemContainer.GetFloatValue(EventName.ExplosionSize, m_ExplosionSize) + GetValue(EventName.ExplosionSize);
    }
    public virtual float ExplosionSize_Base
    {
        get => m_ExplosionSize;
        set => m_ExplosionSize = value;
    }
    #endregion

    #region 投射物反弹
    protected float m_Bounces;
    /// <summary>
    /// 投射物反弹
    /// </summary>
    public virtual float Bounces
    {
        get => ItemContainer.GetFloatValue(EventName.Bounces, m_Bounces) + GetValue(EventName.Bounces);
    }
    public virtual float Bounces_Base
    {
        get => m_Bounces;
        set => m_Bounces = value;
    }
    #endregion

    #region 投射物贯通
    protected float m_Piercing;
    /// <summary>
    /// 投射物贯通
    /// </summary>
    public virtual float Piercing
    {
        get => ItemContainer.GetFloatValue(EventName.Piercing, m_Piercing) + GetValue(EventName.Piercing);
    }
    public virtual float Piercing_Base
    {
        get => m_Piercing;
        set => m_Piercing = value;
    }
    #endregion

    #region 贯通伤害
    protected float m_PiercingDamage;
    /// <summary>
    /// 贯通伤害
    /// </summary>
    public virtual float PiercingDamage
    {
        get => ItemContainer.GetFloatValue(EventName.PiercingDamage, m_PiercingDamage) + GetValue(EventName.PiercingDamage);
    }
    public virtual float PiercingDamage_Base
    {
        get => m_PiercingDamage;
        set => m_PiercingDamage = value;
    }
    #endregion


    #region 对头目和精英怪的伤害系数
    protected float m_DamageAgainstBosses = 1;
    /// <summary>
    /// 对头目和精英怪的伤害系数
    /// </summary>
    public virtual float DamageAgainstBosses
    {
        get => ItemContainer.GetFloatValue(EventName.DamageAgainstBosses, m_DamageAgainstBosses) + GetValue(EventName.DamageAgainstBosses);
    }
    public virtual float DamageAgainstBosses_Base
    {
        get => m_DamageAgainstBosses;
        set => m_DamageAgainstBosses = value;
    }
    #endregion

    #region 燃烧速度
    protected float m_BurningSpeed;
    /// <summary>
    /// 燃烧速度
    /// </summary>
    public virtual float BurningSpeed
    {
        get => ItemContainer.GetFloatValue(EventName.BurningSpeed, m_BurningSpeed) + GetValue(EventName.BurningSpeed);
    }
    public virtual float BurningSpeed_Base
    {
        get => m_BurningSpeed;
        set => m_BurningSpeed = value;
    }
    #endregion

    #region 燃烧速度比率
    protected float m_BurningSpeedRate = 1;
    /// <summary>
    /// 燃烧速度比率
    /// </summary>
    public virtual float BurningSpeedRate
    {
        get => ItemContainer.GetFloatValue(EventName.BurningSpeedRate, m_BurningSpeedRate) + GetValue(EventName.BurningSpeedRate);
    }
    public virtual float BurningSpeedRate_Base
    {
        get => m_BurningSpeedRate;
        set => m_BurningSpeedRate = value;
    }
    #endregion

    #region 燃烧蔓延至附近的一名敌人
    protected float m_BurnOther;
    /// <summary>
    /// 燃烧蔓延至附近的一名敌人
    /// </summary>
    public virtual float BurnOther
    {
        get => ItemContainer.GetFloatValue(EventName.BurnOther, m_BurnOther) + GetValue(EventName.BurnOther);
    }
    public virtual float BurnOther_Base
    {
        get => m_BurnOther;
        set => m_BurnOther = value;
    }
    #endregion

    #region 击退
    protected float m_Knockback;
    /// <summary>
    /// 击退
    /// </summary>
    public virtual float Knockback
    {
        get => ItemContainer.GetFloatValue(EventName.Knockback, m_Knockback) + GetValue(EventName.Knockback);
    }
    public virtual float Knockback_Base
    {
        get => m_Knockback;
        set => m_Knockback = value;
    }
    #endregion


    #region 材料翻倍概率
    protected float m_DoubleMaterialChance;
    /// <summary>
    /// 材料翻倍概率
    /// </summary>
    public virtual float DoubleMaterialChance
    {
        get => ItemContainer.GetFloatValue(EventName.DoubleMaterialChance, m_DoubleMaterialChance) + GetValue(EventName.DoubleMaterialChance);
    }
    public virtual float DoubleMaterialChance_Base
    {
        get => m_DoubleMaterialChance;
        set => m_DoubleMaterialChance = value;
    }
    #endregion

    #region 商店免费刷新次数
    protected float m_FreeRerolls;
    /// <summary>
    /// 商店免费刷新次数
    /// </summary>
    public virtual float FreeRerolls
    {
        get => ItemContainer.GetFloatValue(EventName.FreeRerolls, m_FreeRerolls) + GetValue(EventName.FreeRerolls);
    }
    public virtual float FreeRerolls_Base
    {
        get => m_FreeRerolls;
        set => m_FreeRerolls = value;
    }
    #endregion

    #region 树木
    protected float m_Trees;
    /// <summary>
    /// 树木
    /// </summary>
    public virtual float Trees
    {
        get => ItemContainer.GetFloatValue(EventName.Trees, m_Trees) + GetValue(EventName.Trees);
    }
    public virtual float Trees_Base
    {
        get => m_Trees;
        set => m_Trees = value;
    }
    #endregion

    #region 树木能被一击毙命
    protected float m_TreesKill;
    /// <summary>
    /// 树木能被一击毙命
    /// </summary>
    public virtual float TreesKill
    {
        get => ItemContainer.GetFloatValue(EventName.TreesKill, m_TreesKill) + GetValue(EventName.TreesKill);
    }
    public virtual float TreesKill_Base
    {
        get => m_TreesKill;
        set => m_TreesKill = value;
    }
    #endregion

    #region 敌人
    protected float m_Enemies = 1;
    /// <summary>
    /// 敌人
    /// </summary>
    public virtual float Enemies
    {
        get => ItemContainer.GetFloatValue(EventName.Enemies, m_Enemies) + GetValue(EventName.Enemies);
    }
    public virtual float Enemies_Base
    {
        get => m_Enemies;
        set => m_Enemies = value;
    }
    #endregion


    #region 敌人速度
    protected float m_EnemySpeed = 0;
    /// <summary>
    /// 敌人速度
    /// </summary>
    public virtual float EnemySpeed
    {
        get => ItemContainer.GetFloatValue(EventName.EnemySpeed, m_EnemySpeed) + GetValue(EventName.EnemySpeed);
    }
    public virtual float EnemySpeed_Base
    {
        get => m_EnemySpeed;
        set => m_EnemySpeed = value;
    }
    #endregion

    #region 敌人伤害
    protected float m_EnemyDamageRate;
    /// <summary>
    /// 敌人伤害
    /// </summary>
    public virtual float EnemyDamageRate
    {
        get => ItemContainer.GetFloatValue(EventName.EnemyDamageRate, m_EnemyDamageRate) + GetValue(EventName.EnemyDamageRate);
    }
    public virtual float EnemyDamageRate_Base
    {
        get => m_EnemyDamageRate;
        set => m_EnemyDamageRate = value;
    }
    #endregion

    #region 特殊敌人
    protected float m_SpecialEnemy;
    /// <summary>
    /// 特殊敌人
    /// </summary>
    public virtual float SpecialEnemy
    {
        get => ItemContainer.GetFloatValue(EventName.SpecialEnemy, m_SpecialEnemy) + GetValue(EventName.SpecialEnemy);
    }
    public virtual float SpecialEnemy_Base
    {
        get => m_SpecialEnemy;
        set => m_SpecialEnemy = value;
    }
    #endregion

    #region 无法通过其他途径恢复生命值
    protected float m_HealNegation;
    /// <summary>
    /// 无法通过其他途径恢复生命值
    /// </summary>
    public virtual float HealNegation
    {
        get => ItemContainer.GetFloatValue(EventName.HealNegation, m_HealNegation) + GetValue(EventName.HealNegation);
    }
    public virtual float HealNegation_Base
    {
        get => m_HealNegation;
        set => m_HealNegation = value;
    }
    #endregion

    #region 仇恨值
    protected float m_Hatred;
    /// <summary>
    /// 仇恨值
    /// </summary>
    public virtual float Hatred
    {
        get => ItemContainer.GetFloatValue(EventName.Hatred, m_Hatred) + GetValue(EventName.Hatred);
    }
    public virtual float Hatred_Base
    {
        get => m_Hatred;
        set => m_Hatred = value;
    }
    #endregion


    #region 击中敌人时能使其降低10速度最高30
    protected float m_SpeedCutMax30;
    /// <summary>
    /// 击中敌人时能使其降低10速度最高30
    /// </summary>
    public virtual float SpeedCutMax30
    {
        get => ItemContainer.GetFloatValue(EventName.SpeedCutMax30, m_SpeedCutMax30) + GetValue(EventName.SpeedCutMax30);
    }
    public virtual float SpeedCutMax30_Base
    {
        get => m_SpeedCutMax30;
        set => m_SpeedCutMax30 = value;
    }
    #endregion

    #region 立即吸收掉落的材料
    protected float m_ImmediatelyEatMaterialChance;
    /// <summary>
    /// 立即吸收掉落的材料
    /// </summary>
    public virtual float ImmediatelyEatMaterialChance
    {
        get => ItemContainer.GetFloatValue(EventName.ImmediatelyEatMaterialChance, m_ImmediatelyEatMaterialChance) + GetValue(EventName.ImmediatelyEatMaterialChance);
    }
    public virtual float ImmediatelyEatMaterialChance_Base
    {
        get => m_ImmediatelyEatMaterialChance;
        set => m_ImmediatelyEatMaterialChance = value;
    }
    #endregion

    #region 回收道具价格系数
    protected float m_ItemsRecyclePrice = 1;
    /// <summary>
    /// 回收道具价格系数
    /// </summary>
    public virtual float ItemsRecyclePrice
    {
        get => ItemContainer.GetFloatValue(EventName.ItemsRecyclePrice, m_ItemsRecyclePrice) + GetValue(EventName.ItemsRecyclePrice);
    }
    public virtual float ItemsRecyclePrice_Base
    {
        get => m_ItemsRecyclePrice;
        set => m_ItemsRecyclePrice = value;
    }
    #endregion

    #region 免疫伤害次数
    protected float m_InjuryAvoidanceTimes;
    /// <summary>
    /// 免疫伤害次数
    /// </summary>
    public virtual float InjuryAvoidanceTimes
    {
        get => ItemContainer.GetFloatValue(EventName.InjuryAvoidanceTimes, m_InjuryAvoidanceTimes) + GetValue(EventName.InjuryAvoidanceTimes);
    }
    public virtual float InjuryAvoidanceTimes_Base
    {
        get => m_InjuryAvoidanceTimes;
        set => m_InjuryAvoidanceTimes = value;
    }
    public float CurInjuryAvoidanceTimes;
    #endregion

    #region 随机数
    protected float m_RandomNumber;
    /// <summary>
    /// 随机数
    /// </summary>
    public virtual float RandomNumber
    {
        get => 100f.GetRandom();
    }
    public virtual float RandomNumber_Base
    {
        get => 100f.GetRandom();
        set => m_RandomNumber = value;
    }
    #endregion
    #endregion

    #region 事件相关
    #region 注册注销事件
    /// <summary>
    /// 注册事件
    /// </summary>
    public virtual void RegisterEvent()
    {
        #region 其他事件
        EventMgr.RegisterEvent(EventName.Log, LogEvent);//打印日志
        EventMgr.RegisterEvent(EventName.Tags, TagsEvent);//判断标签事件
        EventMgr.RegisterEvent(EventName.Die, DieEvent);
        EventMgr.RegisterEvent(EventName.Count, CountEvent);
        EventMgr.RegisterEvent(EventName.Count1, Count1Event);
        EventMgr.RegisterEvent(EventName.Count2, Count2Event);
        EventMgr.RegisterEvent(EventName.RandomAttackEnemy, RandomAttackEnemyEvent);
        #endregion

        #region 属性事件
        EventMgr.RegisterEvent(EventName.Position, PositionEvent);

        EventMgr.RegisterEvent(EventName.Material, MaterialEvent);
        EventMgr.RegisterEvent(EventName.CurLevel, CurLevelEvent);
        EventMgr.RegisterEvent(EventName.CurExp, CurExpEvent);
        EventMgr.RegisterEvent(EventName.CurHP, CurHPEvent);
        EventMgr.RegisterEvent(EventName.MaxHP, MaxHPEvent);

        EventMgr.RegisterEvent(EventName.MaxHPUL, MaxHPULEvent);
        EventMgr.RegisterEvent(EventName.HPRegeneration, HPRegenerationEvent);
        EventMgr.RegisterEvent(EventName.LifeSteal, LifeStealEvent);
        EventMgr.RegisterEvent(EventName.Damage, DamageEvent);
        EventMgr.RegisterEvent(EventName.MeleeDamage, MeleeDamageEvent);

        EventMgr.RegisterEvent(EventName.RangedDamage, RangedDamageEvent);
        EventMgr.RegisterEvent(EventName.ElementalDamage, ElementalDamageEvent);
        EventMgr.RegisterEvent(EventName.AttackSpeed, AttackSpeedEvent);
        EventMgr.RegisterEvent(EventName.CritChance, CritChanceEvent);
        EventMgr.RegisterEvent(EventName.Engineering, EngineeringEvent);

        EventMgr.RegisterEvent(EventName.Range, RangeEvent);
        EventMgr.RegisterEvent(EventName.Armor, ArmorEvent);
        EventMgr.RegisterEvent(EventName.DodgeRate, DodgeRateEvent);
        EventMgr.RegisterEvent(EventName.Speed, SpeedEvent);
        EventMgr.RegisterEvent(EventName.Luck, LuckEvent);

        EventMgr.RegisterEvent(EventName.Harvesting, HarvestingEvent);
        EventMgr.RegisterEvent(EventName.ConsumableHeal, ConsumableHealEvent);
        EventMgr.RegisterEvent(EventName.XPGain, XPGainEvent);
        EventMgr.RegisterEvent(EventName.PickupRange, PickupRangeEvent);
        EventMgr.RegisterEvent(EventName.ItemsPrice, ItemsPriceEvent);

        EventMgr.RegisterEvent(EventName.ExplosionDamage, ExplosionDamageEvent);
        EventMgr.RegisterEvent(EventName.ExplosionSize, ExplosionSizeEvent);
        EventMgr.RegisterEvent(EventName.Bounces, BouncesEvent);
        EventMgr.RegisterEvent(EventName.Piercing, PiercingEvent);
        EventMgr.RegisterEvent(EventName.PiercingDamage, PiercingDamageEvent);

        EventMgr.RegisterEvent(EventName.DamageAgainstBosses, DamageAgainstBossesEvent);
        EventMgr.RegisterEvent(EventName.BurningSpeed, BurningSpeedEvent);
        EventMgr.RegisterEvent(EventName.BurningSpeedRate, BurningSpeedRateEvent);
        EventMgr.RegisterEvent(EventName.BurnOther, BurnOtherEvent);
        EventMgr.RegisterEvent(EventName.Knockback, KnockbackEvent);

        EventMgr.RegisterEvent(EventName.DoubleMaterialChance, DoubleMaterialChanceEvent);
        EventMgr.RegisterEvent(EventName.FreeRerolls, FreeRerollsEvent);
        EventMgr.RegisterEvent(EventName.Trees, TreesEvent);
        EventMgr.RegisterEvent(EventName.TreesKill, TreesKillEvent);
        EventMgr.RegisterEvent(EventName.Enemies, EnemiesEvent);

        EventMgr.RegisterEvent(EventName.EnemySpeed, EnemySpeedEvent);
        EventMgr.RegisterEvent(EventName.EnemyDamageRate, EnemyDamageRateEvent);
        EventMgr.RegisterEvent(EventName.SpecialEnemy, SpecialEnemyEvent);
        EventMgr.RegisterEvent(EventName.HealNegation, HealNegationEvent);
        EventMgr.RegisterEvent(EventName.Hatred, HatredEvent);

        EventMgr.RegisterEvent(EventName.SpeedCutMax30, SpeedCutMax30Event);
        EventMgr.RegisterEvent(EventName.ImmediatelyEatMaterialChance, ImmediatelyEatMaterialChanceEvent);
        EventMgr.RegisterEvent(EventName.ItemsRecyclePrice, ItemsRecyclePriceEvent);
        EventMgr.RegisterEvent(EventName.InjuryAvoidanceTimes, InjuryAvoidanceTimesEvent);
        EventMgr.RegisterEvent(EventName.RandomNumber, RandomNumberEvent);
        #endregion
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    public virtual void UnRegisterEvent()
    {
        #region 其他事件
        EventMgr.UnRegisterEvent(EventName.Log, LogEvent);
        EventMgr.UnRegisterEvent(EventName.Tags, TagsEvent);
        EventMgr.UnRegisterEvent(EventName.Die, DieEvent);
        EventMgr.UnRegisterEvent(EventName.Count, CountEvent);
        EventMgr.UnRegisterEvent(EventName.Count1, Count1Event);
        EventMgr.UnRegisterEvent(EventName.Count2, Count2Event);
        EventMgr.UnRegisterEvent(EventName.RandomAttackEnemy, RandomAttackEnemyEvent);
        #endregion

        #region 属性事件
        EventMgr.UnRegisterEvent(EventName.Position, PositionEvent);

        EventMgr.UnRegisterEvent(EventName.Material, MaterialEvent);
        EventMgr.UnRegisterEvent(EventName.CurLevel, CurLevelEvent);
        EventMgr.UnRegisterEvent(EventName.CurExp, CurExpEvent);
        EventMgr.UnRegisterEvent(EventName.CurHP, CurHPEvent);
        EventMgr.UnRegisterEvent(EventName.MaxHP, MaxHPEvent);

        EventMgr.UnRegisterEvent(EventName.MaxHPUL, MaxHPULEvent);
        EventMgr.UnRegisterEvent(EventName.HPRegeneration, HPRegenerationEvent);
        EventMgr.UnRegisterEvent(EventName.LifeSteal, LifeStealEvent);
        EventMgr.UnRegisterEvent(EventName.Damage, DamageEvent);
        EventMgr.UnRegisterEvent(EventName.MeleeDamage, MeleeDamageEvent);

        EventMgr.UnRegisterEvent(EventName.RangedDamage, RangedDamageEvent);
        EventMgr.UnRegisterEvent(EventName.ElementalDamage, ElementalDamageEvent);
        EventMgr.UnRegisterEvent(EventName.AttackSpeed, AttackSpeedEvent);
        EventMgr.UnRegisterEvent(EventName.CritChance, CritChanceEvent);
        EventMgr.UnRegisterEvent(EventName.Engineering, EngineeringEvent);

        EventMgr.UnRegisterEvent(EventName.Range, RangeEvent);
        EventMgr.UnRegisterEvent(EventName.Armor, ArmorEvent);
        EventMgr.UnRegisterEvent(EventName.DodgeRate, DodgeRateEvent);
        EventMgr.UnRegisterEvent(EventName.Speed, SpeedEvent);
        EventMgr.UnRegisterEvent(EventName.Luck, LuckEvent);

        EventMgr.UnRegisterEvent(EventName.Harvesting, HarvestingEvent);
        EventMgr.UnRegisterEvent(EventName.ConsumableHeal, ConsumableHealEvent);
        EventMgr.UnRegisterEvent(EventName.XPGain, XPGainEvent);
        EventMgr.UnRegisterEvent(EventName.PickupRange, PickupRangeEvent);
        EventMgr.UnRegisterEvent(EventName.ItemsPrice, ItemsPriceEvent);

        EventMgr.UnRegisterEvent(EventName.ExplosionDamage, ExplosionDamageEvent);
        EventMgr.UnRegisterEvent(EventName.ExplosionSize, ExplosionSizeEvent);
        EventMgr.UnRegisterEvent(EventName.Bounces, BouncesEvent);
        EventMgr.UnRegisterEvent(EventName.Piercing, PiercingEvent);
        EventMgr.UnRegisterEvent(EventName.PiercingDamage, PiercingDamageEvent);

        EventMgr.UnRegisterEvent(EventName.DamageAgainstBosses, DamageAgainstBossesEvent);
        EventMgr.UnRegisterEvent(EventName.BurningSpeed, BurningSpeedEvent);
        EventMgr.UnRegisterEvent(EventName.BurningSpeedRate, BurningSpeedRateEvent);
        EventMgr.UnRegisterEvent(EventName.BurnOther, BurnOtherEvent);
        EventMgr.UnRegisterEvent(EventName.Knockback, KnockbackEvent);

        EventMgr.UnRegisterEvent(EventName.DoubleMaterialChance, DoubleMaterialChanceEvent);
        EventMgr.UnRegisterEvent(EventName.FreeRerolls, FreeRerollsEvent);
        EventMgr.UnRegisterEvent(EventName.Trees, TreesEvent);
        EventMgr.UnRegisterEvent(EventName.TreesKill, TreesKillEvent);
        EventMgr.UnRegisterEvent(EventName.Enemies, EnemiesEvent);

        EventMgr.UnRegisterEvent(EventName.EnemySpeed, EnemySpeedEvent);
        EventMgr.UnRegisterEvent(EventName.EnemyDamageRate, EnemyDamageRateEvent);
        EventMgr.UnRegisterEvent(EventName.SpecialEnemy, SpecialEnemyEvent);
        EventMgr.UnRegisterEvent(EventName.HealNegation, HealNegationEvent);
        EventMgr.UnRegisterEvent(EventName.Hatred, HatredEvent);

        EventMgr.UnRegisterEvent(EventName.SpeedCutMax30, SpeedCutMax30Event);
        EventMgr.UnRegisterEvent(EventName.ImmediatelyEatMaterialChance, ImmediatelyEatMaterialChanceEvent);
        EventMgr.UnRegisterEvent(EventName.ItemsRecyclePrice, ItemsRecyclePriceEvent);
        EventMgr.UnRegisterEvent(EventName.InjuryAvoidanceTimes, InjuryAvoidanceTimesEvent);
        EventMgr.UnRegisterEvent(EventName.RandomNumber, RandomNumberEvent);
        #endregion
    }
    #endregion

    #region 其他事件
    public Enemy Beattacked;//当前被攻击者
    [HideInInspector] public float Count;
    private object CountEvent(object[] arg)
    {
        return Common(arg, Count, Count, (x) => Count = x);
    }

    [HideInInspector] public float Count1;
    private object Count1Event(object[] arg)
    {
        return Common(arg, Count1, Count1, (x) => Count1 = x);
    }

    [HideInInspector] public float Count2;
    private object Count2Event(object[] arg)
    {
        return Common(arg, Count2, Count2, (x) => Count2 = x);
    }

    private object RandomAttackEnemyEvent(object[] arg)
    {
        //随机对一名敌人造成伤害
        //1.随机抽取一个敌人
        Enemy enemy = BattleManager.Instance.Enemys.RandomFromList(false);
        enemy.TakeDamage(arg);
        return null;
    }

    /// <summary>
    /// 打印日志 【事件目标对象(可以不填)，打印日志(事件名)，日志内容】
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private object LogEvent(object[] arg)
    {
        //判断当前事件的响应者是否是自己 是的话就将内容打印出来
        if (!Replace(arg)) return null;
        //如果第一个参数是事件对象 那么就将第三个参数打印出来  否则就将第二个参数打印出来
        print(arg[ToolFun.IsEventTarget(arg[0]) ? 2 : 1].ToString());
        return null;
    }

    /// <summary>
    /// 对象标签的操作与判断
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    protected virtual object TagsEvent(object[] arg)
    {
        if (!Replace(arg)) return null;
        return ToolFun.EventTemplate(Tags, arg);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    protected virtual object DieEvent(object[] arg)
    {
        if (!Replace(arg)) return null;

        print(name + "死亡");
        return false;
    }
    #endregion

    #region 属性事件
    protected virtual object PositionEvent(object[] arg)
    {
        return Common(arg, Position, Position_Base, (x) => Position_Base = x);
    }

    private object MaterialEvent(object[] arg)
    {
        return Common(arg, Material, Material_Base, (x) => Material_Base = x);
    }
    private object CurLevelEvent(object[] arg)
    {
        return Common(arg, CurLevel, CurLevel_Base, (x) => CurLevel_Base = (int)x);
    }
    private object CurExpEvent(object[] arg)
    {
        return Common(arg, CurExp, CurExp_Base, (x) => CurExp_Base = x);
    }
    private object CurHPEvent(object[] arg)
    {
        return Common(arg, CurHP, CurHP_Base, (x) => CurHP_Base = x);
    }
    private object MaxHPEvent(object[] arg)
    {
        return Common(arg, MaxHP, MaxHP_Base, (x) => MaxHP_Base = x);
    }


    private object MaxHPULEvent(object[] arg)
    {
        return Common(arg, MaxHPUL, MaxHPUL_Base, (x) => MaxHPUL_Base = x);
    }
    private object HPRegenerationEvent(object[] arg)
    {
        return Common(arg, HPRegeneration, HPRegeneration_Base, (x) => HPRegeneration_Base = x);
    }
    private object LifeStealEvent(object[] arg)
    {
        return Common(arg, LifeSteal, LifeSteal_Base, (x) => LifeSteal_Base = x);
    }
    private object DamageEvent(object[] arg)
    {
        return Common(arg, Damage, Damage_Base, (x) => Damage_Base = x);
    }
    private object MeleeDamageEvent(object[] arg)
    {
        return Common(arg, MeleeDamage, MeleeDamage_Base, (x) => MeleeDamage_Base = x);
    }


    private object RangedDamageEvent(object[] arg)
    {
        return Common(arg, RangedDamage, RangedDamage_Base, (x) => RangedDamage_Base = x);
    }
    private object ElementalDamageEvent(object[] arg)
    {
        return Common(arg, ElementalDamage, ElementalDamage_Base, (x) => ElementalDamage_Base = x);
    }
    private object AttackSpeedEvent(object[] arg)
    {
        return Common(arg, AttackSpeed, AttackSpeed_Base, (x) => AttackSpeed_Base = x);
    }
    private object CritChanceEvent(object[] arg)
    {
        return Common(arg, CritChance, CritChance_Base, (x) => CritChance_Base = x);
    }
    private object EngineeringEvent(object[] arg)
    {
        return Common(arg, Engineering, Engineering_Base, (x) => Engineering_Base = x);
    }


    private object RangeEvent(object[] arg)
    {
        return Common(arg, Range, Range_Base, (x) => Range_Base = x);
    }
    private object ArmorEvent(object[] arg)
    {
        return Common(arg, Armor, Armor_Base, (x) => Armor_Base = x);
    }
    private object DodgeRateEvent(object[] arg)
    {
        return Common(arg, DodgeRate, DodgeRate_Base, (x) => DodgeRate_Base = x);
    }
    private object SpeedEvent(object[] arg)
    {
        return Common(arg, Speed, Speed_Base, (x) => Speed_Base = x);
    }
    private object LuckEvent(object[] arg)
    {
        return Common(arg, Luck, Luck_Base, (x) => Luck_Base = x);
    }


    private object HarvestingEvent(object[] arg)
    {
        return Common(arg, Harvesting, Harvesting_Base, (x) => Harvesting_Base = x);
    }
    private object ConsumableHealEvent(object[] arg)
    {
        return Common(arg, ConsumableHeal, ConsumableHeal_Base, (x) => ConsumableHeal_Base = x);
    }
    private object XPGainEvent(object[] arg)
    {
        return Common(arg, XPGain, XPGain_Base, (x) => XPGain_Base = x);
    }
    private object PickupRangeEvent(object[] arg)
    {
        return Common(arg, PickupRange, PickupRange_Base, (x) => PickupRange_Base = x);
    }
    private object ItemsPriceEvent(object[] arg)
    {
        return Common(arg, ItemsPrice, ItemsPrice_Base, (x) => ItemsPrice_Base = x);
    }


    private object ExplosionDamageEvent(object[] arg)
    {
        return Common(arg, ExplosionDamage, ExplosionDamage_Base, (x) => ExplosionDamage_Base = x);
    }
    private object ExplosionSizeEvent(object[] arg)
    {
        return Common(arg, ExplosionSize, ExplosionSize_Base, (x) => ExplosionSize_Base = x);
    }
    private object BouncesEvent(object[] arg)
    {
        return Common(arg, Bounces, Bounces_Base, (x) => Bounces_Base = x);
    }
    private object PiercingEvent(object[] arg)
    {
        return Common(arg, Piercing, Piercing_Base, (x) => Piercing_Base = x);
    }
    private object PiercingDamageEvent(object[] arg)
    {
        return Common(arg, PiercingDamage, PiercingDamage_Base, (x) => PiercingDamage_Base = x);
    }


    private object DamageAgainstBossesEvent(object[] arg)
    {
        return Common(arg, DamageAgainstBosses, DamageAgainstBosses_Base, (x) => DamageAgainstBosses_Base = x);
    }
    private object BurningSpeedEvent(object[] arg)
    {
        return Common(arg, BurningSpeed, BurningSpeed_Base, (x) => BurningSpeed_Base = x);
    }
    private object BurningSpeedRateEvent(object[] arg)
    {
        return Common(arg, BurningSpeedRate, BurningSpeedRate_Base, (x) => BurningSpeedRate_Base = x);
    }
    private object BurnOtherEvent(object[] arg)
    {
        return Common(arg, BurnOther, BurnOther_Base, (x) => BurnOther_Base = x);
    }
    private object KnockbackEvent(object[] arg)
    {
        return Common(arg, Knockback, Knockback_Base, (x) => Knockback_Base = x);
    }


    private object DoubleMaterialChanceEvent(object[] arg)
    {
        return Common(arg, DoubleMaterialChance, DoubleMaterialChance_Base, (x) => DoubleMaterialChance_Base = x);
    }
    private object FreeRerollsEvent(object[] arg)
    {
        return Common(arg, FreeRerolls, FreeRerolls_Base, (x) => FreeRerolls_Base = x);
    }
    private object TreesEvent(object[] arg)
    {
        return Common(arg, Trees, Trees_Base, (x) => Trees_Base = x);
    }
    private object TreesKillEvent(object[] arg)
    {
        return Common(arg, TreesKill, TreesKill_Base, (x) => TreesKill_Base = x);
    }
    private object EnemiesEvent(object[] arg)
    {
        return Common(arg, Enemies, Enemies_Base, (x) => Enemies_Base = x);
    }


    private object EnemySpeedEvent(object[] arg)
    {
        return Common(arg, EnemySpeed, EnemySpeed_Base, (x) => EnemySpeed_Base = x);
    }
    private object EnemyDamageRateEvent(object[] arg)
    {
        return Common(arg, EnemyDamageRate, EnemyDamageRate_Base, (x) => EnemyDamageRate_Base = x);
    }
    private object SpecialEnemyEvent(object[] arg)
    {
        return Common(arg, SpecialEnemy, SpecialEnemy_Base, (x) => SpecialEnemy_Base = x);
    }
    private object HealNegationEvent(object[] arg)
    {
        return Common(arg, HealNegation, HealNegation_Base, (x) => HealNegation_Base = x);
    }
    private object HatredEvent(object[] arg)
    {
        return Common(arg, Hatred, Hatred_Base, (x) => Hatred_Base = x);
    }


    private object SpeedCutMax30Event(object[] arg)
    {
        return Common(arg, SpeedCutMax30, SpeedCutMax30_Base, (x) => SpeedCutMax30_Base = x);
    }
    private object ImmediatelyEatMaterialChanceEvent(object[] arg)
    {
        return Common(arg, ImmediatelyEatMaterialChance, ImmediatelyEatMaterialChance_Base, (x) => ImmediatelyEatMaterialChance_Base = x);
    }
    private object ItemsRecyclePriceEvent(object[] arg)
    {
        return Common(arg, ItemsRecyclePrice, ItemsRecyclePrice_Base, (x) => ItemsRecyclePrice_Base = x);
    }
    private object InjuryAvoidanceTimesEvent(object[] arg)
    {
        return Common(arg, InjuryAvoidanceTimes, InjuryAvoidanceTimes_Base, (x) => InjuryAvoidanceTimes_Base = x);
    }
    private object RandomNumberEvent(object[] arg)
    {
        return Common(arg, RandomNumber, RandomNumber_Base, (x) => RandomNumber_Base = x);
    }
    #endregion
    #endregion

    #region 工具函数
    /// <summary>
    /// 获取某个点周围某个半径范围内的全部敌人
    /// </summary>
    /// <param name="centerPos">中心点位置</param>
    /// <param name="radius">半径距离</param>
    /// <returns></returns>
    public List<Enemy> GetTargetEnemys(Vector3 centerPos, float radius = 99999)
    {
        //创建一个临时存放符合条件的敌人的数组
        List<Enemy> enemys = new List<Enemy>();
        //遍历找到与目标点距离小于给定半径的敌人，加入数组
        BattleManager.Instance.Enemys.ForEach(a =>
        {
            if (Vector3.Distance(centerPos, a.transform.position) < radius)
            {
                enemys.Add(a);
            }
        });
        return enemys;
    }

    /// <summary>
    /// 替换和计算事件字符串里的内容，如果事件目标不是自己则不替换且返回false  比如将-0.1*[层数]*[技能等级] 替换成 5(结算结果)
    /// </summary>
    /// <param name="objs">字符串参数数组</param>
    /// <param name="objs1">存放buff对象 技能对象的特殊参数的数组</param>
    public bool Replace(object[] objs)
    {
        //如果不是事件的目标对象，就无视
        if (!ToolFun.IsEventTarget(objs[0], Owner))
        {
            return false;
        }

        //遍历参数，找到需要计算的字符串，尝试将其计算成一个数字
        for (int i = 0; i < objs.Length; i++)
        {
            string str = objs[i].ToString();
            //如果参数里含有[@表示是一个需要动态计算的值
            if (str.Contains("[@"))
            {
                //将参数替换成计算后的内容
                objs[i] = ItemContainer.TryCalcString(str);
            }
        }
        return true;
    }

    /// <summary>
    /// 通用的事件处理函数
    /// </summary>
    /// <param name="arg">普通参数数组</param>
    /// <param name="arg1">特殊参数数组</param>
    /// <param name="value1">堆叠后的数据</param>
    /// <param name="value2">基础数据</param>
    /// <param name="action">修改属性的回调函数</param>
    /// <returns></returns>
    public object Common(object[] arg, float value1, float value2, Action<float> action)
    {
        //判断是否是事件目标 如果是的话替换一下字符串里的内容 不是的话就无视
        if (!Replace(arg)) return null;

        //如果是判断事件 那么走的是buff堆叠后的数据
        if (arg != null && arg[arg.Length - 1] is Mark)
        {
            return ToolFun.EventTemplate(value1, arg, x => { });
        }
        //如果是对属性进行+-*/运算 那么走的是基础数据
        else
        {
            return ToolFun.EventTemplate(value2, arg, x => action(x));
        }
    }

    /// <summary>
    /// 通用的事件处理函数(变量类型为三维向量)
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="arg1"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public object Common(object[] arg, Vector3 value1, Vector3 value2, Action<Vector3> action)
    {
        //判断是否是事件目标 如果是的话替换一下字符串里的内容
        if (!Replace(arg)) return null;

        //如果是判断事件 那么走的是buff堆叠后的数据
        if (arg != null && arg[arg.Length - 1] is Mark)
        {
            return ToolFun.EventTemplate(value1, arg, x => { });
        }
        //如果是对属性进行+-*/运算 那么走的是基础数据
        else
        {
            return ToolFun.EventTemplate(value2, arg, x => action(x));
        }
    }

    #region 判断是否包含某个标签(当前项目应该无用)
    /// <summary>
    /// 是否包含给定标签
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool ContainTag(string tag)
    {
        return Tags.Contains(tag);
    }

    /// <summary>
    /// 是否至少包含一个给定数组里的标签
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool ContainTag(List<string> tags)
    {
        foreach (var item in tags)
        {
            if (Tags.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 是否至少包含一个给定数组里的标签
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool ContainTag(List<ThingTag> tags)
    {
        foreach (var item in tags)
        {
            if (Tags.Contains(item.ToString()))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 两个数组里是否用重叠的部分
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool ContainTag(List<ThingTag> tags, List<string> tags1)
    {
        bool contain = false;
        tags.ForEach(a =>
        {
            if (tags1.Contains(a.ToString()))
            {
                contain = true;
            }
        });
        return contain;
    }
    #endregion
    #endregion

    #region 生命周期函数
    private float m_CacheCD;
    protected override void Start()
    {
        //注册事件
        RegisterEvent();
        base.Start();

        //初始化Buff容器
        ItemContainer = gameObject.AddComponent<ItemContainer>();
        ItemContainer.Role = this;
    }

    protected virtual void Update()
    {
        //时时减少无敌时长
        if (InvincibleTime > 0)
        {
            InvincibleTime -= Time.deltaTime;
        }
        m_CacheCD += Time.deltaTime;
        if (m_CacheCD > 2)
        {
            PDicCache.Clear();
            m_CacheCD = 0;
        }
    }

    protected virtual void LateUpdate()
    {
        //根据Y轴的值来设置Z轴的值，让越下方的角色的层级越高
        transform.SetPositionZ(transform.position.y * 0.001f);
    }

    protected virtual void FixedUpdate()
    {
        #region 每隔一个固定间隔调用一次  "总是" 事件
        m_AlwaysCD += Time.fixedDeltaTime;
        if (m_AlwaysCD > GameManager.Instance.AlwaysInterval)
        {
            m_AlwaysCD = 0;
            EventMgr.ExecuteEvent(EventName.Always, new object[] { OwnerType.主角 });
        }
        #endregion
    }

    public override void OnDestroy()
    {
        //注销事件
        UnRegisterEvent();
        base.OnDestroy();
    }
    #endregion

    #region 便捷函数
    /// <summary>
    /// 加减经验
    /// </summary>
    /// <param name="exp"></param>
    public void ExpChange(float exp)
    {
        //乘经验系数
        m_CurExp += exp * XPGain;
        //判断是否能升级了
        while (m_CurExp >= ConfigManager.Instance.cfgRole.AllConfigs[CurLevel].needExp)
        {
            //扣除经验
            m_CurExp -= ConfigManager.Instance.cfgRole.AllConfigs[CurLevel].needExp;
            //等级+1
            m_CurLevel++;
            LevelUpTimes++;
            UIMgr.Instance.ShowNumbTip1(transform.position, new List<string>() { "心脏图标", "前臂图标" });
            string str = ConfigManager.Instance.cfgRole[CurLevel].propertys[0];
            string[] strs = str.Split('。');
            strs.ForEach(a =>
            {
                string[] kv = a.Replace("【", "").Replace("】", "").Split('，');
                LevelUp[kv[0]] = kv[1].ToFloat();
            });
            HpChange(1);
            BattlePage.Instance.UpdateTip();
        }
        //刷新UI
        BattlePage.Instance.LevelText.SetText("Lv." + CurLevel);
        BattlePage.Instance.ExpImg.FillAmount(m_CurExp / ConfigManager.Instance.cfgRole.AllConfigs[CurLevel].needExp);
    }

    /// <summary>
    /// 加减血量
    /// </summary>
    /// <param name="hp"></param>
    public void HpChange(float hp)
    {
        //刷新血量
        CurHP = Mathf.Clamp(Mathf.RoundToInt(m_CurHP + hp), 0, MaxHP);
        BattlePage.Instance.HpText.SetText(CurHP + "/" + MaxHP);
        BattlePage.Instance.HpImg.FillAmount(CurHP / MaxHP);
        //血量小于0就死亡
        if (m_CurHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 加减材料
    /// </summary>
    /// <param name="material"></param>
    public void MaterialChange(float material)
    {
        m_Material += material;
        BattlePage.Instance.MaterialText.SetText(Mathf.FloorToInt(Player.Instance.Material).ToString());
        ShopPage.Instance.RemainMaterial.SetText(Mathf.FloorToInt(Player.Instance.Material).ToString());
    }

    /// <summary>
    /// 加减种子
    /// </summary>
    /// <param name="seed"></param>
    public void SeedChange(float seed)
    {
        CurSeed += seed;
        BattlePage.Instance.SeedText.SetText(Player.Instance.CurSeed.ToString());
        //ShopPage.Instance.RemainMaterial.SetText(Player.Instance.Material.ToString());
    }

    /// <summary>
    /// 材料是否足够消耗
    /// </summary>
    /// <param name="cost">需要消耗的材料值</param>
    /// <returns></returns>
    public bool MaterialEnough(float cost)
    {
        return Material >= cost;
    }

    /// <summary>
    /// 受击函数
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Hit(float damage, bool giveInvincibleTime = true) { }

    /// <summary>
    /// 角色死亡
    /// </summary>
    public virtual void Die() { }
    #endregion

    /// <summary>
    /// 属性名，(属性生效条件，值)
    /// </summary>
    public Dictionary<string, List<(string, string)>> PDic = new Dictionary<string, List<(string, string)>>();
    /// <summary>
    /// 缓存数据
    /// </summary>
    public Dictionary<string, float> PDicCache = new Dictionary<string, float>();
    private List<string> m_Values;
    public float GetValue(string p)
    {
        if (PDic.ContainsKey(p))
        {
            if (PDicCache.ContainsKey(p))
            {
                return PDicCache[p];
            }
            var list = PDic[p];
            m_Values.Clear();
            list.ForEach(a =>
            {
                //如果条件满足就计算属性
                if (ToolFun.ExecuteJudgeEvents(a.Item1))
                {
                    m_Values.Add(a.Item2);
                }
            });
            float total = 0;
            m_Values.ForEach(a =>
            {
                total += ItemContainer.CalcString(a);
            });
            PDicCache[p] = total;
            return total;
        }
        else
        {
            return 0;
        }
    }
}
