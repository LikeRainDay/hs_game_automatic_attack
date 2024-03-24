using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 阵营枚举，一个对象可以有多个阵营，两个对象如果有共同的阵营就视作友军，否则就视作敌人
/// </summary>
public enum Camp
{
    [EnumName("无")]
    None = 0,
    [EnumName("蓝方")]
    Blue = 10,
    [EnumName("红方")]
    Red = 20,
    [EnumName("绿方")]
    Green = 30,
}
