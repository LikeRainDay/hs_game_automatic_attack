using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 一波敌人的数据
/// </summary>
[System.Serializable]
public struct WaveData
{
    [FieldName("随机预制体顺序")]
    public bool Random;
    [FieldName("随机范围，不填就在大范围内随机")]
    public Vector2 RandomRange;
    [Header("要创建的敌人预制体")]
    public List<GameObject> Enemys;
    [Header("创建敌人的时间点")]
    public List<float> TimePoint;
}
/// <summary>
/// 极简创建敌人的工具脚本
/// </summary>
public class EnemyCreater : MonoBehaviour
{
    [FieldName("此波(关)时长")]
    public float WaveDuration;
    [Header("可随机范围，不填就都创建在此脚本挂载的物体的位置")]
    public Vector4 Range;
    [Header("下一波时间点，不填就通过其他方式下一波")]
    public List<float> NextWaveTimePoint;
    [Header("敌人创建配置")]
    public List<WaveData> Datas;

    private Coroutine CreateCo;//协程引用，创建下一波的时候先关闭一下之前的协程函数
    private float m_TreeCreateInterval = 20;//平均多久创建一颗树木
    private float m_CurTime;//等待多久创建下一颗树木
    [HideInInspector] public List<GameObject> CurCreateEnemys = new List<GameObject>();//存放当前创建的敌人，后续有可能会对已经创建出来的敌人进行操作

    private int m_CurWaveIndex = -1;//当前波次
    /// <summary>
    /// 当前波次索引，外部会根据情况修改这个值，这个值改动的时候 会创建当前波次的敌人
    /// </summary>
    public int CurWaveIndex
    {
        get => m_CurWaveIndex;
        set
        {
            m_CurWaveIndex = value;
            //关闭上一波的协程函数
            if (CreateCo != null)
            {
                StopCoroutine(CreateCo);
            }
            //如果战斗已经结束 就不再创建
            if (!BattleManager.Instance.Battle) return;
            //开始创建此波敌人
            CreateCo = StartCoroutine(CreateOneWaveEnemy());
        }
    }

    private IEnumerator Start()
    {
        //开启创建树木的任务
        StartCoroutine(TreeCreateTask());
        //开始倒计时
        BattlePage.Instance.StartCountDown(WaveDuration);
        //每隔给定时长，就更新波次来创建下一波敌人
        if (NextWaveTimePoint != null && NextWaveTimePoint.Count > 0)
        {
            for (int i = 0; i < NextWaveTimePoint.Count; i++)
            {
                yield return new WaitForSeconds(i == 0 ? NextWaveTimePoint[i] : NextWaveTimePoint[i] - NextWaveTimePoint[i - 1]);
                CurWaveIndex++;
            }
        }
    }
    /// <summary>
    /// 每隔一个时长，创建一颗树木
    /// </summary>
    /// <returns></returns>
    private IEnumerator TreeCreateTask()
    {
        while (true)
        {
            //随机一个时长  然后等待这个时长创建一颗树木
            m_CurTime = UnityEngine.Random.Range(0, 2 * m_TreeCreateInterval / (Player.Instance.Trees + 1));
            yield return new WaitForSeconds(m_CurTime);
            StartCoroutine(CreateTree());
        }
    }

    /// <summary>
    /// 创建树木
    /// </summary>
    private IEnumerator CreateTree()
    {
        //创建一个树木 
        GameObject tree = Instantiate(GameManager.Instance.Tree);
        tree.Hide();//先隐藏
        tree.transform.position = Range.GetRandomV2FromV4();//随机坐标
        ToolFun.ShowEffect(GameManager.Instance.GreenFork, tree.transform.position, UnityEngine.Random.Range(0, 360));//创建一个绿叉在树木的位置
        yield return new WaitForSeconds(1);//等待一秒之后显示树木
        tree.Show();

    }

    private void Update()
    {
        //如果战斗结束了 就停止协程，以及销毁此物体
        if (!BattleManager.Instance.Battle)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 创建一波敌人
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateOneWaveEnemy()
    {
        //如果要创建的波次已达数组上限就无视
        if (Datas.Count <= CurWaveIndex) yield break;

        //拿到当前波次的数据 按顺序将敌人在配置的时间点创建出来
        WaveData data = Datas[CurWaveIndex];
        //如果需要随机创建组内的敌人的话 就打乱一下预制体数组
        if (data.Random)
        {
            data.Enemys = data.Enemys.Shuffle();
        }
        Vector2 range = Range.GetRandomV2FromV4();//从范围里随机一个中心点
        //遍历数组 挨个将敌人创建出来
        for (int i = 0; i < data.Enemys.Count; i++)
        {
            //每次等待后一个时间点减去前一个时间点的时间
            yield return new WaitForSeconds(i == 0 ? data.TimePoint[i] : data.TimePoint[i] - data.TimePoint[i - 1]);

            GameObject enemy = Instantiate(data.Enemys[i]);
            if (data.RandomRange != Vector2.zero)
            {
                //将敌人创建到前面随机出来的中心点 然后波动一下位置
                enemy.transform.position = Range == Vector4.zero ? transform.position : new Vector3(range.x +
                    UnityEngine.Random.Range(-data.RandomRange.x / 2, data.RandomRange.x / 2), range.y + UnityEngine.Random.Range(-data.RandomRange.y / 2, data.RandomRange.y / 2));
            }
            else
            {
                if (Range != Vector4.zero)
                {
                    enemy.transform.position = Range.GetRandomV2FromV4();//从范围里随机一个点
                }
                else
                {
                    enemy.transform.position = transform.position;
                }

            }

            //先隐藏 延迟一会儿再显示  延迟期间创建一个红叉到敌人所在位置
            enemy.Hide();
            TimeMgr.Timer.AddTimeTask(a =>
            {
                if (enemy != null)
                {
                    enemy.Show();
                }
            }, 1200);
            ToolFun.ShowEffect(GameManager.Instance.RedFork, enemy.transform.position, UnityEngine.Random.Range(0, 360));
            //加入数组
            CurCreateEnemys.Add(enemy);
        }

    }

}
