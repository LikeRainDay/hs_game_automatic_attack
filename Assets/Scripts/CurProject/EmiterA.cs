using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 武器极简发射器逻辑
/// </summary>
public class EmiterA : MonoBehaviour
{
    [FieldName("子弹预制体")]
    public GameObject Bullet;
    [FieldName("射击间隔")]
    public float ShootInterval;
    private float m_CurShootCD;//剩余发射CD

    /// <summary>
    /// 射击
    /// </summary>
    private void Shoot()
    {
        //创建子弹  简单设置位置和角度
        GameObject bullet = Instantiate(Bullet);
        bullet.transform.position = transform.position;
        bullet.transform.eulerAngles = transform.eulerAngles;
    }

    private void Update()
    {
        //每隔一个间隔射击一次
        m_CurShootCD -= Time.deltaTime;
        if (m_CurShootCD < 0)
        {
            Shoot();
            m_CurShootCD = ShootInterval;
        }
    }

}
