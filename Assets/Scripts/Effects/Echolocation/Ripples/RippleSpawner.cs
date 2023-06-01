using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

using System.Collections.Generic;

public class RippleSpawner : MonoBehaviour
{
    public RippleDataParameter RippleData => rippleDataTemplate;

    public RippleManager RippleManager
    {
        get
        {
            if (cachedRippleManager == null)
            {
                cachedRippleManager = RippleManager.Instance;
            }
            return cachedRippleManager;
        }
    }

    [SerializeField, UnityEngine.Min(0f)] private float timeFactor = 1f;
    [SerializeField, UnityEngine.Min(0f)] private float spawnFrequency = 0.05f;
    [SerializeField] private bool spawnOnAwake = true;

    [SerializeField] private Vector3 originOffset;

    [SerializeField] private RippleDataParameter rippleDataTemplate = new RippleDataParameter(
        new RippleData
        {
            origin = new Vector4Parameter { value = Vector4.zero },
            power = new FloatParameter { value = 2f },
            exteriorWidth = new FloatParameter { value = 0.025f },
            interiorWidth = new FloatParameter { value = 25f },
            duration = new FloatParameter { value = 15f },
            time = new FloatParameter { value = 0f },
        }
    );

    private RippleManager cachedRippleManager;

    private List<RippleData> ripples = new List<RippleData>();
    
    private float spawnTimer;

    private float previousSpawnFrequency;

    private void Awake()
    {
        previousSpawnFrequency = spawnFrequency;
        if (!spawnOnAwake)
        {
            spawnFrequency = 0f;
        }
    }

    private void Update()
    {
        rippleDataTemplate.value.origin.Override(transform.position + originOffset);

        spawnTimer += Time.deltaTime;
        if (spawnTimer * spawnFrequency >= 1)
        {
            SpawnRipple();
        }

        for (int i = ripples.Count - 1; i >= 0; i--)
        {
            RippleData rippleData = ripples[i];

            float newRippleTime = rippleData.time.value + timeFactor * Time.deltaTime;
            rippleData.time.Override(newRippleTime);

            if (newRippleTime >= Mathf.Max(rippleData.duration.value, rippleData.interiorWidth.value))
            {
                ripples.RemoveAt(i);
                RippleManager.RemoveRippleData(rippleData);
            }
        }
    }

    public void SpawnRipple()
    {
        RippleData newRippleData = new RippleData(rippleDataTemplate.value);
        newRippleData.time.Override(0f);

        ripples.Add(newRippleData);
        RippleManager.AddRippleData(newRippleData);

        spawnTimer = 0f;
    }

    public void SpawnRipple(Vector3 position)
    {
        RippleData newRippleData = new RippleData(rippleDataTemplate.value);
        newRippleData.origin.Override(position);
        newRippleData.time.Override(0f);

        ripples.Add(newRippleData);
        RippleManager.AddRippleData(newRippleData);

        spawnTimer = 0f;
    }

    public void SpawnRipple(float duration)
    {
        RippleData newRippleData = new RippleData(rippleDataTemplate.value);
        newRippleData.duration.Override(duration);
        newRippleData.time.Override(0f);

        ripples.Add(newRippleData);
        RippleManager.AddRippleData(newRippleData);

        spawnTimer = 0f;
    }

    public void SpawnRipple(Vector3 position, float duration)
    {
        RippleData newRippleData = new RippleData(rippleDataTemplate.value);
        newRippleData.origin.Override(position);
        newRippleData.duration.Override(duration);
        newRippleData.time.Override(0f);

        ripples.Add(newRippleData);
        RippleManager.AddRippleData(newRippleData);

        spawnTimer = 0f;
    }

    public void StartAutoSpawn()
    {
        if (spawnFrequency == 0f)
        {
            spawnFrequency = previousSpawnFrequency;
        }
    }

    public void StopAutoSpawn()
    {
        previousSpawnFrequency = spawnFrequency;
        spawnFrequency = 0f;
    }

    private void Reset()
    {
        for (int i = ripples.Count - 1; i >= 0; i--)
        {
            RippleManager.RemoveRippleData(ripples[i]);
            ripples.RemoveAt(i);
        }
        spawnTimer = 0;
    }

    private void OnDisable()
    {
        Reset();
    }
}
