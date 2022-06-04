using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Features;
using TMPro;
using UnityEngine.UI;

namespace TrafficRoad
{
    public class MenuManager : GenericMenu
    {
        [Header( "Game over popup references" )]
        [SerializeField] private GameObject m_gameOverPopup;
        [SerializeField] private Button m_nextLvlBtn;
        [SerializeField] private Button m_tryAgainBtn;
        [SerializeField] private Button m_goToMenuBtn;
        [SerializeField] private TextMeshProUGUI m_title;
        
        private GameManager m_gameManager;

        private void Start()
        {
            m_gameManager = GameManager.s_instance;
            m_nextLvlBtn.onClick.AddListener(GoToNextLevel);
            m_tryAgainBtn.onClick.AddListener(TryAgain);
            Init();
        }

        protected override void StartGame()
        {
            m_gameManager.StartGame();
            ShowMenuOnNextLoad(true); // if user close the app in game mode, after reload he will see menu panel.
        }

        public void EndGame()
        {
            bool isWin = m_gameManager.IsWin();
            m_gameManager.ActivateGUI(false);
 
            m_goToMenuBtn.onClick.AddListener(() => { GoToMenu(isWin); } );
            
            if (isWin)
            {
                m_nextLvlBtn.gameObject.SetActive(true);
                m_title.text = "You won";
            }
            else
            {
                m_tryAgainBtn.gameObject.SetActive(true);
                m_title.text = "You lose";
            }
            ShowPopup(m_gameOverPopup);
        }

        public void GoToNextLevel()
        {
            ShowMenuOnNextLoad(false);
            HidePopup(m_gameOverPopup, () => m_gameManager.ReloadScene(true));
        }

        public void TryAgain()
        {
            ShowMenuOnNextLoad(false);
            HidePopup(m_gameOverPopup, () => m_gameManager.ReloadScene(false));
        }

        public void GoToMenu(bool isWin)
        {
            ShowMenuOnNextLoad(true);
            HidePopup(m_gameOverPopup, () => m_gameManager.ReloadScene(isWin));
        }
    }
}