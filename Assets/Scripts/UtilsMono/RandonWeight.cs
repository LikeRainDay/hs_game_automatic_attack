using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// 根据权重 随机获取一个游戏对象
/// </summary>
public struct RandonWeight
{
    [FieldName("预制体")]
    public GameObject Prefab;
    [FieldName("权重")]
    public float Weight;

    /// <summary>
    /// 传入一组配置，按照权重随机返回一个对象
    /// </summary>
    /// <param name="randonWeights">配置数据数组</param>
    /// <returns></returns>
    public static GameObject GetRandom(List<RandonWeight> randonWeights)
    {
        //先计算总权重
        float totalWeight = 0;
        foreach (var item in randonWeights)
        {
            totalWeight += item.Weight;
        }
        //在总权重范围内随机一个值 落到什么区间 就返回对应区间的预制体
        float random = UnityEngine.Random.Range(0, totalWeight);
        float temp = 0;
        foreach (var item in randonWeights)
        {
            temp += item.Weight;
            if (temp > random)
            {
                return item.Prefab;
            }
        }
        return randonWeights[0].Prefab;
    }

}
