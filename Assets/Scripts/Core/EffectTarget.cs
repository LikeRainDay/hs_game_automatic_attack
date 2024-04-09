using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 效果作用目标的类型枚举
/// </summary>
public enum EffectTarget
{
    [EnumName("自身")]
    Self = 0,
    [EnumName("友军")]
    Partner = 10,
    [EnumName("友军但不包含自身")]
    PartnerNotContainSelf = 11,
    [EnumName("敌军")]
    Enemy = 20,
    [EnumName("全军")]
    All = 30,
    [EnumName("全军但不包含自身")]
    AllNotContainSelf = 31,
}
