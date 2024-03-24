// ******************************************************************
//       /\ /|       @file       ConfigManager.cs
//       \ V/        @brief      配置表管理器(由python自动生成)
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |
//      /  \\        @Modified   2022-04-23 22:40:15
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************


    public sealed class ConfigManager : BaseSingleTon<ConfigManager>
    {
        public readonly CfgAudioClip cfgAudioClip = new CfgAudioClip();
        public readonly CfgBullet cfgBullet = new CfgBullet();
        public readonly CfgCameraShakeData cfgCameraShakeData = new CfgCameraShakeData();
        public readonly CfgEffect cfgEffect = new CfgEffect();
        public readonly CfgEmitter cfgEmitter = new CfgEmitter();
        public readonly CfgFont cfgFont = new CfgFont();
        public readonly CfgFontMap cfgFontMap = new CfgFontMap();
        public readonly CfgFontSetting cfgFontSetting = new CfgFontSetting();
        public readonly CfgFontSettingPro cfgFontSettingPro = new CfgFontSettingPro();
        public readonly CfgItem cfgItem = new CfgItem();
        public readonly CfgPerformances cfgPerformances = new CfgPerformances();
        public readonly CfgPrefab cfgPrefab = new CfgPrefab();
        public readonly CfgRole cfgRole = new CfgRole();
        public readonly CfgScene cfgScene = new CfgScene();
        public readonly CfgShakeData cfgShakeData = new CfgShakeData();
        public readonly CfgSprite cfgSprite = new CfgSprite();
        public readonly CfgText cfgText = new CfgText();
        public readonly CfgWeapon cfgWeapon = new CfgWeapon();

        public ConfigManager()
        {
            //初始场景有Text的情况 查找翻译文本需要加载资源 因为同为Awake回调 加载顺序可能优于AssetManager 故补充加载
            AssetManager.Instance.OnInit();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            cfgAudioClip.Load();
            cfgBullet.Load();
            cfgCameraShakeData.Load();
            cfgEffect.Load();
            cfgEmitter.Load();
            cfgFont.Load();
            cfgFontMap.Load();
            cfgFontSetting.Load();
            cfgFontSettingPro.Load();
            cfgItem.Load();
            cfgPerformances.Load();
            cfgPrefab.Load();
            cfgRole.Load();
            cfgScene.Load();
            cfgShakeData.Load();
            cfgSprite.Load();
            cfgText.Load();
            cfgWeapon.Load();
        }
    }