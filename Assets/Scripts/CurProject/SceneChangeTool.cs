using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 切换场景背景 以及创建当前波次对应的敌人生成器
/// </summary>
public class SceneChangeTool : MonoBehaviour
{
    [Header("随机背景图片")]
    public List<Sprite> Bgs;
    [Header("敌人生成管理器")]
    public List<GameObject> Waves;
    private static Sprite m_Old;//记录上一波的背景，下一波随到跟上一波一样的会重新随机
    public static SceneChangeTool Instance;
    private void Start()
    {
        Instance = this;
        //随机一张贴图并记录下来
        Sprite sp = Bgs.RandomFromList(m_Old);
        m_Old = sp;
        GetComponent<SpriteRenderer>().sprite = sp;
        //创建当前波次对应的敌人生成器
        Instantiate(Waves[Player.Instance.CurWave - 1]);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

}
