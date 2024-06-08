using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;


[System.Serializable]
public class Sound
{

    public string name;

    public AudioClip clip;
    public AudioClip[] clipsArray;

    [Range(0f, 1f)]
    public float volume = .75f;
    [Range(0f, 1f)]
    public float volumeVariance = .1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float pitchVariance = .1f;

    [Range(0f, 1f)]
    public float spatialBlend = 1;
    public bool loop = false;

    //public AudioMixerGroup mixerGroup;

    [HideInInspector]
    public AudioSource source;
    [HideInInspector]
    public GameObject gameObj;
}


public class AudioManager : SingletonPersistent<AudioManager>
{
    const string MASTER_VOLUME = "MasterVolume";
    const string SOUND_VOLUME = "SoundVolume";
    const string MUSIC_VOLUME = "MusicVolume";
    const float MIXER_MIN_VALUE = 0.001f;

    [SerializeField] private Sound[] sounds;
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioSource soundAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private Transform sfxGameObjectsParent;






    protected override void Awake()
    {
        base.Awake();
        foreach (Sound s in sounds)
        {
            s.gameObj = new GameObject(s.name + "_AudioSource");
            s.gameObj.transform.parent = sfxGameObjectsParent.transform;
            s.source = s.gameObj.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
            s.source.maxDistance = soundAudioSource.maxDistance;
            s.source.minDistance = soundAudioSource.minDistance;

            s.source.outputAudioMixerGroup = mainMixer.outputAudioMixerGroup;
        }

    }



    public void SetMusicVolume(float valueNormalized)
    {
        mainMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(Mathf.Max(MIXER_MIN_VALUE, valueNormalized)) * 20);
    }

    public void SetSoundVolume(float valueNormalized)
    {
        mainMixer.SetFloat(SOUND_VOLUME, Mathf.Log10(Mathf.Max(MIXER_MIN_VALUE, valueNormalized)) * 20);
    }

    public void Play(string sound, bool oneShot = false)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.pitch = 1;

        if (oneShot)
            s.source.PlayOneShot(s.clip);
        else
        {
            if (s.loop)
            {
                if (s.source.isPlaying) return;
            }
            s.source.Play();
        }
    }

    public void Play(string sound, Vector3 position, bool oneShot = false)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            return;
        }

        s.gameObj.transform.position = position;
        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.pitch = 1;

        if (oneShot)
            s.source.PlayOneShot(s.clip);
        else
        {
            if (s.loop)
            {
                if (s.source.isPlaying) return;
            }
            s.source.Play();
        }
    }

    public void Play(string sound, float pitch, Vector3 position, bool oneShot = false)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            return;
        }
        s.pitch = pitch;

        s.gameObj.transform.position = position;
        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = pitch;

        if (oneShot)
            s.source.PlayOneShot(s.clip);
        else
        {
            if (s.loop)
            {
                if (s.source.isPlaying) return;
            }
            s.source.Play();
        }
    }

    public void SetMute(bool val)
    {
        foreach (var s in FindObjectsOfType<AudioSource>(false))
        {
            s.mute = val;
        }
    }

    public void Stop(string sound, bool mute = false)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            //Debug.Log("Sound: " + sound + " not found!");
            return;
        }

        s.source.mute = mute;
        s.source.Stop();
    }

    public void Play2RandomSounds(string s1, string s2)
    {
        var rand = (UnityEngine.Random.Range(0, 2) == 0) ? s1 : s2;

        Sound s = Array.Find(sounds, item => item.name == rand);

        if (s == null)
        {
            //Debug.Log("Sound: " + rand + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.PlayOneShot(s.clip);

    }

    public void Play2RandomSounds(string s1, string s2, Vector3 position)
    {
        var rand = (UnityEngine.Random.Range(0, 2) == 0) ? s1 : s2;

        Sound s = Array.Find(sounds, item => item.name == rand);

        if (s == null)
        {
            //Debug.Log("Sound: " + rand + " not found!");
            return;
        }
        s.gameObj.transform.position = position;
        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.PlayOneShot(s.clip);

    }

    public void PlayRandom(string name)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            //Debug.Log("Sound: " + rand + " not found!");
            return;
        }
        if (s.clipsArray.Length > 0)
        {
            System.Random random = new System.Random();
            var rand = random.Next(0, s.clipsArray.Length);
            //Debug.Log("Radn: " + rand);

            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

            s.source.PlayOneShot(s.clipsArray[rand]);
        }
        else
        {
            Debug.LogError("No Random Clips Here: " + name);
        }

    }

    public void PlayRandom(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            //Debug.Log("Sound: " + rand + " not found!");
            return;
        }
        if (s.clipsArray.Length > 0)
        {
            s.gameObj.transform.position = position;

            System.Random random = new System.Random();
            var rand = random.Next(0, s.clipsArray.Length);
            //Debug.Log("Radn: " + rand);

            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

            s.source.PlayOneShot(s.clipsArray[rand]);
        }
        else
        {
            Debug.LogError("No Random Clips Here: " + name);
        }

    }

    public void PlaySuccesesSound(string name, int index)
    {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null)
        {
            //Debug.Log("Sound: " + rand + " not found!");
            return;
        }
        if (s.clipsArray.Length > 0)
        {
            //Debug.Log("Radn: " + rand);

            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

            s.source.PlayOneShot(s.clipsArray[index]);
        }
        else
        {
            Debug.LogError("No Random Clips Here: " + name);
        }
    }
}



