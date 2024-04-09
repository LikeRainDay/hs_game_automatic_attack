using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Data;
using System.IO;
using Excel;
using Steamworks;
using System;
/// <summary>
/// 游戏管理器
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 通用
    [FieldName("加密存档数据")]
    public bool Encryption;
    [FieldName("显示Debug日志")]
    public bool LogDebug;
    [FieldName("总是事件的调用间隔")]
    public float AlwaysInterval = 0.2f;
    #endregion

    #region 当前项目用  
    [FieldName("残血警告")]
    public GameObject Warning;
    [FieldName("树木预制体")]
    public GameObject Tree;   
    [FieldName("主角预制体")]
    public GameObject Player;
    [FieldName("红叉")]
    public GameObject RedFork;
    [FieldName("绿叉")]
    public GameObject GreenFork;
    [FieldName("场景预制体")]
    public GameObject Scene;
    #endregion

    /// <summary>
    /// 当前语言
    /// </summary>
    [HideInInspector] public Language CurLanguage;
    /// <summary>
    /// 当前分辨率 1 2 4 K
    /// </summary>
    [HideInInspector] public int CurDefinition;
    /// <summary>
    /// 用于判断当前演出事件还是否需要继续进行，场景切换等逻辑之后会++，让之前还未执行完的事件中断掉
    /// </summary>
    [HideInInspector] public int StepIndex;
    [FieldName("简化版多语言方案")]
    public bool SimpleMultilingual;
    [FieldName("简化版多语言文本表路径")]
    public string TextCfgPath = "/文本表.xlsx";
    /// <summary>
    /// 简化版多语言方案情况下，当前项目的文本数据，键是文本id，值是各国语言文本的数组
    /// </summary>
    public Dictionary<int, List<string>> TextDic = new Dictionary<int, List<string>>();
    //简单Mono单例
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
        Warning?.SetActive(false);
        if (SimpleMultilingual)
        {
            DataRowCollection dataRowCollection = ReadExcel(Application.streamingAssetsPath + TextCfgPath, 0);
            //前两行留作备注用 第三行开始是实际数据
            for (int i = 2; i < dataRowCollection.Count; i++)
            {
                if (!string.IsNullOrEmpty(dataRowCollection[i][0].ToString()))
                {
                    int id = int.Parse(dataRowCollection[i][0].ToString());
                    List<string> contents = new List<string>();
                    //第一列是ID 第二列是备注 第三列开始是实际数据
                    contents.Add(dataRowCollection[i][2].ToString());
                    contents.Add(dataRowCollection[i][3].ToString());
                    contents.Add(dataRowCollection[i][4].ToString());
                    TextDic[id] = contents;
                }
            }
        }
    }

    private void Start()
    {
        //初始化存档数据
        //SaveMgr.Instance.Load();
        SaveMgr.Instance.SettingData = new SettingData();
        SaveMgr.Instance.SettingData.Current = new SettingData.Data();
        //初始化输入
        InputMgr.Instance.OnInit();
        //显示主菜单界面
        MainMenuPage.Instance.Show();
    }

    #region 通用工具函数
    /// <summary>
    /// 读取 Excel
    /// </summary>
    /// <param name="_path">Excel 表路径</param>
    /// <param name="_sheetIndex">读取的 Sheet 索引</param>
    /// <returns></returns>
    private DataRowCollection ReadExcel(string _path, int _sheetIndex = 0)
    {
        AESTool.FileDecrypt(_path, DefaultDef.Key);//解密
        FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);//读取 2007及以后的版本
        //加密 有时为了检查数据内容 会选择不加密数据 比如看看存档文件里面的数据情况
        if (Encryption)
        {
            AESTool.FileEncrypt(_path, DefaultDef.Key);
        }
        return excelReader.AsDataSet().Tables[_sheetIndex].Rows;
    }
    
    /// <summary>
    /// 获取当前语言下的文本内容
    /// </summary>
    /// <param name="id">文本ID</param>
    /// <returns></returns>
    public string GetCurLanguageContent(int id)
    {
        if (TextDic.ContainsKey(id))
        {
            return TextDic[id][CurLanguage.GetHashCode()];
        }
        return "";
    }
    #endregion

    #region Steam相关
    /// <summary>
    /// 解锁某个成就
    /// </summary>
    /// <param name="name"></param>
    public void UnlockedAchievement(string name)
    {
        //如果当前Steam在线
        if (SteamMgr.Initialized)
        {
            SteamUserStats.SetAchievement(name);
            SteamUserStats.StoreStats();
        }
    }

    /// <summary>
    /// 当前的运行平台是否是SteamDeck
    /// </summary>
    /// <returns></returns>
    public bool IsSteamRunningOnSteamDeck()
    {
        //如果当前Steam在线
        if (SteamMgr.Initialized)
        {
            return SteamUtils.IsSteamRunningOnSteamDeck();
        }
        return false;
    }
    #endregion
}
