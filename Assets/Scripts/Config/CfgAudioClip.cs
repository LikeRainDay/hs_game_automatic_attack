// ******************************************************************
//       /\ /|       @file       CfgAudioClip.cs
//       \ V/        @brief      excel数据解析(由python自动生成)
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-25 13:25:11
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


    public class RowCfgAudioClip
    {
        public int id; //id
        public string annotate; //注释
        public string enumName; //枚举名
        public float volume; //音量大小
        public float cD; //作为高频音效的CD
        public string path; //路径
    }

    public class CfgAudioClip
    {
        private readonly Dictionary<int, RowCfgAudioClip> _configs = new Dictionary<int, RowCfgAudioClip>(); //cfgId映射row
        public RowCfgAudioClip this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgAudioClip this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgAudioClip> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgAudioClip Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgAudioClip.txt", 3);
            var rows = reader.GetRowCount();
            for (var i = 0; i < rows; ++i)
            {
                var row = reader.GetColValueArray(i);
                var data = ParseRow(row);
                if (!_configs.ContainsKey(data.id))
                {
                    _configs.Add(data.id, data);
                }
            }
        }

        /// <summary>
        /// 解析行
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private RowCfgAudioClip ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 6)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgAudioClip();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //id
            data.annotate = CsvUtility.ToString(rowHelper.ReadNextCol()); //注释
            data.enumName = CsvUtility.ToString(rowHelper.ReadNextCol()); //枚举名
            data.volume = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //音量大小
            data.cD = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //作为高频音效的CD
            data.path = CsvUtility.ToString(rowHelper.ReadNextCol()); //路径
            return data;
        }
    }
