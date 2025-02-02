using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using VInspector;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public enum SFX
    {
        PlayerTakeDamage,
        PlayerAttack_Normal,
        PlayerAttack_Dash,
        PlayerAttack_Spin,
        RockSmash,
        Warning,
    }
    [Serializable]
    public class SfxData
    {
        public AudioClip Clip;
        [Range(0f, 1.5f)]
        public float Volume = 1f;
        public int StartCount = 1;
        [MinMaxSlider(-3f, 3f)]
        public Vector2 PitchRange = new Vector2(1f, 1f);
        [HideInInspector] public GameObject Parent;
    }

    [Serializable]
    public class AudioSourceData
    {
        public int CurrIndex;
        public List<AudioSource> Sources;

        public AudioSourceData(int startCount)
        {
            CurrIndex = 0;
            Sources = new List<AudioSource>(startCount);
        }
    }
    
    [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;
    [SerializeField] private AudioMixerGroup musicAudioMixerGroup;

    [SerializeField] private SerializedDictionary<SFX, SfxData> sfxData;

    [SerializeField] [ReadOnly]
    private SerializedDictionary<SFX, AudioSourceData> audioSources;
    [SerializeField] private float fadeDuration;
    
    [SerializeField] private float bgmVolume;
    
    // Singleton
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than 1 AudioManager. Destroying this. Name: " + name);
            return;
        }

        audioSources = new SerializedDictionary<SFX, AudioSourceData>();
        foreach (var sfx in sfxData)
        {
            var data = sfx.Value;
            data.Parent = new GameObject(data.Clip.name);
            data.Parent.transform.parent = transform;
            audioSources[sfx.Key] = new AudioSourceData(data.StartCount);
            for (int i = 0; i < data.StartCount; i++)
            {
                AddAudioSource(sfx.Key);
            }
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    public static void PlaySFX(SFX sfx)
    {
        Vector2 pitchRange = Instance.sfxData[sfx].PitchRange;
        PlaySFX(sfx, Random.Range(pitchRange.x, pitchRange.y));
    }

    public static void PlaySFX(SFX sfx, float pitch)
    {
        AudioSourceData sourceData = Instance.audioSources[sfx];
        if (!sourceData.Sources[sourceData.CurrIndex].isPlaying)
        {
            sourceData.CurrIndex = 0;
            sourceData.Sources[sourceData.CurrIndex].pitch = pitch;
            sourceData.Sources[sourceData.CurrIndex].Play();
        }
        else
        {
            sourceData.CurrIndex++;

            if (sourceData.CurrIndex < sourceData.Sources.Count)
            {
                sourceData.Sources[sourceData.CurrIndex].pitch = pitch;
                sourceData.Sources[sourceData.CurrIndex].Play();
            }
            else if (!sourceData.Sources[0].isPlaying)
            {
                sourceData.CurrIndex = 0;
                sourceData.Sources[sourceData.CurrIndex].pitch = pitch;
                sourceData.Sources[sourceData.CurrIndex].Play();
            }
            else
            {
                // If all are still playing, add new component, add to list too
                var newSource = Instance.AddAudioSource(sfx);
                newSource.pitch = pitch;
                newSource.Play();
            }
        }
        
        // foreach (AudioSource source in sources)
        // {
        //     if (!source.isPlaying)
        //     {
        //         source.pitch = pitch;
        //         source.Play();
        //         return;
        //     }
        // }
    }

    private AudioSource AddAudioSource(SFX sfx)
    {
        var data = sfxData[sfx];
        GameObject parent = data.Parent;
        AudioSource source = parent.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = sfxAudioMixerGroup;
        source.resource = data.Clip;
        source.volume = data.Volume;
        audioSources[sfx].Sources.Add(source);
        return source;
    }
}