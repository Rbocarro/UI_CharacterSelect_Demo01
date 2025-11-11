using UnityEngine;
public enum AudioType { Music, SFX};
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioType type;
    
    [Range(0f,10f)]
    public float volume;
    
    [Range(0.1f,3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}