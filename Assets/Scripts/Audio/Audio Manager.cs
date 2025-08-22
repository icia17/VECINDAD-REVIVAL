using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public AudioMixer audioMixer;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name) {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null) {
            Debug.Log("Music not found");
        } else {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name) {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null) {
            Debug.Log("SFX not found");
        } else {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
