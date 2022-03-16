using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(InventoryItem))]
public class InventoryItemEditor : ToggleEditor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedObject so = new SerializedObject(target);

        SerializedProperty iconProperty = so.FindProperty("m_Icon");
        SerializedProperty titleProperty = so.FindProperty("m_Title");
        SerializedProperty counterProperty = so.FindProperty("m_Counter");

        EditorGUILayout.PropertyField(iconProperty);
        EditorGUILayout.PropertyField(titleProperty);
        EditorGUILayout.PropertyField(counterProperty);

        so.ApplyModifiedProperties();
    }
}
