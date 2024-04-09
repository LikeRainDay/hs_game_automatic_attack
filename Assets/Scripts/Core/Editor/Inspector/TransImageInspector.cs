using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
/// <summary>
/// 多语言图片编辑器界面扩展
/// </summary>
[CustomEditor(typeof(TransImage))]
public class TransImageInspector : ImageEditor
{
    private SerializedProperty m_SpriteAssetName;//资源名
    private SerializedProperty m_LanguagePanel;//是否是语言界面用的(不关键)
    private SerializedProperty m_LoadType;//资源加载方式，临时加载的资源会在切换场景等大环节卸载掉，永久加载的资源则会一直存在内存里
    protected override void OnEnable()
    {
        base.OnEnable();
        m_SpriteAssetName = serializedObject.FindProperty("SpriteAssetName");
        m_LanguagePanel = serializedObject.FindProperty("LanguagePanel");
        m_LoadType = serializedObject.FindProperty("LoadType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_SpriteAssetName);
        EditorGUILayout.PropertyField(m_LanguagePanel);
        EditorGUILayout.PropertyField(m_LoadType);
        serializedObject.ApplyModifiedProperties();
    }
}
