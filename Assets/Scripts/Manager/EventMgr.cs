using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
/// <summary>
/// 用于记录判断结果的数据类
/// </summary>
public class Mark
{
    public int Result = 0; //0是False 1是True
}

/// <summary>
/// 事件管理器
/// </summary>
public class EventMgr
{
    /// <summary>
    /// 存储全部事件的字典，键是事件名，比如进入关卡，离开关卡，值是注册在这个名字下的委托函数，比如场景管理器里的EnterScene ExitScene函数
    /// 函数格式：一个object类型的返回值和一个object[]类型的参数，这样可以兼容任何形式的返回值以及任何形式的参数，虽然会有装箱拆箱的消耗，但对于多数游戏对象较少的独立游戏来说这些消耗不太要紧
    /// </summary>
    private static Dictionary<string, Func<object[], object>> m_Events = new Dictionary<string, Func<object[], object>>();

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="func">委托函数</param>
    public static void RegisterEvent(string eventName, Func<object[], object> func)
    {
        if (m_Events.ContainsKey(eventName))
        {
            m_Events[eventName] += func;
        }
        else
        {
            m_Events.Add(eventName, func);
        }
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="objs">数组参数</param>
    /// <param name="ignoreParamsCount">使用参数的时候忽略前几个参数，有时拿到的参数的前几位不一定是函数要用的</param>
    /// <returns></returns>
    public static object ExecuteEvent(string eventName, object[] param = null, int ignoreParamsCount = 0)
    {
        if (m_Events.ContainsKey(eventName))
        {
            if (m_Events[eventName] == null) return null;
            //如果不需要忽略参数
            if (ignoreParamsCount == 0)
            {
                //如果最后一个参数是存储判断结果用的数据类 那么说明是判断事件，需要将判断结果返回出去
                if (param != null && param.Length > 0 && param[param.Length - 1] is Mark mark)
                {
                    m_Events[eventName](param);
                    return mark.Result == 1;
                }
                return m_Events[eventName](param);
            }
            else
            {
                //如果没有参数 那么执行执行即可
                if (param == null || param.Length == 0)
                {
                    return m_Events[eventName](param);
                }
                else
                {
                    //截取后几位有效参数
                    List<object> args = new List<object>();
                    for (int i = ignoreParamsCount; i < param.Length; i++)
                    {
                        args.Add(param[i]);
                    }
                    //如果最后一个参数是存储判断结果用的数据类 那么说明是判断事件，那么需要将判断结果返回出去
                    if (args.Count > 0 && args[args.Count - 1] is Mark mark)
                    {
                        m_Events[eventName](args.ToArray());
                        return mark.Result == 1;
                    }
                    return m_Events[eventName](args.ToArray());
                }
            }
        }
        else
        {
            //Debug.LogWarning($"不存在名为{eventName}的事件，请先注册再使用");
            return null;
        }
    }

    /// <summary>
    /// 注销事件，注册和注销要成对出现，如果不注销的话，对象销毁后执行这个对象注册了的事件就会报错，如果注册事件的对象是游戏管理器等不会被销毁的对象，那么不注销也没事
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="func">委托函数</param>
    public static void UnRegisterEvent(string eventName, Func<object[], object> func)
    {
        if (m_Events.ContainsKey(eventName))
        {
            m_Events[eventName] -= func;
        }
    }

    /// <summary>
    /// 删除一个事件(几乎用不到)
    /// </summary>
    /// <param name="eventName">要删除的事件名</param>
    public static void DelEvent(string eventName)
    {
        m_Events.RemoveSafe(eventName);
    }

    /// <summary>
    /// 删除全部事件(几乎用不到)
    /// </summary>
    public static void DelEventAll()
    {
        m_Events.Clear();
    }

}
