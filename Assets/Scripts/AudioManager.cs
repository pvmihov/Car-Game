using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip crashAudioClip;
    [SerializeField] private AudioClip buttonAudioClip;
    [SerializeField] private AudioClip batteryAudioClip;
    [SerializeField] private AudioClip gazAudioClip;
    [SerializeField] private AudioClip engineAudioClip;

    [SerializeField] private AudioClip driftAudioClip;


    private Dictionary<string, AudioSource> loopingAudioSources = new Dictionary<string, AudioSource>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySound(string clipName) {
        AudioClip clip = null;
        //Debug.Log($"Playing {clipName} sound effect!");
        switch (clipName) {
            case "Crash":
                clip = crashAudioClip;
                break;
            case "Button":
                clip = buttonAudioClip;
                break;
            case "Battery":
                clip = batteryAudioClip;
                break;
            case "Gaz":
                clip = gazAudioClip;
                break;
            
        }

        if (clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayLoopingSound(string clipName) {
        //Debug.Log($"Started playing {clipName} sound effect");
        if (!loopingAudioSources.ContainsKey(clipName)) {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            loopingAudioSources[clipName] = newSource;

            switch (clipName) {
                case "Drifting":
                    newSource.clip = driftAudioClip;
                    newSource.volume = 0.3f;
                    break;
                case "Engine":
                    newSource.clip = engineAudioClip;
                    newSource.volume = 0f;
                    break;
            } ///Da se vkluchi kato se dobavqt klipove

            newSource.loop = true;
            newSource.Play();
        } else if (!loopingAudioSources[clipName].isPlaying) {
            loopingAudioSources[clipName].Play();
        }
    }

    public void StopLoopingSound(string clipName) {
        //Debug.Log($"Stopped playing {clipName} sound effect");
        if (loopingAudioSources.ContainsKey(clipName)) {
            if (loopingAudioSources[clipName] != null) {
                if (loopingAudioSources[clipName].isPlaying) {
                    loopingAudioSources[clipName].Stop();
                }
            }
        }
    }

    public void MusicSpeed(float spd) {
        musicSource.pitch = spd;
    }

    public AudioSource GetAudioSource(string clipName) {
        if (loopingAudioSources.ContainsKey(clipName)) {
            return loopingAudioSources[clipName];
        }
        return null;
    }
}