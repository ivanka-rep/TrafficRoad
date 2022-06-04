using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Features
{
     public abstract class GenericMenu : MonoBehaviour
     {
          [SerializeField] protected GameObject m_menuPanel;
          [SerializeField] protected Button m_startButton;
          [SerializeField] protected Button m_settingsButton;
          [SerializeField] protected GameObject m_settingsPopup;
          [SerializeField] protected GameObject m_mask;

          protected const string m_startGameKey = "SHOW_MENU";

          protected void Init()
          {
               m_startButton.onClick.AddListener(() =>
               {
                    StartGame();
                    DisplayMenu(false);
               });
               m_settingsButton.onClick.AddListener(() => { ShowPopup(m_settingsPopup); });

               if (PlayerPrefs.GetInt(m_startGameKey, 1) == 1) DisplayMenu(true);
               else StartGame();
          }

          protected void ShowMenuOnNextLoad(bool show)
          {
               int value = show ? 1 : 0;
               PlayerPrefs.SetInt(m_startGameKey, value);
          }

          protected void DisplayMenu(bool value)
          {
               m_menuPanel.SetActive(value);
          }

          public void ShowPopup(GameObject popup)
          {
               m_mask.SetActive(true);
               popup.SetActive(true);
               popup.transform.DOScale(1f, 0.25f).From(0.1f);
          }

          public void HidePopup(GameObject popup)
          {
               popup.transform.DOScale(0f, 0.25f).From(1f).onComplete =
                    () =>
                    {
                         m_mask.SetActive(false);
                         popup.SetActive(false);
                    };
          }

          public void HidePopup(GameObject popup, Action onComplete)
          {
               popup.transform.DOScale(0f, 0.25f).From(1f).onComplete =
                    () =>
                    {
                         m_mask.SetActive(false);
                         popup.SetActive(false);
                         onComplete?.Invoke();
                    };
          }

          protected abstract void StartGame();
     }
}