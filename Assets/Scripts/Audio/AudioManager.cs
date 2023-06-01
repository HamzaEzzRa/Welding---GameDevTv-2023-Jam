using UnityEngine;
using UnityEngine.Audio;

using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Focus Mechanic")]
    [SerializeField] private float focusOnSoundAddition = 7.5f;
    [SerializeField] private float soundFadeTime = 1.5f;

    [Header("Mixers")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup focusMixerGroup;

    [SerializeField] private Sound[] sounds;

    private Dictionary<string, Sound> dictionary;

    public float InitialMusicVolume { get; set; }
    public float InitialSFXVolume {get; set; }
    public float InitialFocusVolume {get; set; }

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        dictionary = new Dictionary<string, Sound>();
        foreach (Sound sound in sounds)
        {
            if (sound.clip != null)
            {
                //sound.name = sound.clip.name;
                sound.lastPlayedTime = Time.time;

                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.playOnAwake = sound.playOnAwake;
                sound.source.outputAudioMixerGroup = sound.mixerGroup;

                dictionary.Add(sound.name, sound);

                if (sound.playOnAwake)
                {
                    Play(sound.name);
                }
            }
        }
    }

    private void Update()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.clip != null)
            {
                //sound.name = sound.clip.name;
                sound.source.clip = sound.clip;

                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.playOnAwake = sound.playOnAwake;
            }
        }
    }

    public void Play(string name)
    {
        if (!dictionary.TryGetValue(name, out Sound sound))
        {
            Debug.LogWarning(string.Format("Sound file not found: \"{0}\"", name));
            return;
        }

        Play(sound);
    }

    public void PlayWithVolumeFactor(string name, float volumeFactor)
    {
        if (!dictionary.TryGetValue(name, out Sound sound))
        {
            Debug.LogWarning(string.Format("Sound file not found: \"{0}\"", name));
            return;
        }

        float originalVolume = sound.volume;
        sound.volume = originalVolume * volumeFactor;
        sound.source.volume = sound.volume;
        Play(sound);

        sound.volume = originalVolume;
        sound.source.volume = sound.volume;
    }

    public void PlayAfterDelay(Sound sound, float delay)
    {
        if (delay > 0)
        {
            StartCoroutine(PlayerAfterDelayCoroutine(name, delay));
        }
        else
        {
            Play(sound);
        }
    }

    private void Play(Sound sound)
    {
        if (sound == null || (Time.time - sound.lastPlayedTime) < sound.refractoryPeriod)
        {
            return;
        }

        if (sound.mixerGroup == musicMixerGroup)
        {
            StopAllMusic();
        }

        sound.lastPlayedTime = Time.time;
        sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = dictionary.GetValueOrDefault(name);
        if (sound != null && sound.source.isPlaying)
        {
            sound.source.Stop();
        }
    }

    public void StopAllMusic()
    {
        foreach (Sound sound in sounds)
        {
            if (sound != null && sound.source.isPlaying && sound.mixerGroup == musicMixerGroup)
            {
                sound.source.Stop();
            }
        }
    }

    public void StopAllSFXs()
    {
        foreach (Sound sound in sounds)
        {
            if (sound != null && sound.source.isPlaying && sound.mixerGroup != musicMixerGroup)
            {
                sound.source.Stop();
            }
        }
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            if (sound != null && sound.source.isPlaying)
            {
                sound.source.Stop();
            }
        }
    }

    public void MuteAll()
    {
        mainMixer.SetFloat("masterVolume", 20f * Mathf.Log10(0.0001f));
    }

    public void UnmuteAll()
    {
        mainMixer.SetFloat("masterVolume", 20f * Mathf.Log10(1f));
    }

    public void SetSoundVolume(string name, float volume)
    {
        Sound sound = dictionary.GetValueOrDefault(name);
        if (sound != null)
        {
            sound.volume = volume;
            sound.source.volume = volume;
        }
    }

    public void SetSoundRefractoryPeriod(string name, float refractoryPeriod)
    {
        Sound sound = dictionary.GetValueOrDefault(name);
        if (sound != null)
        {
            sound.refractoryPeriod = refractoryPeriod;
        }
    }

    private void OnFocusModeToggle(bool inFocus)
    {
        fadeCoroutine = StartCoroutine(FadeMixerGroupsCoroutine(inFocus, soundFadeTime));
    }

    private IEnumerator FadeMixerGroupsCoroutine(bool inFocus, float fadeTime)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        float minFocusVolume = InitialFocusVolume;
        float maxFocusVolume = InitialFocusVolume + focusOnSoundAddition;
        musicMixerGroup.audioMixer.GetFloat("focusVolume", out float currentFocusVolume);
        float startProgress = Mathf.Abs((currentFocusVolume - minFocusVolume) / (maxFocusVolume - minFocusVolume));

        float progress = inFocus ? startProgress : 1f - startProgress;
        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / fadeTime);

            float evaluationTime = inFocus ? progress : 1f - progress;
            musicMixerGroup.audioMixer.SetFloat("musicVolume", Mathf.Lerp(InitialMusicVolume, InitialMusicVolume - focusOnSoundAddition, evaluationTime));
            sfxMixerGroup.audioMixer.SetFloat("sfxVolume", Mathf.Lerp(InitialSFXVolume, InitialSFXVolume + focusOnSoundAddition, evaluationTime));
            focusMixerGroup.audioMixer.SetFloat("focusVolume", Mathf.Lerp(InitialFocusVolume, InitialFocusVolume + focusOnSoundAddition, evaluationTime));

            yield return null;
        }

        fadeCoroutine = null;
    }

    private IEnumerator PlayerAfterDelayCoroutine(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        Play(name);
    }

    private void OnEnable()
    {
        GameEvents.FocusModeEvent += OnFocusModeToggle;
    }

    private void OnDisable()
    {
        GameEvents.FocusModeEvent -= OnFocusModeToggle;
    }
}
