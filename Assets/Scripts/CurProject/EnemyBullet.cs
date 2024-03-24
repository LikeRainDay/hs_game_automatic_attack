using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 极简敌人子弹逻辑
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    [FieldName("移动速度")]
    public float Speed;
    [FieldName("生命周期")]
    public float LifeTime;
    private float m_CurLifeTime;//剩余生命周期

    private void Start()
    {
        m_CurLifeTime = LifeTime;
        //每波增加0.5点伤害
        GetComponent<ColliderData>().BaseDamage += Player.Instance.CurWave / 2;
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

    private void Update()
    {
        //时间到了就自动销毁
        m_CurLifeTime -= Time.deltaTime;
        if (m_CurLifeTime <= 0 || !BattleManager.Instance.Battle)
        {
            Destroy(gameObject);
        }
        transform.Translate(transform.up * Time.deltaTime * Speed, Space.World);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ColliderTagDef.Player))
        {
            Destroy(gameObject, 0.02f);
        }
    }

}


