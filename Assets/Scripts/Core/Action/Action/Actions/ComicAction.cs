using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 打开漫画    打开漫画，10101(漫画ID)
/// </summary>
public class ComicAction : BaseAction
{
    public ComicAction(RowCfgPerformances rowCfgPerformances) : base(rowCfgPerformances) { }

    protected override void OnStart()
    {
        base.OnStart();
        //打开漫画界面
        //MangaPanel.Instance.InitComic(int.Parse(customParams[0]));
    }

}
