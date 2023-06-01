using UnityEditor;

[CustomEditor(typeof(RippleSoundEmitter))]
public class RippleSoundEmitterEditor : Editor
{
    private SerializedProperty soundName;
    private SerializedProperty soundList;

    private SerializedProperty randomEmissionFrequency;
    private SerializedProperty frequencyRandomRange;

    private SerializedProperty randomEmissionPosition;
    private SerializedProperty positionRandomRange;

    private SerializedProperty randomEmissionDuration;
    private SerializedProperty durationRandomRange;

    private SerializedProperty linkedRippleSpawners;

    private void OnEnable()
    {
        soundName = serializedObject.FindProperty("soundName");
        soundList = serializedObject.FindProperty("soundList");

        randomEmissionFrequency = serializedObject.FindProperty("randomEmissionFrequency");
        frequencyRandomRange = serializedObject.FindProperty("frequencyRandomRange");

        randomEmissionPosition = serializedObject.FindProperty("randomEmissionPosition");
        positionRandomRange = serializedObject.FindProperty("positionRandomRange");

        randomEmissionDuration = serializedObject.FindProperty("randomEmissionDuration");
        durationRandomRange = serializedObject.FindProperty("durationRandomRange");

        linkedRippleSpawners = serializedObject.FindProperty("linkedRippleSpawners");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        RippleSoundEmitter rippleSoundEmitter = (RippleSoundEmitter)target;

        if (rippleSoundEmitter.RandomSounds)
        {
            EditorGUILayout.PropertyField(soundList);
        }
        else
        {
            EditorGUILayout.PropertyField(soundName);
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.PropertyField(randomEmissionFrequency);

        if (rippleSoundEmitter.RandomEmissionFrequency)
        {
            EditorGUILayout.PropertyField(frequencyRandomRange);
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.PropertyField(randomEmissionPosition);

        if (rippleSoundEmitter.RandomEmissionPosition)
        {
            EditorGUILayout.PropertyField(positionRandomRange);
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.PropertyField(randomEmissionDuration);

        if (rippleSoundEmitter.RandomEmissionDuration)
        {
            EditorGUILayout.PropertyField(durationRandomRange);
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.PropertyField(linkedRippleSpawners);

        serializedObject.ApplyModifiedProperties();
    }
}
