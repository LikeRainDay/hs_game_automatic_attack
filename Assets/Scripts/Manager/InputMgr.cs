using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using LitJson;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Events;
using UnityEngine.UI;
using Steamworks;
/// <summary>
/// 按键配置
/// </summary>
public class KeyCfg
{
    //默认按键配置在这里设置 启动游戏后可以调用InputMrg脚本里的 CreateDefaultCfg函数，将这里的数据保存成text文件作为游戏的默认数据
    public Dictionary<string, string> Keys = new Dictionary<string, string>
    {
        #region 角色
        {"Up_P1","<Keyboard>/w" },{"Up_P2","<Keyboard>/upArrow" },
        {"Down_P1","<Keyboard>/s" },{"Down_P2","<Keyboard>/downArrow" },
        {"Left_P1","<Keyboard>/a" },{"Left_P2","<Keyboard>/leftArrow" },
        {"Right_P1","<Keyboard>/d" },{"Right_P2","<Keyboard>/rightArrow" },

        {"JoyL_P1","<Gamepad>/leftStick" },
        {"JoyR_P1","<Gamepad>/rightStick" },
        {"Dpad_P1","<Gamepad>/dpad" },

        {"X_P1","<Gamepad>/buttonWest"}, {"X_P2","<Keyboard>/j"},//这里暂时瞎填的
        {"Y_P1","<Gamepad>/buttonNorth"}, {"Y_P2","<Keyboard>/i"},
        {"A_P1","<Gamepad>/buttonSouth"}, {"A_P2","<Keyboard>/k"},
        {"B_P1","<Gamepad>/buttonEast"}, {"B_P2","<Keyboard>/l"},

        {"LT_P1","<Gamepad>/leftTrigger" }, {"LT_P2","<Gamepad>/q" },
        {"RT_P1","<Gamepad>/rightTrigger" }, {"RT_P2","<Gamepad>/e" },
        {"LB_P1","<Gamepad>/leftShoulder" }, {"LB_P2","<Gamepad>/z" },
        {"RB_P1","<Gamepad>/rightShoulder" }, {"RB_P2","<Gamepad>/c" },

        {"Back_P1","<Gamepad>/select" }, {"Back_P2","<Gamepad>/tab" },
        {"Start_P1","<Gamepad>/start" }, {"Start_P2","<Gamepad>/escape" },

        {"Mouse0_P1","<Mouse>/leftButton"},
        {"Mouse1_P1","<Mouse>/rightButton"},
        {"Mouse2_P1","<Mouse>/middleButton"},
        {"Shift_P1","<Keyboard>/leftShift"},
        #endregion

        #region 系统
        {"Up1","<Keyboard>/w" },{"Up2","<Keyboard>/upArrow" },
        {"Down1","<Keyboard>/s" },{"Down2","<Keyboard>/downArrow" },
        {"Left1","<Keyboard>/a" },{"Left2","<Keyboard>/leftArrow" },
        {"Right1","<Keyboard>/d" },{"Right2","<Keyboard>/rightArrow" },

        {"JoyL1","<Gamepad>/leftStick" },
        {"JoyR1","<Gamepad>/rightStick" },
        {"Dpad1","<Gamepad>/dpad" },

        {"X1","<Gamepad>/buttonWest"}, {"X2","<Keyboard>/j"},//这里暂时瞎填的
        {"Y1","<Gamepad>/buttonNorth"}, {"Y2","<Keyboard>/i"},
        {"A1","<Gamepad>/buttonSouth"}, {"A2","<Keyboard>/k"},
        {"B1","<Gamepad>/buttonEast"}, {"B2","<Keyboard>/l"},

        {"LT1","<Gamepad>/leftTrigger" }, {"LT2","<Gamepad>/q" },
        {"RT1","<Gamepad>/rightTrigger" }, {"RT2","<Gamepad>/e" },
        {"LB1","<Gamepad>/leftShoulder" }, {"LB2","<Gamepad>/z" },
        {"RB1","<Gamepad>/rightShoulder" }, {"RB2","<Gamepad>/c" },

        {"Back1","<Gamepad>/select" }, {"Back2","<Gamepad>/tab" },
        {"Start1","<Gamepad>/start" }, {"Start2","<Gamepad>/escape" },

        {"Mouse01","<Mouse>/leftButton"},
        {"Mouse11","<Mouse>/rightButton"},
        {"Mouse21","<Mouse>/middleButton"},
        {"Shift1","<Keyboard>/leftShift"},
        #endregion
    };

}

/// <summary>
/// 用户输入模块管理器
/// </summary>
public class InputMgr : MonoSingleton<InputMgr>
{
    #region 字段 和 属性   
    #region 其他字段
    [FieldName("时间缩放(编辑器下按上方数字0可以将时间缩放修改成此值，慢放看效果时用，再次按下会将缩放修改回1)")]
    public float TimeScale = 0.3f;
    [FieldName("鼠标移动多少距离视作有输入")]
    public float MouseMoveMin;
    [HideInInspector] public GraphicRaycaster Raycaster;//当不是键鼠的时候，会禁用一下射线检测

    public static string TargetKey;//当前要设置的按键(攻击键，跳跃键)
    public static string TargetPath;//当前要设置的按键对应的按钮路径 eg. <Keyboard>/leftArrow
    public static float CurIdleTime;//多久没有用户输入
    public static bool IsKeyboard = true;//当前是手柄还是键鼠  

    //配置相关
    public static KeyCfg KeyCfg;//键位数据类  
    private static string m_JsonPath = "Assets/AddressableAssets/ConfigOther/InputActions.txt";//输入配置文件路径
    private static string m_DefaultSettingPath = "Assets/AddressableAssets/ConfigOther/DefaultKeySetting.txt";//默认键位设置配置文件路径
    private static string m_CustomSettingPath = "CustomKeySetting.txt";//存档里的键位配置文件名字 保存在persistentDataPath文件夹里
    private static PlayerInput m_PlayerInput;//输入配置文件转换的对象

    //判断的灵敏度
    public static float Sensitivity1 = 0.2f;//超级灵敏
    public static float Sensitivity2 = 0.4f;//一般灵敏
    public static float Sensitivity3 = 0.6f;//不太灵敏  

    private static float ChangeCDX = 0.25f;//长按方向键每隔多久视作一次输入
    private static float m_CurChangeCDX_P;//当前主角输入CD
    private static float m_CurChangeCDX;//当前系统输入CD

    private static float ChangeCDY = 0.25f;//长按方向键每隔多久视作一次输入
    private static float m_CurChangeCDY_P;//当前主角输入CD
    private static float m_CurChangeCDY;//当前系统输入CD


    private static bool m_Focus = false;//程序是否处于聚焦中
    private Vector3 m_OldMousePos = DefaultDef.Vector3;//鼠标上一帧的位置
    private Vector3 m_MouseDelta;//鼠标上一帧的移动间隔
    #endregion

    #region 角色控制相关变量
    public static float Up_P;//这个键当前的值 大于0视作按着这个键
    public static bool Up_PDown;//是否按下了这个键
    public static bool Up_PUp;//是否抬起了这个键
    public static Action<float> Up_PD;//这个键按下时要触发的事件
    public static Action<float> Up_PU;//这个键抬起时要触发的事件

    public static float Down_P;
    public static bool Down_PDown;
    public static bool Down_PUp;
    public static Action<float> Down_PD;
    public static Action<float> Down_PU;

    public static float Left_P;
    public static bool Left_PDown;
    public static bool Left_PUp;
    public static Action<float> Left_PD;
    public static Action<float> Left_PU;

    public static float Right_P;
    public static bool Right_PDown;
    public static bool Right_PUp;
    public static Action<float> Right_PD;
    public static Action<float> Right_PU;

    public static Vector2 JoyL_P;
    public static bool JoyL_PDown;
    public static bool JoyL_PUp;
    public static Action<Vector2> JoyL_PD;
    public static Action<Vector2> JoyL_PU;

    public static Vector2 JoyR_P;
    public static bool JoyR_PDown;
    public static bool JoyR_PUp;
    public static Action<Vector2> JoyR_PD;
    public static Action<Vector2> JoyR_PU;

    public static Vector2 Dpad_P;
    public static bool Dpad_PDown;
    public static bool Dpad_PUp;
    public static Action<Vector2> Dpad_PD;
    public static Action<Vector2> Dpad_PU;

    public static float X_P;
    public static bool X_PDown;
    public static bool X_PUp;
    public static Action<float> X_PD;
    public static Action<float> X_PU;

    public static float Y_P;
    public static bool Y_PDown;
    public static bool Y_PUp;
    public static Action<float> Y_PD;
    public static Action<float> Y_PU;

    public static float A_P;
    public static bool A_PDown;
    public static bool A_PUp;
    public static Action<float> A_PD;
    public static Action<float> A_PU;

    public static float B_P;
    public static bool B_PDown;
    public static bool B_PUp;
    public static Action<float> B_PD;
    public static Action<float> B_PU;

    public static float LT_P;
    public static bool LT_PDown;
    public static bool LT_PUp;
    public static Action<float> LT_PD;
    public static Action<float> LT_PU;

    public static float RT_P;
    public static bool RT_PDown;
    public static bool RT_PUp;
    public static Action<float> RT_PD;
    public static Action<float> RT_PU;

    public static float LB_P;
    public static bool LB_PDown;
    public static bool LB_PUp;
    public static Action<float> LB_PD;
    public static Action<float> LB_PU;

    public static float RB_P;
    public static bool RB_PDown;
    public static bool RB_PUp;
    public static Action<float> RB_PD;
    public static Action<float> RB_PU;

    public static float Back_P;
    public static bool Back_PDown;
    public static bool Back_PUp;
    public static Action<float> Back_PD;
    public static Action<float> Back_PU;

    public static float Start_P;
    public static bool Start_PDown;
    public static bool Start_PUp;
    public static Action<float> Start_PD;
    public static Action<float> Start_PU;

    public static float Mouse0_P;
    public static bool Mouse0_PDown;
    public static bool Mouse0_PUp;
    public static Action<float> Mouse0_PD;
    public static Action<float> Mouse0_PU;

    public static float Mouse1_P;
    public static bool Mouse1_PDown;
    public static bool Mouse1_PUp;
    public static Action<float> Mouse1_PD;
    public static Action<float> Mouse1_PU;

    public static float Mouse2_P;
    public static bool Mouse2_PDown;
    public static bool Mouse2_PUp;
    public static Action<float> Mouse2_PD;
    public static Action<float> Mouse2_PU;

    public static float Shift_P;
    public static bool Shift_PDown;
    public static bool Shift_PUp;
    public static Action<float> Shift_PD;
    public static Action<float> Shift_PU;
    #endregion

    #region 系统控制相关变量
    public static float Up;
    public static bool UpDown;
    public static bool UpUp;
    public static Action<float> UpD;
    public static Action<float> UpU;

    public static float Down;
    public static bool DownDown;
    public static bool DownUp;
    public static Action<float> DownD;
    public static Action<float> DownU;

    public static float Left;
    public static bool LeftDown;
    public static bool LeftUp;
    public static Action<float> LeftD;
    public static Action<float> LeftU;

    public static float Right;
    public static bool RightDown;
    public static bool RightUp;
    public static Action<float> RightD;
    public static Action<float> RightU;

    public static Vector2 JoyL;
    public static bool JoyLDown;
    public static bool JoyLUp;
    public static Action<Vector2> JoyLD;
    public static Action<Vector2> JoyLU;

    public static Vector2 JoyR;
    public static bool JoyRDown;
    public static bool JoyRUp;
    public static Action<Vector2> JoyRD;
    public static Action<Vector2> JoyRU;

    public static Vector2 Dpad;
    public static bool DpadDown;
    public static bool DpadUp;
    public static Action<Vector2> DpadD;
    public static Action<Vector2> DpadU;

    public static float X;
    public static bool XDown;
    public static bool XUp;
    public static Action<float> XD;
    public static Action<float> XU;

    public static float Y;
    public static bool YDown;
    public static bool YUp;
    public static Action<float> YD;
    public static Action<float> YU;

    public static float A;
    public static bool ADown;
    public static bool AUp;
    public static Action<float> AD;
    public static Action<float> AU;

    public static float B;
    public static bool BDown;
    public static bool BUp;
    public static Action<float> BD;
    public static Action<float> BU;

    public static float LT;
    public static bool LTDown;
    public static bool LTUp;
    public static Action<float> LTD;
    public static Action<float> LTU;

    public static float RT;
    public static bool RTDown;
    public static bool RTUp;
    public static Action<float> RTD;
    public static Action<float> RTU;

    public static float LB;
    public static bool LBDown;
    public static bool LBUp;
    public static Action<float> LBD;
    public static Action<float> LBU;

    public static float RB;
    public static bool RBDown;
    public static bool RBUp;
    public static Action<float> RBD;
    public static Action<float> RBU;

    public static float Back;
    public static bool BackDown;
    public static bool BackUp;
    public static Action<float> BackD;
    public static Action<float> BackU;

    public static float Start;
    public static bool StartDown;
    public static bool StartUp;
    public static Action<float> StartD;
    public static Action<float> StartU;

    public static float Mouse0;
    public static bool Mouse0Down;
    public static bool Mouse0Up;
    public static Action<float> Mouse0D;
    public static Action<float> Mouse0U;

    public static float Mouse1;
    public static bool Mouse1Down;
    public static bool Mouse1Up;
    public static Action<float> Mouse1D;
    public static Action<float> Mouse1U;

    public static float Mouse2;
    public static bool Mouse2Down;
    public static bool Mouse2Up;
    public static Action<float> Mouse2D;
    public static Action<float> Mouse2U;

    public static float Shift;
    public static bool ShiftDown;
    public static bool ShiftUp;
    public static Action<float> ShiftD;
    public static Action<float> ShiftU;
    #endregion

    #region 是否有输入
    /// <summary>
    /// 是否按下了任意键
    /// </summary>
    public static bool InputAnyKeyDown { get => Input.anyKeyDown || Dpad.magnitude > 0.2f || JoyL.magnitude > 0.2f || JoyR.magnitude > 0.2f; }
    /// <summary>
    /// 是否按下了上方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsUp1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsUp2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsUp3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    /// <summary>
    /// 是否按下了下方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsDown1 { get => Down_P > Sensitivity1 || JoyL.y < -Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsDown2 { get => Down_P > Sensitivity2 || JoyL.y < -Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsDown3 { get => Down_P > Sensitivity3 || JoyL.y < -Sensitivity3 || Dpad_P.y > Sensitivity3; }
    /// <summary>
    /// 是否按下了左方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsLeft1 { get => Left_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsLeft2 { get => Left_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsLeft3 { get => Left_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity3; }
    /// <summary>
    /// 是否按下了右方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsRight1 { get => Right_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsRight2 { get => Right_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsRight3 { get => Right_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity3; }
    /// <summary>
    /// 是否按下了左右方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsH1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsH2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsH3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    /// <summary>
    /// 是否按下了上下方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsV1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsV2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsV3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    /// <summary>
    /// 是否按下了方向键，不同级别对应不同灵敏度(系统相关的输入)
    /// </summary>
    public static bool IsHV1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsHV2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsHV3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }


    /// <summary>
    /// 是否按下了上方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsUp_P1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsUp_P2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsUp_P3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    /// <summary>
    /// 是否按下了下方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsDown_P1 { get => Down_P > Sensitivity1 || JoyL.y < -Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsDown_P2 { get => Down_P > Sensitivity2 || JoyL.y < -Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsDown_P3 { get => Down_P > Sensitivity3 || JoyL.y < -Sensitivity3 || Dpad_P.y > Sensitivity3; }
    /// <summary>
    /// 是否按下了左方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsLeft_P1 { get => Left_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsLeft_P2 { get => Left_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsLeft_P3 { get => Left_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity3; }
    /// <summary>
    /// 是否按下了右方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsRight_P1 { get => Right_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsRight_P2 { get => Right_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsRight_P3 { get => Right_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity3; }
    /// <summary>
    /// 是否按下了左右方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsH_P1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsH_P2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsH_P3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    /// <summary>
    /// 是否按下了上下方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsV_P1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsV_P2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsV_P3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    /// <summary>
    /// 是否按下了方向键，不同级别对应不同灵敏度(角色相关的输入)
    /// </summary>
    public static bool IsHV_P1 { get => Up_P > Sensitivity1 || JoyL.y > Sensitivity1 || Dpad_P.y > Sensitivity1; }
    public static bool IsHV_P2 { get => Up_P > Sensitivity2 || JoyL.y > Sensitivity2 || Dpad_P.y > Sensitivity2; }
    public static bool IsHV_P3 { get => Up_P > Sensitivity3 || JoyL.y > Sensitivity3 || Dpad_P.y > Sensitivity2; }
    #endregion

    #region 上下左右切换事件
    public static Action<int> HAction_P;//短按水平方向键(角色控制)
    public static Action<int> HActionLong_P;//长按水平方向键(角色控制)
    public static Action<int> VAction_P;//短按垂直方向键(角色控制)
    public static Action<int> VActionLong_P;//长按垂直方向键(角色控制)
    public static Action<int> HAction;//短按水平方向键(系统控制)
    public static Action<int> HActionLong;//长按水平方向键(系统控制)
    public static Action<int> VAction;//短按垂直方向键(系统控制)
    public static Action<int> VActionLong;//长按垂直方向键(系统控制)
    public static Action<int> ControllerChange;//控制器切换的事件
    #endregion
    #endregion

    #region 初始化 
    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        //添加一个PlayerInput组件
        m_PlayerInput = gameObject.AddComponent<PlayerInput>();
        //注册一下事件 每次有输入的时候检测一下设备 如果切换了设备就调用函数处理一下相关逻辑
        InputSystem.onActionChange += (obj, change) =>
        {
            if (change == InputActionChange.ActionPerformed)
            {
                CheckDevice(obj);
            }
        };
        //每次有设备添加的时候就移除旧的配置，然后重新初始化
        InputSystem.onDeviceChange += (a, b) =>
        {
            if (b == InputDeviceChange.Added)
            {
                m_PlayerInput.onActionTriggered -= Event;
                StartCoroutine(RealInit());
            }
        };
        //初始化按键配置
        StartCoroutine(RealInit());

        #region 注册一下角色按键相关的事件
        Up_PD += (a) => Up_PDown = true;
        Up_PU += (a) => Up_PUp = true;
        Down_PD += (a) => Down_PDown = true;
        Down_PU += (a) => Down_PUp = true;
        Left_PD += (a) => Left_PDown = true;
        Left_PU += (a) => Left_PUp = true;
        Right_PD += (a) => Right_PDown = true;
        Right_PU += (a) => Right_PUp = true;

        JoyL_PD += (a) => JoyL_PDown = true;
        JoyL_PU += (a) => JoyL_PUp = true;
        JoyR_PD += (a) => JoyR_PDown = true;
        JoyR_PU += (a) => JoyR_PUp = true;
        Dpad_PD += (a) => Dpad_PDown = true;
        Dpad_PU += (a) => Dpad_PUp = true;

        X_PD += (a) => X_PDown = true;
        X_PU += (a) => X_PUp = true;
        Y_PD += (a) => Y_PDown = true;
        Y_PU += (a) => Y_PUp = true;
        A_PD += (a) => A_PDown = true;
        A_PU += (a) => A_PUp = true;
        B_PD += (a) => B_PDown = true;
        B_PU += (a) => B_PUp = true;

        LT_PD += (a) => LT_PDown = true;
        LT_PU += (a) => LT_PUp = true;
        RT_PD += (a) => RT_PDown = true;
        RT_PU += (a) => RT_PUp = true;
        LB_PD += (a) => LB_PDown = true;
        LB_PU += (a) => LB_PUp = true;
        RB_PD += (a) => RB_PDown = true;
        RB_PU += (a) => RB_PUp = true;

        Back_PD += (a) => Back_PDown = true;
        Back_PU += (a) => Back_PUp = true;
        Start_PD += (a) => Start_PDown = true;
        Start_PU += (a) => Start_PUp = true;

        Mouse0_PD += (a) => Mouse0_PDown = true;
        Mouse0_PU += (a) => Mouse0_PUp = true;
        Mouse1_PD += (a) => Mouse1_PDown = true;
        Mouse1_PU += (a) => Mouse1_PUp = true;
        Mouse2_PD += (a) => Mouse2_PDown = true;
        Mouse2_PU += (a) => Mouse2_PUp = true;

        Shift_PD += (a) => Shift_PDown = true;
        Shift_PU += (a) => Shift_PUp = true;
        #endregion

        #region 注册一下系统按键相关的事件
        UpD += (a) => UpDown = true;
        UpU += (a) => UpUp = true;
        DownD += (a) => DownDown = true;
        DownU += (a) => DownUp = true;
        LeftD += (a) => LeftDown = true;
        LeftU += (a) => LeftUp = true;
        RightD += (a) => RightDown = true;
        RightU += (a) => RightUp = true;

        JoyLD += (a) => JoyLDown = true;
        JoyLU += (a) => JoyLUp = true;
        JoyRD += (a) => JoyRDown = true;
        JoyRU += (a) => JoyRUp = true;
        DpadD += (a) => DpadDown = true;
        DpadU += (a) => DpadUp = true;

        XD += (a) => XDown = true;
        XU += (a) => XUp = true;
        YD += (a) => YDown = true;
        YU += (a) => YUp = true;
        AD += (a) => ADown = true;
        AU += (a) => AUp = true;
        BD += (a) => BDown = true;
        BU += (a) => BUp = true;

        LTD += (a) => LTDown = true;
        LTU += (a) => LTUp = true;
        RTD += (a) => RTDown = true;
        RTU += (a) => RTUp = true;
        LBD += (a) => LBDown = true;
        LBU += (a) => LBUp = true;
        RBD += (a) => RBDown = true;
        RBU += (a) => RBUp = true;

        BackD += (a) => BackDown = true;
        BackU += (a) => BackUp = true;
        StartD += (a) => StartDown = true;
        StartU += (a) => StartUp = true;

        Mouse0D += (a) => Mouse0Down = true;
        Mouse0U += (a) => Mouse0Up = true;
        Mouse1D += (a) => Mouse1Down = true;
        Mouse1U += (a) => Mouse1Up = true;
        Mouse2D += (a) => Mouse2Down = true;
        Mouse2U += (a) => Mouse2Up = true;

        ShiftD += (a) => ShiftDown = true;
        ShiftU += (a) => ShiftUp = true;
        #endregion

        #region 上下左右切换事件绑定 这里处理按钮相关的事件(包括十字键)，摇杆的在update里面处理
        //绑定左右输入事件 主角       
        Left_PD += (a) =>
        {
            //刷新CD
            m_CurChangeCDX_P = ChangeCDX;
            HAction_P?.Invoke(-1);
        };
        Right_PD += (a) =>
        {
            m_CurChangeCDX_P = ChangeCDX;
            HAction_P?.Invoke(1);
        };

        //绑定上下输入事件 主角
        Down_PD += (a) =>
        {
            m_CurChangeCDY_P = ChangeCDY;
            VAction_P?.Invoke(-1);
        };
        Up_PD += (a) =>
        {
            m_CurChangeCDY_P = ChangeCDY;
            VAction_P?.Invoke(1);
        };

        Dpad_PD += (a) =>
        {
            if (a.x > Sensitivity1)
            {
                m_CurChangeCDX_P = ChangeCDX;
                HAction_P?.Invoke(1);
            }
            else if (a.x < -Sensitivity1)
            {
                m_CurChangeCDX_P = ChangeCDX;
                HAction_P?.Invoke(-1);
            }
            if (a.y > Sensitivity1)
            {
                m_CurChangeCDY_P = ChangeCDY;
                VAction_P?.Invoke(1);
            }
            else if (a.y < -Sensitivity1)
            {
                m_CurChangeCDY_P = ChangeCDY;
                VAction_P?.Invoke(-1);
            }
        };


        //绑定左右输入事件 系统   
        LeftD += (a) =>
        {
            m_CurChangeCDX = ChangeCDX;
            HAction?.Invoke(-1);
        };
        RightD += (a) =>
        {
            m_CurChangeCDX = ChangeCDX;
            HAction?.Invoke(1);
        };
        //绑定上下输入事件 系统        
        DownD += (a) =>
        {
            m_CurChangeCDY = ChangeCDY;
            VAction?.Invoke(-1);
        };
        UpD += (a) =>
        {
            m_CurChangeCDY = ChangeCDY;
            VAction?.Invoke(1);
        };

        DpadD += (a) =>
        {
            if (a.x > Sensitivity1)
            {
                m_CurChangeCDX = ChangeCDX;
                HAction?.Invoke(1);
            }
            else if (a.x < -Sensitivity1)
            {
                m_CurChangeCDX = ChangeCDX;
                HAction?.Invoke(-1);
            }
            if (a.y > Sensitivity1)
            {
                m_CurChangeCDY = ChangeCDY;
                VAction?.Invoke(1);
            }
            else if (a.y < -Sensitivity1)
            {
                m_CurChangeCDY = ChangeCDY;
                VAction?.Invoke(-1);
            }
        };
        #endregion

    }

    /// <summary>
    /// 检查一下是否切换了控制器，切换了控制器就处理一下切换相关的逻辑
    /// </summary>
    /// <param name="obj"></param>
    private void CheckDevice(object obj)
    {
        //刷新计时
        CurIdleTime = 0;
        //获取当前的控制器
        Mouse mouse = Mouse.current;
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        //如果没有传入对象 那么就默认视作是键鼠
        if (obj == null)
        {
            if (!IsKeyboard)
            {
                ControllerChange?.Invoke(1);//1是键鼠 0是手柄
                IsKeyboard = true;
            }
        }
        else
        {
            InputDevice device = ((InputAction)obj).activeControl.device;
            //如果输入指令的设备是键盘或者鼠标 则视作键鼠模式 否则视作手柄模式
            if (IsKeyboard != (device == mouse || device == keyboard))
            {
                IsKeyboard = (device == mouse || device == keyboard);
                ControllerChange?.Invoke(IsKeyboard ? 1 : 0);
            }
            IsKeyboard = (device == mouse || device == keyboard);
        }

        //如果不是键鼠 就隐藏指针，且关闭UI的射线检测
        Cursor.visible = IsKeyboard;
        if (Raycaster != null)
        {
            Raycaster.enabled = IsKeyboard;
        }

    }

    /// <summary>
    /// 真正的初始化
    /// </summary>
    /// <returns></returns>
    private static IEnumerator RealInit()
    {
        //读取键位设置存档，如果没有的话就读取默认的
        if (File.Exists(Application.persistentDataPath + "/" + m_CustomSettingPath))
        {
            WWW www = new WWW(Application.persistentDataPath + "/" + m_CustomSettingPath);
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                KeyCfg = JsonMapper.ToObject<KeyCfg>(www.text);
            }
            //如果加载失败 就加载默认设置 
            else
            {
                TextAsset textAsset = AssetMgr.LoadAssetSync<TextAsset>(m_DefaultSettingPath);
                KeyCfg = JsonMapper.ToObject<KeyCfg>(textAsset.text);
            }
        }
        //如果存档文件不存在  就加载默认设置
        else
        {
            TextAsset textAsset = AssetMgr.LoadAssetSync<TextAsset>(m_DefaultSettingPath);
            KeyCfg = JsonMapper.ToObject<KeyCfg>(textAsset.text);
        }

        //读取Action的Json文件
        AssetMgr.LoadAssetAsync<TextAsset>(m_JsonPath, (asset) =>
        {
            //替换里面与键位相关的字符串
            string json = Replace(asset.text, KeyCfg);

            m_PlayerInput.actions = InputActionAsset.FromJson(json);
            //启用anction 并把各种事件注册到这里面
            m_PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            m_PlayerInput.actions.Enable();
            m_PlayerInput.onActionTriggered += Event;
        });
    }

    /// <summary>
    /// 绑定按下 和 抬起 事件
    /// </summary>
    /// <param name="context"></param>
    private static void Event(InputAction.CallbackContext context)
    {
        //按下相关
        if (context.phase == InputActionPhase.Performed)
        {
            switch (context.action.name)
            {
                #region 角色操控
                case "Up_P":
                    Up_P = context.ReadValue<float>();
                    Up_PD?.Invoke(Up_P);
                    break;
                case "Down_P":
                    Down_P = context.ReadValue<float>();
                    Down_PD?.Invoke(Down_P);
                    break;
                case "Left_P":
                    Left_P = context.ReadValue<float>();
                    Left_PD?.Invoke(Left_P);
                    break;
                case "Right_P":
                    Right_P = context.ReadValue<float>();
                    Right_PD?.Invoke(Right_P);
                    break;


                case "JoyL_P":
                    JoyL_P = context.ReadValue<Vector2>();
                    JoyL_PD?.Invoke(JoyL_P);
                    break;
                case "JoyR_P":
                    JoyR_P = context.ReadValue<Vector2>();
                    JoyR_PD?.Invoke(JoyR_P);
                    break;
                case "Dpad_P":
                    Dpad_P = context.ReadValue<Vector2>();
                    Dpad_PD?.Invoke(Dpad_P);
                    break;


                case "X_P":
                    X_P = context.ReadValue<float>();
                    X_PD?.Invoke(X_P);
                    break;
                case "Y_P":
                    Y_P = context.ReadValue<float>();
                    Y_PD?.Invoke(Y_P);
                    break;
                case "A_P":
                    A_P = context.ReadValue<float>();
                    A_PD?.Invoke(A_P);
                    break;
                case "B_P":
                    B_P = context.ReadValue<float>();
                    B_PD?.Invoke(B_P);
                    break;

                case "LT_P":
                    LT_P = context.ReadValue<float>();
                    LT_PD?.Invoke(LT_P);
                    break;
                case "RT_P":
                    RT_P = context.ReadValue<float>();
                    RT_PD?.Invoke(RT_P);
                    break;
                case "LB_P":
                    LB_P = context.ReadValue<float>();
                    LB_PD?.Invoke(LB_P);
                    break;
                case "RB_P":
                    RB_P = context.ReadValue<float>();
                    RB_PD?.Invoke(RB_P);
                    break;

                case "Back_P":
                    Back_P = context.ReadValue<float>();
                    Back_PD?.Invoke(Back_P);
                    break;
                case "Start_P":
                    Start_P = context.ReadValue<float>();
                    Start_PD?.Invoke(Start_P);
                    break;
                case "Mouse0_P":
                    Mouse0_P = context.ReadValue<float>();
                    Mouse0_PD?.Invoke(Mouse0_P);
                    break;
                case "Mouse1_P":
                    Mouse1_P = context.ReadValue<float>();
                    Mouse1_PD?.Invoke(Mouse1_P);
                    break;
                case "Mouse2_P":
                    Mouse2_P = context.ReadValue<float>();
                    Mouse2_PD?.Invoke(Mouse2_P);
                    break;
                case "Shift_P":
                    Shift_P = context.ReadValue<float>();
                    Shift_PD?.Invoke(Shift_P);
                    break;
                #endregion

                #region 系统操控
                case "Up":
                    Up = context.ReadValue<float>();
                    UpD?.Invoke(Up);
                    break;
                case "Down":
                    Down = context.ReadValue<float>();
                    DownD?.Invoke(Down);
                    break;
                case "Left":
                    Left = context.ReadValue<float>();
                    LeftD?.Invoke(Left);
                    break;
                case "Right":
                    Right = context.ReadValue<float>();
                    RightD?.Invoke(Right);
                    break;


                case "JoyL":
                    JoyL = context.ReadValue<Vector2>();
                    JoyLD?.Invoke(JoyL);
                    break;
                case "JoyR":
                    JoyR = context.ReadValue<Vector2>();
                    JoyRD?.Invoke(JoyR);
                    break;
                case "Dpad":
                    Dpad = context.ReadValue<Vector2>();
                    DpadD?.Invoke(Dpad);
                    break;


                case "X":
                    X = context.ReadValue<float>();
                    XD?.Invoke(X);
                    break;
                case "Y":
                    Y = context.ReadValue<float>();
                    YD?.Invoke(Y);
                    break;
                case "A":
                    A = context.ReadValue<float>();
                    AD?.Invoke(A);
                    break;
                case "B":
                    B = context.ReadValue<float>();
                    BD?.Invoke(B);
                    break;

                case "LT":
                    LT = context.ReadValue<float>();
                    LTD?.Invoke(LT);
                    break;
                case "RT":
                    RT = context.ReadValue<float>();
                    RTD?.Invoke(RT);
                    break;
                case "LB":
                    LB = context.ReadValue<float>();
                    LBD?.Invoke(LB);
                    break;
                case "RB":
                    RB = context.ReadValue<float>();
                    RBD?.Invoke(RB);
                    break;

                case "Back":
                    Back = context.ReadValue<float>();
                    BackD?.Invoke(Back);
                    break;
                case "Start":
                    Start = context.ReadValue<float>();
                    StartD?.Invoke(Start);
                    break;
                case "Mouse0":
                    Mouse0 = context.ReadValue<float>();
                    Mouse0D?.Invoke(Mouse0);
                    break;
                case "Mouse1":
                    Mouse1 = context.ReadValue<float>();
                    Mouse1D?.Invoke(Mouse1);
                    break;
                case "Mouse2":
                    Mouse2 = context.ReadValue<float>();
                    Mouse2D?.Invoke(Mouse2);
                    break;
                case "Shift":
                    Shift = context.ReadValue<float>();
                    ShiftD?.Invoke(Shift);
                    break;
                    #endregion

            }
        }
        //抬起相关
        if (context.phase == InputActionPhase.Canceled)
        {
            switch (context.action.name)
            {
                #region 角色操控
                case "Up_P":
                    Up_P = context.ReadValue<float>();
                    Up_PU?.Invoke(Up_P);
                    break;
                case "Down_P":
                    Down_P = context.ReadValue<float>();
                    Down_PU?.Invoke(Down_P);
                    break;
                case "Left_P":
                    Left_P = context.ReadValue<float>();
                    Left_PU?.Invoke(Left_P);
                    break;
                case "Right_P":
                    Right_P = context.ReadValue<float>();
                    Right_PU?.Invoke(Right_P);
                    break;


                case "JoyL_P":
                    JoyL_P = context.ReadValue<Vector2>();
                    JoyL_PU?.Invoke(JoyL_P);
                    break;
                case "JoyR_P":
                    JoyR_P = context.ReadValue<Vector2>();
                    JoyR_PU?.Invoke(JoyR_P);
                    break;
                case "Dpad_P":
                    Dpad_P = context.ReadValue<Vector2>();
                    Dpad_PU?.Invoke(Dpad_P);
                    break;


                case "X_P":
                    X_P = context.ReadValue<float>();
                    X_PU?.Invoke(X_P);
                    break;
                case "Y_P":
                    Y_P = context.ReadValue<float>();
                    Y_PU?.Invoke(Y_P);
                    break;
                case "A_P":
                    A_P = context.ReadValue<float>();
                    A_PU?.Invoke(A_P);
                    break;
                case "B_P":
                    B_P = context.ReadValue<float>();
                    B_PU?.Invoke(B_P);
                    break;

                case "LT_P":
                    LT_P = context.ReadValue<float>();
                    LT_PU?.Invoke(LT_P);
                    break;
                case "RT_P":
                    RT_P = context.ReadValue<float>();
                    RT_PU?.Invoke(RT_P);
                    break;
                case "LB_P":
                    LB_P = context.ReadValue<float>();
                    LB_PU?.Invoke(LB_P);
                    break;
                case "RB_P":
                    RB_P = context.ReadValue<float>();
                    RB_PU?.Invoke(RB_P);
                    break;

                case "Back_P":
                    Back_P = context.ReadValue<float>();
                    Back_PU?.Invoke(Back_P);
                    break;
                case "Start_P":
                    Start_P = context.ReadValue<float>();
                    Start_PU?.Invoke(Start_P);
                    break;
                case "Mouse0_P":
                    Mouse0_P = context.ReadValue<float>();
                    Mouse0_PU?.Invoke(Mouse0_P);
                    break;
                case "Mouse1_P":
                    Mouse1_P = context.ReadValue<float>();
                    Mouse1_PU?.Invoke(Mouse1_P);
                    break;
                case "Mouse2_P":
                    Mouse2_P = context.ReadValue<float>();
                    Mouse2_PU?.Invoke(Mouse2_P);
                    break;
                case "Shift_P":
                    Shift_P = context.ReadValue<float>();
                    Shift_PU?.Invoke(Shift_P);
                    break;
                #endregion

                #region 系统操控
                case "Up":
                    Up = context.ReadValue<float>();
                    UpU?.Invoke(Up);
                    break;
                case "Down":
                    Down = context.ReadValue<float>();
                    DownU?.Invoke(Down);
                    break;
                case "Left":
                    Left = context.ReadValue<float>();
                    LeftU?.Invoke(Left);
                    break;
                case "Right":
                    Right = context.ReadValue<float>();
                    RightU?.Invoke(Right);
                    break;


                case "JoyL":
                    JoyL = context.ReadValue<Vector2>();
                    JoyLU?.Invoke(JoyL);
                    break;
                case "JoyR":
                    JoyR = context.ReadValue<Vector2>();
                    JoyRU?.Invoke(JoyR);
                    break;
                case "Dpad":
                    Dpad = context.ReadValue<Vector2>();
                    DpadU?.Invoke(Dpad);
                    break;


                case "X":
                    X = context.ReadValue<float>();
                    XU?.Invoke(X);
                    break;
                case "Y":
                    Y = context.ReadValue<float>();
                    YU?.Invoke(Y);
                    break;
                case "A":
                    A = context.ReadValue<float>();
                    AU?.Invoke(A);
                    break;
                case "B":
                    B = context.ReadValue<float>();
                    BU?.Invoke(B);
                    break;

                case "LT":
                    LT = context.ReadValue<float>();
                    LTU?.Invoke(LT);
                    break;
                case "RT":
                    RT = context.ReadValue<float>();
                    RTU?.Invoke(RT);
                    break;
                case "LB":
                    LB = context.ReadValue<float>();
                    LBU?.Invoke(LB);
                    break;
                case "RB":
                    RB = context.ReadValue<float>();
                    RBU?.Invoke(RB);
                    break;

                case "Back":
                    Back = context.ReadValue<float>();
                    BackU?.Invoke(Back);
                    break;
                case "Start":
                    Start = context.ReadValue<float>();
                    StartU?.Invoke(Start);
                    break;
                case "Mouse0":
                    Mouse0 = context.ReadValue<float>();
                    Mouse0U?.Invoke(Mouse0);
                    break;
                case "Mouse1":
                    Mouse1 = context.ReadValue<float>();
                    Mouse1U?.Invoke(Mouse1);
                    break;
                case "Mouse2":
                    Mouse2 = context.ReadValue<float>();
                    Mouse2U?.Invoke(Mouse2);
                    break;
                case "Shift":
                    Shift = context.ReadValue<float>();
                    ShiftU?.Invoke(Shift);
                    break;
                    #endregion
            }
        }

    }
    #endregion

    #region 改键相关
    /// <summary>
    /// 尝试修改按键
    /// </summary>
    /// <param name="targetKey">目标按键名(eg.JoyL_P JoyR_P)</param>
    public async void TryChangeKeySetting(string targetKey)
    {
        //稍微延迟一下 避免将第一次点击操作识别为改键操作
        await Task.Delay(50);

        //如果要修改的是摇杆，那么走这块逻辑(摇杆不是按钮，需要特殊处理)
        if (targetKey == "JoyL_P" || targetKey == "JoyR_P")
        {
            TargetKey = targetKey;
            m_PlayerInput.onActionTriggered += TempEvent;
        }
        //修改的是按钮
        else
        {
            InputSystem.onAnyButtonPress.CallOnce((a) =>
            {
                //如果按下的是esc或者手柄B键 就无视这次改键
                if (a.path.Contains("esc") || a.path.Contains("east"))
                {
                    return;
                }

                //拼装键位路径
                string[] strs = a.path.Split('/');
                TargetKey = targetKey;

                //手柄的路径拼接
                if (!IsKeyboard)
                {
                    TargetPath = "<Gamepad>/" + strs[strs.Length - 1]; // <Gamepad>/escape
                }
                //键鼠的路径拼接
                else
                {
                    TargetPath = "<" + strs[1] + ">/" + strs[2];// <Keyboard>/leftShift
                }

                //键位合法性校验
                LegalCheck(TargetKey, TargetPath, () => { /*PausePanel.Instance.KeySettingPanel.KeySettingReset(); */});

            });
        }

    }

    /// <summary>
    /// 临时事件
    /// </summary>
    /// <param name="context"></param>
    private static void TempEvent(InputAction.CallbackContext context)
    {
        //按下相关
        if (context.phase == InputActionPhase.Performed)
        {
            switch (context.action.name)
            {
                //只响应摇杆的输入，其他输入无视(可以出个弹窗提示一下)，摇杆只允许对调，不能修改成其他按钮
                case "JoyL_P":
                case "JoyR_P":
                    string[] pathes = context.control.path.Split('/');
                    //得到当前输入的按键路径
                    string path = "<Gamepad>/" + pathes[pathes.Length - 1];
                    string oldPath = KeyCfg.Keys[TargetKey];
                    KeyCfg.Keys[TargetKey] = path;
                    if (TargetKey == "JoyL_P")
                    {
                        KeyCfg.Keys["JoyR_P"] = oldPath;
                    }
                    else
                    {
                        KeyCfg.Keys["JoyL_P"] = oldPath;
                    }
                    break;
            }
            m_PlayerInput.onActionTriggered -= TempEvent;
        }

    }

    /// <summary>
    /// 新键位合法性校验，防止键位冲突
    /// </summary>
    public void LegalCheck(string key, string value, UnityAction yes)
    {
        //如果目标按键不允许设置，就无视
        if (Bad(value, yes))
        {
            return;
        }

        //摇杆只能左右替换
        if (key.Contains("Joy") && !value.Contains("Joy"))
        {
            //不合法提示
            //DynamicPanel.Instance.ShowAlert(97, yes, true);
            return;
        }

        //如果已经包含该键
        bool contain = false;
        string tempKey = "";
        foreach (var item in KeyCfg.Keys)
        {
            //如果按键配置里配置了当前的这个按钮 且他们不属于同一个键名，说明已经在别处使用了这个按钮，且都是角色控制用的
            if (item.Value == value && item.Key != key && item.Key.Contains("P"))
            {
                contain = true;
                tempKey = item.Key;
            }
        }
        if (contain)
        {
            //问是否需要对调两个按钮
            //DynamicPanel.Instance.ShowComfirm(95, () =>
            //{
            //    //把当前key的值给另一个key  然后把当前key的值设置的给的值即可
            //    KeyCfg.Keys[tempKey] = KeyCfg.Keys[key];
            //    KeyCfg.Keys[key] = value;
            //    yes?.Invoke();
            //}, true, () => { yes?.Invoke(); });
        }
        //不包含改键  就设置
        else
        {
            KeyCfg.Keys[key] = value;
            yes?.Invoke();
        }
    }

    /// <summary>
    /// 是否是禁用的按键，不允许玩家设置的按键可以在这个函数里配置
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool Bad(string value, UnityAction yes)
    {
        //这里暂时将 鼠标右键视作不可修改按键(可根据实际需求定义)
        List<string> badKeys = new List<string> { "leftButton" };

        foreach (var a in badKeys)
        {
            if (a.Contains(value))
            {
                //禁用提示
                //DynamicPanel.Instance.ShowAlert(96, yes, true);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 保存按键设置
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public IEnumerator SaveKeySetting(Action callBack = null)
    {
        //将按键设置存到本地
        string json = JsonMapper.ToJson(KeyCfg);
        File.WriteAllText(Application.persistentDataPath + "/" + m_CustomSettingPath, json);

        //移除掉旧的事件 然后重新初始化
        callBack?.Invoke();
        yield return new WaitForSeconds(0.2f);
        m_PlayerInput.onActionTriggered -= Event;
        yield return StartCoroutine(RealInit());

    }

    /// <summary>
    /// 重置按键设置
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public IEnumerator ResetKetSetting(Action callBack = null)
    {
        //删除旧的自定义的按键设置数据
        if (File.Exists(Application.persistentDataPath + "/" + m_CustomSettingPath))
        {
            File.Delete(Application.persistentDataPath + "/" + m_CustomSettingPath);
        }
        //移除事件 然后重新初始化
        m_PlayerInput.onActionTriggered -= Event;
        yield return StartCoroutine(RealInit());
        callBack?.Invoke();
    }

    /// <summary>
    /// 将配置文本里，跟按键相关的内容替换成自定义的内容
    /// </summary>
    /// <param name="str">Json字符串</param>
    /// <param name="cfg">按键配置</param>
    /// <returns></returns>
    private static string Replace(string str, KeyCfg cfg)
    {
        string str1 = str;
        foreach (var item in cfg.Keys)
        {
            str1 = str1.Replace(item.Key, item.Value);
        }

        return str1;
    }

    /// <summary>
    /// 创建默认配置文件
    /// </summary>
    public static void CreateDefaultCfg()
    {
        //将按键设置存到本地
        KeyCfg keyCfg = new KeyCfg();
        string json = JsonMapper.ToJson(keyCfg);
        File.WriteAllText(Application.dataPath + "/" + m_DefaultSettingPath.Substring(7), json);
    }
    #endregion

    #region 好确认的部分
    #region 生命周期函数    
    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Time.timeScale = Time.timeScale == 1 ? TimeScale : 1;
        }
#endif
        //刷新CD
        m_CurChangeCDX -= TimeMgr.RealDeltaTime;
        m_CurChangeCDY -= TimeMgr.RealDeltaTime;
        m_CurChangeCDX_P -= TimeMgr.RealDeltaTime;
        m_CurChangeCDY_P -= TimeMgr.RealDeltaTime;

        //如果鼠标移动了，且当前不是键鼠的话，就切换成键鼠
        if (Mouse.current != null)
        {
            if (m_OldMousePos == DefaultDef.Vector3)
            {
                m_OldMousePos = Input.mousePosition;
            }
            m_MouseDelta = Input.mousePosition - m_OldMousePos;
            m_OldMousePos = Input.mousePosition;
            if (m_MouseDelta.magnitude > MouseMoveMin * Screen.width / 1920 && !IsKeyboard)
            {
                CheckDevice(null);
            }
        }

        //如果没有按下任何键 且鼠标没有移动 那么就累计闲置时长
        if (!Input.anyKey && m_MouseDelta.magnitude < MouseMoveMin)
        {
            CurIdleTime += Time.deltaTime;
        }
        else
        {
            CurIdleTime = 0;
        }

        if (IsHV_P1)
        {
            if (m_CurChangeCDX_P < 0)
            {
                if (IsRight_P1)
                {
                    m_CurChangeCDX_P = ChangeCDX;
                    HActionLong_P?.Invoke(1);
                }
                else if (IsLeft_P1)
                {
                    m_CurChangeCDX_P = ChangeCDX;
                    HActionLong_P?.Invoke(-1);
                }
            }
            if (m_CurChangeCDY_P < 0)
            {
                if (IsUp_P1)
                {
                    m_CurChangeCDY_P = ChangeCDY;
                    VActionLong_P?.Invoke(1);
                }
                else if (IsDown_P1)
                {
                    m_CurChangeCDY_P = ChangeCDY;
                    VActionLong_P?.Invoke(-1);
                }
            }
        }

        if (IsHV1)
        {
            if (m_CurChangeCDX < 0)
            {
                if (IsLeft1)
                {
                    m_CurChangeCDX = ChangeCDX;
                    HActionLong?.Invoke(-1);
                }
                else if (IsRight1)
                {
                    m_CurChangeCDX = ChangeCDX;
                    HActionLong?.Invoke(1);
                }
            }
            if (m_CurChangeCDY < 0)
            {
                if (IsUp1)
                {
                    m_CurChangeCDY = ChangeCDY;
                    VActionLong?.Invoke(1);
                }
                else if (IsDown1)
                {
                    m_CurChangeCDY = ChangeCDY;
                    VActionLong?.Invoke(-1);
                }
            }
        }

    }

    private void LateUpdate()
    {
        #region 角色 Up Down 状态重置
        Up_PDown = false;
        Up_PUp = false;
        Down_PDown = false;
        Down_PUp = false;
        Left_PDown = false;
        Left_PUp = false;
        Right_PDown = false;
        Right_PUp = false;

        JoyL_PDown = false;
        JoyL_PUp = false;
        JoyR_PDown = false;
        JoyR_PUp = false;
        Dpad_PDown = false;
        Dpad_PUp = false;

        X_PDown = false;
        X_PUp = false;
        Y_PDown = false;
        Y_PUp = false;
        A_PDown = false;
        A_PUp = false;
        B_PDown = false;
        B_PUp = false;

        LT_PDown = false;
        LT_PUp = false;
        RT_PDown = false;
        RT_PUp = false;
        LB_PDown = false;
        LB_PUp = false;
        RB_PDown = false;
        RB_PUp = false;

        Back_PDown = false;
        Back_PUp = false;
        Start_PDown = false;
        Start_PUp = false;

        Mouse0_PDown = false;
        Mouse0_PUp = false;
        Mouse1_PDown = false;
        Mouse1_PUp = false;
        Mouse2_PDown = false;
        Mouse2_PUp = false;
        Shift_PUp = false;
        #endregion

        #region 系统 Up Down 状态重置
        UpDown = false;
        UpUp = false;
        DownDown = false;
        DownUp = false;
        LeftDown = false;
        LeftUp = false;
        RightDown = false;
        RightUp = false;

        JoyLDown = false;
        JoyLUp = false;
        JoyRDown = false;
        JoyRUp = false;
        DpadDown = false;
        DpadUp = false;

        XDown = false;
        XUp = false;
        YDown = false;
        YUp = false;
        ADown = false;
        AUp = false;
        BDown = false;
        BUp = false;

        LTDown = false;
        LTUp = false;
        RTDown = false;
        RTUp = false;
        LBDown = false;
        LBUp = false;
        RBDown = false;
        RBUp = false;

        BackDown = false;
        BackUp = false;
        StartDown = false;
        StartUp = false;

        Mouse0Down = false;
        Mouse0Up = false;
        Mouse1Down = false;
        Mouse1Up = false;
        Mouse2Down = false;
        Mouse2Up = false;
        ShiftUp = false;
        #endregion
    }

    private void OnApplicationFocus(bool focus)
    {
        if (m_PlayerInput != null)
        {
            m_PlayerInput.enabled = focus;
        }

        m_Focus = focus;
        if (!focus)
        {
            #region 角色输入重置
            Up_P = 0;
            Down_P = 0;
            Left_P = 0;
            Right_P = 0;
            JoyL_P = Vector2.zero;
            JoyR_P = Vector2.zero;
            Dpad_P = Vector2.zero;
            X_P = 0;
            Y_P = 0;
            A_P = 0;
            B_P = 0;
            LT_P = 0;
            RT_P = 0;
            LB_P = 0;
            RB_P = 0;
            Back_P = 0;
            Start_P = 0;
            Mouse0_P = 0;
            Mouse1_P = 0;
            Mouse2_P = 0;
            Shift_P = 0;
            #endregion

            #region 系统输入重置
            Up = 0;
            Down = 0;
            Left = 0;
            Right = 0;
            JoyL = Vector2.zero;
            JoyR = Vector2.zero;
            Dpad = Vector2.zero;
            X = 0;
            Y = 0;
            A = 0;
            B = 0;
            LT = 0;
            RT = 0;
            LB = 0;
            RB = 0;
            Back = 0;
            Start = 0;
            Mouse0 = 0;
            Mouse1 = 0;
            Mouse2 = 0;
            Shift = 0;
            #endregion
        }
    }
    #endregion
    #endregion
}
