using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 奖励品：材料、水果、宝箱
/// </summary>
public class Reward : MonoBehaviour
{
    [FieldName("是水果")]
    public bool Fruit;
    [FieldName("是宝箱")]
    public bool Box;
    [FieldName("增加多少材料")]
    public int AddMaterialCount;
    [EnumName("吃掉的音效")]
    public EnumAudioClip EatClip;
    [FieldName("吃掉的特效")]
    public GameObject Effect;
    [Header("可随机贴图数组")]
    public List<Sprite> Sprites;
    private bool m_Pickup;//是否已经在吸收

    private void Start()
    {
        EventMgr.RegisterEvent(EventName.Clear, Clear);
        //随机一个形象
        GetComponentInChildren<SpriteRenderer>().sprite = Sprites.RandomFromList();
        //如果不是箱子和水果，就随机一个角度
        if (!Box && !Fruit)
        {
            transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        }

        //判断是否需要立即吸收
        if (Player.Instance.ImmediatelyEatMaterialChance.Random())
        {
            Pickup();
        }
    }

    private object Clear(object[] arg)
    {
        Destroy(gameObject);
        return null;
    }

    private void Update()
    {
        //战斗结束了，吸收掉还未被吸收的奖励
        if (!BattleManager.Instance.Battle && !m_Pickup)
        {
            Pickup();
            return;
        }

        //如果在拾取范围内，且还未被拾取 
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) < Player.Instance.PickRange && !m_Pickup)
        {
            Pickup();
            return;
        }

        //如果离主角很近，那么就吃掉
        if (Vector2.Distance(transform.position, Player.Instance.transform.position) < 1)
        {
            Eat();
        }
    }

    /// <summary>
    /// 吸收
    /// </summary>
    private void Pickup()
    {
        m_Pickup = true;
        //如果是箱子的话就直接吃掉
        if (Box)
        {
            Eat();
            return;
        }
        //如果是材料或者水果就开启追踪脚本的追踪，让其自动靠近主角直到被吃掉
        TransformFollow follow = GetComponent<TransformFollow>();
        follow.Enable = true;
        follow.Target = Player.Instance.transform;
    }

    /// <summary>
    /// 吃掉
    /// </summary>
    private void Eat()
    {
        //材料需要判断一下是否需要双倍
        float mat = Player.Instance.DoubleMaterialChance.Random() ? AddMaterialCount * 2 : AddMaterialCount;
        //如果战斗还未结束
        if (BattleManager.Instance.Battle)
        {
            //如果是水果就调用吃掉消耗品的事件，如果是材料就调用吃掉材料的事件
            if (Fruit)
            {
                EventMgr.ExecuteEvent(EventName.PickConsumables, new object[] { OwnerType.主角 });
            }
            else if (!Box)
            {
                EventMgr.ExecuteEvent(EventName.PickMaterial, new object[] { OwnerType.主角 });

            }
            //增加材料和经验
            Player.Instance.ExpChange(AddMaterialCount);
            //如果有种子存在 就扣除种子数量，额外增加一倍材料
            if (Player.Instance.CurSeed > 0)
            {
                if (Player.Instance.CurSeed >= mat)
                {
                    Player.Instance.CurSeed -= mat;
                    mat *= 2;
                }
                else
                {
                    mat += Player.Instance.CurSeed;
                    Player.Instance.CurSeed = 0;
                }
            }
            Player.Instance.MaterialChange(mat);
        }
        //战斗结束了，就改为增加种子
        else
        {
            Player.Instance.SeedChange(mat);
        }

        //创建吃掉的特效
        ToolFun.ShowEffect(Effect, transform.position, 360.GetRandom(), 1);
        //如果增加的材料大于0，就显示一个绿色瓢字
        if (mat != 0)
        {
            UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), Mathf.RoundToInt(mat).ToString(), Color.green);
        }
        //如果是箱子 
        if (Box)
        {
            //调用获得箱子的事件 
            EventMgr.ExecuteEvent(EventName.PickBox, new object[] { OwnerType.主角 });
            //奖励数组里随机增加一个道具，然后刷新显示
            Player.Instance.RewardIds.Add(DataMgr.RandomGetItemID(false));
            BattlePage.Instance.UpdateTip();
        }
        //如果是水果就恢复血量
        if (Fruit) Player.Instance.HpChange(Player.Instance.ConsumableHeal);
        //播放吸收音效
        AudioMgr.PlaySound(EatClip);
        //销毁对象
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventMgr.UnRegisterEvent(EventName.Clear, Clear);
    }

}
