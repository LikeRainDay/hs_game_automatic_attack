using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 游戏内各种物体的基类
/// </summary>
public class Thing : SoraMono
{
    [FieldName("中心点")]
    public Transform Center;
    [FieldName("两帧之间的位置间隔")]
    public Vector3 DeltaPos;
    [FieldName("上一帧的位置")]
    public Vector3 OldPos;
    [EnumName("阵营")]
    public List<Camp> Camps;
    [EnumName("标签")]
    [SerializeField] private List<ThingTag> m_ThingTags;
    private List<string> m_Tags;//因为会解析表格里面的字符串数据来使用，因此代码内部字符串会比枚举泛用一些，所以这里将配置的枚举数组转换成字符串数组来使用
    public List<string> Tags
    {
        get
        {
            //如果数组还不存在 就构建一个数组
            if (m_Tags == null || m_Tags.Count == 0)
            {
                m_Tags = new List<string>();
                m_ThingTags.ForEach(a =>
                {
                    m_Tags.Add(a.ToString());
                });
            }
            return m_Tags;
        }
    }

    /// <summary>
    /// 实例ID，用于比对是否是事件等的作用目标，以及方便外部通过指定一个实例ID从战场管理器找到目标对象
    /// </summary>
    [HideInInspector] public int InstanceID;

    /// <summary>
    /// 各种类型点位的缓存字典，键是点位类型枚举，值这个类型点位的数组
    /// </summary>
    private Dictionary<PointType, List<PointBase>> m_PointCacheDic = new Dictionary<PointType, List<PointBase>>();

    protected virtual void Start()
    {
        //记录一下当前对象的实例ID #为什么要取绝对值?
        InstanceID = Mathf.Abs(GetInstanceID());
    }

    /// <summary>
    /// 获取这个物体上目标类型的一个未被占用的点位
    /// </summary>
    /// <param name="pointType">点位类型(特效点，技能点，创造物生成点...)</param>
    /// <param name="getPointMode">获取点位的方式，随机获取 or 获取离给定值最近的点 or 获取离给定值最远的点</param>
    /// <param name="position">诸如获取离给定值最近的点这类情况需要给一个参考坐标</param>
    /// <returns></returns>
    public PointBase GetPoint(PointType pointType, GetPointMode getPointMode = GetPointMode.Random, Vector3 position = new Vector3())
    {
        PointBase pointBase = null;
        //如果字典里还没目标点位的缓存 就缓存一下目标点位的数据
        if (!m_PointCacheDic.ContainsKey(pointType))
        {
            switch (pointType)
            {
                case PointType.CreatePoint:
                    m_PointCacheDic[pointType] = new List<PointBase>(transform.GetComponentsInChildren<CreatePoint>().ToList());
                    break;
                default:
                    break;
            }
        }

        //如果没有点位数据 就直接返回空
        List<PointBase> points = m_PointCacheDic[pointType];
        if (points.Count == 0) return pointBase;
        //如果已经没有空点位了 也最直接返回空
        bool noEmpty = true;
        foreach (var item in points)
        {
            if (item.IsEmpty)
            {
                noEmpty = false;
                break;
            }
        }
        if (noEmpty)
        {
            return pointBase;
        }

        //根据需求 从数组里拿到一个点位
        switch (getPointMode)
        {
            case GetPointMode.Random:
                int index = 0;
                do
                {
                    pointBase = points[UnityEngine.Random.Range(0, points.Count)];
                    index++;
                } while (!pointBase.IsEmpty && index <= 1000);//如果不是空的 且随机还不到5000次就继续随机 如果超过1千次了可能因为配置问题导致一直没能有合法值了 为了防止死循环这里返回一个空点位
                if (index >= 1000)
                {
                    Debug.LogError($"{name}获取点位时出现死循环。点位类型:{pointType}");
                    return null;
                }
                break;
            case GetPointMode.Nearby:
                float distance = 99999999;
                points.ForEach(a =>
                {
                    if (a.IsEmpty && Vector3.Distance(a.transform.position, position) < distance)
                    {
                        distance = Vector3.Distance(a.transform.position, position);
                        pointBase = a;
                    }
                });
                break;
            case GetPointMode.Far:
                float distance1 = 0;
                points.ForEach(a =>
                {
                    if (a.IsEmpty && Vector3.Distance(a.transform.position, position) > distance1)
                    {
                        distance1 = Vector3.Distance(a.transform.position, position);
                        pointBase = a;
                    }
                });
                break;
            default:
                break;
        }
        return pointBase;
    }

}
