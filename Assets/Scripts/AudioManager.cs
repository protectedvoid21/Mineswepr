using System;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Serializable]
    private class Sound
    {
        public string name;
        public AudioClip audioClip;
        public float volume;

        [HideInInspector]
        public AudioSource audioSource;
    }

    [SerializeField]
    private Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        foreach (var sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = PlayerPrefs.GetFloat(sound.name, 0.5f) * PlayerPrefs.GetFloat("MainVolume", 1f);
        }
    }

    public void AdjustVolume()
    {
        foreach (var sound in sounds)
        {
            sound.audioSource.volume = PlayerPrefs.GetFloat(sound.name, 0.5f) * PlayerPrefs.GetFloat("MainVolume", 1f);
        }
    }

    public void Play(string name)
    {
        Sound sound = sounds.First(f => f.name == name);
        sound?.audioSource.Play();
    }
}