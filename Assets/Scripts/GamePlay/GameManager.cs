using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

namespace TrafficRoad
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager s_instance;

        [SerializeField] private List<TrafficPair> m_routePairs;
        [SerializeField] private List <Crossroad> m_crossroads;
        [SerializeField] private GameTag m_turningVehicleTag;
        
        [Inject(Id = "CurrentLevel")] private LevelSettings m_levelSettings;
        [Inject] private CameraController m_camCtrl;
        [Inject] private Prefabs m_prefabs;
        [Inject] private GuiManager m_guiManager;
        [Inject] private FxEffectsPools m_fxEffectsPools;

        public GameTag TurningVehicleTag => m_turningVehicleTag;

        private GameSettings m_gameSettings;
        private bool m_canControl = false;
        private int m_currentLevel;
        private int m_nextRouteNum = 0;
        private int m_turnedCarsOnRoute = 0;
        private int m_successfulCars = 0;
        private int m_playerScore = 0;
        private float m_fxVolume;
        
        private void Awake()
        {
            if (s_instance == null) s_instance = this;
            else {Debug.LogError("Multiple singleton instances of " + this); Destroy(this);}
        }
        
        private void Start()
        {
            m_gameSettings = m_levelSettings.gameSettings;
            m_currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            SetFxVolume(PlayerPrefs.GetFloat("FX_VOLUME", 0.75f));
            MuteFxAudio(PlayerPrefs.GetInt("FX_STATE", 1) == 0);
        }

        public void StartGame()
        {
            Debug.Log("CURRENT LEVEL = " + m_currentLevel);
            if (m_currentLevel == 1) { m_guiManager.StartTutorial( InitGUI ); }
            else InitGUI();
            
            SpawnTurningVehicles();
            GoToNextRoute();
        }

        private void Update()
        {
            if (false == m_canControl) return;

            if (Input.GetMouseButtonDown(0))
            {
                m_canControl = false;
                m_crossroads[m_nextRouteNum - 1].MoveCar(); 
            }
        }

        private void GoToNextRoute()
        {
            if (m_nextRouteNum > 0) m_guiManager.IncreaseSliderValue();
            
            if (m_nextRouteNum == m_gameSettings.intersectionsCount)
            { m_guiManager.MenuManagerRef.EndGame(); m_canControl = false; return; }
            
            m_crossroads[m_nextRouteNum].MoveCameraToCenter();
            m_crossroads[m_nextRouteNum].HighlightFirstCar();
            if (m_crossroads[m_nextRouteNum].IsNewStreet) { DisableUnseenRoutes(m_crossroads[m_nextRouteNum]); }
            m_nextRouteNum++;
            m_canControl = true;
        }

        public void ReloadScene(bool isWin)
        {
            if (isWin) { PlayerPrefs.SetInt("CurrentLevel", m_currentLevel + 1); }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        private void SpawnTurningVehicles()
        {
            List<int> carsCountValues = GetCarsCountList();
            
            for (int idx = 0; idx < m_gameSettings.intersectionsCount ; idx++)
            {
                List<GameObject> vehiclesList = new List<GameObject>();
                for (int j = 0; j < carsCountValues[idx] ; j++) 
                { vehiclesList.Add(m_prefabs.turningVehicles[Random.Range(0, m_prefabs.turningVehicles.Count)] ); }

                m_crossroads[idx].SpawnVehicles(vehiclesList, m_turningVehicleTag);
            }
        }
        
        /// <summary> Returns list with cars count values in every intersection for current level. </summary>
        /// <returns></returns>
        private List<int> GetCarsCountList()
        {
            List<int> iList = new List<int>();
            int carsCount = m_gameSettings.turningCarsCount;
            int routesCount = m_gameSettings.intersectionsCount;
            int routesMod = carsCount % routesCount;
            
            for (int i = 0; i < routesCount; i++) { iList.Add(carsCount / routesCount); }
            if (routesMod != 0) { for (int j = routesMod; j > 0; j--) iList[iList.Count - j] += 1; }
            return iList;
        }

        public void SetNextCar()
        {
            m_turnedCarsOnRoute++;
            List<int> carsCountValues = GetCarsCountList();

            if (m_turnedCarsOnRoute < carsCountValues[m_nextRouteNum - 1])
            { Debug.Log("Next car set"); }
            else
            {
                Debug.Log("Last car, go to new route");
                m_turnedCarsOnRoute = 0;
                GoToNextRoute(); 
            }
        }

        private void InitGUI()
        {
            ActivateGUI(true);
            m_guiManager.InitLevelTexts(m_currentLevel, m_currentLevel + 1);
            m_guiManager.InitSliderLimit(m_gameSettings.intersectionsCount);
        }
        
        public void ActivateGUI(bool value) => m_guiManager.ActivateObjects(value);

        public void SetFxVolume(float value) => m_fxVolume = value;
        
        public void ActivateControl(bool value) => m_canControl = value;

        public void AddScore(int value, bool success)
        {
            m_playerScore += value;
            if (success) m_successfulCars++;
            if (m_playerScore < 0) m_playerScore = 0;
            m_guiManager.SetScoreText(m_playerScore);

            
            //TODO: Disable late ?
            int hintCount = PlayerPrefs.GetInt("GIVE_WAY_HINT_COUNT", 0);
            if (value < 0 && success && hintCount < 3)
            {
                PlayerPrefs.SetInt("GIVE_WAY_HINT_COUNT", hintCount + 1);
                m_guiManager.ShowGiveWayHint();
            }
        }

        private void DisableUnseenRoutes(Crossroad nextCrossroad)
        {
            TrafficPair nextPair = nextCrossroad.IsLeftCrossroad 
                ? m_routePairs.Find(pair => pair.leftRoute == nextCrossroad.Route) 
                : m_routePairs.Find(pair => pair.rightRoute == nextCrossroad.Route);
            
            // enable new traffic routes
            nextPair.rightRoute.EnableTraffic(true); nextPair.leftRoute.EnableTraffic(true);

            // disable old traffic routes with delay
            StartCoroutine(DelayAction(1f, () =>
            {
                foreach (var pair in m_routePairs.Where(pair => pair != nextPair))
                { pair.rightRoute.EnableTraffic(false); pair.leftRoute.EnableTraffic(false); }
            }));


        }
        
        public bool IsWin()
        {
            // if player successfully turned 75% of cars he won.
            return m_successfulCars >= m_gameSettings.turningCarsCount * 0.75;
        }

        public void AnimateScoreAddition(int score, Vector3 carPos) => m_guiManager.AnimateScoreAddition(score, carPos);
        
        public void AnimatePointingArrow(Vector3 carPos) => m_guiManager.AnimatePointingArrow(carPos);

        public void DisablePointingArrow() => m_guiManager.EnablePointingArrow(false);
        
        public void PlayFxEffect(EffectType type, Vector3 pos)
        {
            GameObject effect = new GameObject();
            switch (type)
            {
                case EffectType.Explosion: effect = m_fxEffectsPools.explosionEffectPool.GetPooledObject();
                    break;
            }
            
            AudioSource effectAudio = effect.GetComponent<AudioSource>();
            
            effect.SetActive(true);
            effect.transform.position = pos;
            effect.GetComponent<ParticleSystem>().Play();
            effectAudio.volume = m_fxVolume; 
            effectAudio.Play();
            StartCoroutine(DelayAction(1f, () => effect.SetActive(false)));
        }

        public void MuteFxAudio(bool value)
        {
            m_fxEffectsPools.explosionEffectPool.PooledObjects.ForEach(effect =>
                effect.GetComponent<AudioSource>().mute = value);
        }

        public float GetFxVolume() => m_fxVolume;

        public static IEnumerator DelayAction(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act?.Invoke();
        }

    }

    [Serializable] public class GameSettings
    {
        [Header("Level intersections settings")]
        public int intersectionsCount;
        public int turningCarsCount;
    }

    [Serializable] public class FxEffectsPools
    {
        public ObjectPool explosionEffectPool;
    }

    [Serializable] public class TrafficPair
    {
        public TrafficRoute rightRoute;
        public TrafficRoute leftRoute;
    }
    public enum EffectType
    {
        Explosion
    }
}