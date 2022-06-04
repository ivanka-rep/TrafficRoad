using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class GameTagReference : MonoBehaviour
    {
        [SerializeField] private List<GameTag> m_gameTags;

        public List<GameTag> GameTags
        {
            get => m_gameTags;
            set => m_gameTags = value;
        }

        public bool ExistsTagName(string searchName)
        {
            foreach (GameTag gameTag in m_gameTags)
            {
                if (gameTag.GameTagName.Equals(searchName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}