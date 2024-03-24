using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
/// <summary>
/// 场景管理器
/// </summary>
public class SceneManager : Singleton<SceneManager>
{
    private readonly Dictionary<string, SceneInstance>
        _loadedSceneDic = new Dictionary<string, SceneInstance>(); //已加载的场景 k:资源路径 v:实例

    private readonly HashSet<string> _loadingHashSet = new HashSet<string>(); //正在加载的场景
    private readonly HashSet<string> _unloadingHashSet = new HashSet<string>(); //正在卸载的场景

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="scenePath"> 场景名称 </param>
    public async Task LoadSceneAsync(string scenePath)
    {
        //根据当前语言当前分辨率修改一下资源路径
        scenePath = AssetMgr.ReplacePath(scenePath);
        if (string.IsNullOrEmpty(scenePath))
        {
            LogUtil.Error($"无效路径:{scenePath}");
            return;
        }

        if (_unloadingHashSet.Contains(scenePath))
        {
            LogUtil.Error($"场景正在卸载:{scenePath}");
            return;
        }

        if (_loadingHashSet.Contains(scenePath))
        {
            LogUtil.Error($"场景正在加载:{scenePath}");
            return;
        }

        if (_loadedSceneDic.ContainsKey(scenePath))
        {
            LogUtil.Error($"场景已经加载:{scenePath}");
            return;
        }

        _loadingHashSet.Add(scenePath);
        //叠加模式添加场景
        var sceneInstance = await AssetMgr.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        _loadingHashSet.Remove(scenePath);
        _loadedSceneDic.Add(scenePath, sceneInstance);
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    /// <param name="scenePath"> 场景名称 </param>
    private async Task UnloadScene(string scenePath)
    {
        //卸载场景前 玩家可能修改过分辨率 所以这里不确定加载资源的时候是什么路径，只能是路径去掉分辨率和语言的部分匹配上了就算作是目标对象
        string temp = scenePath.Replace("4K", "").Replace("_cn", "");
        scenePath = "";
        _loadedSceneDic.ForEach(a =>
        {
            if (a.Key.Replace("4K", "").Replace("_cn", "") == temp)
            {
                scenePath = a.Key;
            }
        });

        if (string.IsNullOrEmpty(scenePath))
        {
            LogUtil.Error($"无效路径：{scenePath}");
            return;
        }

        if (_unloadingHashSet.Contains(scenePath))
        {
            LogUtil.Error($"场景正在卸载:{scenePath}");

            return;
        }

        if (_loadingHashSet.Contains(scenePath))
        {
            LogUtil.Error($"场景正在加载:{scenePath}");
            return;
        }

        if (!_loadedSceneDic.ContainsKey(scenePath))
        {
            LogUtil.Error($"场景没有被加载:{scenePath}");
            return;
        }

        //标记卸载中
        _unloadingHashSet.Add(scenePath);
        //卸载场景
        await AssetMgr.UnloadSceneAsync(_loadedSceneDic[scenePath]);
        if (_unloadingHashSet.Contains(scenePath))
        {
            _unloadingHashSet.Remove(scenePath);
        }
        if (_loadedSceneDic.ContainsKey(scenePath))
        {
            _loadedSceneDic.Remove(scenePath);
        }

    }

    /// <summary>
    /// 卸载全部场景
    /// </summary>
    public async Task UnloadAllScenes()
    {
        //卸载字典里没处于卸载的场景
        foreach (var loadedScene in _loadedSceneDic.Where(loadedScene => !_unloadingHashSet.Contains(loadedScene.Key)))
        {
            await UnloadScene(loadedScene.Key);
        }

        _loadingHashSet.Clear();
        _unloadingHashSet.Clear();
        _loadedSceneDic.Clear();
    }

}