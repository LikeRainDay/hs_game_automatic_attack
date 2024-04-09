using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class SplashScreenSetting : MonoBehaviour
{

    const string BlackMenuPath = "EditorTools/SkipUnityLogo/������������Ϊ��ɫ���� ";
    const string WhiteMenuPath = "EditorTools/SkipUnityLogo/������������Ϊ��ɫ����";
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
        print("��ɫ��Ϸ�����������������");
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
        print("��ɫ��Ϸ�����������������");
    }
}
#endif