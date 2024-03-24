using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 道具持有者接口
/// </summary>
public interface IItemOwner
{
    #region 基础数据
    OwnerType Owner { get; }//持有者类型  有些事件会指定某一类型的对象响应
    int OwnerInstanceID { get; set; }//持有者的实例id
    ItemContainer ItemContainer { get; set; }//容器
    MonoBehaviour Mono { get; }//持有者的mono类对象 用于获取对象transform等Mono类的属性

    #endregion

    #region 属性
    Vector3 Position { get; }
    Vector3 Position_Base { get; set; }

    float Material { get; }
    float Material_Base { get; set; }
    int CurLevel { get; }
    int CurLevel_Base { get; set; }
    float CurExp { get; }
    float CurExp_Base { get; set; }
    float CurHP { get; }
    float CurHP_Base { get; set; }
    float MaxHP { get; }
    float MaxHP_Base { get; set; }

    float MaxHPUL { get; }
    float MaxHPUL_Base { get; set; }
    float HPRegeneration { get; }
    float HPRegeneration_Base { get; set; }
    float LifeSteal { get; }
    float LifeSteal_Base { get; set; }
    float Damage { get; }
    float Damage_Base { get; set; }
    float MeleeDamage { get; }
    float MeleeDamage_Base { get; set; }

    float RangedDamage { get; }
    float RangedDamage_Base { get; set; }
    float ElementalDamage { get; }
    float ElementalDamage_Base { get; set; }
    float AttackSpeed { get; }
    float AttackSpeed_Base { get; set; }
    float CritChance { get; }
    float CritChance_Base { get; set; }
    float Engineering { get; }
    float Engineering_Base { get; set; }

    float Range { get; }
    float Range_Base { get; set; }
    float Armor { get; }
    float Armor_Base { get; set; }
    float DodgeRate { get; }
    float DodgeRate_Base { get; set; }
    float Speed { get; }
    float Speed_Base { get; set; }
    float Luck { get; }
    float Luck_Base { get; set; }

    float Harvesting { get; }
    float Harvesting_Base { get; set; }
    float ConsumableHeal { get; }
    float ConsumableHeal_Base { get; set; }
    float XPGain { get; }
    float XPGain_Base { get; set; }
    float PickupRange { get; }
    float PickupRange_Base { get; set; }
    float ItemsPrice { get; }
    float ItemsPrice_Base { get; set; }

    float ExplosionDamage { get; }
    float ExplosionDamage_Base { get; set; }
    float ExplosionSize { get; }
    float ExplosionSize_Base { get; set; }
    float Bounces { get; }
    float Bounces_Base { get; set; }
    float Piercing { get; }
    float Piercing_Base { get; set; }
    float PiercingDamage { get; }
    float PiercingDamage_Base { get; set; }

    float DamageAgainstBosses { get; }
    float DamageAgainstBosses_Base { get; set; }
    float BurningSpeed { get; }
    float BurningSpeed_Base { get; set; }
    float BurningSpeedRate { get; }
    float BurningSpeedRate_Base { get; set; }
    float BurnOther { get; }
    float BurnOther_Base { get; set; }
    float Knockback { get; }
    float Knockback_Base { get; set; }

    float DoubleMaterialChance { get; }
    float DoubleMaterialChance_Base { get; set; }
    float FreeRerolls { get; }
    float FreeRerolls_Base { get; set; }
    float Trees { get; }
    float Trees_Base { get; set; }
    float TreesKill { get; }
    float TreesKill_Base { get; set; }
    float Enemies { get; }
    float Enemies_Base { get; set; }

    float EnemySpeed { get; }
    float EnemySpeed_Base { get; set; }
    float EnemyDamageRate { get; }
    float EnemyDamageRate_Base { get; set; }
    float SpecialEnemy { get; }
    float SpecialEnemy_Base { get; set; }
    float HealNegation { get; }
    float HealNegation_Base { get; set; }
    float Hatred { get; }
    float Hatred_Base { get; set; }

    float SpeedCutMax30 { get; }
    float SpeedCutMax30_Base { get; set; }
    float ImmediatelyEatMaterialChance { get; }
    float ImmediatelyEatMaterialChance_Base { get; set; }
    float ItemsRecyclePrice { get; }
    float ItemsRecyclePrice_Base { get; set; }
    float InjuryAvoidanceTimes { get; }
    float InjuryAvoidanceTimes_Base { get; set; }
    float RandomNumber { get; }
    float RandomNumber_Base { get; set; }
    #endregion

}
