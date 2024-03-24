// ******************************************************************
//       /\ /|       @file       EnumEmitter.cs
//       \ V/        @brief      excel枚举(由python自动生成)
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-25 13:25:11
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************


    public enum EnumEmitter
    {
        None = 0,
        [EnumName("测试发射器1")] A1 = 1,  //测试发射器1
        [EnumName("测试发射器2")] A2 = 2,  //测试发射器2
        [EnumName("测试发射器3")] A3 = 3,  //测试发射器3
        [EnumName("测试发射器4")] A4 = 4,  //测试发射器4
        [EnumName("测试发射器5")] A5 = 5,  //测试发射器5
    }
