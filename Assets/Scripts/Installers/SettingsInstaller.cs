using UnityEngine;
using Zenject;

namespace TrafficRoad
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "ScriptablesObjects/SettingsInstaller", order = 1)]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private Prefabs m_prefabs;
        [SerializeField] private TrafficSettings m_trafficSettings;
        [SerializeField] private PedestrianSettings m_pedestrianSettings;
        
        public override void InstallBindings()
        {
            Container.BindInstance(m_prefabs).AsSingle();
            Container.BindInstance(m_trafficSettings).AsSingle();
            Container.BindInstance(m_pedestrianSettings).AsSingle();
            BindLevel("Levels/");
        }

        private void BindLevel(string resourcesPath)
        {
            int levelNum = PlayerPrefs.GetInt("CurrentLevel", 1);
            LevelSettings levelSettings = Resources.Load<LevelSettings>(resourcesPath + "Level_" + levelNum);
            
            if (levelSettings == null)
            {
                // if there is no next level settings, return to the settings of the previous one.
                for (int lvl = levelNum; lvl > 0; lvl--)
                {
                    levelSettings = Resources.Load<LevelSettings>(resourcesPath + "Level " + lvl);
                    if (levelSettings != null) break;
                }
                if (levelSettings == null) { Debug.LogError("There is no level settings on specified path"); return; }
            }
            
            Container.BindInstance(levelSettings).WithId("CurrentLevel").AsSingle();
        }
    }
}