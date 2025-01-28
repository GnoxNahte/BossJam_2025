using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    #endregion
    
    #region Private Variables
	
    #endregion
    
    #region Public Methods

    public void OnMasterVolumeChanged(float value)
    {
        // mixerGroup
    }
    #endregion
    
    #region Unity Methods
    
    #endregion
    
    #region Private Methods
    #endregion
}
