using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 事件持有者类型
/// </summary>
public enum OwnerType
{
    None,
    主角 = 10,
    主角召唤物 = 11,
    敌人 = 20,
    敌人召唤物 = 21,
    BOSS = 30,
}
/// <summary>
/// 工具函数
/// </summary>
public class ToolFun : MonoBehaviour
{
    #region 独立小功能
    /// <summary>
    /// 用传入的参数构建一个二维向量
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2 NewVector2(object x, object y)
    {
        return new Vector2(x.ToFloat(), y.ToFloat());
    }

    /// <summary>
    /// 用传入的参数构建一个三维向量
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 NewVector3(object x, object y, object z = null)
    {
        return new Vector3(x.ToFloat(), y.ToFloat(), z.ToFloat());
    }

    /// <summary>
    /// 用传入的参数构建一个四维向量
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Vector4 NewVector4(object x, object y, object z = null, object w = null)
    {
        return new Vector4(x.ToFloat(), y.ToFloat(), z.ToFloat(), w.ToFloat());
    }

    /// <summary>
    /// 将值映射到新的范围
    /// </summary>
    /// <param name="value">当前值</param>
    /// <param name="oldMin">旧范围下限</param>
    /// <param name="oldMax">旧范围上限</param>
    /// <param name="newMin">新范围下限</param>
    /// <param name="newMax">新范围上限</param>
    /// <returns></returns>
    public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float rate = (value - oldMin) / (oldMax - oldMin);
        return newMin + rate * (newMax - newMin);
    }

    /// <summary>
    /// 椭圆上的一点，单位度
    /// </summary>
    /// <param name="a">横轴长</param>
    /// <param name="b">竖轴长</param>
    /// <param name="angle">椭圆上的点与中心点的连线和X轴的夹角</param>
    /// <param name="rotate">椭圆相对于X轴的旋转角度</param>
    /// <returns></returns>
    public static Vector2 GetArcPoint(float a, float b, float angle, float rotate = 0)
    {
        //先转换成弧度
        angle = angle * Mathf.PI / 180;
        float x = a * Mathf.Cos(angle);
        float y = b * Mathf.Sin(angle);
        if (rotate != 0)
        {
            rotate = rotate * Mathf.PI / 180;
            double xPrime = x * Math.Cos(rotate) - y * Math.Sin(rotate); // x'坐标  
            double yPrime = x * Math.Sin(rotate) + y * Math.Cos(rotate); // y'坐标  
            x = (float)xPrime;
            y = (float)yPrime;
        }
        return new Vector2(x, y);
    }

    /// <summary>
    /// 以给定范围内数字随机创建一个数组
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="count">元素个数</param>
    /// <param name="minInterval">元素之间的最小间隔</param>
    /// <param name="maxRandomTimes">每个元素最大随机次数，防止死循环</param>
    /// <returns></returns>
    public static List<float> GetListInRange(float min, float max, int count, float minInterval = 0, float maxRandomTimes = 100)
    {
        int curRandomTimes = 0;//当前随机次数
        List<float> randomList = new List<float>();
        for (int i = 0; i < count; i++)
        {
            curRandomTimes = 0;
            float random = min < max ? UnityEngine.Random.Range(min, max) : UnityEngine.Random.Range(max, min);
            while (Contain(randomList, random, minInterval) && curRandomTimes < maxRandomTimes)
            {
                random = min < max ? UnityEngine.Random.Range(min, max) : UnityEngine.Random.Range(max, min);
                curRandomTimes++;
            }
            randomList.Add(random);
        }
        return randomList;
    }

    /// <summary>
    /// 数组里是否包含与新元素误差小于给定值的元素
    /// </summary>
    /// <param name="list"></param>
    /// <param name="newNum">新元素的值</param>
    /// <param name="errorRange">误差范围</param>
    /// <returns></returns>
    public static bool Contain(List<float> list, float newNum, float errorRange)
    {
        foreach (var item in list)
        {
            if (Mathf.Abs(item - newNum) <= errorRange)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 从两个角度里选择一个更接近当前角度的
    /// </summary>
    /// <param name="curAngle">当前角度</param>
    /// <param name="target1">目标角度1</param>
    /// <param name="target2">目标角度2</param>
    /// <returns></returns>
    public static float GetNearestAngle(float curAngle, float target1, float target2)
    {
        //将角度换算成向量 然后通过判断向量与向量之间的夹角来判断哪个角度更加接近当前角度
        Vector2 v1 = curAngle.GetUnitVector2ByAngle();
        Vector2 v2 = target1.GetUnitVector2ByAngle();
        Vector2 v3 = target2.GetUnitVector2ByAngle();

        float angle1 = Vector2.SignedAngle(v1, v2);
        float angle2 = Vector2.SignedAngle(v1, v3);

        return Mathf.Abs(angle1) < Mathf.Abs(angle2) ? target1 : target2;
    }

    /// <summary>
    /// 获取两个角度之间的夹角(取较小的那个方向的夹角)
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static float GetAngleDistance(float angle1, float angle2)
    {
        if (Mathf.Abs(angle1 - angle2) > 180)
        {
            return 360 - Mathf.Abs(angle1 - angle2);
        }
        return Mathf.Abs(angle1 - angle2);
    }

    /// <summary>
    /// 创建一个特效到目标位置
    /// </summary>
    /// <param name="prefab">特效预制体</param>
    /// <param name="pos">创建坐标</param>
    /// <param name="angle">特效的角度，为零代表不修改角度</param>
    /// <param name="autoDesTime">自毁延时，不填不自毁</param>
    /// <returns></returns>
    public static GameObject ShowEffect(GameObject prefab, Vector3 pos, float angle = 0, float autoDesTime = 5)
    {
        if (prefab == null) return null;
        GameObject go = Instantiate(prefab);
        go.SetActive(true);
        go.transform.position = pos;
        if (angle != 0)
        {
            go.transform.eulerAngles = new Vector3(0, 0, angle);
        }
        if (autoDesTime != 0)
        {
            Destroy(go, autoDesTime);
        }
        return go;
    }

    /// <summary>
    /// 创建一个特效到目标位置
    /// </summary>
    /// <param name="pos">特效生成位置</param>
    /// <param name="poolName">对象池名字</param>
    /// <param name="angle">特效的角度，为零代表不修改角度</param>
    /// <param name="recycleTime">多久回收到池子</param>
    public static GameObject ShowEffect(string poolName, Vector2 pos, float angle = 0, float recycleTime = 2)
    {
        if (string.IsNullOrEmpty(poolName)) return null;
        GameObject go = PoolMgr.Spawn(poolName);
        go.SetActive(true);
        go.transform.position = pos;
        if (angle != 0)
        {
            go.transform.eulerAngles = new Vector3(0, 0, angle);
        }
        TimeMgr.Timer.AddTimeTask(a => PoolMgr.Recycle(go, poolName), recycleTime * 1000);
        return go;
    }

    /// <summary>
    /// 给定坐标是否在某个上下左右范围内
    /// </summary>
    /// <param name="pos">需要检测的坐标</param>
    /// <param name="range">范围 上下左右</param>
    /// <returns></returns>
    public static bool InRange(Vector2 pos, Vector4 range)
    {
        if (pos.x < range.w && pos.x > range.z && pos.y < range.x && pos.y > range.y)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 给定坐标是否在某个上下左右范围内
    /// </summary>
    /// <param name="pos">需要检测的坐标</param>
    /// <param name="range">范围 上下左右</param>
    /// <returns></returns>
    public static bool InRange(Vector3 pos, Vector4 range)
    {
        if (pos.x < range.w && pos.x > range.z && pos.y < range.x && pos.y > range.y)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 事件
    //【队列，[XXX]，[XXX]】 内容里包含这串字符的事件视作队列事件，代码检测到队列后，会自动为事件加入这个标识以便后续判断。队列事件会有先后顺序，前面的执行完了才会执行后面的
    public static string QueueSign = "队列事件标识";
    public static int QueueIndex;//队列事件的索引 每次索引+1之后 才会进行队列事件后面的事件 队列事件同时只能存在一个，大家共用一个索引，所以一般用于演出(如果需要用到同时多个队列的情况，那么类似保底事件将这个变量改成字典形式即可)
    //保底事件计数用的字典，管理器会记录保底事件的失败次数，次数达到一定程度下次会必定成功 不论什么情况下的成功 都会重置保底
    //键是标识，值是这个标识当前未触发的次数。比如 武器SSR，5 武器已经5次没出SSR了
    public static Dictionary<string, int> CountDic = new Dictionary<string, int>();//如果需要将数据存到存档里，那么在选择存档后，将存档里的数据注入到这个字典  以及保存数据的时候，将这里的数据存入存档即可

    /// <summary>
    /// 执行判断事件，返回一个True or False，如果参数字符串为空或者空字符串则返回True，因为相当于没有条件
    /// </summary>
    /// <param name="eventStr">事件字符串</param>
    /// <returns></returns>
    public static bool ExecuteJudgeEvents(string eventStr)
    {
        //如果字符串为空则直接返回true  表示条件判断通过了，因为没有条件
        if (string.IsNullOrEmpty(eventStr)) return true;

        //分割字符串 得到多个判断事件  【】。【】。【】
        string[] events = eventStr.Split('。');
        //【或】。【XXX】。【XXXXXX】 如果数组里第一个元素是 或 那么一开始是false 后续事件有一个判断通过就是true 反之
        bool result = events[0].Replace("【", "").Replace("】", "") == "或" ? false : true;
        bool and = result;//是否是 与判断  且(and)  因为或判断的时候result初始值为false 与判断的时候为true，因此这里直接将result的值拿来赋值即可  

        //遍历执行子事件。如果是与判断，那么子事件有一个false就直接返回false，如果是或判断，子事件有一个true就直接返回true即可
        foreach (var item in events)
        {
            //将可能用于辅助区分事件的【】去掉   例如：【血量，大于，3】。【能量，小于，5】
            string tempEvent = item.Replace("【", "").Replace("】", "");
            //无视掉开头可能存在的用于标识 与 或 的事件 【与】 【或】
            if (tempEvent == "或" || tempEvent == "与") continue;

            //分割参数，将一长串字符串分割成事件的若干个参数  【与判断，[事件响应者，血量，大于，5]，[关卡数，大于3]，[事件响应者，血量，等于，3]】            
            string firstParam = tempEvent.Split(tempEvent.Contains("，") ? '，' : ',')[0];
            //同上 如果是或判断 一开始是false 与判断一开始true
            bool result1 = firstParam == "或判断" ? false : true;
            bool and1 = result1;
            //把用于标识 与 或 的字符替换掉
            tempEvent = tempEvent.Replace("或判断,", "").Replace("或判断，", "").Replace("与判断,", "").Replace("与判断，", "");

            //分割出若干个小的判断事件 [事件响应者，血量，大于，5]，[关卡数，大于3]，[事件响应者，血量，等于，3]
            string[] events1 = tempEvent.SplitEventStr();
            //遍历执行这些小的判断事件
            foreach (var item1 in events1)
            {
                //[事件响应者，血量，大于，5] 分割参数
                string[] args = item1.Split(item1.Contains('，') ? '，' : ',');

                //判断一下 第一个参数是否是用于标识事件对象的 如果是的话 后续使用参数的时候 都需要向后顺延一位
                int containOwner = ContainOwner(args[0]);
                //如果是与判断 并且如果某个小事件的判断返回的是false的话  就修改标识为false  因为与判断失败一个就失败了
                if (and1 && !(bool)EventMgr.ExecuteEvent(args[0 + containOwner], CreateParamsList(args)))
                {
                    result1 = false;
                }
                //如果是或判断 并且某个小事件的判断返回的是true的话 就修改标识为true 因为或判断成功一个就成功了
                else if (!and1 && (bool)EventMgr.ExecuteEvent(args[0 + containOwner], CreateParamsList(args)))
                {
                    result1 = true;
                }
            }
            //如果是与判断 且标识是false 就返回false
            if (and && !result1)
            {
                return false;
            }
            //如果是或判断 且标识是true 就返回true
            if (!and && result1)
            {
                return true;
            }
        }

        //如果执行到这里 就返回默认的值 与的默认值是true 或的默认值是false
        return result;
    }

    /// <summary>
    /// 执行一组事件
    /// </summary>
    /// <param name="eventStr">事件字符串</param>
    /// <param name="taskID">延时任务ID，用于删除还未执行的延时事件任务</param>
    /// <param name="queue">是否是队列事件，队列事件会等待前一个事件执行完毕才会继续执行后续事件</param>
    public static async void ExecuteEvents(string eventStr, List<int> taskID = null, bool queue = false)
    {
        //如果是空字符串就不进行后续逻辑
        if (string.IsNullOrEmpty(eventStr)) return;

        //将用于标识这是一个事件的字符串去掉  【Event，添加Buff，5(buffID)，2(添加层数)】 
        eventStr = eventStr.Replace("Event，", "").Replace("Event,", "");

        //将字符串分割成若干个子事件 
        string[] events = eventStr.Split('。');
        //如果延时任务id数组为空的话则会new一个
        if (taskID == null) taskID = new List<int>();

        //遍历执行事件
        foreach (var item in events)
        {
            //将可能用于辅助区分事件的【】去掉   例如：【Event，添加Buff，5(buffID)，2(添加层数)】 
            string tempEvent = item.Replace("【", "").Replace("】", "");
            //拿到第一个参数 确认事件类型 
            string firstParam = tempEvent.Split(tempEvent.Contains("，") ? '，' : ',')[0];

            //与事件需要左侧条件全部满足才执行右侧的事件  或事件只要左侧的条件满足一个就执行右侧的事件
            //【与事件，[[血量，大于，5]，[血量，等于，3]]，[[Event，血量，+5]，[Event，创建冲击波]]】
            if (firstParam == "或事件" || firstParam == "与事件")
            {
                //或事件一开始是false 满足一个条件结果就是true 反之
                bool result = firstParam == "或事件" ? false : true;
                bool and = result;
                //把标识用的字符去掉
                tempEvent = tempEvent.Replace("与事件，", "").Replace("与事件,", "").Replace("或事件，", "").Replace("或事件,", "");
                //这里的分割会将数据分割成  条件事件组  和 具体要执行的事件组 两部分
                string[] part = tempEvent.SplitEventStr();
                //执行条件事件 返回True or False
                Judge(and, out result, part[0]);
                //如果条件判断成功了 就执行后边块里的全部事件
                if (result)
                {
                    //先将后边块的内容分割成若干个小事件
                    string[] realEvents = part[1].SplitEventStr();
                    //遍历执行事件
                    foreach (var item1 in realEvents)
                    {
                        //上面已经设置过对象 所以这里最后一个参数给false，避免重复设置浪费性能
                        ExecuteEvents(item1, taskID, false);
                    }
                }
            }
            //【概率事件，[Event，攻击者ID，30-血量，5]，[Event，被攻者ID，30-血量，-3]，[20]】 
            // 30-血量 事件名左侧的数字代表这个事件触发的权重 [20]这种子事件代表概率触发的空事件  
            else if (firstParam == "概率事件")
            {
                ExecutePEvent(tempEvent, taskID, queue);
            }
            //【保底事件，[抽卡]，[1000，100，10，9999]，[Event，1-抽卡，SSR]，[Event，4-抽卡，SR]，[Event，15-抽卡，R]，[Event，80-抽卡，N]】 
            //[抽卡] 这个用于标识这个保底事件的类型(使用场合)  
            //[1000,100,10,9999] 代表后面各个事件 分别几次触发保底
            //[Event，1-抽卡，SSR] 事件名左侧的数字代表触发这个事件的权重 类似概率事件
            else if (firstParam == "保底事件")
            {
                ExecuteBDEvent(tempEvent, taskID, queue);
            }
            //【队列事件，[打开对话，10101]，[执行演出，10101]，[打开界面，留声机]】 
            //队列事件会等待上一个事件执行完毕才执行下一个事件
            //队列事件结束的时候 需要手动让 QueueIndex++ 以便结束当前事件
            else if (firstParam == "队列事件")
            {
                //将用于标识的字符串去除
                tempEvent = tempEvent.Replace("队列事件，", "").Replace("队列事件,", "");
                //拿到队列里面的每个子事件
                string[] part = tempEvent.SplitEventStr();
                //遍历执行子事件
                foreach (var item1 in part)
                {
                    //先记录一下当前队列ID
                    int oldQueueIndex = QueueIndex;
                    //执行事件
                    ExecuteEvents(item1, taskID, true);
                    //事件执行完毕的时候 会让队列索引自增1  所以在队列索引自增之前 就一直等待
                    while (QueueIndex == oldQueueIndex)
                    {
                        await Task.Delay(50);
                    }
                }
            }
            //普通事件
            else
            {
                //先去掉辅助用的括号 然后根据情况按照中英文逗号分割获的参数数组    再把记录延时任务id的数组也作为参数传入进去即可
                ExecuteEvent(item.Replace("【", "").Replace("】", "").Split(item.Contains("，") ? '，' : ','), taskID, queue);
            }
        }

    }

    /// <summary>
    /// 执行普通事件
    /// </summary>
    /// <param name="strs">事件参数数组</param>
    /// <param name="taskID">延时id数组</param>
    /// <param name="queue">是否是队列事件</param>
    private static void ExecuteEvent(string[] strs, List<int> taskID, bool queue = false)
    {
        //如果是队列的话 那么最后一个参数会加一些标识符  具体函数里面会先检测一下最后一个参数是否有这个标识符 如果有的话 那么就是队列事件
        if (queue)
        {
            //第一个参数要作为事件名 因此如果加了标识会无法将事件分发到位，所以将标识加在最后一个参数比较合适，但也意味着队列事件对应的函数至少有两个参数才行
            strs[strs.Length - 1] = strs[strs.Length - 1] + QueueSign;
        }

        //判断参数里是否包含事件对象 包含的话 使用参数时，索引顺延一位
        int containOwner = ContainOwner(strs[0].ToString());
        //如果事件名所在位置的参数里包含 - 符号，表示这是一个延时事件  创建一个敌人-0.5  0.5S后执行创建一个敌人这个事件  
        if (strs[0 + containOwner].Contains("-"))
        {
            //分割一下字符串  一共三种情况 50-血量-3  50-血量 血量-3
            string[] strs1 = strs[0 + containOwner].Split('-');
            //50-血量-3  参数2是事件名 参数3是延时
            if (strs1.Length == 3)
            {
                taskID.Add(TimeMgr.Timer.AddTimeTask((a) => { EventMgr.ExecuteEvent(strs1[1], strs); },
                   strs1[2].ToFloat() * 1000));
            }
            //50-血量  参数2是事件名
            else if (Regex.IsMatch(strs1[0], @"^[0-9]*$"))
            {
                EventMgr.ExecuteEvent(strs1[1], strs);
            }
            //血量-3 参数1是事件名 参数2是延时
            else
            {
                taskID.Add(TimeMgr.Timer.AddTimeTask((a) => { EventMgr.ExecuteEvent(strs1[0], strs); },
                   strs1[1].ToFloat() * 1000));
            }
        }
        else
        {
            EventMgr.ExecuteEvent(strs[0 + containOwner], strs);
        }

    }

    /// <summary>
    /// 执行概率事件
    /// </summary>
    /// <param name="tempEvent">事件字符串</param>
    /// <param name="taskID">延时id数组</param>
    /// <param name="queue">是否是队列事件</param>
    private static void ExecutePEvent(string tempEvent, List<int> taskID, bool queue = false)
    {
        //【概率事件，[攻击者ID，30-血量，5]，[被攻者ID，30-血量，-3]，[20]】
        //事件名左侧的数字是执行这个事件的权重    [20]   代表一个空事件 也就是不执行事件的权重的20     
        //攻击者有3/8概率血量+5  被攻者有3/8概率血量-3  有2/8概率无事发生

        //去除标识用的字符串 然后分割事件字符串 得到若干个子事件
        tempEvent = tempEvent.Replace("概率事件，", "").Replace("概率事件,", "");
        string[] pEvents = tempEvent.SplitEventStr();

        //得到全部事件的权重值 
        List<float> weights = new List<float>();
        foreach (var item1 in pEvents)
        {
            //分割获取子事件参数
            string[] strs = item1.Split(item1.Contains("，") ? '，' : ',');
            //如果只有一个参数 且是数字 那么就是权重值
            if (strs.Length == 1 && Regex.IsMatch(strs[0], @"^[0-9]*$"))
            {
                weights.Add(strs[0].ToFloat());
                continue;
            }
            //30-血量  第一个带有 - 且是数字开头的参数的 - 前面部分作为权重值   
            foreach (var item2 in strs)
            {
                if (item2.Contains("-") && Regex.IsMatch(item2[0].ToString(), @"^[0-9]*$"))
                {
                    weights.Add(item2.Split('-')[0].ToFloat());
                    break;
                }
            }
        }

        //随机一个值 根据这个值落的位置来确定执行哪个事件
        float total = weights.Sum();
        UnityEngine.Random.InitState(int.Parse(DateTime.Now.ToString("HHmmssfff")));
        float random = UnityEngine.Random.Range(0, total);
        float current = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            current += weights[i];
            if (random < current)
            {
                ExecuteEvents(pEvents[i], taskID, queue);
                break;
            }
        }
    }

    /// <summary>
    /// 执行保底事件
    /// </summary>
    /// <param name="tempEvent">事件字符串</param>
    /// <param name="taskID">延时id数组</param>
    /// <param name="queue">是否是队列事件</param>
    private static void ExecuteBDEvent(string tempEvent, List<int> taskID, bool queue = false)
    {
        //【保底事件，[抽卡]，[1000，100，10，9999]，[1-抽卡，SSR]，[4-抽卡，SR]，[15-抽卡，R]，[80-抽卡，N]】           
        //[抽卡]:这个参数作为计数用的键名的一部分 比如上面这个保底事件
        //记录SSR卡没出的次数用 抽卡1作为键名  值是当前没出的次数
        //记录SR卡没出的次数用  抽卡2作为键名 值是当前没出的次数
        //记录R卡没出的次数用 抽卡3作为键名  值是当前没出的次数
        //记录N卡没出的次数用  抽卡4作为键名 值是当前没出的次数
        //[1000，100，100，9999]:这个参数是指后面这些事件的保底次数 按顺序一一对应
        //比如上面这个保底事件 1000次必出SSR  100次必出SR  10次必出R  9999次必出N
        //一旦出了就会重置对应的计数，比如当前抽到了SR，那么抽卡2的值就会变成0
        //[15-抽卡，R]:15是抽到这个事件的权重

        //去除标识用的字符串 然后分割事件字符串 得到若干个子事件
        tempEvent = tempEvent.Replace("保底事件，", "").Replace("保底事件,", "");
        //索引0是计数键名 索引1是各事件保底次数  [抽卡]，[1000，100，10，9999]
        string[] pEvents = tempEvent.SplitEventStr();
        //得到全部事件的权重值 
        List<float> weights = new List<float>();
        int index = 0;
        foreach (var item1 in pEvents)
        {
            //从索引2开始计算
            if (index < 2)
            {
                index++;
                continue;
            }
            //分割获取子事件参数
            string[] strs = item1.Split(item1.Contains("，") ? '，' : ',');
            //如果只有一个参数 且是数字 那么就是权重值
            if (strs.Length == 1 && Regex.IsMatch(strs[0], @"^[0-9]*$"))
            {
                weights.Add(strs[0].ToFloat());
                continue;
            }
            //30-血量  第一个带有 - 且是数字开头的参数的 - 前面部分作为权重值
            foreach (var item2 in strs)
            {
                if (item2.Contains("-") && Regex.IsMatch(item2[0].ToString(), @"^[0-9]*$"))
                {
                    weights.Add(item2.Split('-')[0].ToFloat());
                    break;
                }
            }
        }

        //需要触发的事件的索引
        index = -1;
        //先判断是否有低保要触发
        string[] counts = pEvents[1].Split(pEvents[1].Contains("，") ? '，' : ',');//拿到保底次数
        for (int i = 0; i < weights.Count; i++)
        {
            //如果当前计数已经超过或等于保底 那么就触发这个事件
            if (CountDic[pEvents[0] + i] >= counts[i].ToFloat())
            {
                index = i;
                break;
            }
        }
        //如果没有低保要触发 就根据权重随机一个事件
        if (index == -1)
        {
            //随机一个值 根据这个值落的位置来确定执行哪个事件
            float total = weights.Sum();
            UnityEngine.Random.InitState(int.Parse(DateTime.Now.ToString("HHmmssfff")));
            float random = UnityEngine.Random.Range(0, total);
            float current = 0;
            //计算要触发的事件索引
            for (int i = 0; i < weights.Count; i++)
            {
                current += weights[i];
                if (random < current)
                {
                    index = i;
                    break;
                }
            }
        }

        //设置计数
        for (int i = 0; i < weights.Count; i++)
        {
            //如果当前触发的不是这个事件 计数+1
            if (i != index)
            {
                //如果当前字典里还没有这个键 就直接将值设置成1
                if (!CountDic.ContainsKey(pEvents[0] + i))
                {
                    CountDic[pEvents[0] + i] = 1;
                }
                //否则就是之前的值+1
                else
                {
                    CountDic[pEvents[0] + i] = CountDic[pEvents[0] + i] + 1;
                }

            }
            else
            {
                //归零当前要触发的事件的计数
                CountDic[pEvents[0] + index] = 0;
            }
        }
        ExecuteEvents(pEvents[index + 2], taskID, queue);

    }

    /// <summary>
    /// 非数组集合字典 变量的事件模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="oldData">旧数据</param>
    /// <param name="args">参数数组</param>
    /// <param name="action">修改属性的回调函数</param>
    /// <returns></returns>
    public static object EventTemplate<T>(T oldData, object[] args, Action<T> action)
    {
        //参数里是否包含事件对象 如果包含这个值为1
        int containOwner = ContainOwner(args[0].ToString());
        //如果是判断事件 参数数组里会多一个mark 因此后续判断参数数组长度的时候 需要动态调整一下
        bool isJudge = args[args.Length - 1] is Mark ? true : false;

        //如果不是判断事件
        //【事件响应者，血量，+5】【血量，+5】
        if (!isJudge)
        {
            //计算变量 以及 将变量转换成目标类型之后  作为参数给回调函数使用
            action((T)Calc(oldData, args[1 + containOwner]).ChangeType<T>());
            return true;
        }
        //如果不是修改变量的事件 那么就是条件判断事件
        //【主角，血量，大于，3】
        else
        {
            //【关卡等级，大于，3】 【主角，血量，大于，3】 第三(四)个参数是判断的目标值 第二(三)个参数是判断运算符 
            return Judge(oldData, args[2 + containOwner], args[1 + containOwner], args);
        }
    }

    /// <summary>
    /// 数组变量的事件模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="oldData">旧的数据</param>
    /// <param name="args">参数数组</param>
    /// <returns></returns>
    public static object EventTemplate<T>(List<T> oldData, object[] args)
    {
        //参数里是否包含事件对象 如果包含这个值为1
        int containOwner = ContainOwner(args[0].ToString());
        //如果是判断事件 参数数组里会多一个mark 因此后续判断参数数组长度的时候 需要动态调整一下
        int isJudge = args[args.Length - 1] is Mark ? 1 : 0;

        //对数组变量的操作有 增删改查
        // 1.增加元素 
        // 2.删除元素(根据元素)
        // 3.删除元素(根据索引) 
        // 4.修改元素(+-*/=等运算) 
        //查
        // 5.判断数组长度 
        // 6.判断数组是否包含某个元素
        // 7.判断某个元素是否大于小于等于...

        //增加元素 【温高(事件对象)，3班学生，+，小王】【3班学生，+，小王】
        if (args[1 + containOwner].ToString() == "+")
        {
            T t = (T)args[2 + containOwner].ChangeType<T>();
            oldData.Add(t);
        }
        //删除元素(根据元素)  【3班学生，-，小王】删除找到的第一个小王    【3班学生，-，小王，All】删除找到的全部小王
        else if (args[1 + containOwner].ToString() == "-")
        {
            T t = (T)args[2 + containOwner].ChangeType<T>();
            if (args.Length > 3 + containOwner && args[3 + containOwner].ToString() == "All")
            {
                while (oldData.Contains(t))
                {
                    oldData.Remove(t);
                }
            }
            else
            {
                if (oldData.Contains(t))
                {
                    oldData.Remove(t);
                }
            }
        }
        //删除元素(根据索引)  【3班学生，RemoveAt，5】 【3班学生，RemoveAt，First】【3班学生，RemoveAt，Last】
        else if (args[1 + containOwner].ToString() == "RemoveAt")
        {
            T t = (T)args[2 + containOwner].ChangeType<T>();
            //判断是否是移除第一个元素  或者 最后一个元素
            if (args[2 + containOwner].ToString() == "First" || args[2 + containOwner].ToString() == "first")
            {
                oldData.RemoveAt(0);
            }
            else if (args[2 + containOwner].ToString() == "Last" || args[2 + containOwner].ToString() == "last")
            {
                oldData.RemoveAt(oldData.Count - 1);
            }
            //否则就是移除指定索引的元素
            else
            {
                oldData.RemoveAt(args[2 + containOwner].ToInt());
            }
        }
        //修改元素 如果第二个参数是数字说明是修改元素的操作 【成就，5，+-*/=3】 对索引为5的元素做+-*/=的运算
        else if (Regex.IsMatch(args[1 + containOwner].ToString(), @"^[0-9]*$"))
        {
            //获得要操作的元素的索引
            int index = args[1 + containOwner].ToInt();
            //如果数组长度大于索引 就对这个索引的值进行操作运算
            if (oldData.Count > index)
            {
                oldData[index] = (T)Calc(oldData[index], args[2 + containOwner].ChangeType<T>());
            }
        }
        //剩下的都是判断事件 数组能进行 数组长度的判断 是否包含某个元素的判断  以及具体元素的> >=...等判断
        else
        {
            //对数组长度的判断   【温高，3班学生，长度，大于，3】
            if (args[1 + containOwner].ToString() == "长度" || args[1 + containOwner].ToString() == "Length")
            {
                bool result = Judge(oldData.Count, args[3 + containOwner], args[2 + containOwner], args);
                if (args[args.Length - 1] is Mark mark)
                {
                    mark.Result = result ? 1 : 0;
                }
                return result;
            }
            //对数组是否包含某个元素的判断  【温高，3班学生，包含，小王】
            else if (args[1 + containOwner].ToString() == "包含")
            {
                T t = (T)args[2 + containOwner].ChangeType<T>();
                if (args[args.Length - 1] is Mark mark)
                {
                    mark.Result = oldData.Contains(t) ? 1 : 0;
                }
                return oldData.Contains(t);
            }
            else if (args[1 + containOwner].ToString() == "不包含")
            {
                T t = (T)args[2 + containOwner].ChangeType<T>();
                if (args[args.Length - 1] is Mark mark)
                {
                    mark.Result = !oldData.Contains(t) ? 1 : 0;
                }
                return !oldData.Contains(t);
            }
            //对数组具体元素的大于小于的判断 【温高，3班学生，元素，4，大于，3】
            else if (args[1 + containOwner].ToString() == "元素")
            {
                int index = args[2 + containOwner].ToInt();
                if (oldData.Count > index)
                {
                    bool result = Judge(oldData[index], args[4 + containOwner], args[3 + containOwner], args);
                    if (args[args.Length - 1] is Mark mark)
                    {
                        mark.Result = result ? 1 : 0;
                    }
                    return result;
                }
                return false;
            }
        }
        return oldData;
    }

    /// <summary>
    /// 字典类型变量的事件模板
    /// </summary>
    /// <typeparam name="K">键的类型</typeparam>
    /// <typeparam name="V">值的类型</typeparam>
    /// <param name="oldData">旧的数据</param>
    /// <param name="args">事件参数</param>
    /// <returns></returns>
    public static object EventTemplate<K, V>(Dictionary<K, V> oldData, object[] args)
    {
        //参数里是否包含事件对象 如果包含这个值为1
        int containOwner = ContainOwner(args[0].ToString());

        //对字典你变量的操作有 增删改查
        //1.增加一组键值对  小王，18
        //2.移除一个键  
        //3.移除某个值的全部键 
        //4.对某个键里的值进行+-*/= 运算
        //查
        //5.判断字典长度
        //6.判断字典是否包含某个键
        //7.判断字典是否包含某个值
        //8.对字典某个具体元素进行判断

        //增加元素 【温高，3班学生，+，小王，18】【3班学生，+，小王，18】 
        if (args[1 + containOwner].ToString() == "+" || args[1 + containOwner].ToString() == "加")
        {
            //找到key
            K k = (K)args[2 + containOwner].ChangeType<K>();
            V v = default;
            //如果事件里有四个参数 那么说明指定了值  这里获取一下对应的值
            if (args.Length > 3 + containOwner)
            {
                v = (V)args[3 + containOwner].ChangeType<V>();
            }
            oldData[k] = v;

        }
        //删除对应元素 【温高，3班学生，-，小王】【3班学生，-，小王，18】
        else if (args[1 + containOwner].ToString() == "-" || args[1 + containOwner].ToString() == "减")
        {
            //根据值来移除
            if (args.Length > 3 + containOwner)
            {
                V v = (V)args[3 + containOwner].ChangeType<V>();
                List<K> delK = new List<K>();
                foreach (var item in oldData)
                {
                    if (item.Value.Equals(v))
                    {
                        delK.Add(item.Key);
                    }
                }
                foreach (var key in delK)
                {
                    oldData.Remove(key);
                }
            }
            else//根据键名来移除
            {
                K k = (K)args[2 + containOwner].ChangeType<K>();
                if (oldData.ContainsKey(k))
                {
                    oldData.Remove(k);
                }
            }
        }
        //判断字典长度 【关卡状态，长度，大于(小于等于....)，8】
        else if (args[1 + containOwner].ToString() == "长度" || args[1 + containOwner].ToString() == "数量"
            || args[1 + containOwner].ToString() == "Length" || args[1 + containOwner].ToString() == "Count")
        {
            return Judge(oldData.Count, args[3 + containOwner], args[2 + containOwner], args);
        }
        //判断字典是否包含某个键或者值  【三班学生，包含，小王】 【三班学生，包含，小王，17】
        else if (args[1 + containOwner].ToString() == "包含" || args[1 + containOwner].ToString() == "Contain")
        {
            //如果参数大于4个 说明是判断是否包含某个值
            if (args.Length > 3 + containOwner)//是否包含某个值
            {
                V v = (V)args[3 + containOwner].ChangeType<V>();
                foreach (var item in oldData)
                {
                    if (item.Value.Equals(v))
                    {
                        if (args[args.Length - 1] is Mark mark)
                        {
                            mark.Result = 1;
                        }
                        return true;
                    }
                }
            }
            else//是否包含某个键
            {
                K k = (K)args[2 + containOwner].ChangeType<K>();
                foreach (var item in oldData)
                {
                    if (item.Key.Equals(k))
                    {
                        if (args[args.Length - 1] is Mark mark)
                        {
                            mark.Result = 1;
                        }
                        return true;
                    }
                }
            }

        }
        else if (args[1 + containOwner].ToString() == "不包含")
        {
            //如果参数大于4个 说明是判断是否包含某个值
            if (args.Length > 3 + containOwner)//是否包含某个值
            {
                V v = (V)args[3 + containOwner].ChangeType<V>();
                foreach (var item in oldData)
                {
                    if (item.Value.Equals(v))
                    {
                        if (args[args.Length - 1] is Mark mark)
                        {
                            mark.Result = 0;
                        }
                        return false;
                    }
                }
                if (args[args.Length - 1] is Mark mark1)
                {
                    mark1.Result = 1;
                }
                return true;
            }
            else//是否包含某个键
            {
                K k = (K)args[2 + containOwner].ChangeType<K>();
                foreach (var item in oldData)
                {
                    if (item.Key.Equals(k))
                    {
                        if (args[args.Length - 1] is Mark mark)
                        {
                            mark.Result = 0;
                        }
                        return false;
                    }
                }
                if (args[args.Length - 1] is Mark mark1)
                {
                    mark1.Result = 1;
                }
                return true;
            }

        }
        //判断字典某个元素是否大于小于等于某个值    【温高，3班学生，元素，小王，大于，5】
        else if (args[1 + containOwner].ToString() == "元素")
        {
            K k = (K)args[2 + containOwner].ChangeType<K>();
            if (oldData.ContainsKey(k))
            {
                bool result = Judge(oldData[k], args[4 + containOwner], args[3 + containOwner], args);
                if (args[args.Length - 1] is Mark mark)
                {
                    mark.Result = result ? 1 : 0;
                }
                return result;
            }
            else
            {
                return false;
            }

        }
        //如果不是上述操作的话 那么就是对某个键里的值进行+-*/=运算 【关卡状态，小王，+-*/=4】
        else
        {
            K k = (K)args[1 + containOwner].ChangeType<K>();
            if (oldData.ContainsKey(k))
            {
                oldData[k] = (V)Calc(oldData[k], args[2 + containOwner].ChangeType<V>());
            }
        }
        return oldData;
    }

    /// <summary>
    /// 判断给定的条件是否满足
    /// </summary>
    /// <param name="and">true为与判断，false为或判断</param>
    /// <param name="result">存储判断结果</param>
    /// <param name="part0">条件字符串</param>
    public static void Judge(bool and, out bool result, string part0)
    {
        //将字符串分割成若干个子条件事件       
        string[] conditionEvents = part0.SplitEventStr();
        //遍历执行子条件事件
        bool changed = false;
        foreach (var item in conditionEvents)
        {
            //分割字符串获取到事件参数 [事件响应者，血量，大于，5] 
            string[] args = item.Split(item.Contains('，') ? '，' : ',');
            //第一个参数如果是 标识事件对象的字符串 那么后续使用参数的时候索引需要加上1
            int containOwner = ContainOwner(args[0]);
            object[] param = CreateParamsList(args);
            bool resule = (bool)EventMgr.ExecuteEvent(args[0 + containOwner], param);
            //与事件有一个结果为false  或事件有一个结果为true  就修改标识并结束 
            if (and && !resule)
            {
                changed = true;
                break;
            }
            else if (!and && resule)
            {
                changed = true;
                break;
            }
        }
        //如果改变过标识 那么就取and的反面 否则就and的值
        result = changed ? !and : and;
    }

    /// <summary>
    /// 不同类型数据的条件判断
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="value1">关系运算符左侧的值</param>
    /// <param name="value2">关系运算符右侧的值</param>
    /// <param name="opration">关系运算符</param>
    /// <param name="args">事件的参数数组，最后一个参数可能是存储判断结果的标识参数</param>
    /// <returns></returns>
    public static bool Judge<T>(T value1, object value2, object opration, object[] args = null)
    {
        //向量支持 等于 不等于 以及模长的 大于 小于 大于等于 小于等于 的判断
        if (typeof(T) == typeof(Vector2))
        {
            object tempValue = value1;
            switch (opration.ToString())
            {
                case "等于":
                case "=":
                case "==":
                    if ((Vector2)tempValue == value2.ToVector2())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不等于":
                case "!=":
                    if ((Vector2)tempValue != value2.ToVector2())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于":
                case ">":
                    if (((Vector2)tempValue).magnitude > value2.ToVector2().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于":
                case "<":
                    if (((Vector2)tempValue).magnitude < value2.ToVector2().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于等于":
                case ">=":
                    if (((Vector2)tempValue).magnitude >= value2.ToVector2().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于等于":
                case "<=":
                    if (((Vector2)tempValue).magnitude <= value2.ToVector2().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
            }
            return false;
        }
        else if (typeof(T) == typeof(Vector3))
        {
            object tempValue = value1;
            switch (opration.ToString())
            {
                case "等于":
                case "=":
                case "==":
                    if ((Vector3)tempValue == value2.ToVector3())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不等于":
                case "!=":
                    if ((Vector3)tempValue != value2.ToVector3())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于":
                case ">":
                    if (((Vector3)tempValue).magnitude > value2.ToVector3().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于":
                case "<":
                    if (((Vector3)tempValue).magnitude < value2.ToVector3().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于等于":
                case ">=":
                    if (((Vector3)tempValue).magnitude >= value2.ToVector3().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于等于":
                case "<=":
                    if (((Vector3)tempValue).magnitude <= value2.ToVector3().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
            }
            return false;
        }
        else if (typeof(T) == typeof(Vector4))
        {
            object tempValue = value1;
            switch (opration.ToString())
            {
                case "等于":
                case "=":
                case "==":
                    if ((Vector4)tempValue == value2.ToVector4())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不等于":
                case "!=":
                    if ((Vector4)tempValue != value2.ToVector4())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于":
                case ">":
                    if (((Vector4)tempValue).magnitude > value2.ToVector4().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于":
                case "<":
                    if (((Vector4)tempValue).magnitude < value2.ToVector4().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于等于":
                case ">=":
                    if (((Vector4)tempValue).magnitude >= value2.ToVector4().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于等于":
                case "<=":
                    if (((Vector4)tempValue).magnitude <= value2.ToVector4().magnitude)
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
            }
            return false;
        }
        else if (typeof(T) == typeof(Color))
        {
            object tempValue = value1;
            switch (opration.ToString())
            {
                case "等于":
                case "=":
                case "==":
                    if ((Color)tempValue == value2.ToColor())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不等于":
                case "!=":
                    if ((Color)tempValue != value2.ToColor())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
            }
            return false;
        }
        else if ((typeof(T) == typeof(Color32)))
        {
            object tempValue = value1;
            switch (opration.ToString())
            {
                case "等于":
                case "=":
                case "==":
                    if ((Color32)tempValue == value2.ToColor())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不等于":
                case "!=":
                    if ((Color32)tempValue != value2.ToColor())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
            }
            return false;
        }
        else
        //枚举 字符串 数值类型
        {
            //枚举 数值类型 都可以转换成double来判断大小
            //等于不等可以通过转换成字符串来判断
            //包含 不包含 含于 不含于判断是字符串独有的 字符串里是否包含某个子字符串 ...
            switch (opration.ToString())
            {
                case "大于":
                case ">":
                    if (value1.ToFloat() > value2.ToFloat())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于":
                case "<":
                    if (value1.ToFloat() < value2.ToFloat())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "大于等于":
                case ">=":
                    if (value1.ToFloat() >= value2.ToFloat())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "小于等于":
                case "<=":
                    if (value1.ToFloat() <= value2.ToFloat())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "等于"://等于不等于的判断可以直接转换成字符串来判断  
                case "==":
                case "=":
                    if (value1.ToString().Trim() == value2.ToString().Trim())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不等于":
                case "!=":
                    if (value1.ToString().Trim() != value2.ToString().Trim())
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "包含":
                    if (value1.ToString().Trim().Contains(value2.ToString().Trim()))
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不包含":
                    if (!value1.ToString().Trim().Contains(value2.ToString().Trim()))
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "含于":
                    if (value2.ToString().Trim().Contains(value1.ToString().Trim()))
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
                case "不含于":
                    if (!value2.ToString().Trim().Contains(value1.ToString().Trim()))
                    {
                        if (args != null && args[args.Length - 1] is Mark sign) sign.Result = 1;
                        return true;
                    }
                    break;
            }
            return false;
        }
    }

    /// <summary>
    /// 判断第一个参数是否是标识对象的参数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static int ContainOwner(string str)
    {
        int containOwner = 0;
        if (str == "主角" || str == "主角召唤物" || str == "敌人" || str == "敌人召唤物" || str == "BOSS")
        {
            containOwner = 1;
        }

        return containOwner;
    }

    /// <summary>
    /// 是否包含事件目标，包含的情况下，使用参数的时候，索引需要后移一位
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsEventTarget(object obj, OwnerType owner = OwnerType.None)
    {
        //如果没有指定事件目标 或者 事件目标是自己 
        if (ContainOwner(obj.ToString()) == 0 || obj.ToString() == owner.ToString())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 重构参数数组，将用于辅助判断用的对象类加入进去
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private static object[] CreateParamsList(string[] param)
    {
        List<object> list = new List<object>(param);
        //在数组最后加入用于标识的Sign对象
        list.Add(new Mark());
        return list.ToArray();
    }

    /// <summary>
    /// 如果是数值类型的变量，那么可以进行加减乘除等于和取余运算 其他类型只能进行部分运算
    /// </summary>
    /// <param name="oldData">旧值</param>
    /// <param name="operation">要进行的运算</param>
    /// <returns></returns>
    public static object Calc<T>(T oldData, object operation)
    {
        if (typeof(T) == typeof(Vector2))//v2类型  +-= */  */仅作用于向量乘给定的小数或整数
        {
            //先将T类型转换成object类型 然后再强转成vector类型
            object tempValue = oldData;
            Vector2 v2 = (Vector2)tempValue;
            if (operation.ToString().Trim().StartsWith("+"))
            {
                return v2 + operation.ToString().Substring(1).ToVector2();
            }
            else if (operation.ToString().Trim().StartsWith("-"))
            {
                return v2 - operation.ToString().Substring(1).ToVector2();
            }
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                return v2 * operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("/"))
            {
                return v2 / operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1).ToVector2();
            }
            else
            {
                return v2 + operation.ToString().ToVector2();
            }
        }
        else if (typeof(T) == typeof(Vector3))
        {
            object tempValue = oldData;
            Vector3 v3 = (Vector3)tempValue;
            if (operation.ToString().Trim().StartsWith("+"))
            {
                return v3 + operation.ToString().Substring(1).ToVector3();
            }
            else if (operation.ToString().Trim().StartsWith("-"))
            {
                return v3 - operation.ToString().Substring(1).ToVector3();
            }
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                return v3 * operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("/"))
            {
                return v3 / operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1).ToVector3();
            }
            else
            {
                return v3 + operation.ToString().ToVector3();
            }
        }
        else if (typeof(T) == typeof(Vector4))
        {
            object tempValue = oldData;
            Vector4 v4 = (Vector4)tempValue;
            if (operation.ToString().Trim().StartsWith("+"))
            {
                return v4 + operation.ToString().Substring(1).ToVector4();
            }
            else if (operation.ToString().Trim().StartsWith("-"))
            {
                return v4 - operation.ToString().Substring(1).ToVector4();
            }
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                return v4 * operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("/"))
            {
                return v4 / operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1).ToVector4();
            }
            else
            {
                return v4 + operation.ToString().ToVector4();
            }
        }
        else if (typeof(T) == typeof(Color))//color +-*/= */只作用于 颜色 */ 某个小数或整数
        {
            object tempValue = oldData;
            Color color = (Color)tempValue;
            if (operation.ToString().Trim().StartsWith("+"))
            {
                Color c = operation.ToString().Substring(1).ToColor();
                return new Color(color.r + c.r, color.g + c.g, color.b + c.b, color.a + c.a);
            }
            else if (operation.ToString().Trim().StartsWith("-"))
            {
                Color c = operation.ToString().Substring(1).ToColor();
                return new Color(color.r - c.r, color.g - c.g, color.b - c.b, color.a - c.a);
            }
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                float a = operation.ToString().Substring(1).ToFloat();
                return new Color(color.r * a, color.g * a, color.b * a, color.a * a);
            }
            else if (operation.ToString().Trim().StartsWith("/"))
            {
                float a = operation.ToString().Substring(1).ToFloat();
                return new Color(color.r / a, color.g / a, color.b / a, color.a / a);
            }
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1).ToColor();
            }
            else
            {
                Color c = operation.ToString().ToColor();
                return new Color(color.r + c.r, color.g + c.g, color.b + c.b, color.a + c.a);
            }
        }
        else if (typeof(T) == typeof(Color32))
        {
            object tempValue = oldData;
            Color32 color = (Color32)tempValue;
            if (operation.ToString().Trim().StartsWith("+"))
            {
                Color32 c = operation.ToString().Substring(1).ToColor();
                return new Color32((byte)(color.r + c.r), (byte)(color.g + c.g), (byte)(color.b + c.b), (byte)(color.a + c.a));
            }
            else if (operation.ToString().Trim().StartsWith("-"))
            {
                Color32 c = operation.ToString().Substring(1).ToColor();
                return new Color32((byte)(color.r - c.r), (byte)(color.g - c.g), (byte)(color.b - c.b), (byte)(color.a - c.a));
            }
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                float a = operation.ToString().Substring(1).ToFloat();
                return new Color32((byte)(color.r * a), (byte)(color.g * a), (byte)(color.b * a), (byte)(color.a * a));
            }
            else if (operation.ToString().Trim().StartsWith("/"))
            {
                float a = operation.ToString().Substring(1).ToFloat();
                return new Color32((byte)(color.r / a), (byte)(color.g / a), (byte)(color.b / a), (byte)(color.a / a));
            }
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1).ToColor();
            }
            else
            {
                Color32 c = operation.ToString().ToColor();
                return new Color32((byte)(color.r + c.r), (byte)(color.g + c.g), (byte)(color.b + c.b), (byte)(color.a + c.a));
            }
        }
        //string类型 + = 和 * (替换)
        else if (typeof(T) == typeof(string))
        {
            //如果是+法运算，那么把目标字符串里面的+号去掉 然后拼接在原字符串后面就行
            if (operation.ToString().Trim().StartsWith("+"))
            {
                return oldData.ToString() + operation.ToString().Substring(1);
            }
            //如果是替换操作 就用新的字符串替换旧的字符串   *乾隆-康熙 用康熙替换原字符串里的乾隆   *乾隆-  用空字符串替换原字符串里的乾隆
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                string[] arg = operation.ToString().Substring(1).Split('-');
                return oldData.ToString().Replace(arg[0], arg[1]);
            }
            //如果是等于操作 把型字符串里的等于去掉之后直接返回 替代旧字符串
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1);
            }
            //如果没有 + * = 等运算符标识 那么默认就是字符串拼接的操作
            else
            {
                return oldData.ToString() + operation.ToString();
            }
        }
        //数值类型 和枚举类型  +-*/=
        else
        {
            //首先去掉可能误添加的空格  判断运算符类型
            if (operation.ToString().Trim().StartsWith("+") || operation.ToString().Trim().StartsWith("-"))
            {
                return oldData.ToFloat() + operation.ToString().Replace("+", "").ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("*"))
            {
                return oldData.ToFloat() * operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("/"))
            {
                return oldData.ToFloat() / operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("="))
            {
                return operation.ToString().Substring(1).ToFloat();
            }
            else if (operation.ToString().Trim().StartsWith("%"))
            {
                return oldData.ToFloat() % operation.ToString().Substring(1).ToFloat();
            }
            else
            {
                return oldData.ToFloat() + operation.ToString().ToFloat();
            }
        }
    }

    /// <summary>
    /// 判断当前事件是否是队列事件，是的话返回true，并且将标识去掉
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static bool IsQueueEvent(object[] arg)
    {
        //如果是队列事件，那么代码会在事件的最后一个参数里注入一段标识 如果这里匹配上了就返回true 且将标识从参数里移除 
        bool queue = arg[arg.Length - 1].ToString().Contains(QueueSign);
        //如果是队列调用的时候需要把标识符号去掉
        if (queue)
        {
            arg[arg.Length - 1] = arg[arg.Length - 1].ToString().Replace(QueueSign, "");
        }
        return queue;
    }
    #endregion
}
