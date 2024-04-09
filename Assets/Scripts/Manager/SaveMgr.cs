using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// 存档管理器
/// </summary>
public class SaveMgr : Singleton<SaveMgr>
{
    public int BackupsCount = 5;//存档备份数量，每次选择一个存档开始游戏时都会备份一下存档，

    public SettingData SettingData;//设置数据 亮度 音量 分辨率 ...
    //一类数据:关卡进度等大节点数据  二类数据:一段演出是否结束等小节点数据  三类数据:需要时时保存的数据，比如NPC一条一次性对话是否看过
    public List<Save1Data> Save1s;//存储全部存档的一类数据
    public List<Save2Data> Save2s;//存储全部存档的二类数据
    public List<Save3Data> Save3s;//存储全部存档的三类数据

    public Dictionary<int, List<Save1Data>> Save1BackupsDic;//存储全部存档的一类数据备份 键是存档索引，值是这个存档索引的一类数据备份
    public Dictionary<int, List<Save2Data>> Save2BackupsDic;//同上
    public Dictionary<int, List<Save3Data>> Save3BackupsDic;//同上

    public Save1Data CurSave1Data;//当前存档的一类数据  
    public Save2Data CurSave2Data;//当前存档的二类数据
    public Save3Data CurSave3Data;//当前存档的三类数据

    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="only2">只加载二类存档，比如一段演出只进行了一部分就返回主界面，那么需要将这期间存的脏数据舍弃掉，重新加载最后一次存到本地的二类存档数据</param>
    public void Load(bool only2 = false)
    {
        if (!only2)
        {
            //初始化容器数组
            Save1s = new List<Save1Data>();
            Save2s = new List<Save2Data>();
            Save3s = new List<Save3Data>();
            Save1BackupsDic = new Dictionary<int, List<Save1Data>>();
            Save2BackupsDic = new Dictionary<int, List<Save2Data>>();
            Save3BackupsDic = new Dictionary<int, List<Save3Data>>();

            //加载一类数据文件夹下的全部文件
            string directoryPath = Path.Combine(Application.persistentDataPath, "Save1");
            directoryPath.CreateDirIfNotExists();
            List<string> allFilePaths = directoryPath.GetAllFilePaths();
            allFilePaths.ForEach(a =>
            {
                //先解密
                AESTool.FileDecrypt(a, DefaultDef.Key);
                //序列化成类对象
                Save1Data data = (Save1Data)SerializeTool.Xml2Class(typeof(Save1Data), a);
                //如果是备份文件 就存到备份数据字典里 否则就放到存储全部存档的一类数据的数组里
                if (a.GetAssetName().Contains("_"))
                {
                    if (!Save1BackupsDic.ContainsKey(data.Index))
                    {
                        Save1BackupsDic[data.Index] = new List<Save1Data>();
                    }
                    Save1BackupsDic[data.Index].Add(data);
                }
                else
                {
                    Save1s.Add(data);
                }
                //将文件加密回去
                if (GameManager.Instance.Encryption)
                {
                    AESTool.FileEncrypt(directoryPath, DefaultDef.Key);
                }
            });

            //加载二类数据文件夹下的全部文件
            directoryPath = Path.Combine(Application.persistentDataPath, "Save2");
            directoryPath.CreateDirIfNotExists();
            allFilePaths = directoryPath.GetAllFilePaths();
            allFilePaths.ForEach(a =>
            {
                AESTool.FileDecrypt(a, DefaultDef.Key);
                Save2Data data = (Save2Data)SerializeTool.Xml2Class(typeof(Save2Data), a);
                if (a.GetAssetName().Contains("_"))
                {
                    if (!Save2BackupsDic.ContainsKey(data.Index))
                    {
                        Save2BackupsDic[data.Index] = new List<Save2Data>();
                    }
                    Save2BackupsDic[data.Index].Add(data);
                }
                else
                {
                    Save2s.Add(data);
                }
                if (GameManager.Instance.Encryption)
                {
                    AESTool.FileEncrypt(directoryPath, DefaultDef.Key);
                }
            });

            //加载三类数据文件夹下的全部文件
            directoryPath = Path.Combine(Application.persistentDataPath, "Save3");
            directoryPath.CreateDirIfNotExists();
            allFilePaths = directoryPath.GetAllFilePaths();
            allFilePaths.ForEach(a =>
            {
                AESTool.FileDecrypt(a, DefaultDef.Key);
                Save3Data data = (Save3Data)SerializeTool.Xml2Class(typeof(Save3Data), a);
                if (a.GetAssetName().Contains("_"))
                {
                    if (!Save3BackupsDic.ContainsKey(data.Index))
                    {
                        Save3BackupsDic[data.Index] = new List<Save3Data>();
                    }
                    Save3BackupsDic[data.Index].Add(data);
                }
                else
                {
                    Save3s.Add(data);
                }
                if (GameManager.Instance.Encryption)
                {
                    AESTool.FileEncrypt(directoryPath, DefaultDef.Key);
                }
            });

        }
        //只读取存档2
        else
        {
            //获取到目标存档文件路径
            string path = Path.Combine(Application.persistentDataPath, $"Save2/{CurSave2Data.Index}.xml");
            //解密
            AESTool.FileDecrypt(path, DefaultDef.Key);
            //将旧数据从数组里移除
            Save2s.RemoveSafe(CurSave2Data);
            //读取文件转换成类对象
            CurSave2Data = (Save2Data)SerializeTool.Xml2Class(typeof(Save2Data), path);
            //将新数据加入数组
            Save2s.Add(CurSave2Data);
            //将文件加密回去
            if (GameManager.Instance.Encryption)
            {
                AESTool.FileEncrypt(path, DefaultDef.Key);
            }

        }
    }

    /// <summary>
    /// 将某个备份存档设置为当前存档
    /// </summary>
    /// <param name="index">要加载的备份存档索引(名字)</param>
    public void LoadBackups(int index)
    {
        //获取存档路径
        int backupsIndex = CurSave1Data.CurBackupsIndex;

        //拿到备份存档路径
        string path = Path.Combine(Application.persistentDataPath, $"Save1/{CurSave1Data.Index + "_" + index}.xml");
        AESTool.FileDecrypt(path, DefaultDef.Key);
        //读取文件转换成Class
        CurSave1Data = (Save1Data)SerializeTool.Xml2Class(typeof(Save1Data), path);
        CurSave1Data.CurBackupsIndex = backupsIndex;//同步一下当前的备份存档索引
        //将文件加密回去
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(path, DefaultDef.Key);
        }

        path = Path.Combine(Application.persistentDataPath, $"Save2/{CurSave2Data.Index + "_" + index}.xml");
        AESTool.FileDecrypt(path, DefaultDef.Key);
        CurSave2Data = (Save2Data)SerializeTool.Xml2Class(typeof(Save2Data), path);
        CurSave2Data.CurBackupsIndex = backupsIndex;
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(path, DefaultDef.Key);
        }

        path = Path.Combine(Application.persistentDataPath, $"Save3/{CurSave3Data.Index + "_" + index}.xml");
        AESTool.FileDecrypt(path, DefaultDef.Key);
        CurSave3Data = (Save3Data)SerializeTool.Xml2Class(typeof(Save3Data), path);
        CurSave3Data.CurBackupsIndex = backupsIndex;
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(path, DefaultDef.Key);
        }

        //保存到本地，替换掉之前的存档
        Save1();
    }

    /// <summary>
    /// 备份存档，每次选择存档开始游戏后，可以备份一下当前存档
    /// </summary>
    public void Backups()
    {
        if (CurSave1Data == null) return;
        //获取备份存档索引
        int curBackupsIndex = CurSave1Data.CurBackupsIndex.MapIndex(BackupsCount);
        CurSave1Data.CurBackupsIndex = (CurSave1Data.CurBackupsIndex + 1).MapIndex(BackupsCount);
        Save1(curBackupsIndex);
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    /// <param name="index">存档位索引</param>
    public void DelSave(int index)
    {
        //删除本地文件
        string path = Path.Combine(Application.persistentDataPath, $"Save1/{index}.xml");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        path = Path.Combine(Application.persistentDataPath, $"Save2/{index}.xml");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        path = Path.Combine(Application.persistentDataPath, $"Save3/{index}.xml");
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        //删除本地的备份文件
        for (int i = 0; i < BackupsCount; i++)
        {
            path = Path.Combine(Application.persistentDataPath, $"Save1/{index + "_" + i}.xml");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            path = Path.Combine(Application.persistentDataPath, $"Save2/{index + "_" + i}.xml");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            path = Path.Combine(Application.persistentDataPath, $"Save3/{index + "_" + i}.xml");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        //重新加载存档
        Load();
    }

    /// <summary>
    /// 保存一类数据
    /// </summary>
    /// <param name="backupsIndex"></param>
    public void Save1(int backupsIndex = -1)
    {
        //保存字典数据 记录存档时刻
        CurSave1Data?.SaveDic();
        CurSave1Data.SaveTime = DateTime.Now.Ticks;

        //如果是保存备份存档，原先的文件名 加 _备份索引  例如1号存档3号备份 1_3
        string path = backupsIndex == -1 ? Path.Combine(Application.persistentDataPath, $"Save1/{CurSave1Data.Index}.xml") :
            Path.Combine(Application.persistentDataPath, $"Save1/{CurSave1Data.Index + "_" + backupsIndex}.xml");

        //保存数据
        SerializeTool.Class2Xml(typeof(Save1Data), CurSave1Data, path);
        //加密一下保存的数据文件
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(path, DefaultDef.Key);
        }
        Save2(backupsIndex);
        Save3(backupsIndex);
    }

    /// <summary>
    /// 保存二类数据
    /// </summary>
    /// <param name="backupsIndex"></param>
    public void Save2(int backupsIndex = -1)
    {
        CurSave2Data?.SaveDic();
        CurSave2Data.SaveTime = DateTime.Now.Ticks;
        string path = backupsIndex == -1 ? Path.Combine(Application.persistentDataPath, $"Save2/{CurSave2Data.Index}.xml") :
            Path.Combine(Application.persistentDataPath, $"Save2/{CurSave2Data.Index + "_" + backupsIndex}.xml");
        SerializeTool.Class2Xml(typeof(Save2Data), CurSave2Data, path);
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(path, DefaultDef.Key);
        }
    }

    /// <summary>
    /// 保存三类数据
    /// </summary>
    /// <param name="backupsIndex"></param>
    public void Save3(int backupsIndex = -1)
    {
        CurSave3Data?.SaveDic();
        CurSave3Data.SaveTime = DateTime.Now.Ticks;
        string path = backupsIndex == -1 ? Path.Combine(Application.persistentDataPath, $"Save3/{CurSave3Data.Index}.xml") :
            Path.Combine(Application.persistentDataPath, $"Save1/{CurSave3Data.Index + "_" + backupsIndex}.xml");
        SerializeTool.Class2Xml(typeof(Save3Data), CurSave3Data, path);
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(path, DefaultDef.Key);
        }
    }

    /// <summary>
    /// 保存设置数据
    /// </summary>
    public void SaveSetting()
    {
        //存到本地，然后加密
        SerializeTool.Class2Xml(typeof(SettingData), SettingData, Path.Combine(Application.persistentDataPath, "Setting.xml"));
        if (GameManager.Instance.Encryption)
        {
            AESTool.FileEncrypt(Path.Combine(Application.persistentDataPath, "Setting.xml"), DefaultDef.Key);
        }
    }

}

/// <summary>
/// 一类数据:关卡进度等大节点数据
/// </summary>
public class Save1Data : SaveDataBase
{
    /// <summary>
    /// 当前关卡进度
    /// </summary>
    public int CurLevel;

}

/// <summary>
/// 二类数据:一段演出是否结束等小节点数据 
/// </summary>
public class Save2Data : SaveDataBase
{
    /// <summary>
    /// 当前存档统计数据
    /// </summary>
    public StatisticsData StatisticsData;//统计数据没必要时时存到本地，不精准一点没事，所以视作二类数据
}

/// <summary>
/// 三类数据:需要时时保存的数据，比如NPC一条一次性对话是否看过
/// </summary>
public class Save3Data : SaveDataBase
{
    /// <summary>
    /// 已经触发过的补丁id
    /// </summary>
    public List<int> HistoryPatchID;
}

/// <summary>
/// 数据基类
/// </summary>
public class SaveDataBase
{
    /// <summary>
    /// 对应哪个索引的存档 0 1 2 ...
    /// </summary>
    public int Index;
    /// <summary>
    /// 存档保存时间，可以用于显示给玩家看以及对备份存档按时间进行排序，显示的时候可以换算成年月日时分秒
    /// </summary>
    public long SaveTime;
    /// <summary>
    /// 当前备份索引，存储备份的时候，文件名格式为 Index_CurBackupsIndex(存档索引_当前备份索引)
    /// </summary>
    public int CurBackupsIndex;

    #region 通用字典
    /// <summary>
    /// 通用字典数据，字典不能序列化，所以转存成一个list
    /// </summary>
    public List<KV> m_DicList;
    [System.Serializable]
    public struct KV
    {
        public string Key;//键
        public string Value;//值
        public KV(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
    [System.Xml.Serialization.XmlIgnore]
    private Dictionary<string, string> m_Dic;
    /// <summary>
    /// 通用的字典
    /// </summary>
    [System.Xml.Serialization.XmlIgnore]
    public Dictionary<string, string> Dic
    {
        get
        {
            if (m_Dic == null || m_Dic.Count == 0)
            {
                m_Dic = new Dictionary<string, string>();
                //如果还没数据 就new一个数组
                if (m_DicList == null)
                {
                    m_DicList = new List<KV>();
                }
                //将数组里的数据转存到字典里
                foreach (var item in m_DicList)
                {
                    m_Dic.AddSafe(item.Key, item.Value);
                }
            }
            return m_Dic;
        }

    }
    /// <summary>
    /// 保存字典数据，本地化存档前调用一下，让其将字典里的数据转存到数组里，本地化会保存数组里的数据，不会保存字典里的数据
    /// </summary>
    public void SaveDic()
    {
        if (m_Dic != null && m_Dic.Count > 0)
        {
            m_DicList = new List<KV>();
            foreach (var item in m_Dic)
            {
                m_DicList.Add(new KV(item.Key, item.Value));
            }
        }

    }
    #endregion
}

/// <summary>
/// 设置数据，所有存档用同一份设置数据
/// </summary>
public class SettingData
{
    /// <summary>
    /// 当前设置
    /// </summary>
    public Data Current;

    [System.Serializable]
    public class Data
    {
        //默认数据
        public Language CurLanguageId = Language.Chinese;//当前语言
        public float SoundVolume = 0.8f;//音效音量
        public float BGMVolume = 0.5f;//音乐音量 
        public bool SoundMute = false;//音效静音
        public bool BGMMute = false;//音乐静音
        public float Brightness = 0.8f;//亮度

        public Vector2Int Resolution = new Vector2Int(1920, 1080);//分辨率
        public int Definition = 1;//清晰度 1 2 4 K

        public bool OpenGamepadShake = true;//开启手柄震动
        public bool Window = false;//窗口模式
        public bool Sync = true;//开启垂直同步
    }

}

/// <summary>
/// 统计数据
/// </summary>
public class StatisticsData
{
    public int Deaths;//总死亡次数
    public int HitTimes;//总受击次数  
    public int JumpTimes;//总跳跃次数
    public float MoveDistance;//总移动距离        
}
