using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class SplashScreenSetting : MonoBehaviour
{

    const string BlackMenuPath = "EditorTools/SkipUnityLogo/设置启动画面为黑色背景 ";
    const string WhiteMenuPath = "EditorTools/SkipUnityLogo/设置启动画面为白色背景";
    [MenuItem(BlackMenuPath)]
    static void BlackSplashScreen()
    {
        Sprite background = AssetDatabase.LoadAssetAtPath("Assets/SkipUnityLogo/img/BlackSplashScreenMask.png", typeof(Sprite)) as Sprite;
        PlayerSettings.SplashScreen.background = background;
        PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
        PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
        PlayerSettings.SplashScreen.overlayOpacity = 1;
        PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.UnityLogoBelow;
        PlayerSettings.SplashScreen.blurBackgroundImage = false;
        print("黑色游戏启动画面已设置完成");
    }

    [MenuItem(WhiteMenuPath)]
    static void WhiteSplashScreen()
    {
        Sprite background = AssetDatabase.LoadAssetAtPath("Assets/SkipUnityLogo/img/WhiteSplashScreenMask.png", typeof(Sprite)) as Sprite;
        PlayerSettings.SplashScreen.background = background;
        PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight;
        PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
        PlayerSettings.SplashScreen.overlayOpacity = 1;
        PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.UnityLogoBelow;
        PlayerSettings.SplashScreen.blurBackgroundImage = false;
        print("白色游戏启动画面已设置完成");
    }
}
#endif