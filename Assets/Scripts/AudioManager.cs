using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    private Dictionary<string, Sound> soundsDictionary;
    public Sound[] sounds;

    public static AudioManager _instance;

	// Use this for initialization
	void Awake ()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        soundsDictionary = new Dictionary<string, Sound>();
        foreach(Sound s in sounds)
        {
            soundsDictionary.Add(s.name, s);
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;
            s.audioSource.volume = s.volume * ScenesHelper.VolumeScalar;
            s.audioSource.loop = s.loop;
        }
	}
	
    void Start()
    {
        this.Play("themeSong");
    }

	public void Play(string soundName)
    {
        if (soundsDictionary.ContainsKey(soundName) && !soundsDictionary[soundName].audioSource.isPlaying)
            soundsDictionary[soundName].audioSource.Play();
    }

    public void UpdateVolume()
    {
        foreach (Sound s in sounds)
            s.audioSource.volume = s.volume * ScenesHelper.VolumeScalar;
    }
}
