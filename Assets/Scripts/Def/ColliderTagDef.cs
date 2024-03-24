using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 碰撞器标签定义
/// </summary>
public class ColliderTagDef
{
    /// <summary>
    /// 主角的碰撞器标签
    /// </summary>
    public static string Player = "Player";

    /// <summary>
    /// 敌人的碰撞器标签
    /// </summary>
    public static string Enemy = "Enemy";

    /// <summary>
    /// 主角对敌人造成伤害的碰撞器标签
    /// </summary>
    public static string PlayerAttack = "PlayerAttack";

    /// <summary>
    /// 敌人对主角造成伤害的碰撞器标签
    /// </summary>
    public static string EnemyAttack = "EnemyAttack";

    /// <summary>
    /// 奖励物品
    /// </summary>
    public static string Reward = "Reward";

}
