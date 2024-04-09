using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// *85相机震动管理器
/// </summary>
public class CameraShakeMgr : MonoSingleton<CameraShakeMgr>
{
    static CameraShakeMgr()
    {
        //调用静态字段 函数 的时候会先调用静态构造(可能?)，所以在静态构造里随便调用一下Instance，就会去创建一个Mono单例，Mono单例可以提供一些Mono类才能支持的功能，比如开启协程和挂载其他Unity组件
        print(Instance.name);
    }
    #region 字段
    /// <summary>
    /// 相机震动效果的tweem的引用，开启新震动和关闭震动时清理动画效果用
    /// </summary>
    private static Tween m_ShakeTween;
    /// <summary>
    /// 震动任务自增ID
    /// </summary>
    private static int m_UniqueId;
    /// <summary>
    /// 当前震动任务的自增ID
    /// </summary>
    private static int m_CurTaskUniqueId;
    /// <summary>
    /// 存放需要删除的任务
    /// </summary>
    private List<ShakeTask> m_Del = new List<ShakeTask>();
    /// <summary>
    /// 当前全部震动任务的数组
    /// </summary>
    private static List<ShakeTask> m_Tasks = new List<ShakeTask>();

    private static Dictionary<EnumCameraShakeData, RowCfgCameraShakeData> m_ShakeDataDic;
    /// <summary>
    /// 震动数据字典，读取Csv数据后转存成字典形式来使用
    /// </summary>
    public static Dictionary<EnumCameraShakeData, RowCfgCameraShakeData> ShakeDataDic
    {
        get
        {
            if (m_ShakeDataDic == null || m_ShakeDataDic.Count == 0)
            {
                m_ShakeDataDic = new Dictionary<EnumCameraShakeData, RowCfgCameraShakeData>();
                foreach (var item in ConfigManager.Instance.cfgCameraShakeData.AllConfigs)
                {
                    if (item.id == 0) continue;
                    EnumCameraShakeData enumShake = (EnumCameraShakeData)Enum.Parse(typeof(EnumCameraShakeData), item.enumName);
                    if (!m_ShakeDataDic.ContainsKey(enumShake))
                    {
                        m_ShakeDataDic.Add(enumShake, item);
                    }
                    else
                    {
                        Debug.LogWarning("相机震动配置存在同名键:" + item.annotate);
                    }
                }
            }
            return m_ShakeDataDic;
        }

    }

    /// <summary>
    /// 震动任务数据类
    /// </summary>
    [System.Serializable]
    public class ShakeTask
    {
        public int UniqueId;//自增ID
        public float Order;//优先级，同时存在多个震动任务的时候会优先使用优先级更高的任务的震动数据
        public float StrengthX;//X轴的震动强度
        public float StrengthY;//Y轴的震动强度
        public int Vibrate;//震动频率
        public float Randomness;//震动随机程度
        public float Time;//震动时长

        public ShakeTask(float order, float strengthX, float strengthY, float vibrate, float randomness, float time)
        {
            UniqueId = m_UniqueId++;
            Order = order;
            StrengthX = strengthX;
            StrengthY = strengthY;
            Vibrate = (int)vibrate;
            Randomness = randomness;
            Time = time;
        }

    }
    #endregion

    /// <summary>
    /// 添加震动任务
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static ShakeTask SetVibration(EnumCameraShakeData pos)
    {
        RowCfgCameraShakeData data = ShakeDataDic[pos];
        ShakeTask task = new ShakeTask(data.param[0], data.param[1], data.param[2], data.param[3], data.param[4], data.param[5]);
        //如果当前没有其他震动任务，那么就直接启用刚刚添加的这条任务
        if (m_Tasks.Count <= 0)
        {
            m_CurTaskUniqueId = task.UniqueId;
            Shake(task);
        }
        //将震动任务加入数组
        m_Tasks.Add(task);
        //返回震动任务，以便外部进行一些额外操作
        return task;
    }

    /// <summary>
    /// 相机抖动
    /// </summary>
    /// <param name="task"></param>
    private static void Shake(ShakeTask task)
    {
        m_CurTaskUniqueId = task.UniqueId;
        //关闭之前的抖动任务
        m_ShakeTween.Kill();
        m_ShakeTween = Camera.main.transform.DOShakePosition(task.Time, new Vector2(task.StrengthX, task.StrengthY), task.Vibrate, task.Randomness);
        m_ShakeTween.onComplete += () => Camera.main.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 删除震动任务
    /// </summary>
    /// <param name="task"></param>
    public static void DelTask(ShakeTask task)
    {
        //如果数组里没有这个任务就无视
        if (!m_Tasks.Contains(task)) return;
        //如果要删除的任务是正在使用的任务 需要修改一下当前任务的id 以便下一帧换成其他任务
        if (m_CurTaskUniqueId == task.UniqueId)
        {
            m_CurTaskUniqueId = -1;
        }
        m_Tasks.Remove(task);

    }

    /// <summary>
    /// 清理当前的全部震动任务
    /// </summary>
    public void ClearAllShakes()
    {
        //停止震动
        m_ShakeTween.Kill();
        //清理任务 和规整坐标
        m_Tasks.Clear();
        Camera.main.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        //删除掉到期的任务
        m_Del.Clear();
        foreach (var item in m_Tasks)
        {
            item.Time -= Time.deltaTime;
            if (item.Time <= 0)
            {
                m_Del.Add(item);
            }
        }
        m_Del.ForEach(a => m_Tasks.Remove(a));

        //如果存在多个任务，那么会选择优先级最高的来执行
        if (m_Tasks.Count > 0)
        {
            //遍历找到优先级最高的震动 判断他是否跟当前震动相同，是的话就无视 不是的话就切换成他
            m_Tasks = m_Tasks.OrderByDescending(x => x.Order).ToList();
            if (m_CurTaskUniqueId != m_Tasks[0].UniqueId)
            {
                Shake(m_Tasks[0]);
            }
        }
        else
        {
            //规则相机局部坐标
            if (Camera.main != null)
            {
                Camera.main.transform.localPosition = Vector3.zero;
            }

        }

    }
}