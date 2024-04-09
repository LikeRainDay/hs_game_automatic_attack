// ******************************************************************
//       /\ /|       @file       CfgWeapon.cs
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


    public class RowCfgWeapon
    {
        public int id; //id
        public string desc; //描述
        public string bullet; //子弹
        public float bulletSpeed; //子弹速度
        public float damageBase; //基础伤害
        public float meleeDamage; //近战系数
        public float rangedDamage; //远程系数
        public float elementalDamage; //元素系数
        public float engineering; //工程学系数
        public float attackSpeed; //攻速系数
        public float level; //等级系数
        public float maxHP; //最大生命值系数
        public float speed; //速度系数
        public float armor; //护甲系数
        public float rangeRate; //范围系数
        public float critMultiply; //暴击倍率
        public float critChance; //基础暴击率
        public float coolDown; //冷却
        public float knockback; //击退
        public float range; //范围
        public int melee; //近战
        public int bounces; //反弹次数
        public int piercing; //贯通
        public float piercingDamage; //贯通衰减
        public int special; //特殊描述
        public int gainDes; //获得属性描述
    }

    public class CfgWeapon
    {
        private readonly Dictionary<int, RowCfgWeapon> _configs = new Dictionary<int, RowCfgWeapon>(); //cfgId映射row
        public RowCfgWeapon this[Enum cid] => _configs.ContainsKey(cid.GetHashCode()) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public RowCfgWeapon this[int cid] => _configs.ContainsKey(cid) ? _configs[cid.GetHashCode()] : throw new Exception($"找不到配置 Cfg:{GetType()} configId:{cid}");
        public List<RowCfgWeapon> AllConfigs => _configs.Values.ToList();

        /// <summary>
        /// 获取行数据
        /// </summary>
        public RowCfgWeapon Find(int i)
        {
            return this[i];
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        public void Load()
        {
            var reader = new CsvReader();
            reader.LoadText("Assets/AddressableAssets/Config/CfgWeapon.txt", 3);
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
        private RowCfgWeapon ParseRow(string[] col)
        {
            //列越界
            if (col.Length < 26)
            {
                Debug.LogError($"配置表字段行数越界:{GetType()}");
                return null;
            }

            var data = new RowCfgWeapon();
            var rowHelper = new RowHelper(col);
            data.id = CsvUtility.ToInt(rowHelper.ReadNextCol()); //id
            data.desc = CsvUtility.ToString(rowHelper.ReadNextCol()); //描述
            data.bullet = CsvUtility.ToString(rowHelper.ReadNextCol()); //子弹
            data.bulletSpeed = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //子弹速度
            data.damageBase = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //基础伤害
            data.meleeDamage = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //近战系数
            data.rangedDamage = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //远程系数
            data.elementalDamage = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //元素系数
            data.engineering = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //工程学系数
            data.attackSpeed = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //攻速系数
            data.level = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //等级系数
            data.maxHP = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //最大生命值系数
            data.speed = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //速度系数
            data.armor = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //护甲系数
            data.rangeRate = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //范围系数
            data.critMultiply = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //暴击倍率
            data.critChance = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //基础暴击率
            data.coolDown = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //冷却
            data.knockback = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //击退
            data.range = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //范围
            data.melee = CsvUtility.ToInt(rowHelper.ReadNextCol()); //近战
            data.bounces = CsvUtility.ToInt(rowHelper.ReadNextCol()); //反弹次数
            data.piercing = CsvUtility.ToInt(rowHelper.ReadNextCol()); //贯通
            data.piercingDamage = CsvUtility.ToFloat(rowHelper.ReadNextCol()); //贯通衰减
            data.special = CsvUtility.ToInt(rowHelper.ReadNextCol()); //特殊描述
            data.gainDes = CsvUtility.ToInt(rowHelper.ReadNextCol()); //获得属性描述
            return data;
        }
    }
