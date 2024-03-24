using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 让数组里的物体跟着当前物体一起显示隐藏
/// </summary>
public class ShowHideOtherTogether : MonoBehaviour
{
    [Header("要跟着一起显示隐藏的物体")]
    public List<GameObject> Others = new List<GameObject>();
    private void OnEnable()
    {
        Others.ForEach(a => a.Show());
    }
    private void OnDisable()
    {
        Others.ForEach(a => a.Hide());
    }

}
