using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 相机抖动工具
/// </summary>
public class CameraShakeTool : MonoBehaviour
{
    [EnumName("抖动枚举")]
    public EnumCameraShakeData Type;
    [Tooltip("抖动延时")]
    public float Delay;
    private CameraShakeMgr.ShakeTask m_Task;//抖动任务的引用，关闭抖动时用

    private void OnEnable()
    {
        //等待给定延时 执行抖动效果
        Invoke("DelayFun", Delay);
    }

    public void DelayFun()
    {
        m_Task = CameraShakeMgr.SetVibration(Type);
    }

    private void OnDisable()
    {
        //禁用的时候 删除一下抖动
        CameraShakeMgr.DelTask(m_Task);
    }

}
