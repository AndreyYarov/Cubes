using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Point))]
public class PointPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        var labelWidth = EditorGUIUtility.labelWidth;

        EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.25f, labelWidth);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("X")).x;

        var xRect = new Rect(position.x, position.y, position.width * 0.32f, position.height);
        var yRect = new Rect(position.x + position.width * 0.34f, position.y, position.width * 0.32f, position.height);
        var zRect = new Rect(position.x + position.width * 0.68f, position.y, position.width * 0.32f, position.height);

        EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"));
        EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"));
        EditorGUI.PropertyField(zRect, property.FindPropertyRelative("z"));

        EditorGUI.indentLevel = indent;
        EditorGUIUtility.labelWidth = labelWidth;

        EditorGUI.EndProperty();
    }
}
