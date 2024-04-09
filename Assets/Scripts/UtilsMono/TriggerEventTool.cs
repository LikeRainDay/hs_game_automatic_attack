using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 触发时执行字符串事件的工具
/// </summary>
public class TriggerEventTool : MonoBehaviour
{
    [FieldName("事件字符串")]
    public string EffectStr;
    [Header("能交互的碰撞器标签")]
    public List<ColliderTagDef> ColliderTags;
    [FieldName("触发后自动禁用")]
    public bool AutoDisable;
    private List<string> m_Tags;//代码内部使用字符串比较方便，这里将面板上配置的枚举数组转换成字符串数组来使用
    private List<Collider2D> m_Enters = new List<Collider2D>();//为了防止重复触发 之前进入触发区域的物体需要出去后才能再次触发

    private void Start()
    {
        //将面板上配置的碰撞器枚举数组转换成字符串数组来使用
        m_Tags = new List<string>();
        ColliderTags.ForEach(a => m_Tags.Add(a.ToString()));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果是可交互的碰撞器 
        if (m_Tags.Contains(collision.tag) && !m_Enters.Contains(collision))
        {
            //执行事件
            ToolFun.ExecuteEvents(EffectStr);

            //如果是一次性的触发器，那么触发后就禁用掉
            if (AutoDisable)
            {
                GetComponents<Collider2D>().ForEach(a => a.enabled = false);
            }
            //如果不是一次性的 就将当前交互的碰撞器加入过滤数组，在他退出触发范围之前不重复触发
            else
            {
                m_Enters.Add(GetComponent<Collider2D>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //退出了触发范围后 就将碰撞器从过滤数组里移除出去
        m_Enters.RemoveSafe(collision);
    }

}
