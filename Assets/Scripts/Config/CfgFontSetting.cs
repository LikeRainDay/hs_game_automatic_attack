// ******************************************************************
//       /\ /|       @file       CfgFontSetting.cs
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


    public class RowCfgFontSetting
    {
        public int id; //id
        public string desc; //描述
        public TextSetting[] textSettings; //TextSetting
    }

    public struct TextSetting
    {
        public float fontSizeScale; //中文Text字号缩放比
        public float lineSpacingScale; //中文Text行间距缩放比
    }

    public class CfgFontSetting
    {
        private readonly Dictionary<int, RowCfgFontSetting> _configs = new Dictionary<int, RowCfgFontSetting>(); //cfgId映射row
        public RowCfgFontSetting this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgFontSetting this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgFontSetting> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgFontSetting Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgFontSetting.txt", 3);
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
        private RowCfgFontSetting ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 8)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgFontSetting();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //id
            data.desc = CsvUtility.ToString(rowHelper.ReadNextCol()); //描述
            data.textSettings = new TextSetting[3];
            data.textSettings[0] = new TextSetting();
            data.textSettings[0].fontSizeScale = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //中文Text字号缩放比
            data.textSettings[0].lineSpacingScale = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //中文Text行间距缩放比
            data.textSettings[1] = new TextSetting();
            data.textSettings[1].fontSizeScale = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //中文Text字号缩放比
            data.textSettings[1].lineSpacingScale = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //中文Text行间距缩放比
            data.textSettings[2] = new TextSetting();
            data.textSettings[2].fontSizeScale = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //中文Text字号缩放比
            data.textSettings[2].lineSpacingScale = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //中文Text行间距缩放比
            return data;
        }
    }
