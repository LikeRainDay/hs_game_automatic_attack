using System;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 公共的Mono类 一些C#类可以使用这个公共Mono类来进行一些Mono类才能有的操作，比如Update刷新
/// </summary>
public class CommonMono : MonoSingleton<CommonMono>
{
    static CommonMono()
    {
        //在静态构造里面随便使用下Instance变量，让其自动创建一个mono对象
        string str = Instance.name;
    }

    public static Action OnUpdateEvent;
    public static Action OnLateUpdateEvent;
    public static Action OnFixedUpdateEvent;
    public static Action OnGUIEvent;
    public static Action OnDestroyEvent;
    public static Action OnApplicationQuitEvent;

    private void Update()
    {
        OnUpdateEvent?.Invoke();
    }
    private void LateUpdate()
    {
        OnLateUpdateEvent?.Invoke();
    }
    private void FixedUpdate()
    {
        OnFixedUpdateEvent?.Invoke();
    }
    private void OnGUI()
    {
        OnGUIEvent?.Invoke();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        OnDestroyEvent?.Invoke();

        OnUpdateEvent = null;
        OnLateUpdateEvent = null;
        OnFixedUpdateEvent = null;
        OnGUIEvent = null;
        OnDestroyEvent = null;
        OnApplicationQuitEvent = null;
    }
    private void OnApplicationQuit()
    {
        OnApplicationQuitEvent?.Invoke();
    }

}