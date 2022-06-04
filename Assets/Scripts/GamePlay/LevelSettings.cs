using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TrafficRoad
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptablesObjects/LevelSettings", order = 2)]
    public class LevelSettings : ScriptableObject
    {
        public GameSettings gameSettings;

    }
}