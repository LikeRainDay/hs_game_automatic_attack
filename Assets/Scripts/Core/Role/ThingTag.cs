using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏内各种对象的标签，包括不动的和会动的
/// </summary>
public enum ThingTag
{
    [EnumName("无")]
    None = 0,
    [EnumName("非角色")]
    UnRole = 1,
    [EnumName("石头")]
    Stone = 10,
    [EnumName("水")]
    Water = 20,
    [EnumName("树木")]
    Tree = 30,
    [EnumName("地面")]
    Ground = 40,
    [EnumName("角色")]
    Role = 1000,
    [EnumName("狮子")]
    Lion = 1010,
    [EnumName("狼")]
    Wolf = 1020,
    [EnumName("老虎")]
    Tiger = 1030,
    [EnumName("蛇")]
    Snake = 1040,
    [EnumName("猴子")]
    Monkey = 1050,
    [EnumName("猫头鹰")]
    Owl = 1060,
    [EnumName("鹿")]
    Deer = 1070,

    //特殊
    [EnumName("Boss")]
    Boss = 9990,
    [EnumName("精英")]
    Elite = 9991,
    [EnumName("代理(马甲)")]
    Agent = 9999,
}

