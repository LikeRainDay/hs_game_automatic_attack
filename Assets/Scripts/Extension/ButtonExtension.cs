using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 多语言Button扩展
/// </summary>
public static class ButtonExtension
{
    /// <summary>
    /// 根据当前语言，更新按钮图片
    /// </summary>
    /// <param name="transButton"></param>
    public static async void UpdateSpriteState(this TransButton transButton, AssetLoadType loadType = AssetLoadType.Permanent)
    {
        //原状态
        var highlightedSprite = transButton.spriteState.highlightedSprite;
        var pressedSprite = transButton.spriteState.pressedSprite;
        var selectedSprite = transButton.spriteState.selectedSprite;
        var disabledSprite = transButton.spriteState.disabledSprite;

        //如果设置了多语言图片资源名，就加载和更新
        if (!string.IsNullOrEmpty(transButton.HighlightedAssetName))
        {
            highlightedSprite = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[transButton.HighlightedAssetName], loadType);
        }
        if (!string.IsNullOrEmpty(transButton.PressedAssetName))
        {
            pressedSprite = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[transButton.PressedAssetName], loadType);
        }
        if (!string.IsNullOrEmpty(transButton.SelectedAssetName))
        {
            selectedSprite = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[transButton.SelectedAssetName], loadType);
        }
        if (!string.IsNullOrEmpty(transButton.DisabledAssetName))
        {
            disabledSprite = await AssetMgr.LoadAssetAsync<Sprite>(DataMgr.AssetPathDic[transButton.DisabledAssetName], loadType);
        }

        //设置新的状态
        transButton.spriteState = new SpriteState
        {
            highlightedSprite = highlightedSprite,
            pressedSprite = pressedSprite,
            selectedSprite = selectedSprite,
            disabledSprite = disabledSprite
        };
    }
}