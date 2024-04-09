using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;
/// <summary>
/// 主角控制器
/// </summary>
public class Player : Role
{
    [Header("测试道具数组")]
    public List<int> TestItemID = new List<int>();
    public GameObject Prefab;
    public int LifeStealMax = 10;
    [EnumName("移动音效")]
    public EnumAudioClip Move;
    public int RoleID;
    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 简单单例
    /// </summary>
    public static Player Instance;
    private List<int> m_Tasks = new List<int>();
    protected override void Start()
    {

        base.Start();
        //设置相机跟随
        CameraFollow.Instance.Target = transform;
        CameraFollow.Instance.Follow = true;
        switch (RoleID)
        {
            case 0:
                ItemContainer.AddItem(401);
                ItemContainer.AddItem(2033);
                break;
            default:
                break;
        }
        TestItemID.ForEach(a =>
        {
            print(a);
            ItemContainer.AddItem(a);
        });
        EventMgr.RegisterEvent(EventName.NextWave, NextWave);
        InitWeapon();
        TimeMgr.Timer.AddTimeTask((a) =>
        {
            LifeStealMax = 10;
        }, 1000, TimeUnit.Millisecond, 999999);
        m_Tasks.Add(TimeMgr.Timer.AddTimeTask((a) =>
        {
            HpChange(HPRegeneration / 10);
        }, 1000, TimeUnit.Millisecond, 999999));


        //刷新战斗界面UI
        BattlePage.Instance.Renew();
        TimeMgr.Timer.AddTimeTask(a => CurHP = MaxHP, 50);



    }

    private object NextWave(object[] arg)
    {
        LevelUpTimes = 0;
        RewardIds.Clear();
        InitWeapon();
        //刷新战斗界面UI
        BattlePage.Instance.Renew();
        return null;
    }
    public async void InitWeapon()
    {
        await Task.Delay(50);
        //根据数量 来显示对应的父物体
        List<ItemBase> weapons = new List<ItemBase>();
        ItemContainer.m_ItemDic.ForEach(a =>
        {
            if (a.Key >= 2000)
            {
                a.Value.ForEach(b => weapons.Add(b));
            }
        });
        transform.Find("武器槽123").gameObject.Hide();
        transform.Find("武器槽4").gameObject.Hide();
        transform.Find("武器槽5").gameObject.Hide();
        transform.Find("武器槽6").gameObject.Hide();
        if (weapons.Count == 6)
        {
            transform.Find("武器槽6").gameObject.Show();
            for (int i = 0; i < 6; i++)
            {
                Sprite sp = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[weapons[i].ItemData.icon].Replace("_icon", ""));
                RowCfgWeapon row = ConfigManager.Instance.cfgWeapon[weapons[i].ItemData.effectIntroduce];
                transform.Find("武器槽6").GetChild(i).GetComponentInChildren<Weapon>().Init(sp, row.coolDown, row.range, row.damageBase, row.knockback, row.critChance, row.critMultiply, row);
            }
        }
        else if (weapons.Count == 5)
        {
            transform.Find("武器槽5").gameObject.Show();
            for (int i = 0; i < 5; i++)
            {
                Sprite sp = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[weapons[i].ItemData.icon].Replace("_icon", ""));
                RowCfgWeapon row = ConfigManager.Instance.cfgWeapon[weapons[i].ItemData.effectIntroduce];
                transform.Find("武器槽5").GetChild(i).GetComponentInChildren<Weapon>().Init(sp, row.coolDown, row.range, row.damageBase, row.knockback, row.critChance, row.critMultiply, row);
            }
        }
        else if (weapons.Count == 4)
        {
            transform.Find("武器槽4").gameObject.Show();
            for (int i = 0; i < 4; i++)
            {
                Sprite sp = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[weapons[i].ItemData.icon].Replace("_icon", ""));
                RowCfgWeapon row = ConfigManager.Instance.cfgWeapon[weapons[i].ItemData.effectIntroduce];
                transform.Find("武器槽4").GetChild(i).GetComponentInChildren<Weapon>().Init(sp, row.coolDown, row.range, row.damageBase, row.knockback, row.critChance, row.critMultiply, row);
            }
        }
        else
        {
            transform.Find("武器槽123").gameObject.Show();
            transform.Find("武器槽123").GetChild(0).gameObject.Hide();
            transform.Find("武器槽123").GetChild(1).gameObject.Hide();
            transform.Find("武器槽123").GetChild(2).gameObject.Hide();
            for (int i = 0; i < weapons.Count; i++)
            {
                Sprite sp = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[weapons[i].ItemData.icon].Replace("_icon", ""));
                transform.Find("武器槽123").GetChild(i).gameObject.Show();
                RowCfgWeapon row = ConfigManager.Instance.cfgWeapon[weapons[i].ItemData.effectIntroduce];
                transform.Find("武器槽123").GetChild(i).GetComponentInChildren<Weapon>().Init(sp, row.coolDown, row.range, row.damageBase, row.knockback, row.critChance, row.critMultiply, row);
            }

        }

    }

    protected override void Update()
    {
        base.Update();
        if (!BattleManager.Instance.Battle)
        {
            Anim.Play("Idle 1");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            MaterialChange(100);
            HpChange(-5);
            ExpChange(10);
        }

        //移动控制
        Vector2 v = Vector2.zero;
        if (InputMgr.IsKeyboard)
        {
            v = new Vector2(InputMgr.Left > 0 ? -InputMgr.Left : InputMgr.Right, InputMgr.Up > 0 ? InputMgr.Up : -InputMgr.Down);
        }
        else
        {
            v = InputMgr.JoyL_P != Vector2.zero ? InputMgr.JoyL_P : InputMgr.Dpad_P;
        }
        transform.Translate(v * MoveSpeed * Time.deltaTime, Space.World);
        transform.eulerAngles = new Vector3(0, v.x == 0 ? transform.eulerAngles.y : v.x > 0 ? 0 : 180, 0);
        Anim.Play(v == Vector2.zero ? "Idle" : "Move");
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -23, 23), Mathf.Clamp(transform.position.y, -13, 12), transform.position.z);
        if (v != Vector2.zero)
        {
            AudioMgr.PlaySound(Move);
        }
    }

    public override void OnDestroy()
    {
        m_Tasks.ForEach(a => TimeMgr.Timer.DeleteTimeTask(a));
        Instance = null;
        EventMgr.UnRegisterEvent(EventName.NextWave, NextWave);
        base.OnDestroy();
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    public async override void Die()
    {
        if (!BattleManager.Instance.Battle) return;
        BattleManager.Instance.Battle = false;
        //回到主界面
        await Task.Delay(1000);
        MainMenuPage.Instance.Show();
        EventMgr.ExecuteEvent(EventName.Clear);
        //删除敌人
        BattlePage.Instance.Hide();
        BattleManager.Instance.ClearAllEnemys();
        Destroy(GameObject.FindObjectOfType<SceneChangeTool>().gameObject);
        CameraFollow.Instance.transform.position = Vector3.zero;
        Destroy(gameObject);

    }

    /// <summary>
    /// 角色受击
    /// </summary>
    /// <param name="damage">伤害</param>
    public override void Hit(float damage, bool giveInvincibleTime = true)
    {
        base.Hit(damage, giveInvincibleTime);
        if (InvincibleTime > 0) return;
        if (CurInjuryAvoidanceTimes > 0)
        {
            CurInjuryAvoidanceTimes--;
            return;
        }
        //执行被打事件
        EventMgr.ExecuteEvent(EventName.PlayerHit, new object[] { OwnerType.主角 });
        //计算躲避
        if (DodgeRate.Random())
        {
            //执行躲避事件
            EventMgr.ExecuteEvent(EventName.Dodge, new object[] { OwnerType.主角 });
            UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), "Miss", Color.white);
            return;
        }

        //给予短暂无敌
        if (giveInvincibleTime)
        {
            InvincibleTime = 0.5f;
        }
        //受击音效 特效 动画
        BattlePage.Instance.Anim.Play("血条闪白", 1);
        AudioMgr.PlaySound(HitClip);
        ToolFun.ShowEffect(HitEffect, transform.position, 360.GetRandom(), 1);
        if (!string.IsNullOrEmpty(HitAnimName))
        {
            Anim.Play(HitAnimName, 1);//受击动画放在一层，让他叠在底层动画上
        }
        //计算伤害减免后扣除血量  暂定 伤害-护甲的一半 作为最终伤害 最小为0，不会为负数
        float hp = -Mathf.Clamp(Mathf.RoundToInt(damage - Armor * 0.5f), 0, 99999999);
        HpChange(hp);
        UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), "" + Mathf.RoundToInt(hp), Color.red);
        //执行受击扣血事件
        EventMgr.ExecuteEvent(EventName.PlayerRealHit, new object[] { OwnerType.主角 });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果撞击到了 敌人的伤害碰撞器 就扣血
        if (collision.CompareTag(ColliderTagDef.EnemyAttack))
        {
            ColliderData data = collision.GetComponent<ColliderData>();
            if (!data)
            {
                data = collision.GetComponentInParent<ColliderData>();
            }
            Hit(data.Damage);
        }
    }

    public string GetWeaponInfo(int configId)
    {
        RowCfgWeapon weapon = DataMgr.WeaponInfoDataDic[configId];
        string info = "";
        //伤害  23
        string damage = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[23].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        float otherDamage = 0;
        string str = " (";
        #region 构建括号内的内容
        if (weapon.meleeDamage != 0)
        {
            otherDamage += weapon.meleeDamage * MeleeDamage;
            if (weapon.meleeDamage == 1)
            {
                str += "<sprite=8>";
            }
            else
            {
                str += weapon.meleeDamage * 100 + "%" + "<sprite=8>";
            }

        }
        if (weapon.rangedDamage != 0)
        {
            otherDamage += weapon.rangedDamage * RangedDamage;
            if (weapon.rangedDamage == 1)
            {
                str += "<sprite=11>";
            }
            else
            {
                str += weapon.rangedDamage * 100 + "%" + "<sprite=11>";
            }
        }
        if (weapon.elementalDamage != 0)
        {
            otherDamage += weapon.elementalDamage * ElementalDamage;
            if (weapon.elementalDamage == 1)
            {
                str += "<sprite=14>";
            }
            else
            {
                str += weapon.elementalDamage * 100 + "%" + "<sprite=14>";
            }
        }
        if (weapon.engineering != 0)
        {
            otherDamage += weapon.engineering * Engineering;
            if (weapon.engineering == 1)
            {
                str += "<sprite=15>";
            }
            else
            {
                str += weapon.engineering * 100 + "%" + "<sprite=15>";
            }
        }
        if (weapon.attackSpeed != 0)
        {
            otherDamage += weapon.attackSpeed * AttackSpeed;
            if (weapon.attackSpeed == 1)
            {
                str += "<sprite=6>";
            }
            else
            {
                str += weapon.attackSpeed * 100 + "%" + "<sprite=6>";
            }
        }
        if (weapon.level != 0)
        {
            otherDamage += weapon.level * CurLevel;
            if (weapon.level == 1)
            {
                str += "<sprite=7>";
            }
            else
            {
                str += weapon.level * 100 + "%" + "<sprite=7>";
            }
        }
        if (weapon.maxHP != 0)
        {
            otherDamage += weapon.maxHP * MaxHP;
            if (weapon.maxHP == 1)
            {
                str += "<sprite=3>";
            }
            else
            {
                str += weapon.maxHP * 100 + "%" + "<sprite=3>";
            }
        }
        if (weapon.speed != 0)
        {
            otherDamage += weapon.speed * Speed;
            if (weapon.speed == 1)
            {
                str += "<sprite=5>";
            }
            else
            {
                str += weapon.speed * 100 + "%" + "<sprite=5>";
            }
        }
        if (weapon.armor != 0)
        {
            otherDamage += weapon.armor * Armor;
            if (weapon.armor == 1)
            {
                str += "<sprite=4>";
            }
            else
            {
                str += weapon.armor * 100 + "%" + "<sprite=4>";
            }
        }
        if (weapon.rangeRate != 0)
        {
            otherDamage += weapon.rangeRate * Range;
            if (weapon.rangeRate == 1)
            {
                str += "<sprite=0>";
            }
            else
            {
                str += weapon.rangeRate * 100 + "%" + "<sprite=0>";
            }
        }
        str += ")";
        #endregion
        if (otherDamage == 0)
        {
            //伤害: 基础伤害 (....)
            info += damage + " : " + weapon.damageBase + str;
        }
        else if (otherDamage > 0)
        {
            //伤害: 最终伤害(绿色) |(灰色) 基础伤害(灰色) (....)
            info += $"{damage} :<color=#00FF00> {weapon.damageBase + otherDamage} </color> <color=#737373>| {weapon.damageBase}</color> {str}";
        }
        else
        {
            //伤害: 最终伤害(红色) |(灰色) 基础伤害(灰色) (....)
            info += $"{damage} :<color=#FF0000> {weapon.damageBase + otherDamage} </color> <color=#737373>| {weapon.damageBase}</color> {str}";
        }
        info += "\n";

        //暴击  24  25 
        string crit = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[24].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        string p = ConfigManager.Instance.cfgText[25].contents[GameManager.Instance.CurLanguage.GetHashCode()];
        str = " (";
        if (CritChance == 0)
        {
            str += weapon.critChance * 100 + "%" + p + ")";
        }
        else if (CritChance > 0)
        {

            str += $"<color=#00FF00>{(weapon.critChance + CritChance) * 100}%</color>{p})";
        }
        else
        {
            str += $"<color=#FF0000>{(weapon.critChance + CritChance) * 100}%</color>{p})";
        }
        info += $"{crit} : x{weapon.critMultiply} {str} \n";

        //冷却  26
        string cooldown = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[26].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        str = "";
        if (AttackSpeed == 1)
        {
            str = weapon.attackSpeed + "s";
        }
        else if (AttackSpeed > 1)
        {
            str = $"<color=#00FF00>{(weapon.attackSpeed / AttackSpeed * 100).ToString("F0")}</color>s";
        }
        else
        {
            str = $"<color=#FF0000>{(weapon.attackSpeed / AttackSpeed * 100).ToString("F0")}</color>s";
        }
        info += $"{cooldown} : {str} \n";

        //击退  27
        string knockback = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[27].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        if (weapon.knockback + Knockback > 0)
        {
            if (Knockback > 0)
            {
                info += $"{knockback} : <color=#00FF00>{weapon.knockback + Knockback}</color> \n";
            }
            else
            {
                info += $"{knockback} : {weapon.knockback} \n";
            }
        }

        //范围  28   远程32 近战33
        string range = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[28].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        string ranged = ConfigManager.Instance.cfgText[32].contents[GameManager.Instance.CurLanguage.GetHashCode()];
        string melee = ConfigManager.Instance.cfgText[33].contents[GameManager.Instance.CurLanguage.GetHashCode()];
        str = weapon.melee == 1 ? $"({melee})" : $"({ranged})";
        if (Range == 0)
        {
            //伤害: 基础伤害 (....)
            info += range + " : " + weapon.range + str;
        }
        else if (Range > 0)
        {
            //伤害: 最终伤害(绿色) |(灰色) 基础伤害(灰色) (....)
            float totalRange = weapon.melee == 1 ? Range / 2 + weapon.range : Range + weapon.range;
            info += $"{range} :<color=#00FF00> {totalRange} </color> <color=#737373>| {weapon.range}</color> {str}";
        }
        else
        {
            //伤害: 最终伤害(绿色) |(灰色) 基础伤害(灰色) (....)
            float totalRange = weapon.melee == 1 ? Range / 2 + weapon.range : Range + weapon.range;
            info += $"{range} :<color=#FF0000> {totalRange} </color> <color=#737373>| {weapon.range}</color> {str}";
        }
        info += "\n";


        //反弹  29
        string bounces = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[29].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        if (weapon.bounces + Bounces > 0)
        {
            if (Bounces == 0)
            {
                info += $"{bounces} : {weapon.bounces} \n";
            }
            else if (Bounces > 0)
            {
                info += $"{bounces} : <color=#00FF00>{weapon.bounces + Bounces}</color> \n";
            }
            else
            {
                info += $"{bounces} : <color=#FF0000>{weapon.bounces + Bounces}</color> \n";
            }
        }

        //贯穿  30
        string piercing = $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[30].contents[GameManager.Instance.CurLanguage.GetHashCode()]}</color>";
        if (weapon.piercing + Piercing > 0)
        {
            str = "";
            if (PiercingDamage == 0)
            {
                str = $"({weapon.piercingDamage * 100}%{ConfigManager.Instance.cfgText[23].contents[GameManager.Instance.CurLanguage.GetHashCode()]})";
            }
            else if (PiercingDamage > 0)
            {
                str = $"(<color=#00FF00>{(weapon.piercingDamage + PiercingDamage) * 100}%</color>{ConfigManager.Instance.cfgText[23].contents[GameManager.Instance.CurLanguage.GetHashCode()]})";
            }
            else
            {
                str = $"(<color=#FF0000>{(weapon.piercingDamage + PiercingDamage) * 100}%</color>{ConfigManager.Instance.cfgText[23].contents[GameManager.Instance.CurLanguage.GetHashCode()]})";
            }

            if (Piercing == 0)
            {
                info += $"{piercing} : {weapon.piercing} {str}\n";
            }
            else if (Piercing > 0)
            {
                info += $"{piercing} : <color=#00FF00>{weapon.piercing + Piercing}</color> {str}\n";
            }
            else
            {
                info += $"{piercing} : <color=#FF0000>{weapon.piercing + Piercing}</color> {str}\n";
            }

        }
        if (weapon.special != 0)
        {
            info += ConfigManager.Instance.cfgText[weapon.special].contents[GameManager.Instance.CurLanguage.GetHashCode()] + "\n";
        }
        if (weapon.gainDes != 0)
        {
            info += $"<color=#DDD6A4>{ConfigManager.Instance.cfgText[weapon.gainDes].contents[GameManager.Instance.CurLanguage.GetHashCode()]} :</color> 0";
        }

        return info;
    }

    /// <summary>
    /// 获取武器数量
    /// </summary>
    public int GetWeaponCount()
    {
        int count = 0;
        Player.Instance.ItemContainer.m_ItemDic.ForEach(a =>
        {
            if (a.Key >= 2000)
            {
                count += a.Value.Count;
            }
        });
        return count;
    }

    /// <summary>
    /// 获取不同武器数量
    /// </summary>
    public int GetDifferentWeaponCount()
    {
        int count = 0;
        Player.Instance.ItemContainer.m_ItemDic.ForEach(a =>
        {
            if (a.Key >= 2000 && a.Value.Count >= 1)
            {
                count++;
            }
        });
        return count;
    }
}