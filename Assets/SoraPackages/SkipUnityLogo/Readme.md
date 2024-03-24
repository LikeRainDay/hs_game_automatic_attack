## 在游戏启动时取消 unity logo

- #### 使用方法

  - ##### 将 SkipUnityLogo 这个文件夹放在 Assets 下的任意路径

  - ##### 在编辑器上方的工具栏 EditorTools/SkipUnityLogo 下，有两个我做的快速设置启动画面的选项

    - 设置启动画面为黑色背景 ------ 适用于你自定义的 Logo 是纯白的
    - 设置启动画面为白色背景 ------ 适用于你自定义的 Logo 是纯黑的

  - ##### 附带了我从 UnityChan 的包里提取的启动画面场景，可以用来测试一下
    1.  在 File-> BuildSetting 里把 <b>SplashScreen_Dark（对应黑色背景启动画面）</b>或者 <b>SplashScreen_Light（对应白色背景启动画面）</b>拖进去设置为第一个（右边索引为 0）
    2.  Build 打包出去，enjoy！

- #### 原理
  - 在 2019.2 以后的版本，unity 官方“好心”的在脚本 api 中加入了能停止绘制启动画面的方法和枚举：
    - 在 UnityEngine.Rendering 下面的 SplashScreen 类，有一个 Stop 方法
    - `public static void Stop(Rendering.SplashScreen.StopBehavior stopBehavior);`
      只要传入这个枚举，就可以立即停止渲染 unity 启动 logo
    - `SplashScreen.StopBehavior.StopImmediate`
      <br/>
    - 这样还不行，还是会有启动 logo（<b>如果你写的这个类继承了 MonoBehavior，并挂在第一个场景的任意 GameObject 上也不行，因为想要执行这个脚本，就必须得先加载包含这个 GameObject 的场景，而启动画面是在场景加载前渲染的，所以行不通</b>），原因在于它根本不会被执行，所以还需要在你写的方法调用 Stop() 前加上：`RuntimeInitializeOnLoadMethodAttribute` 这个特性，
      用它另一个重载，能接收 RuntimeInitializeLoadType 枚举参数的这个：<br/>`RuntimeInitializeOnLoadMethodAttribute ( RuntimeInitializeLoadType loadType )`<br/>
      它有一个枚举叫`RuntimeInitializeLoadType.BeforeSplashScreen`，把这个传进去就可以了<br/>
      即： `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]`
      <br/>
  - 已经实现了跳过启动 logo，但是效果还不够好，现在运行打包好的程序你会发现画面会灰一下，原因在于：
    - 调用 stop()它实际上是直接跳到渲染完启动 logo 的最后一帧，而且在 ProjectSettings->Player->SplashImage 选项卡下，它默认使用的是设置好的背景颜色(Background Color)，就算你调成了纯黑（或纯白），不透明度(Overlay Opacity)它不让你调到 50%以下，所以才会出现这种情况。
  - 解决办法：虽然不透明度有限制，但是它有背景图片（BackgroudImage）的选项，所以最简单的办法就是，去 ps 里做一张很小的图片遮罩，填充纯黑（或纯白），然后放在 BackgroudImage 上 把不透明度调成 1，完全盖住下面的颜色，就解决了
