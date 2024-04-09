using UnityEditor;
using UnityEditor.UI;
/// <summary>
/// 多语言按钮编辑器界面扩展
/// </summary>
[CustomEditor(typeof(TransButton))]
public class TransButtonInspector : ButtonEditor
{
    private SerializedProperty m_HighlightedAssetName;
    private SerializedProperty m_PressedAssetName;
    private SerializedProperty m_SelectedAssetName;
    private SerializedProperty m_DisabledAssetName;
    private SerializedProperty m_LanguagePanel;
    private SerializedProperty m_LoadType;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_HighlightedAssetName = serializedObject.FindProperty("HighlightedAssetName");
        m_PressedAssetName = serializedObject.FindProperty("PressedAssetName");
        m_SelectedAssetName = serializedObject.FindProperty("SelectedAssetName");
        m_DisabledAssetName = serializedObject.FindProperty("DisabledAssetName");
        m_LanguagePanel = serializedObject.FindProperty("LanguagePanel");
        m_LoadType = serializedObject.FindProperty("LoadType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_HighlightedAssetName);
        EditorGUILayout.PropertyField(m_PressedAssetName);
        EditorGUILayout.PropertyField(m_SelectedAssetName);
        EditorGUILayout.PropertyField(m_DisabledAssetName);
        EditorGUILayout.PropertyField(m_LanguagePanel);
        EditorGUILayout.PropertyField(m_LoadType);
        serializedObject.ApplyModifiedProperties();
    }

}