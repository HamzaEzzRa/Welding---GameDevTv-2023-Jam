using UnityEditor;

[CustomEditor(typeof(LightFader))]
public class LightFaderEditor : Editor
{
    private SerializedProperty emissionFadeInColor;
    private SerializedProperty emissionFadeOutColor;

    private void OnEnable()
    {
        emissionFadeInColor = serializedObject.FindProperty("emissionFadeInColor");
        emissionFadeOutColor = serializedObject.FindProperty("emissionFadeOutColor");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        LightFader lightFader = (LightFader)target;

        if (lightFader.LightRenderer != null)
        {
            EditorGUILayout.PropertyField(emissionFadeInColor);
            EditorGUILayout.PropertyField(emissionFadeOutColor);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
