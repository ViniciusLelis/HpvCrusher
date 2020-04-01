using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound {

    public string name;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;

    public AudioClip audioClip;

    [HideInInspector]
    public AudioSource audioSource;

}
