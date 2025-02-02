using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private AudioMixer mixerGroup;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    #endregion
    
    #region Private Variables
	private readonly string MasterVolumeKey = "MasterVolume";
	private readonly string MusicVolumeKey = "MusicVolume";
	private readonly string SfxVolumeKey = "SfxVolume";
    #endregion
    
    #region Public Methods

    public void OnMasterVolumeChanged(float value)
    {
        mixerGroup.SetFloat(MasterVolumeKey, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        mixerGroup.SetFloat(MusicVolumeKey, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        mixerGroup.SetFloat(SfxVolumeKey, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
    }
    #endregion
    
    #region Unity Methods

    private void Start()
    {
        float defaultValue = 0.6f;
        
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, defaultValue);
        OnMasterVolumeChanged(masterVolume);
        masterVolumeSlider.value = masterVolume;
        
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, defaultValue);
        OnMusicVolumeChanged(musicVolume);
        musicVolumeSlider.value = musicVolume;
        
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, defaultValue);
        OnSFXVolumeChanged(sfxVolume);
        sfxVolumeSlider.value = sfxVolume;

        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    #endregion
    
    #region Private Methods
    #endregion
}
