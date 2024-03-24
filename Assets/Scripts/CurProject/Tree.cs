using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 树木
/// </summary>
public class  Tree : Enemy
{
    [FieldName("死亡特效")]
    public GameObject DieEffect;
    [FieldName("波次结束销毁特效")]
    public GameObject DieEffect1;

    private bool m_Die;
    private Animator m_Anim;
    protected override void Start()
    {
        base.Start();
        m_Anim = GetComponent<Animator>();
        //每4波，血量翻一倍(瞎填的)
        Hp *= (1 + Player.Instance.CurWave / 4);
        EventMgr.RegisterEvent(EventName.BattleFinish, Des);
    }
    private object Des(object[] arg)
    {
        Die();
        return null;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventMgr.UnRegisterEvent(EventName.BattleFinish, Des);
    }
    protected override void Update() { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果撞击到了 玩家的伤害碰撞器 就扣血
        if (collision.CompareTag(ColliderTagDef.PlayerAttack))
        {
            ColliderData data = collision.GetComponent<ColliderData>();
            if (!data)
            {
                data = collision.GetComponentInParent<ColliderData>();
            }
            //扣血以及屏幕震动
            Hit(data.Damage, data);
            CameraShakeMgr.SetVibration(EnumCameraShakeData.A1);
        }
    }

    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="damage">伤害</param>
    /// <param name="data">碰撞器数据</param>
    public override void Hit(float damage, ColliderData data)
    {
        //受击音效
        AudioMgr.PlaySound(HitClip);

        //计算暴击，如果暴击就增伤
        if (data.Crit.Random())
        {
            damage *= data.CritMultiply;
            Hp -= damage * Player.Instance.Damage;
            UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), "" + Mathf.RoundToInt(damage), new Color(1, 0.5f, 0));
        }
        else
        {
            Hp -= damage * Player.Instance.Damage;
            UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), "" + Mathf.RoundToInt(damage), Color.white);
        }

        //血量小于0或者玩家可以一击必杀树木就死亡 否则就播放受击动画
        if (Hp < 0 || Player.Instance.TreesKill > 0)
        {
            Die();
        }
        else
        {
            m_Anim.Play("敌人受击", 1);
        }
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    public override void Die()
    {
        if (m_Die) return;
        m_Die = true;
        //如果是波次结束时销毁的，就创建另一个死亡特效
        if (BattleManager.Instance.Battle)
        {
            ToolFun.ShowEffect(DieEffect, transform.position, 360.GetRandom(), 1);
        }
        else
        {
            ToolFun.ShowEffect(DieEffect1, transform.position, 360.GetRandom(), 1);
        }
        //如果不是波次结束时销毁的，就创建一个奖励
        if (BattleManager.Instance.Battle)
        {
            ToolFun.ShowEffect(RewardPrefabs.RandomFromList(), transform.position, 0, 0);
        }
        //缩放消失
        transform.DOScale(Vector3.zero, 0.3f);
        Destroy(gameObject, 0.35f);
    }

}
