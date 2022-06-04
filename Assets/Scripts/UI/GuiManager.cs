using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficRoad
{
    public class GuiManager : MonoBehaviour
    {
        [SerializeField] private MenuManager m_menuManager;
        [SerializeField] private Slider m_levelProgress;
        [SerializeField] private TextMeshProUGUI m_scoreTMP;
        [SerializeField] private TextMeshProUGUI m_currentLevelTMP;
        [SerializeField] private TextMeshProUGUI m_nextLevelTMP;
        [SerializeField] private GameObject m_tutorial;
        [SerializeField] private Button m_tutorialButton;
        [SerializeField] private ObjectPool m_floatingTextPool;
        [SerializeField] private Transform m_giveWayHint;
        [SerializeField] private GameObject m_pointingArrow;
        
        public MenuManager MenuManagerRef => m_menuManager;

        public void SetScoreText(int value) => m_scoreTMP.text = value.ToString();

        public void IncreaseSliderValue() => StartCoroutine (AnimateSliderOverTime(m_levelProgress, 1f) );

        public void InitSliderLimit(float maxValue)
        {
            m_levelProgress.maxValue = maxValue;
            m_levelProgress.value = 0;
        }

        public void InitLevelTexts(int currentLevel, int nextLevel)
        {
            m_currentLevelTMP.text = currentLevel.ToString();
            m_nextLevelTMP.text = nextLevel.ToString();
        }

        public void ActivateObjects(bool value)
        {
            m_levelProgress.gameObject.SetActive(value);
            m_scoreTMP.gameObject.SetActive(value);
            m_currentLevelTMP.gameObject.SetActive(value);
            m_nextLevelTMP.gameObject.SetActive(value);
        }

        public void ShowGiveWayHint()
        {
            TextMeshProUGUI tmpGiveWay = m_giveWayHint.GetComponent<TextMeshProUGUI>();
            Color oldColor = tmpGiveWay.color; oldColor.a = 1f;
            Color newColor = oldColor; newColor.a = 0f;
            
            m_giveWayHint.gameObject.SetActive(true);
            tmpGiveWay.DOColor(newColor, 3f).From(oldColor).onComplete = () => {m_giveWayHint.gameObject.SetActive(false);};
        }
        
        public void AnimateScoreAddition(int value, Vector3 carPos)
        {
            GameObject floatingTextObj = m_floatingTextPool.GetPooledObject();
            TextMeshPro floatingText = floatingTextObj.GetComponent<TextMeshPro>();
            floatingTextObj.SetActive(true);
            floatingTextObj.transform.position = new Vector3(carPos.x, floatingTextObj.transform.position.y, carPos.z);
            floatingText.text = value > 0 ? "+ " + value : value.ToString();
            floatingText.color = value > 0 ? Color.green : Color.red;
            floatingTextObj.GetComponent<Animator>().Play("Base Layer.FloatingText");
            StartCoroutine(GameManager.DelayAction(1f, () => floatingTextObj.SetActive(false) ));
        }

        public void AnimatePointingArrow(Vector3 carPos)
        {
            m_pointingArrow.transform.position = new Vector3(carPos.x, m_pointingArrow.transform.position.y, carPos.z);
            Vector3 localPos = m_pointingArrow.transform.localPosition; localPos.z += 3f;
            m_pointingArrow.transform.localPosition = localPos;
            EnablePointingArrow(true);
        }
        
        public void EnablePointingArrow(bool value) { m_pointingArrow.SetActive(value); }
        

        public void StartTutorial(Action onEnd)
        {
            m_tutorial.SetActive(true);
            m_tutorialButton.onClick.AddListener(() => { m_tutorial.SetActive(false); onEnd.Invoke(); });
        }
        
        private IEnumerator AnimateSliderOverTime(Slider slider, float seconds)
        {
            float animationTime = 0f;
            float startValue = slider.value;
            
            while (animationTime < seconds)
            {
                animationTime += Time.deltaTime;
                float lerpValue = animationTime / seconds;
                slider.value = Mathf.Lerp(slider.value, startValue + 1, lerpValue);
                yield return null;
            }
        }
    }

}