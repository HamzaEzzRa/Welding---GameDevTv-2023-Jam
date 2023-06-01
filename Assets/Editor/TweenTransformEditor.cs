using UnityEditor;

[CustomEditor(typeof(TweenTransform))]
public class TweenTransformEditor : Editor
{
    private SerializedProperty onTweenHalfway;
    private SerializedProperty onTweenFinish;
    private SerializedProperty onTweenStop;

    private void OnEnable()
    {
        onTweenFinish = serializedObject.FindProperty("onTweenFinish");
        onTweenHalfway = serializedObject.FindProperty("onTweenHalfway");
        onTweenStop = serializedObject.FindProperty("onTweenStop");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        TweenTransform tweenTransform = (TweenTransform)target;

        if (tweenTransform.BackAndForth)
        {
            EditorGUILayout.PropertyField(onTweenHalfway);
        }

        EditorGUILayout.PropertyField(onTweenFinish);
        EditorGUILayout.PropertyField(onTweenStop);

        serializedObject.ApplyModifiedProperties();
    }
}
