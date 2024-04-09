// ******************************************************************
//       /\ /|       @file       CfgFontMap.cs
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


    public class RowCfgFontMap
    {
        public int id; //字体配置id
        public string desc; //描述
        public int[] fontIds; //中文字体
    }

    public class CfgFontMap
    {
        private readonly Dictionary<int, RowCfgFontMap> _configs = new Dictionary<int, RowCfgFontMap>(); //cfgId映射row
        public RowCfgFontMap this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgFontMap this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgFontMap> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgFontMap Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgFontMap.txt", 3);
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
        private RowCfgFontMap ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 5)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgFontMap();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //字体配置id
            data.desc = CsvUtility.ToString(rowHelper.ReadNextCol()); //描述
            data.fontIds = new int[3];
            data.fontIds[0] = CsvUtility.ToInt(rowHelper.ReadNextCol()); //中文字体
            data.fontIds[1] = CsvUtility.ToInt(rowHelper.ReadNextCol()); //中文字体
            data.fontIds[2] = CsvUtility.ToInt(rowHelper.ReadNextCol()); //中文字体
            return data;
        }
    }
