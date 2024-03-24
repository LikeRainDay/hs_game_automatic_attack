// ******************************************************************
//       /\ /|       @file       CfgRole.cs
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


    public class RowCfgRole
    {
        public int id; //id
        public float needExp; //所需经验
        public string[] propertys; //当前等级属性
    }

    public class CfgRole
    {
        private readonly Dictionary<int, RowCfgRole> _configs = new Dictionary<int, RowCfgRole>(); //cfgId映射row
        public RowCfgRole this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgRole this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgRole> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgRole Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgRole.txt", 3);
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
        private RowCfgRole ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 5)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgRole();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //id
            data.needExp = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //所需经验
            data.propertys = new string[3];
            data.propertys[0] = CsvUtility.ToString(rowHelper.ReadNextCol()); //当前等级属性
            data.propertys[1] = CsvUtility.ToString(rowHelper.ReadNextCol()); //当前等级属性
            data.propertys[2] = CsvUtility.ToString(rowHelper.ReadNextCol()); //当前等级属性
            return data;
        }
    }
