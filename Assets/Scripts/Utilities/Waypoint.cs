using System;
using System.Collections.Generic;
using Features;
using UnityEngine;
using UnityEngine.Events;

namespace TrafficRoad.Waypoints
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] private UnityEvent onReach;
        
        public bool IsLastWaypoint { get => m_isLastWaypoint; set => m_isLastWaypoint = value; }
        public Waypoint NextWaypoint { get => m_nextWaypoint; set => m_nextWaypoint = value; }
        
        private bool m_isLastWaypoint = false;
        private Waypoint m_nextWaypoint;
        

        private void OnTriggerEnter(Collider col)
        {
            BodyToMove body = col.gameObject.GetComponent<BodyToMove>();
            
            if (body == null || body.CurrentWaypoint != this) return;
            
            if (m_isLastWaypoint == false) { body.CurrentWaypoint = m_nextWaypoint; }
            else { body.CanMove = false; Destroy(body); }
            
            onReach?.Invoke();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color col = Color.red; col.a = 0.75f;
            Gizmos.color = col;
            var cube = transform;
            Gizmos.DrawCube(cube.position, cube.localScale);
        }
#endif
    }
}