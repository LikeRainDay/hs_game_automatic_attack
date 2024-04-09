/// <summary>
/// 演出事件工厂模式
/// </summary>
public class ActionFactory
{
    /// <summary>
    /// 创建事件节点
    /// </summary>
    /// <param name="rowCfgPerformances"></param>
    /// <returns></returns>
    public static BaseAction Create(RowCfgPerformances rowCfgPerformances)
    {
        switch (rowCfgPerformances.actionType)
        {
            case "打开对话":
                return new DialogueAction(rowCfgPerformances);
            case "打开漫画":
                return new ComicAction(rowCfgPerformances);
            case "主角移动":
                return new PlayerMoveAction(rowCfgPerformances);
            case "自由行动":
                return new FreeTimeAction(rowCfgPerformances);
            //Todo:其他事件根据具体情况补充

            //没有特别定义的事件，都视作通用事件交给EventAction处理
            default: return new EventAction(rowCfgPerformances, rowCfgPerformances.actionType);

        }
    }

}