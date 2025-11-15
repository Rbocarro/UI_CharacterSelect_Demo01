using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    private Dictionary<string, Sound> soundDictionary;
    [Range(0f, 1f)]
    public float globalVolume=1f;
    [Range(0f, 1f)]
    public float globalMusicVolume;
    [Range(0f, 1f)]
    public float globalSFXVolume;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        soundDictionary = new Dictionary<string, Sound>();
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            soundDictionary[s.name] = s;
        }
        UpdateGlobalVolume(globalVolume);
    }
    public void Play(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
            s.source.Play();
        else
            Debug.LogWarning("Sound: " + name + " not found");
    }
    public void Stop(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
            s.source.Stop();
        else
            Debug.LogWarning("Sound: " + name + " not found");
    }
    public void UpdateVolumeByType(AudioType type, float volume)
    {
        // Update global variables
        if (type == AudioType.Music)
            globalMusicVolume = volume;
        else
            globalSFXVolume = volume;

        // Apply changes to all sounds of that type
        foreach (Sound s in sounds)
        {
            if (s.type == type)
            {
                float globalMultiplier = (type == AudioType.Music) ? globalMusicVolume : globalSFXVolume;
                s.source.volume = s.volume * globalMultiplier* globalVolume;
            }
        }
    }
    public void UpdateGlobalVolume(float volume)
    {
        globalVolume= volume;
        foreach(Sound s in sounds)
        {
            float typeMultiplier = (s.type == AudioType.Music) ? globalMusicVolume : globalSFXVolume;
            s.source.volume = s.volume * typeMultiplier * globalVolume;
        }
    }
}
