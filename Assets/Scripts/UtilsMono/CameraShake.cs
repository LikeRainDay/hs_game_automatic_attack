using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 相机抖动工具
/// </summary>
public class CameraShake : MonoBehaviour
{
    #region 抖动相机Rect的方式所需的变量
    [Header("抖动相机Rect的方式的默认设置")]
    [FieldName("抖动强度")]
    public float ShakeRectLevel = 3f;
    [FieldName("抖动时长")]
    public float ShakeRectTime = 0.35f;
    [FieldName("抖动帧率")]
    public float ShakeRectFps = 45f;

    //用静态变量记录界面上设置的强度 时长 帧率 给静态函数使用
    private static float m_DefaultShakeRectLevel;
    private static float m_DefaultShakeRectTime;
    private static float m_DefaultRectFps;

    private static float m_CurShakeRectLevel;//此次相机抖动的强度
    private static float m_CurShakeRectTime;//此次相机抖动的时长
    private static float m_CurShakeRectFps;//此次相机抖动的帧率

    private float m_FrameTime = 0.03f;//当前累计的时长。根据帧率，累计一定时长就抖动一下
    private float m_ShakeDelta = 0.005f;//抖动强度
    private Rect m_ChangeRect;
    #endregion

    #region 抖动相机Postion的方式所需的变量
    [Header("抖动Position的方式的默认设置")]
    [FieldName("抖动时长")]
    public float ShakePositionTime = 0.45f;
    [FieldName("X强度")]
    public float ShakePositionStrengthX = 0.15f;
    [FieldName("Y强度")]
    public float ShakePositionStrengthY = 0.15f;
    [FieldName("随机程度")]
    public float ShakePositionRandomness = 90;
    [FieldName("频率")]
    public int ShakePositionVibrate = 20;

    //同上
    private static float m_DefaultShakePositionTime;//默认抖动时长
    private static float m_DefaultShakePositionStrengthX;//默认X强度
    private static float m_DefaultShakePositionStrengthY;//默认Y强度
    private static float m_DefaultShakePositionRandomness;//默认随机程度
    private static int m_DefaultShakePositionVibrate;//默认频率
    #endregion

    private static bool IsShakeCamera = false;//是否在抖动中 
    private static Camera m_SelfCamera;//相机的引用   
    private static Tween m_ShakeTween;//抖动动画的tween

    public static CameraShake Instance;
    private void Awake()
    {
        //记录界面上设置的强度 时长 帧率 给静态函数使用
        m_DefaultRectFps = ShakeRectFps;
        m_DefaultShakeRectLevel = ShakeRectLevel;
        m_DefaultShakeRectTime = ShakeRectTime;

        //同上
        m_DefaultShakePositionTime = ShakePositionTime;
        m_DefaultShakePositionStrengthX = ShakePositionStrengthX;
        m_DefaultShakePositionStrengthY = ShakePositionStrengthY;
        m_DefaultShakePositionRandomness = ShakePositionRandomness;
        m_DefaultShakePositionVibrate = ShakePositionVibrate;

        //获取引用
        m_SelfCamera = GetComponent<Camera>();
        m_ChangeRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// 相机抖动
    /// </summary>
    public static void ShakeRect()
    {
        CheckInstance();
        m_CurShakeRectTime = m_DefaultShakeRectTime;
        m_CurShakeRectFps = m_DefaultRectFps;
        m_CurShakeRectLevel = m_DefaultShakeRectLevel;
        IsShakeCamera = true;

    }

    /// <summary>
    /// 相机抖动
    /// </summary>
    /// <param name="shakeLevel">抖动强度</param>
    /// <param name="shakeTime">抖动时长</param>
    /// <param name="shakeFps">抖动帧率</param>
    public static void ShakeRect(float shakeLevel, float shakeTime, float shakeFps)
    {
        CheckInstance();
        m_CurShakeRectLevel = shakeLevel;
        m_CurShakeRectTime = shakeTime;
        m_CurShakeRectFps = shakeFps;
        IsShakeCamera = true;

    }

    /// <summary>
    /// 相机抖动
    /// </summary>
    public static void ShakePosition()
    {
        CheckInstance();
        m_ShakeTween.Kill();
        m_ShakeTween = m_SelfCamera.transform.DOShakePosition(m_DefaultShakePositionTime,
            new Vector2(m_DefaultShakePositionStrengthX, m_DefaultShakePositionStrengthY), m_DefaultShakePositionVibrate, m_DefaultShakePositionRandomness);
    }

    /// <summary>
    /// 相机抖动
    /// </summary>
    /// <param name="shakePositionTime">抖动时长</param>
    /// <param name="shakePositionStrengthX">抖动强度X</param>
    /// <param name="shakePositionStrengthY">抖动强度Y</param>
    /// <param name="shakePositionVibrate">抖动频率</param>
    /// <param name="positionRandomness">抖动随机度</param>
    public static void ShakePosition(float shakePositionTime, float shakePositionStrengthX, float shakePositionStrengthY, int shakePositionVibrate, float positionRandomness)
    {
        CheckInstance();
        m_ShakeTween.Kill();
        m_ShakeTween = m_SelfCamera.transform.DOShakePosition(shakePositionTime,
            new Vector2(shakePositionStrengthX, shakePositionStrengthY), shakePositionVibrate, positionRandomness);
    }

    /// <summary>
    /// 抖动Rect的情况需要Update函数，这里可以创建一个实例去处理Update任务
    /// </summary>
    public static void CheckInstance()
    {
        //如果还没有实例 就给主相机添加一个
        if (Instance == null)
        {
            Instance = Camera.main.gameObject.AddComponent<CameraShake>();
        }
    }

    /// <summary>
    /// 停止抖动
    /// </summary>
    public static void StopShake()
    {
        m_CurShakeRectTime = 0;
        m_ShakeTween.Kill();
    }

    private void Update()
    {
        if (!IsShakeCamera) return;

        m_CurShakeRectTime -= Time.deltaTime;
        //抖动结束 规整一下数据
        if (m_CurShakeRectTime <= 0)
        {
            IsShakeCamera = false;
            m_ChangeRect.xMin = 0.0f;
            m_ChangeRect.yMin = 0.0f;
            m_SelfCamera.rect = m_ChangeRect;
            m_FrameTime = 0.03f;
            m_ShakeDelta = 0.005f;

        }
        else
        {
            m_FrameTime += Time.deltaTime;
            if (m_FrameTime > 1.0 / m_CurShakeRectFps)
            {
                m_FrameTime = 0;
                m_ChangeRect.xMin = m_ShakeDelta * (-1.0f + ShakeRectLevel * Random.value);
                m_ChangeRect.yMin = m_ShakeDelta * (-1.0f + ShakeRectLevel * Random.value);
                m_SelfCamera.rect = m_ChangeRect;
            }
        }
    }

}
