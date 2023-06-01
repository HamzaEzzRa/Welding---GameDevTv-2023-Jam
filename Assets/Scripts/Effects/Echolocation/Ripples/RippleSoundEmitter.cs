using UnityEngine;

public class RippleSoundEmitter : MonoBehaviour
{
    public bool RandomSounds => randomSounds;
    public bool RandomEmissionFrequency => randomEmissionFrequency;
    public bool RandomEmissionPosition => randomEmissionPosition;
    public bool RandomEmissionDuration => randomEmissionDuration;

    [SerializeField] private bool emitOnStart;

    [SerializeField] private bool randomSounds;
    [SerializeField, HideInInspector] private string soundName;
    [SerializeField, HideInInspector] private string[] soundList;

    [SerializeField, HideInInspector] private bool randomEmissionFrequency;
    [SerializeField, HideInInspector, FloatRangeSlider(0.01f, 10f)] private FloatRange frequencyRandomRange = new FloatRange(0.1f, 1f);

    [SerializeField, HideInInspector] private bool randomEmissionPosition;
    [SerializeField, HideInInspector] private Vector3Range positionRandomRange;

    [SerializeField, HideInInspector] private bool randomEmissionDuration;
    [SerializeField, HideInInspector, FloatRangeSlider(0.01f, 10f)] private FloatRange durationRandomRange = new FloatRange(0.1f, 1f);

    [SerializeField, HideInInspector] private RippleSpawner[] linkedRippleSpawners;

    private float lastEmissionTime;
    private float currentRandomFrequency;

    private void Start()
    {
        if (emitOnStart)
        {
            lastEmissionTime = Time.time;
            Emit();
        }

        currentRandomFrequency = frequencyRandomRange.RandomInRange;
    }

    private void Update()
    {
        if (randomEmissionFrequency && (Time.time - lastEmissionTime) >= (1f / currentRandomFrequency))
        {
            Emit();
            lastEmissionTime = Time.time;
            currentRandomFrequency = frequencyRandomRange.RandomInRange;
        }
    }

    public void Emit()
    {
        foreach (RippleSpawner rippleSpawner in linkedRippleSpawners)
        {
            if (!randomEmissionPosition && !randomEmissionDuration)
            {
                rippleSpawner.SpawnRipple();
            }
            else if (!randomEmissionPosition)
            {
                rippleSpawner.SpawnRipple(durationRandomRange.RandomInRange);
            }
            else if (!randomEmissionDuration)
            {
                rippleSpawner.SpawnRipple(positionRandomRange.RandomInRange);
            }
            else
            {
                rippleSpawner.SpawnRipple(positionRandomRange.RandomInRange, durationRandomRange.RandomInRange);
            }
        }

        if (!randomSounds)
        {
            if (!string.IsNullOrEmpty(soundName))
            {
                AudioManager.Instance.Play(soundName);
            }
        }
        else if (soundList.Length > 0)
        {
            string randomSoundName = soundList[Random.Range(0, soundList.Length)];
            AudioManager.Instance.Play(randomSoundName);
        }
    }

    public void StopSound()
    {
        AudioManager.Instance.Stop(soundName);
    }
}
