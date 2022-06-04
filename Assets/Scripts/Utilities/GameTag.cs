using UnityEngine;

namespace Features
{
    [CreateAssetMenu(fileName = "GameTag", menuName = "Game Tag", order = 0)]
    public class GameTag : ScriptableObject
    {
        [SerializeField] private string m_gameTagName;

        public string GameTagName
        {
            get => m_gameTagName;
            set => m_gameTagName = value;
        }
    }
}