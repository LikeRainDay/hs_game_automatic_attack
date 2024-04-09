// ******************************************************************
//       /\ /|       @file       CfgItem.cs
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


    public class RowCfgItem
    {
        public int id; //id
        public string desc; //描述
        public int order; //排序
        public int type; //类型
        public string mark; //标签
        public int markTextID; //标签文本ID
        public int maxCount; //最大数量
        public int minWave; //最小波次
        public float weight; //出现的权重
        public string icon1; //穿在角色身上的图标
        public float offsetY; //高度偏移
        public int layer; //层级
        public string icon; //图标
        public int rank; //品质
        public int name; //text名字
        public int effectIntroduce; //效果描述
        public int price; //基础价钱
        public string gainEffect; //获取瞬间生效的效果
        public string baseEffect; //基础效果
        public string triggerEffect; //触发效果(可有多个触发效果，用|分割)
        public string trigger; //触发时机(用|分割)
        public List<float> triggerCD; //触发CD
        public int canTriggerTimes; //能触发的次数
        public string growUpEffect; //成长效果
        public int growUpEffectCount; //成长效果层数
        public int curTimes; //当前次数
        public int levelUpNeedTimes; //升级所需次数
        public float count; //计数用，比如一个道具总计造成了多少伤害(代码用)
    }

    public class CfgItem
    {
        private readonly Dictionary<int, RowCfgItem> _configs = new Dictionary<int, RowCfgItem>(); //cfgId映射row
        public RowCfgItem this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgItem this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgItem> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgItem Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgItem.txt", 3);
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
        private RowCfgItem ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 28)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgItem();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //id
            data.desc = CsvUtility.ToString(rowHelper.ReadNextCol()); //描述
            data.order = CsvUtility.ToInt(rowHelper.ReadNextCol()); //排序
            data.type = CsvUtility.ToInt(rowHelper.ReadNextCol()); //类型
            data.mark = CsvUtility.ToString(rowHelper.ReadNextCol()); //标签
            data.markTextID = CsvUtility.ToInt(rowHelper.ReadNextCol()); //标签文本ID
            data.maxCount = CsvUtility.ToInt(rowHelper.ReadNextCol()); //最大数量
            data.minWave = CsvUtility.ToInt(rowHelper.ReadNextCol()); //最小波次
            data.weight = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //出现的权重
            data.icon1 = CsvUtility.ToString(rowHelper.ReadNextCol()); //穿在角色身上的图标
            data.offsetY = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //高度偏移
            data.layer = CsvUtility.ToInt(rowHelper.ReadNextCol()); //层级
            data.icon = CsvUtility.ToString(rowHelper.ReadNextCol()); //图标
            data.rank = CsvUtility.ToInt(rowHelper.ReadNextCol()); //品质
            data.name = CsvUtility.ToInt(rowHelper.ReadNextCol()); //text名字
            data.effectIntroduce = CsvUtility.ToInt(rowHelper.ReadNextCol()); //效果描述
            data.price = CsvUtility.ToInt(rowHelper.ReadNextCol()); //基础价钱
            data.gainEffect = CsvUtility.ToString(rowHelper.ReadNextCol()); //获取瞬间生效的效果
            data.baseEffect = CsvUtility.ToString(rowHelper.ReadNextCol()); //基础效果
            data.triggerEffect = CsvUtility.ToString(rowHelper.ReadNextCol()); //触发效果(可有多个触发效果，用|分割)
            data.trigger = CsvUtility.ToString(rowHelper.ReadNextCol()); //触发时机(用|分割)
            data.triggerCD = CsvUtility.ToList<float>(rowHelper.ReadNextCol()); //触发CD
            data.canTriggerTimes = CsvUtility.ToInt(rowHelper.ReadNextCol()); //能触发的次数
            data.growUpEffect = CsvUtility.ToString(rowHelper.ReadNextCol()); //成长效果
            data.growUpEffectCount = CsvUtility.ToInt(rowHelper.ReadNextCol()); //成长效果层数
            data.curTimes = CsvUtility.ToInt(rowHelper.ReadNextCol()); //当前次数
            data.levelUpNeedTimes = CsvUtility.ToInt(rowHelper.ReadNextCol()); //升级所需次数
            data.count = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //计数用，比如一个道具总计造成了多少伤害(代码用)
            return data;
        }
    }
