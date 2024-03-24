using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 极简玩家子弹逻辑
/// </summary>
public class PlayerBullet : MonoBehaviour
{
    [FieldName("生命周期")]
    public float LifeTime;
    private float m_CurLifeTime;//剩余生命周期
    /// <summary>
    /// 移动速度
    /// </summary>
    private float m_Speed;
    /// <summary>
    /// 反弹次数
    /// </summary>
    private int m_Bounces;
    /// <summary>
    /// 贯穿次数
    /// </summary>
    private int m_Piercing;
    /// <summary>
    /// 已贯穿次数
    /// </summary>
    private int m_PiercingTimes;
    /// <summary>
    /// 武器数据
    /// </summary>
    private RowCfgWeapon m_Row;
    /// <summary>
    /// 碰撞器数据
    /// </summary>
    private ColliderData m_Data;
    private void Start()
    {
        m_CurLifeTime = LifeTime;
        EventMgr.RegisterEvent(EventName.WaveCompleted, WaveCompleted);
    }
    private object WaveCompleted(object[] arg)
    {
        Destroy(gameObject, 0.1f);
        return null;
    }
    private void OnDestroy()
    {
        EventMgr.UnRegisterEvent(EventName.WaveCompleted, WaveCompleted);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="rowCfg">武器数据</param>
    public void Init(RowCfgWeapon rowCfg)
    {
        m_Row = rowCfg;
        m_Data = GetComponent<ColliderData>();
        //设置伤害 击退 暴击率 暴击倍率 
        m_Data.Row = rowCfg;
        m_Data.DamageRate = 1;
        m_Data.BaseDamage = m_Row.damageBase;
        m_Data.BaseKnockback = m_Row.knockback;
        m_Data.BaseCrit = m_Row.critChance;
        m_Data.CritMultiply = m_Row.critMultiply;
       
        m_Speed = m_Row.bulletSpeed;
        m_PiercingTimes = 0;
        m_Bounces = m_Row.bounces + (int)Player.Instance.Bounces;
        m_Piercing = m_Row.piercing + (int)Player.Instance.Piercing;
    }

    /// <summary>
    /// 每次有敌人被这个子弹攻击了就会调用此函数
    /// </summary>
    public void Hit()
    {
        //如果还剩贯穿次数 就减去一次贯穿次数 增加一次已贯穿次数 然后修正伤害倍率
        if (m_Piercing > 0)
        {
            m_Piercing--;
            m_PiercingTimes++;
            //修正伤害倍率
            m_Data.DamageRate = Mathf.Clamp(1 + m_Row.piercingDamage * m_PiercingTimes + Player.Instance.PiercingDamage, 0, 9999999);

        }
        //如果还剩反弹次数 就反弹次数减一 然后朝向任意地方单位
        else if (m_Bounces > 0)
        {
            m_Bounces--;
            //朝向一个随机敌人
            Enemy enemy = BattleManager.Instance.Enemys.RandomFromList(false);
            transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, enemy.transform.position - transform.position));
            //修正伤害
            m_Data.DamageRate = 1;
            m_PiercingTimes = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //时间到了就自动销毁
        m_CurLifeTime -= Time.deltaTime;
        if (m_CurLifeTime <= 0 || !BattleManager.Instance.Battle)
        {
            Destroy(gameObject);
        }
        //朝自身前方匀速移动
        transform.Translate(transform.up * Time.deltaTime * m_Speed, Space.World);
    }

}