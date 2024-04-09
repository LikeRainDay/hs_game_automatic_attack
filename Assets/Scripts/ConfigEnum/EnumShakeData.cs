// ******************************************************************
//       /\ /|       @file       EnumShakeData.cs
//       \ V/        @brief      excel枚举(由python自动生成)
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-25 13:25:11
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************


    public enum EnumShakeData
    {
        None = 0,
        [EnumName("主角受击")] A1 = 1,  //主角受击
        [EnumName("主角冲刺")] A2 = 2,  //主角冲刺
        [EnumName("主角飞行")] A3 = 3,  //主角飞行
        [EnumName("主角大激光")] A4 = 4,  //主角大激光
        [EnumName("主角释放技能")] A5 = 5,  //主角释放技能
        [EnumName("Boss阶段击破")] A6 = 6,  //Boss阶段击破
        [EnumName("Boss击败小震动")] A7 = 7,  //Boss击败小震动
        [EnumName("Boss击败大震动")] A8 = 8,  //Boss击败大震动
    }
