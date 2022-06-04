using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

namespace TrafficRoad
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private Slider m_musicVolume;
        [SerializeField] private Slider m_FxVolume;

        [SerializeField] private Toggle m_MusicToggle;
        [SerializeField] private Toggle m_FxToggle;
        [SerializeField] private Toggle m_vibrationToggle; 
        
        private void Start()
        {
            // VOLUME SETTINGS
            
            m_FxVolume.onValueChanged.AddListener(value =>
            {
                GameManager.s_instance.SetFxVolume(value);
                PlayerPrefs.SetFloat("FX_VOLUME", value);
            });

            m_musicVolume.onValueChanged.AddListener(value =>
            {
                AudioManager.s_instance.SetMusicVolume(value);
                PlayerPrefs.SetFloat("MUSIC_VOLUME", value);
            });

            // TOGGLES SETTINGS
            
            int togglesValue = 0;

            m_MusicToggle.onValueChanged.AddListener(value =>
            {
                AudioManager.s_instance.MuteMusic(!value);
                togglesValue = value ? 1 : 0;
                PlayerPrefs.SetInt("MUSIC_STATE", togglesValue );
            });
            
            m_FxToggle.onValueChanged.AddListener(value =>
            {
                GameManager.s_instance.MuteFxAudio(!value);
                togglesValue = value ? 1 : 0;
                PlayerPrefs.SetInt("FX_STATE", togglesValue );
            });
            
            m_vibrationToggle.onValueChanged.AddListener(value =>
            {
                MMVibrationManager.SetHapticsActive(value);
                togglesValue = value ? 1 : 0;
                PlayerPrefs.SetInt("VIBRATION_STATE", togglesValue );
            });

            m_MusicToggle.isOn = PlayerPrefs.GetInt("MUSIC_STATE", 1) == 1;
            m_FxToggle.isOn = PlayerPrefs.GetInt("FX_STATE", 1) == 1;
            m_vibrationToggle.isOn = PlayerPrefs.GetInt("VIBRATION_STATE", 1) == 1;
        }

    }
}