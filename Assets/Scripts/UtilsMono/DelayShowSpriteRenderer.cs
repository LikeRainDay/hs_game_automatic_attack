using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 延时启用SpriteRenderer组件。有时需要给点时间让物体刷新一下图片等数据 然后再呈现出来，避免切换资源时穿帮
/// </summary>
public class DelayShowSpriteRenderer : MonoBehaviour
{
    [FieldName("等待多久启用图片组件")]
    public float Delay = 0.1f;
    private async void OnEnable()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            //先禁用 然后延时显示  
            sr.enabled = false;
            await Task.Delay((int)(Delay * 1000));
            sr.enabled = true;
        }
    }

}
