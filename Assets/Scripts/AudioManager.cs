using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrafficRoad
{

   public class AudioManager : MonoBehaviour
   {
      public static AudioManager s_instance;

      [SerializeField] private AudioSource m_menuMusicSource;

      private void Awake()
      {
         if (s_instance == null)
         {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
         }
         else { Destroy(this); }

         //Restart progress due to changing level scriptable objects names!
         if (PlayerPrefs.GetInt("RESTART_LEVEL_PROGRESS", 0) == 0)
         {
            PlayerPrefs.SetInt("RESTART_LEVEL_PROGRESS", 1);
            PlayerPrefs.SetInt("CurrentLevel", 1);
         }

         SetMusicVolume(PlayerPrefs.GetFloat("MUSIC_VOLUME", 0.75f));
         MuteMusic(PlayerPrefs.GetInt("MUSIC_STATE", 1) == 0);
         SetVibration();

         SceneManager.LoadScene(1);
      }

      public void SetMusicVolume(float value)
      {
         m_menuMusicSource.volume = value;
      }

      public void MuteMusic(bool value)
      {
         m_menuMusicSource.mute = value;
      }

      private void SetVibration()
      {
         MMVibrationManager.SetHapticsActive(PlayerPrefs.GetInt("VIBRATION_STATE", 1) == 1);
      }
   }
}