using UnityEditor;
using UnityEditor.UI;
/// <summary>
/// 多语言Text文本编辑器界面扩展
/// </summary>
[CustomEditor(typeof(TransText))]
public class TransTextInspector : TextEditor
{
    private SerializedProperty m_TextId;
    private SerializedProperty m_FontSettingId;
    private SerializedProperty m_SettingId;
    private SerializedProperty m_LanguagePanel;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_TextId = serializedObject.FindProperty("TextId");
        m_FontSettingId = serializedObject.FindProperty("FontSettingId");
        m_SettingId = serializedObject.FindProperty("SettingId");
        m_LanguagePanel = serializedObject.FindProperty("LanguagePanel");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_TextId);
        EditorGUILayout.PropertyField(m_FontSettingId);
        EditorGUILayout.PropertyField(m_LanguagePanel);
        serializedObject.ApplyModifiedProperties();
    }
}