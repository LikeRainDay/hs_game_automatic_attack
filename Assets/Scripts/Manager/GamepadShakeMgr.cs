using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 手柄震动管理器
/// </summary>
public class GamepadShakeMgr : MonoSingleton<GamepadShakeMgr>
{
    #region 数据
    private float m_ShakeTime;//剩余震动时长
    private static bool m_HaveGamapad;//当前是否连接了手柄
    private static float m_OldStrength;//之前的震动强度，如果之前的震动强度跟当前优先级最高的任务的震动强度不同，就切换任务
    private static List<ShakeTask> m_Tasks = new List<ShakeTask>();//当前的全部震动任务的数组
    private List<ShakeTask> m_Del = new List<ShakeTask>();//临时存放需要删除的任务的数组
    private static Dictionary<EnumShakeData, RowCfgShakeData> m_ShakeDataDic;
    /// <summary>
    /// 手柄震动数据字典
    /// </summary>
    public static Dictionary<EnumShakeData, RowCfgShakeData> ShakeDataDic
    {
        get
        {
            if (m_ShakeDataDic == null)
            {
                m_ShakeDataDic = new Dictionary<EnumShakeData, RowCfgShakeData>();
                foreach (var item in ConfigManager.Instance.cfgShakeData.AllConfigs)
                {
                    EnumShakeData enumShake = (EnumShakeData)Enum.Parse(typeof(EnumShakeData), item.enumName);
                    if (!m_ShakeDataDic.ContainsKey(enumShake))
                    {
                        m_ShakeDataDic.Add(enumShake, item);
                    }
                    else
                    {
                        Debug.LogWarning("手柄震动配置存在同名键:" + item.annotate);
                    }
                }

            }
            return m_ShakeDataDic;
        }

    }
    [System.Serializable]
    public class ShakeTask
    {
        public float Strength;//震动强度
        public float RemainTime;//震动时长
        public float Order;//优先级
        /// <summary>
        /// 强度，时长，优先级
        /// </summary>
        /// <param name="power">强度</param>
        /// <param name="remainTime">震动时长</param>
        /// <param name="order">震动优先级</param>
        public ShakeTask(float power, float remainTime, float order)
        {
            Strength = power;
            RemainTime = remainTime;
            Order = order;
        }

    }
    #endregion

    private void Start()
    {
        StartCoroutine(CheckGamepad());
    }

    /// <summary>
    /// 检测手柄
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckGamepad()
    {
        while (true)
        {
            //每隔一秒会检测一下是否有手柄
            if (Input.GetJoystickNames() != null && Input.GetJoystickNames().Length > 0)
            {
                m_HaveGamapad = true;
            }
            else
            {
                m_HaveGamapad = false;
            }
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// 根据配置设置震动任务
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static ShakeTask SetVibration(EnumShakeData pos)
    {
        RowCfgShakeData data = ShakeDataDic[pos];
        return SetVibration(new Vector3(data.param[0], data.param[1], data.param[2]));
    }

    /// <summary>
    /// 设置震动:强度，时长，优先级
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static ShakeTask SetVibration(Vector3 param)
    {
        //创建一个任务
        ShakeTask task = new ShakeTask(param.x, param.y, param.z);
        //如果当前没有任务，那么就将当前新增的作为当前任务
        if (m_Tasks.Count <= 0)
        {
            m_OldStrength = task.Strength;
            if (!InputMgr.IsKeyboard && SaveMgr.Instance.SettingData.Current.OpenGamepadShake)
            {
                ReInput.players.GetPlayer(0).SetVibration(0, param.x, true);
            }

        }
        //将震动任务加入数组
        m_Tasks.Add(task);
        return task;
    }

    /// <summary>
    /// 删除某个震动任务
    /// </summary>
    /// <param name="task"></param>
    public static void DelTask(ShakeTask task)
    {
        m_Tasks.RemoveSafe(task);
    }

    /// <summary>
    /// 清除全部震动任务
    /// </summary>
    public void ClearAllShake()
    {
        m_Tasks.Clear();
    }

    private void Update()
    {
        //如果没有连接手柄直接无视
        if (!m_HaveGamapad) return;
        //暂停或者不是手柄模式 或者没开启手柄震动 就暂停震动
        if (Time.timeScale == 0 || InputMgr.IsKeyboard || !SaveMgr.Instance.SettingData.Current.OpenGamepadShake)
        {
            //停止震动
            ReInput.players.GetPlayer(0).StopVibration();
            //删除掉到期的任务
            m_Del.Clear();
            foreach (var item in m_Tasks)
            {
                item.RemainTime -= Time.deltaTime;
                if (item.RemainTime <= 0)
                {
                    m_Del.Add(item);
                }
            }
            m_Del.ForEach(a => m_Tasks.Remove(a));
        }
        else
        {
            //删除掉到期的任务
            m_Del.Clear();
            foreach (var item in m_Tasks)
            {
                item.RemainTime -= Time.deltaTime;
                if (item.RemainTime <= 0)
                {
                    m_Del.Add(item);
                }
            }
            m_Del.ForEach(a => m_Tasks.Remove(a));

            //如果存在多个任务，那么会选择优先级最高的来执行
            if (m_Tasks.Count > 0)
            {
                m_ShakeTime += Time.deltaTime;
                //相同的强度值震动久了之后会不再震动，这时需要稍微调整一下强度值让其继续震动
                if (m_ShakeTime > 1.5f)
                {
                    m_ShakeTime = 0;
                    ReInput.players.GetPlayer(0).SetVibration(0, m_OldStrength + 0.1f, true);
                }
                else if (m_ShakeTime > 0.1)
                {
                    m_Tasks = m_Tasks.OrderBy(x => x.Order).ToList();
                    //如果之前的震动强度与当前优先级最高的任务的震动强度不同 就切换一下任务
                    if (m_OldStrength != m_Tasks[m_Tasks.Count - 1].Strength)
                    {
                        m_OldStrength = m_Tasks[m_Tasks.Count - 1].Strength;
                        ReInput.players.GetPlayer(0).SetVibration(0, m_Tasks[m_Tasks.Count - 1].Strength, true);
                    }
                }
            }
            //如果没有任务 则会停止震动
            else
            {
                ReInput.players.GetPlayer(0).StopVibration();
            }
        }
    }
}
