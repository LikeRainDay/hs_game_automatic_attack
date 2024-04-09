using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;


[Preserve]
public class SkipUnityLogo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void CancelSplashScreen()
    {
        Task.Run(()=>SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate));
    }
}