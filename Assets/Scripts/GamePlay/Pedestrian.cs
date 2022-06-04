using System;
using Features;
using UnityEngine;

namespace TrafficRoad
{
    public class Pedestrian : MonoBehaviour
    {

        // private void OnCollisionEnter(Collision col)
        // {
        //     GameTagReference gameTag = col.gameObject.GetComponent<GameTagReference>();
        //     if (gameTag == null) return;
        //     if (gameTag.ExistsTagName("Vehicle")) { gameObject.SetActive(false); }
        // }

        private void OnTriggerEnter(Collider col)
        {
            GameTagReference gameTag = col.gameObject.GetComponent<GameTagReference>();
            if (gameTag == null) return;
            if (gameTag.ExistsTagName("ReachWaypoint")) { gameObject.SetActive(false); }
        }
    }
    
}