// ******************************************************************
//       /\ /|       @file       CfgShakeData.cs
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


    public class RowCfgShakeData
    {
        public int id; //id
        public string annotate; //注释
        public string enumName; //枚举名
        public List<float> param; //强度，时长，优先级
        public string des; //描述
    }

    public class CfgShakeData
    {
        private readonly Dictionary<int, RowCfgShakeData> _configs = new Dictionary<int, RowCfgShakeData>(); //cfgId映射row
        public RowCfgShakeData this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgShakeData this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgShakeData> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgShakeData Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgShakeData.txt", 3);
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
        private RowCfgShakeData ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 5)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgShakeData();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //id
            data.annotate = CsvUtility.ToString(rowHelper.ReadNextCol()); //注释
            data.enumName = CsvUtility.ToString(rowHelper.ReadNextCol()); //枚举名
            data.param = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //强度，时长，优先级
            data.des = CsvUtility.ToString(rowHelper.ReadNextCol()); //描述
            return data;
        }
    }
