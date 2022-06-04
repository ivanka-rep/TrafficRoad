using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficRoad
{
    public class FlashersController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_flashers;

        private void Start()
        {
            EnableFlashers(false);
        }
        
        private void OnBecameVisible()
        {
            EnableFlashers(true);
        }
        
        private void OnBecameInvisible()
        {
            EnableFlashers(false);
        }

        private void EnableFlashers(bool value)
        {
            m_flashers.ForEach(go => go.SetActive(value));
        }
    }
}