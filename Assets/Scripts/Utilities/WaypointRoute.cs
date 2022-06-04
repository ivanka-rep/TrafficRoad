using System;
using System.Collections;
using System.Collections.Generic;
using Features;
using UnityEngine;

namespace TrafficRoad.Waypoints
{
    public class WaypointRoute : MonoBehaviour
    {
        [SerializeField] private List<Waypoint> m_waypoints;

        [Tooltip("Look direction changing smoothness ")] 
        [SerializeField] private float m_smoothness;

        public List<Waypoint> Waypoints => m_waypoints;
        public List<BodyToMove> m_bodies;
        
        private void Awake()
        {
            m_bodies = new List<BodyToMove>();
            
            for (int i = 0; i < m_waypoints.Count - 1; i++)
            {
                int idx = i;
                m_waypoints[i].NextWaypoint = m_waypoints[idx + 1];
            }

            m_waypoints[m_waypoints.Count - 1].IsLastWaypoint = true;
        }

        public void AddObjectToRoute(GameObject obj, float objectSpeed, int startWaypointIdx = 0)
        {
            BodyToMove body = obj.AddComponent<BodyToMove>();
            body.Init(m_waypoints[startWaypointIdx], obj.GetComponent<Rigidbody>(), true, objectSpeed);
            m_bodies.Add(body);
        }

        private void FixedUpdate()
        {
            MoveBodies();
        }

        private void MoveBodies()
        {
            foreach (var body in m_bodies)
            {
                if (body == null || !body.CanMove) { m_bodies.Remove(body); return;}
                
                Transform objTransform = body.transform;
                Quaternion rot = objTransform.rotation;
                Vector3 lookDirection = body.CurrentWaypoint.transform.position - objTransform.position;

                if (lookDirection != Vector3.zero)
                { body.transform.rotation = Quaternion.Lerp(rot, 
                    Quaternion.LookRotation(lookDirection, Vector3.up), Time.fixedDeltaTime * m_smoothness); }
                
                body.RigidBody.velocity = objTransform.forward * (body.Speed * Time.fixedDeltaTime * 6f);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_waypoints== null || m_waypoints.Count < 1) return;
            
            Gizmos.color = Color.yellow;
            for (int i = 0; i < m_waypoints.Count - 1; i++)
            {
                if (m_waypoints[i] == null) continue;
                Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[i + 1].transform.position);
            }
        }
#endif
        
    }
    
}