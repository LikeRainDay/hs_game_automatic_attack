// ******************************************************************
//       /\ /|       @file       CfgPerformances.cs
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


    public class RowCfgPerformances
    {
        public int id; //None
        public int nextID; //None
        public int 关卡; //释用
        public int 段落; //释用
        public int 演出; //释用
        public string level; //仅注释用
        public string annotate; //注释
        public string conditionList; //演出进入条件
        public float delay; //事件开启延时(s)
        public string actionType; //演出事件类型
        public List<string> customParams; //演出事件参数
        public float duration; //演出总时长(s)
    }

    public class CfgPerformances
    {
        private readonly Dictionary<int, RowCfgPerformances> _configs = new Dictionary<int, RowCfgPerformances>(); //cfgId映射row
        public RowCfgPerformances this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgPerformances this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgPerformances> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgPerformances Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgPerformances.txt", 3);
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
        private RowCfgPerformances ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 12)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgPerformances();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //None
            data.nextID = CsvUtility.ToInt(rowHelper.ReadNextCol()); //None
            data.关卡 = CsvUtility.ToInt(rowHelper.ReadNextCol()); //释用
            data.段落 = CsvUtility.ToInt(rowHelper.ReadNextCol()); //释用
            data.演出 = CsvUtility.ToInt(rowHelper.ReadNextCol()); //释用
            data.level = CsvUtility.ToString(rowHelper.ReadNextCol()); //仅注释用
            data.annotate = CsvUtility.ToString(rowHelper.ReadNextCol()); //注释
            data.conditionList = CsvUtility.ToString(rowHelper.ReadNextCol()); //演出进入条件
            data.delay = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //事件开启延时(s)
            data.actionType = CsvUtility.ToString(rowHelper.ReadNextCol()); //演出事件类型
            data.customParams = CsvUtility.ToList<string>(rowHelper.ReadNextCol()); //演出事件参数
            data.duration = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //演出总时长(s)
            return data;
        }
    }
