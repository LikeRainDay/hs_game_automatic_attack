using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Weapon : MonoBehaviour
{
    public void Init(Sprite sprite, float attackSpeed, float range, float baseDamage, float baseKnockback,float baseCrit,float critMultiply,RowCfgWeapon row)
    {
        Row = row;
        GetComponent<SpriteRenderer>().sprite = sprite;
        m_Collider2D = GetComponent<Collider2D>();
        m_Parent = transform.parent;
        transform.parent = null;
        AttackInterval = attackSpeed;
        m_BaseAttackRange = range / 35;
        GetComponentInChildren<ColliderData>().BaseDamage = baseDamage;
        GetComponentInChildren<ColliderData>().BaseKnockback = baseKnockback;
        GetComponentInChildren<ColliderData>().BaseCrit = baseCrit;
        GetComponentInChildren<ColliderData>().CritMultiply = critMultiply;
    }
    public float Smooth1;
    public float Smooth2;
    public float Smooth3;

    public GameObject BulletPrefab;
    public float AttackInterval = 1;
    private float m_CurInterval;
    private float m_BaseAttackRange;
    public float AttackRange { get => m_BaseAttackRange + Player.Instance.Range / 70; }
    private Collider2D m_Collider2D;
    private RowCfgWeapon Row;
    private Transform m_Parent;
    public Transform ShootPoint;
    private bool m_Attack;
    private void Start()
    {
        EventMgr.RegisterEvent(EventName.Clear, Clear);
        EventMgr.RegisterEvent(EventName.NextWave, NextWave);
    }

    private object Clear(object[] arg)
    {
        Destroy(gameObject);
        return null;
    }

    private object NextWave(object[] arg)
    {
        transform.position = m_Parent.position;
        transform.eulerAngles = Vector3.zero;
        transform.parent = m_Parent;
        return null;
    }

    private void Update()
    {
       
        m_CurInterval -= Time.deltaTime;

        //朝向范围内的目标
        Transform target = null;
        float distance = 9999;
        BattleManager.Instance.Enemys.ForEach(a =>
        {
            float tempDistance = Vector2.Distance(a.transform.position, transform.position);
            if (a != Player.Instance && tempDistance < AttackRange && tempDistance < distance)
            {
                distance = tempDistance;
                target = a.transform;
            }
        });
        if (target != null)
        {
            transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, target.position - transform.position));
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }

        //如果CD好了 就攻击
        if (m_CurInterval <= 0 && target != null&& BattleManager.Instance.Battle)
        {
            m_CurInterval = AttackInterval*Player.Instance.AttackSpeed;
            Attack(target);
        }

        if (!m_Attack)
        {           
            if (Vector2.Distance(transform.position, m_Parent.position) < 0.5f)
            {
                transform.SetEulerAnglesY(Player.Instance.transform.eulerAngles.y);
                transform.position = Vector3.Lerp(transform.position, m_Parent.position, Time.deltaTime * 1 / Smooth3);
            }
            else
            {
                transform.position += (m_Parent.position - transform.position).normalized * AttackRange/Smooth2 * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="target"></param>
    public async void Attack(Transform target)
    {
        //近战的攻击方式
        if (Row.melee==1)
        {
            m_Collider2D.enabled = true;
            m_Attack = true;
            transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, target.position - transform.position));
            transform.DOMove(transform.position + (target.position - transform.position).normalized * AttackRange, Smooth3 * AttackInterval * Player.Instance.AttackSpeed / AttackRange).onComplete += () =>
            {
                TimeMgr.Timer.AddTimeTask(a =>
                {
                    m_Collider2D.enabled = false;
                    m_Attack = false;
                }, 50);

            };
        }
        //远程的攻击方式
        else
        {
            //创建一枚子弹
            var prefab = await AssetMgr.LoadAssetAsync<GameObject>(DataMgr.AssetPathDic[Row.bullet]);
            GameObject bullet = Instantiate(prefab);
            //设置字典位置 角度 和初始化子弹
            bullet.transform.position = ShootPoint.position;
            bullet.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, target.position - transform.position));
        }
        
       
    }

    private void OnDestroy()
    {
        EventMgr.UnRegisterEvent(EventName.Clear, Clear);
        EventMgr.UnRegisterEvent(EventName.NextWave, NextWave);
    }
}
