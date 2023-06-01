using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector3Range))]
public class Vector3RangeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.indentLevel = 2;
        SerializedProperty minProperty = property.FindPropertyRelative("min");
        SerializedProperty maxProperty = property.FindPropertyRelative("max");
        Vector3 minValue = minProperty.vector3Value;
        Vector3 maxValue = maxProperty.vector3Value;

        EditorGUIUtility.labelWidth = 64f;
        position.xMin -= 32f;
        minValue = EditorGUI.Vector3Field(position, "Min", minValue);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        maxValue = EditorGUI.Vector3Field(position, "Max", maxValue);

        minProperty.vector3Value = minValue;
        maxProperty.vector3Value = maxValue;
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;
    }
}
