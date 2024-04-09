using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取点位的方式
/// </summary>
public enum GetPointMode
{
    /// <summary>
    /// 随机
    /// </summary>
    [EnumName("随机")]
    Random = 10,
    /// <summary>
    /// 就近
    /// </summary>
    [EnumName("就近")]
    Nearby = 20,
    /// <summary>
    /// 就远
    /// </summary>
    [EnumName("就远")]
    Far = 30,
}
