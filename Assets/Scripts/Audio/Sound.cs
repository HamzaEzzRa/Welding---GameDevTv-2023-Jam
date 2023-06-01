using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [HideInInspector] public AudioSource source;

    public AudioClip clip;
    public string name;

    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)] public float volume = 0.5f;

    [Range(0.1f, 3f)] public float pitch = 1f;

    public float refractoryPeriod = 0f;

    public bool playOnAwake;
    public bool loop;

    [HideInInspector] public float lastPlayedTime;
}
