using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 多语言Image扩展
/// </summary>
public static class ImageExtension
{
    /// <summary>
    /// 异步加载精灵图片
    /// </summary>
    /// <param name="image">需要加载图片的组件</param>
    /// <param name="spritePath">资源名或全路径</param>
    /// <param name="fullPath">给的路径是否是全路径</param>
    /// <param name="loadType">资源加载类型，临时资源(卸载场景等大节点卸载资源)，永久资源</param>
    public static async void SetSpriteAsync(this Image image, string spritePath, bool fullPath = false, AssetLoadType loadType = AssetLoadType.Permanent)
    {
        //如果资源表里不存在给定名字的资源 就提示一下
        if (!fullPath && !DataMgr.AssetPathDic.ContainsKey(spritePath))
        {
            Debug.LogError($"找不到资源 spritePath:{spritePath} imgae:{image.name}");
            return;
        }

        //加载图片
        var sp = await AssetMgr.LoadAssetAsync<Sprite>(fullPath ? spritePath : DataMgr.AssetPathDic[spritePath], loadType);
        image.sprite = sp;

        //没有设置为自动拉伸的图片 使用资源本身的大小
        if (image.type == Image.Type.Simple || image.type == Image.Type.Filled)
        {
            if (image.GetComponent<RectTransform>().anchorMax - image.GetComponent<RectTransform>().anchorMin == Vector2.zero)
            {
                image.SetNativeSize();
            }

        }
    }

}