using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 碰撞器数据类，挂载在各种碰撞器上，记录碰撞器信息，方便代码内部计算时获取所需数据(数据可以是运行时从别处获取填充进来的，不一定都是在面板上配置好的)
/// </summary>
public class ColliderData : MonoBehaviour
{
    [HideInInspector] public RowCfgWeapon Row;
    [HideInInspector] public float DamageRate = 1;//伤害倍率 子弹贯穿后需要降低伤害
    [FieldName("主角相关的碰撞器")]
    public bool IsPlayer;
    [FieldName("基础伤害")]
    public float BaseDamage;

    /// <summary>
    /// 最终伤害
    /// </summary>
    public float Damage
    {
        //如果是武器相关，就动态计算一下伤害，如果不是武器相关就直接返回基础伤害
        get => Row != null ? GetDamage() * DamageRate : BaseDamage;
    }

    [FieldName("基础击退")]
    public float BaseKnockback;
    /// <summary>
    /// 最终击退
    /// </summary>
    public float Knockback
    {
        get => BaseKnockback + Player.Instance.Knockback;
    }
    [FieldName("基础暴击")]
    public float BaseCrit;
    /// <summary>
    /// 最终暴击
    /// </summary>
    public float Crit
    {
        get => BaseCrit + Player.Instance.CritChance;
    }

    [FieldName("暴击伤害倍率")]
    public float CritMultiply;

    /// <summary>
    /// 计算武器相关的碰撞器的伤害
    /// </summary>
    /// <returns></returns>
    public float GetDamage()
    {
        float damage = Row.damageBase;//基础伤害
        //计算各种加成
        damage += Row.meleeDamage * Player.Instance.MeleeDamage;
        damage += Row.rangedDamage * Player.Instance.RangedDamage;
        damage += Row.elementalDamage * Player.Instance.ElementalDamage;
        damage += Row.engineering * Player.Instance.Engineering;
        damage += Row.attackSpeed * Player.Instance.AttackSpeed * 100;
        damage += Row.level * Player.Instance.CurLevel;
        damage += Row.maxHP * Player.Instance.MaxHP;
        damage += Row.speed * Player.Instance.Speed * 100;
        damage += Row.armor * Player.Instance.Armor;
        damage += Row.rangeRate * Player.Instance.Range;
        return damage;
    }

}