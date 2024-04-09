using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using GameDevWare.Dynamic.Expressions.CSharp;
/// <summary>
/// 道具容器
/// </summary>
public class ItemContainer : MonoBehaviour
{
    #region 一些字段
    /// <summary>
    /// 道具容器的所属对象，通过这个变量可以获取到道具所需要作用的大部分属性
    /// </summary>
    public Role Role;

    /// <summary>
    /// 道具字典，键是道具ID，值是这个ID的全部道具对象
    /// </summary>
    public Dictionary<int, List<ItemBase>> m_ItemDic = new Dictionary<int, List<ItemBase>>();

    /// <summary>
    /// 道具堆叠属性的字典，键是属性名字，值是全部道具对这个属性的堆叠
    /// </summary>
    private Dictionary<string, List<string>> m_DataDic = new Dictionary<string, List<string>>();

    /// <summary>
    /// 道具堆叠属性的字典，键是属性名字，值是全部道具对这个属性的堆叠
    /// </summary>
    private Dictionary<string, float> m_DataCacheDic = new Dictionary<string, float>();

    /// <summary>
    /// 是否需要重新构建道具堆叠属性的字典，道具增加和删除的时候会修改标识，然后在下一帧会重新构建道具堆叠属性的字典
    /// </summary>
    public bool NeedRebuild;
    #endregion

    #region 道具添加 道具移除
    /// <summary>
    /// 道具添加
    /// </summary>
    /// <param name="itemId">要添加的道具的id</param>
    public ItemBase AddItem(int itemId)
    {
        //创建一个道具并调用它的添加函数
        ItemBase itemBase = new ItemBase();
        itemBase.OnAdd(this, DataMgr.ItemDic[itemId]);
        LogUtil.Debug("添加道具:" + itemBase.ItemData.desc);
        //如果字典里包含这个ID的道具 直接加入数组即可
        if (!m_ItemDic.ContainsKey(itemId))
        {
            m_ItemDic[itemId] = new List<ItemBase>();

            if (itemId<100)
            {
                GameObject go = Instantiate(Player.Instance.Prefab, transform.Find("外观").GetChild(0));
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(0, itemBase.ItemData.offsetY, 0);
                AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[itemBase.ItemData.icon1], (a) =>
                {
                    go.GetComponentInChildren<SpriteRenderer>().sprite = a;
                    go.GetComponentInChildren<SpriteRenderer>().sortingOrder = itemBase.ItemData.layer;
                });
            }
        }
        m_ItemDic[itemId].Add(itemBase);

        return itemBase;
    }

    /// <summary>
    /// 道具移除
    /// </summary>
    /// <param name="item">要移除的道具对象</param>
    public void RemoveItem(ItemBase item)
    {
        if (m_ItemDic.ContainsKey(item.ItemData.id))
        {
            m_ItemDic[item.ItemData.id].RemoveSafe(item);
        }
    }
    #endregion

    #region 重建道具堆叠属性的字典
    /// <summary>
    /// 道具添加或移除之后，重新构建堆叠数组
    /// </summary>
    public void RebuildDataStack()
    {
        //先将数据清除
        m_DataDic.Clear();
        m_DataCacheDic.Clear();
        //挨种Buff将所需堆叠数据加入数据字典
        foreach (var item in m_ItemDic)
        {
            item.Value.ForEach(a =>
            {
                RebuildDataStack(a);
            });
        }
        EventMgr.ExecuteEvent(EventName.ItemChange, new object[] { OwnerType.主角 });
        NeedRebuild = false;
    }

    /// <summary>
    /// 构建堆叠数组
    /// </summary>
    /// <param name="itemBase">道具对象</param>
    public void RebuildDataStack(ItemBase itemBase)
    {
        //如果这个道具没有基础效果就直接略过
        if (!string.IsNullOrEmpty(itemBase.ItemData.baseEffect))
        {
            //分割效果
            string[] effects = itemBase.ItemData.baseEffect.Split('。');
            foreach (var item in effects)
            {
                //将用于分隔的【】去掉，以及将参数数组分割获取到  血量，+-*/值
                string[] strs = item.Replace("【", "").Replace("】", "").Split(item.Contains("，") ? '，' : ',');

                //如果这个属性 字典里还没有 就添加一组键值对 
                if (!m_DataDic.ContainsKey(strs[0]))
                {
                    m_DataDic[strs[0]] = new List<string>() { strs[1] };
                }
                else
                {
                    //如果字典里已经有这个属性 就将其加到数组里
                    m_DataDic[strs[0]].Add(strs[1]);
                }
            }
        }

        if (!string.IsNullOrEmpty(itemBase.ItemData.growUpEffect))
        {
            //分割效果
            string[] effects = itemBase.ItemData.growUpEffect.Split('。');
            foreach (var item in effects)
            {
                //将用于分隔的【】去掉，以及将参数数组分割获取到  血量，+-*/值
                string[] strs = item.Replace("【", "").Replace("】", "").Split(item.Contains("，") ? '，' : ',');
                string value = strs[1];
                if (value.Contains("[@层数]"))
                {
                    value = value.Replace("[@层数]", itemBase.GrowUpEffectCount.ToString());
                }
                //如果这个属性 字典里还没有 就添加一组键值对 
                if (!m_DataDic.ContainsKey(strs[0]))
                {
                    m_DataDic[strs[0]] = new List<string>() { value };
                }
                else
                {
                    //如果字典里已经有这个属性 就将其加到数组里
                    m_DataDic[strs[0]].Add(value);
                }
            }
        }
    }
    #endregion

    #region 通用工具函数
    /// <summary>
    /// 解析效果，并对目标属性进行操作
    /// </summary>
    /// <param name="effect">效果</param>
    public void DoEffect(string effect,ItemBase itemBase)
    {
        //如果效果为空 直接返回
        if (string.IsNullOrEmpty(effect)) return;

        //分割效果，取得若干个子效果
        string[] effects = effect.Split('。');
        foreach (var item in effects)
        {
            //分割参数 
            string[] strs = item.Replace("【", "").Replace("】", "").Split(item.Contains("，") ? '，' : ',');
            //如果效果是执行事件  【事件，创建预制体，树木，5，3】
            if (strs[0] == StringDef.Event || strs[0].Contains(StringDef.事件))
            {
                ToolFun.ExecuteEvents(item);
            }
            //如果效果是修改属性 【血量，-50】
            else
            {
                //将参数二换算成一个具体的数值，然后赋值给value
                float value = 0;
                //如果不是数字开头
                if (!Regex.IsMatch(strs[1][0].ToString(), @"^[0-9]*$"))
                {
                    //如果表达式的开头是+-运算符 ， 那么计算属性的时候视作 + 表达式的结果
                    if (strs[1][0] == '+' || strs[1][0] == '-')
                    {
                        value = CalcString(strs[1]);
                        Calc(strs[0], "+", value, itemBase);
                    }
                    //如果表达式开头是其他运算符号，那么计算表达式的时候先把符号去掉，计算属性的时候，把表达式拿回来
                    else
                    {
                        // *(5+3) = 8  计算表达式时忽略* 得到8   计算属性的时候 把乘拿回来变成  属性 *  8     
                        value = CalcString(strs[1].Substring(1));
                        Calc(strs[0], strs[1][0].ToString(), value, itemBase);
                    }
                }
                else
                {
                    //如果表达式开头没有运算符 那么计算属性的时候视作 + 表达式的结果
                    value = CalcString(strs[1]);
                    Calc(strs[0], "+", value, itemBase);
                }
            }
        }
    }

    /// <summary>
    /// 修改对象的消耗类属性的值，比如血量，经验，蓝量，钱这些
    /// </summary>
    /// <param name="p">属性名</param>
    /// <param name="oprate">要进行的操作</param>
    /// <param name="value">表达式计算得到的结果</param>
    public void Calc(string p, string oprate, float value,ItemBase itemBase)
    {
        if (p == EventName.Material.ToString())
        {
            Role.Material_Base = Oprate(Role.Material_Base, oprate, value);
        }
        else if (p == EventName.CurExp.ToString())
        {
            Role.CurExp_Base = Oprate(Role.CurExp_Base, oprate, value);
        }
        else if (p == EventName.CurHP.ToString())
        {
            Role.CurHP_Base = Oprate(Role.CurHP_Base, oprate, value);
        }
        else if (p==StringDef.次数)
        {
            if (value==0)
            {
                itemBase.CurTimes+= (int)Player.Instance.Count1;
            }
            else
            {
                itemBase.CurTimes = (int)Oprate(itemBase.CurTimes, oprate, value);
            }
            
        }
        else if (p==StringDef.可触发次数)
        {
            if (value==0)
            {
                itemBase.CanTriggerTimes = (int)Player.Instance.Count2;
            }
            else
            {
                itemBase.CanTriggerTimes = (int)Oprate(itemBase.CanTriggerTimes, oprate, value);
            }
            
        }
        else if (p==StringDef.计数)
        {
            if (value==0)
            {
                itemBase.Count += (int)Player.Instance.Count;
                //print("计数:"+ (int)Player.Instance.Count);
                Player.Instance.Count = 0;
            }
            else
            {
                itemBase.Count = (int)Oprate(itemBase.Count, oprate, value);
            }
        }
        else if (p==StringDef.层数)
        {
            itemBase.GrowUpEffectCount= (int)Oprate(itemBase.GrowUpEffectCount, oprate, value);
        }
    }

    /// <summary>
    /// 计算 
    /// </summary>
    /// <param name="old">旧值</param>
    /// <param name="oprate">要进行的运算操作+-*/%=</param>
    /// <param name="value">对旧值进行何种程度的运算操作</param>
    /// <returns></returns>
    private float Oprate(float old, string oprate, float value)
    {
        switch (oprate)
        {
            case "+":
                old += value;
                break;
            case "-":
                old += value;//#这里的负号是数字自己带的 比如-8  所以视作 +  -8即可
                break;
            case "*":
                old *= value;
                break;
            case "/":
                old /= value;
                break;
            case "%":
                old %= value;
                break;
            case "=":
                old = value;
                break;
            default:
                old += value;
                break;
        }
        return old;
    }

    /// <summary>
    /// 获取到道具影响后的某个属性的最终值
    /// </summary>
    /// <param name="property">属性名</param>
    /// <param name="baseValue">基础值</param>
    /// <returns></returns>
    public float GetFloatValue(string property, float baseValue)
    {
        //如果此属性没有道具在影响，就直接返回基础值
        if (!m_DataDic.ContainsKey(property)) return baseValue;

        //先从缓存获取
        if (m_DataCacheDic.ContainsKey(property))
        {
            return m_DataCacheDic[property];
        }

        //获取到这个属性的堆叠数据
        List<string> strs = m_DataDic[property];
        //先算加减 后算乘除 最后算等于
        strs.ForEach(a =>
        {
            if (a[0] == '+')
            {
                baseValue += CalcString(a.Substring(1));
            }
            else if (a[0] == '-')
            {
                baseValue -= CalcString(a.Substring(1));
            }
            //如果是数字开头 那么就默认是+
            else if (Regex.IsMatch(a[0].ToString(), @"^[0-9]*$"))
            {
                baseValue += CalcString(a);
            }
        });
        strs.ForEach(a =>
        {
            if (a[0] == '*')
            {
                baseValue *= CalcString(a.Substring(1));

            }
            else if (a[0] == '/')
            {
                baseValue /= CalcString(a.Substring(1));
            }
        });
        strs.ForEach(a =>
        {
            if (a[0] == '=')
            {
                //如果填的是 == 的话，那么多截取一位
                baseValue = CalcString(a.Substring(a[1] == '=' ? 2 : 1));
            }
        });
        m_DataCacheDic[property] = baseValue;
        return baseValue;
    }

    /// <summary>
    /// 获取到道具影响后的某个属性的最终值
    /// </summary>
    /// <param name="property">属性名</param>
    /// <param name="baseValue">基础值</param>
    /// <returns></returns>
    public int GetIntValue(string property, int baseValue)
    {
        return (int)GetFloatValue(property, baseValue);
    }

    /// <summary>
    /// 计算字符串表达式，返回float结果
    /// </summary>
    /// <param name="str">字符串表达式</param>
    /// <returns></returns>
    public float CalcString(string str)
    {
        return CSharpExpression.Evaluate<float>(TryCalcString(str));
    }

    /// <summary>
    /// 尝试计算表达式，如果暂时还计算不了就返回调整过后的字符串，比如部分替换过后的字符串  [技能等级*3+层数]  变成  [技能等级*3+5]  
    /// </summary>
    /// <param name="str">字符串表达式</param>
    /// <returns></returns>
    public string TryCalcString(string str,bool info=false)
    {
        // 5 * [@层数] * [@RTN,500,敌人-小怪,小王-小李] + 3   先都将[]内的内容转换成一个具体的数字 然后通过插件计算这个表达式
        string newStr = new string(str);//拷贝一份字符串
        for (int i = 0; i < str.Length; i++)
        {
            //[@xxx] 一个待转换的内容大概是这么一个格式   所以遇到 [@ 那么就将 [@ 和 ] 之间的内容截取出来  然后根据不同情况对他进行转换
            if (str[i] == '[' && str[i + 1] == '@')
            {
                //跳过[@
                i++;
                i++;
                //将 [@ 与 ] 之间的内容截取出来
                string tempStr = "";
                while (str[i] != ']')
                {
                    tempStr += str[i];
                    i++;
                }

                //解析字符串，然后计算得到一个值   
                float value = DefaultDef.Float;//存放计算结果的变量
                string[] param = tempStr.Split(',');
                //这里就是傻瓜式地根据具体情况做处理
                if (info)
                {
                    param = tempStr.Split('*');
                    for (int j = 0; j <param.Length ; j++)
                    {
                        switch (param[j])
                        {
                            case "材料":
                                param[j] = Role.Material.ToString();
                                break;
                            case "当前等级":
                                param[j] = Role.CurLevel.ToString();
                                break;
                            case "当前经验":
                                param[j] = Role.CurExp.ToString();
                                break;
                            case "当前生命值":
                                param[j] = Role.CurHP.ToString();
                                break;
                            case "最大生命值":
                                param[j] = Role.MaxHP.ToString();
                                break;


                            case "最大生命值上限":
                                param[j] = Role.MaxHPUL.ToString();
                                break;
                            case "生命再生":
                                param[j] = Role.HPRegeneration.ToString();
                                break;
                            case "生命窃取":
                                param[j] = Role.LifeSteal.ToString();
                                break;
                            case "伤害":
                                param[j] = Role.Damage.ToString();
                                break;
                            case "近战伤害":
                                param[j] = Role.MeleeDamage.ToString();
                                break;


                            case "远程伤害":
                                param[j] = Role.RangedDamage.ToString();
                                break;
                            case "属性伤害":
                                param[j] = Role.ElementalDamage.ToString();
                                break;
                            case "攻击速度":
                                param[j] = Role.AttackSpeed.ToString();
                                break;
                            case "暴击率":
                                param[j] = Role.CritChance.ToString();
                                break;
                            case "工程学":
                                param[j] = Role.Engineering.ToString();
                                break;



                            case "范围":
                                param[j] = Role.Range.ToString();
                                break;
                            case "护甲":
                                param[j] = Role.Armor.ToString();
                                break;
                            case "闪避":
                                param[j] = Role.DodgeRate.ToString();
                                break;
                            case "速度":
                                param[j] = Role.Speed.ToString();
                                break;
                            case "幸运":
                                param[j] = Role.Luck.ToString();
                                break;


                            case "收获":
                                param[j] = Role.Harvesting.ToString();
                                break;
                            case "使用消耗品恢复":
                                param[j] = Role.ConsumableHeal.ToString();
                                break;
                            case "获得经验":
                                param[j] = Role.XPGain.ToString();
                                break;
                            case "拾取范围":
                                param[j] = Role.PickupRange.ToString();
                                break;
                            case "道具价格":
                                param[j] = Role.ItemsPrice.ToString();
                                break;

                            case "爆炸伤害":
                                param[j] = Role.ExplosionDamage.ToString();
                                break;
                            case "爆炸范围":
                                param[j] = Role.ExplosionSize.ToString();
                                break;
                            case "投射物反弹":
                                param[j] = Role.Bounces.ToString();
                                break;
                            case "投射物贯通":
                                param[j] = Role.Piercing.ToString();
                                break;
                            case "贯通伤害":
                                param[j] = Role.PiercingDamage.ToString();
                                break;

                            case "对头目和精英怪的伤害系数":
                                param[j] = Role.DamageAgainstBosses.ToString();
                                break;
                            case "燃烧速度":
                                param[j] = Role.BurningSpeed.ToString();
                                break;
                            case "燃烧速度比率":
                                param[j] = Role.BurningSpeedRate.ToString();
                                break;
                            case "燃烧蔓延至附近的一名敌人":
                                param[j] = Role.BurnOther.ToString();
                                break;
                            case "击退":
                                param[j] = Role.Knockback.ToString();
                                break;

                            case "材料翻倍概率":
                                param[j] = Role.DoubleMaterialChance.ToString();
                                break;
                            case "商店免费刷新次数":
                                param[j] = Role.FreeRerolls.ToString();
                                break;
                            case "树木":
                                param[j] = Role.Trees.ToString();
                                break;
                            case "树木能被一击毙命":
                                param[j] = Role.TreesKill.ToString();
                                break;
                            case "敌人":
                                param[j] = Role.Enemies.ToString();
                                break;


                            case "敌人速度":
                                param[j] = Role.EnemySpeed.ToString();
                                break;
                            case "敌人伤害":
                                param[j] = Role.EnemyDamageRate.ToString();
                                break;
                            case "特殊敌人":
                                param[j] = Role.SpecialEnemy.ToString();
                                break;
                            case "无法通过其他途径恢复生命值":
                                param[j] = Role.HealNegation.ToString();
                                break;
                            case "仇恨值":
                                param[j] = Role.Hatred.ToString();
                                break;

                            case "击中敌人时能使其降低10速度最高30":
                                param[j] = Role.SpeedCutMax30.ToString();
                                break;
                            case "立即吸收掉落的材料":
                                param[j] = Role.ImmediatelyEatMaterialChance.ToString();
                                break;
                            case "回收道具价格系数":
                                param[j] = Role.ItemsRecyclePrice.ToString();
                                break;
                            case "免疫伤害次数":
                                param[j] = Role.InjuryAvoidanceTimes.ToString();
                                break;
                            case "随机数":
                                param[j] = Role.RandomNumber.ToString();
                                break;

                            case "不同武器数量":
                                param[j] = Player.Instance.GetDifferentWeaponCount().ToString();
                                break;
                            case "持有武器数量":
                                param[j] = Player.Instance.GetWeaponCount().ToString();
                                break;
                            case "敌人数量":
                                param[j] = BattleManager.Instance.Enemys.Count.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                    value = param[0].ToFloat();
                    for (int j = 1; j < param.Length; j++)
                    {
                        value *= param[j].ToFloat();
                        value = Mathf.RoundToInt(value);
                    }
                }
                else
                {
                    switch (param[0])
                    {
                        case "材料":
                            value = Role.Material;
                            break;
                        case "当前等级":
                            value = Role.CurLevel;
                            break;
                        case "当前经验":
                            value = Role.CurExp;
                            break;
                        case "当前生命值":
                            value = Role.CurHP;
                            break;
                        case "最大生命值":
                            value = Role.MaxHP;
                            break;


                        case "最大生命值上限":
                            value = Role.MaxHPUL;
                            break;
                        case "生命再生":
                            value = Role.HPRegeneration;
                            break;
                        case "生命窃取":
                            value = Role.LifeSteal;
                            break;
                        case "伤害":
                            value = Role.Damage;
                            break;
                        case "近战伤害":
                            value = Role.MeleeDamage;
                            break;


                        case "远程伤害":
                            value = Role.RangedDamage;
                            break;
                        case "属性伤害":
                            value = Role.ElementalDamage;
                            break;
                        case "攻击速度":
                            value = Role.AttackSpeed;
                            break;
                        case "暴击率":
                            value = Role.CritChance;
                            break;
                        case "工程学":
                            value = Role.Engineering;
                            break;



                        case "范围":
                            value = Role.Range;
                            break;
                        case "护甲":
                            value = Role.Armor;
                            break;
                        case "闪避":
                            value = Role.DodgeRate;
                            break;
                        case "速度":
                            value = Role.Speed;
                            break;
                        case "幸运":
                            value = Role.Luck;
                            break;


                        case "收获":
                            value = Role.Harvesting;
                            break;
                        case "使用消耗品恢复":
                            value = Role.ConsumableHeal;
                            break;
                        case "获得经验":
                            value = Role.XPGain;
                            break;
                        case "拾取范围":
                            value = Role.PickupRange;
                            break;
                        case "道具价格":
                            value = Role.ItemsPrice;
                            break;

                        case "爆炸伤害":
                            value = Role.ExplosionDamage;
                            break;
                        case "爆炸范围":
                            value = Role.ExplosionSize;
                            break;
                        case "投射物反弹":
                            value = Role.Bounces;
                            break;
                        case "投射物贯通":
                            value = Role.Piercing;
                            break;
                        case "贯通伤害":
                            value = Role.PiercingDamage;
                            break;

                        case "对头目和精英怪的伤害系数":
                            value = Role.DamageAgainstBosses;
                            break;
                        case "燃烧速度":
                            value = Role.BurningSpeed;
                            break;
                        case "燃烧速度比率":
                            value = Role.BurningSpeedRate;
                            break;
                        case "燃烧蔓延至附近的一名敌人":
                            value = Role.BurnOther;
                            break;
                        case "击退":
                            value = Role.Knockback;
                            break;

                        case "材料翻倍概率":
                            value = Role.DoubleMaterialChance;
                            break;
                        case "商店免费刷新次数":
                            value = Role.FreeRerolls;
                            break;
                        case "树木":
                            value = Role.Trees;
                            break;
                        case "树木能被一击毙命":
                            value = Role.TreesKill;
                            break;
                        case "敌人":
                            value = Role.Enemies;
                            break;


                        case "敌人速度":
                            value = Role.EnemySpeed;
                            break;
                        case "敌人伤害":
                            value = Role.EnemyDamageRate;
                            break;
                        case "特殊敌人":
                            value = Role.SpecialEnemy;
                            break;
                        case "无法通过其他途径恢复生命值":
                            value = Role.HealNegation;
                            break;
                        case "仇恨值":
                            value = Role.Hatred;
                            break;


                        case "击中敌人时能使其降低10速度最高30":
                            value = Role.SpeedCutMax30;
                            break;
                        case "立即吸收掉落的材料":
                            value = Role.ImmediatelyEatMaterialChance;
                            break;
                        case "回收道具价格系数":
                            value = Role.ItemsRecyclePrice;
                            break;
                        case "免疫伤害次数":
                            value = Role.InjuryAvoidanceTimes;
                            break;
                        case "随机数":
                            value = Role.RandomNumber;
                            break;

                        case "不同武器数量":
                            value = Player.Instance.GetDifferentWeaponCount();
                            break;
                        case "持有武器数量":
                            value = Player.Instance.GetWeaponCount();
                            break;
                        case "敌人数量":
                            value = BattleManager.Instance.Enemys.Count;
                            break;
                    }
                }
                
                //如果上面对value进行赋值了，那么就将结果替换到原字符串里
                if (value != DefaultDef.Float)
                {
                    newStr = newStr.Replace("@" + tempStr, value.ToString());
                }
            }
        }

        //试着去将表达式计算成一个数字 如果计算失败，那么就先将字符串返回
        try
        {
            return CSharpExpression.Evaluate<float>(newStr.Replace("[", "").Replace("]", "")).ToString();
        }
        catch (System.Exception)
        {
            return newStr;
            throw;
        }
    }
    #endregion

    private void Update()
    {
        if (NeedRebuild)
        {
            RebuildDataStack();
        }
    }

    private void OnDestroy()
    {
        //清除全部的道具
        m_ItemDic.ForEach(a => 
        {
            a.Value.ForEach(b => b.OnRemove());
        });
        m_ItemDic.Clear();
        m_DataDic.Clear();
    }
}