using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
/// <summary>
/// 敌人
/// </summary>
public class Enemy : MonoBehaviour
{
    [FieldName("精英")]
    public bool Elite;
    [EnumName("受击音效")]
    public EnumAudioClip HitClip;
    [FieldName("死亡特效")]
    public GameObject HitEffect;
    [FieldName("死亡特效1")]
    public GameObject HitEffect1;
    [FieldName("死亡掉落奖励概率")]
    public float RewardP;
    [FieldName("奖励预制体数组(随机抽取一个)")]
    public List<GameObject> RewardPrefabs;
    [FieldName("移动速度")]
    public float Speed;
    public float SpeedRate
    {
        //速度倍率=0.7 + 0.3(能被丑牙影响的最大倍率) + 其他道具加成
        get => m_SpeedRate + m_UglyTeethSpeedRate + Player.Instance.EnemySpeed;
    }
    private float m_SpeedRate = 0.7f;
    private float m_UglyTeethSpeedRate = 0.3f;//丑牙能影响的速度上线
    [FieldName("血量")]
    public float Hp;
    [FieldName("伤害")]
    public float Damage;
    [FieldName("质量，质量越大击退作用越不明显")]
    public float Mass = 1;

    [Header("会射击的敌人")]
    public bool Shoot;
    [FieldName("射击用时")]
    public float ShootTime;
    [FieldName("射击蓄力用时")]
    public float ShootXuliTime;
    [FieldName("可射击距离范围")]
    public Vector2 ShootRange;
    [FieldName("射击概率")]
    public float ShootP;
    [FieldName("射击CD")]
    public float ShootCD;
    [FieldName("射击点")]
    public Transform ShootPoint;
    [FieldName("子弹预制体")]
    public GameObject BulletPrefab;
    [FieldName("类型一:单颗子弹")]
    public bool Type1;
    [FieldName("类型二:两颗条形子弹")]
    public bool Type2;
    [FieldName("两颗子弹的夹角")]
    public float Angle;
    [FieldName("类型三:范围内随机点位生成一批子弹")]
    public bool Type3;
    [FieldName("随机范围半径")]
    public float Radius;
    [FieldName("子弹数量")]
    public float BulletCount;
    private float m_CurShootCD;//当前射击CD
    private bool m_Shoot;//射击中

    [Header("会冲击的敌人")]
    public bool Dash;
    [FieldName("冲击距离")]
    public float DashDistance;
    [FieldName("冲击用时")]
    public float DashTime;
    [FieldName("冲击蓄力用时")]
    public float DashXuliTime;
    [FieldName("可冲击距离范围")]
    public Vector2 DashRange;
    [FieldName("冲击概率")]
    public float DashP;
    [FieldName("冲击CD")]
    public float DashCD;
    private float m_CurDashCD;//当前冲击CD
    private bool m_Dash;//冲击中
    private Tween m_DashTween;//冲击动画


    [HideInInspector] public Vector2 CurForce;
    private float m_TurnCD;//转向CD
    private Vector3 m_V;//速度
    private float m_KnockbackSmooth = 0.5f;
    private bool m_Die;
    private int m_Dir = 1;
    private Animator m_Anim;
    protected virtual void Start()
    {
        EventMgr.RegisterEvent(EventName.TakeDamage, TakeDamage);
        m_Anim = GetComponent<Animator>();
        //加入敌人数组
        BattleManager.Instance.AddEnemy(this);
        //设置数据
        Speed *= (1 + Player.Instance.CurWave / 100);
        Hp *= (1 + Player.Instance.CurWave / 3);
        Damage += Player.Instance.CurWave;
        GetComponentsInChildren<ColliderData>().ForEach(a =>
        {
            a.BaseDamage = Damage;
        });
    }

    public object TakeDamage(object[] arg)
    {
        float damage= Player.Instance.ItemContainer.CalcString(arg[1].ToString());
        Hp -= damage;
        print("对随机一名敌人造成伤害:"+damage);
        if (Hp < 0)
        {
            EventMgr.ExecuteEvent(EventName.EnemyDie, new object[] { OwnerType.主角 });
            Die();
        }
        return null;
    }

    protected virtual void Update()
    {
        if (!BattleManager.Instance.Battle)
        {
            Die();
            return;
        }
        m_CurDashCD -= Time.deltaTime;
        m_CurShootCD -= Time.deltaTime;
        //冲击中不执行后续逻辑
        if (Dash && m_Dash)
        {
            transform.SetEulerAnglesZ(0);
            return;
        }
        if (Shoot && m_Shoot)
        {
            transform.SetEulerAnglesZ(0);
            return;
        }

        if (Shoot)
        {
            if (Vector2.Distance(transform.position, Player.Instance.transform.position) > ShootRange.x && Vector2.Distance(transform.position, Player.Instance.transform.position) < ShootRange.y)
            {
                StartCoroutine(ShootFun());
            }
            else if (Vector2.Distance(transform.position, Player.Instance.transform.position) < ShootRange.x)
            {
                //远离主角
                m_Dir = -1;
            }
            //移动向主角
            else
            {
                m_Dir = 1;
            }

        }

        if (Dash)
        {
            if (Vector2.Distance(transform.position, Player.Instance.transform.position) > DashRange.x && Vector2.Distance(transform.position, Player.Instance.transform.position) < DashRange.y)
            {
                StartCoroutine(DashFun());
            }
        }

        //如果转向CD冷却好了，就将速度方向修改为主角所在方向，如果想时时追踪主角将转向CD修改为0即可
        m_TurnCD -= Time.deltaTime;
        if (m_TurnCD <= 0)
        {
            m_V = Player.Instance.transform.position - transform.position;
            m_V *= m_Dir;
            m_TurnCD = 0.5f;
        }
        Vector2 delta = Vector2.zero;
        if (CurForce.magnitude > 0.2)
        {
            delta = CurForce - Vector2.Lerp(CurForce, Vector2.zero, Time.deltaTime / m_KnockbackSmooth);
            CurForce -= delta;
        }
        else
        {
            CurForce = Vector2.zero;
        }

        //位移 以及 调整朝向
        m_V = m_V.normalized * Speed * SpeedRate * Time.deltaTime;
        Vector2 v = m_V + (Vector3)delta;
        transform.Translate(v, Space.World);
        transform.eulerAngles = new Vector3(0, m_V.x == 0 ? transform.eulerAngles.y : m_V.x > 0 ? 0 : 180, 0);

    }

    protected virtual void LateUpdate()
    {
        //根据Y轴的值来设置Z轴的值，让越下方的角色的层级越高
        transform.SetPositionZ(transform.position.y * 0.001f);
    }

    protected virtual void OnDestroy()
    {
        //从敌人数组里移除
        BattleManager.Instance.RemoveEnemy(this);
        EventMgr.UnRegisterEvent(EventName.TakeDamage, TakeDamage);
    }

    /// <summary>
    /// 伤害
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Hit(float damage, ColliderData data)
    {
        //如果玩家有丑牙这个道具 那么敌人每次受击都会减少10%速度，上限30%
        if (Player.Instance.SpeedCutMax30 == 1 && m_UglyTeethSpeedRate > 0) m_UglyTeethSpeedRate -= 0.1f;
        Player.Instance.Beattacked = this;
        EventMgr.ExecuteEvent(EventName.AttackEnemy, new object[] { OwnerType.主角 });        
        //受击音效 特效 动画
        AudioMgr.PlaySound(HitClip);
        //扣除血量 血量小于0就死亡
        //如果是精英就让伤害乘对头目和精英怪的伤害系数
        if (Elite)
        {
            damage *= Player.Instance.DamageAgainstBosses;
        }
        //计算暴击 如果暴击就增伤
        if (data.Crit.Random())
        {
            damage *= data.CritMultiply;
            Hp -= damage * Player.Instance.Damage;
            UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), "" + Mathf.RoundToInt(damage), new Color(1, 0.5f, 0));
            if (Hp < 0)
            {
                EventMgr.ExecuteEvent(EventName.CritKillEnemy, new object[] { OwnerType.主角 });
            }
        }
        else
        {
            Hp -= damage * Player.Instance.Damage;
            UIMgr.Instance.ShowNumbTip(transform.position + new Vector3(0, 1, 0), "" + Mathf.RoundToInt(damage), Color.white);
        }

        //生命窃取
        if (Player.Instance.LifeSteal.Random() && Player.Instance.LifeStealMax > 0)
        {
            Player.Instance.LifeStealMax--;
            Player.Instance.HpChange(1);
        }

        if (Hp < 0)
        {
            EventMgr.ExecuteEvent(EventName.EnemyDie, new object[] { OwnerType.主角 });
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
    public virtual void Die()
    {
        if (m_Die) return;
        m_Die = true;
        GetComponentsInChildren<Collider2D>().ForEach(a => a.enabled = false);
        m_DashTween.Kill();
        if (BattleManager.Instance.Battle)
        {
            ToolFun.ShowEffect(HitEffect, transform.position, 360.GetRandom(), 1);
        }
        else
        {
            ToolFun.ShowEffect(HitEffect1, transform.position, 360.GetRandom(), 1);
        }
        //死亡的时候 概率掉落一个奖励物品
        if (RewardP.Random() && BattleManager.Instance.Battle)
        {
            ToolFun.ShowEffect(RewardPrefabs.RandomFromList(), transform.position, 0, 0);
        }
        transform.DOScale(Vector3.zero, 0.3f);
        m_Anim.Play("敌人死亡", 1);
        Destroy(gameObject, 0.35f);
    }

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
            if (data.Knockback != 0)
            {
                Vector3 dir = transform.position - collision.transform.position;
                AddForce(dir.normalized * data.Knockback);
            }
            Hit(data.Damage, data);
            CameraShakeMgr.SetVibration(EnumCameraShakeData.A1);
        }
    }

    public void AddForce(Vector2 force)
    {
        CurForce += force / Mass;
    }

    public IEnumerator DashFun()
    {
        if (m_CurDashCD > 0) yield break;
        m_CurDashCD = DashCD;
        if (!DashP.Random())
        {
            yield break;
        }
        m_Dash = true;
        Vector3 dir = Player.Instance.transform.position - transform.position;
        //设置朝向
        transform.eulerAngles = new Vector3(0, dir.x > 0 ? 0 : 180, 0);
        //蓄力
        m_Anim.Play("蓄力");
        yield return new WaitForSeconds(DashXuliTime);
        m_Anim.Play("冲击");
        //设置动画
        m_DashTween = transform.DOMove(transform.position + dir.normalized * DashDistance, DashTime);
        m_DashTween.onComplete += () =>
        {
            m_Dash = false;
            m_Anim.Play("Move");
        };
    }
    public IEnumerator ShootFun()
    {
        if (m_CurShootCD > 0) yield break;
        m_CurShootCD = ShootCD;
        if (!ShootP.Random())
        {
            yield break;
        }
        m_Shoot = true;
        Vector3 dir = Player.Instance.transform.position - transform.position;

        //设置朝向
        transform.eulerAngles = new Vector3(0, dir.x > 0 ? 180 : 0, 0);
        //蓄力
        m_Anim.Play("蓄力");
        yield return new WaitForSeconds(ShootXuliTime);
        //临时逻辑 创建个子弹
        if (Type1)
        {
            GameObject bullet = Instantiate(BulletPrefab);
            bullet.transform.position = ShootPoint.position;
            bullet.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, Player.Instance.transform.position - ShootPoint.position));
        }
        else if (Type2)
        {
            GameObject bullet = Instantiate(BulletPrefab);
            bullet.transform.position = ShootPoint.position;
            bullet.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, Player.Instance.transform.position - ShootPoint.position) - Angle / 2);

            bullet = Instantiate(BulletPrefab);
            bullet.transform.position = ShootPoint.position;
            bullet.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, Player.Instance.transform.position - ShootPoint.position) + Angle / 2);
        }
        else if (Type3)
        {
            for (int i = 0; i < BulletCount; i++)
            {
                //具体逻辑交给子弹自身
                GameObject bullet = Instantiate(BulletPrefab);
                bullet.transform.position = transform.position + (Vector3)(360.GetRandom().GetUnitVector2ByAngle() * Radius);
            }

        }
        m_Anim.Play("Move");
        yield return new WaitForSeconds(ShootTime);
        m_Shoot = false;

    }
}
