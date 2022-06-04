using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficRoad.Waypoints
{
    public class BodyToMove : MonoBehaviour
    {
        [HideInInspector] public Waypoint CurrentWaypoint;
        [HideInInspector] public bool CanMove;
        [HideInInspector] public Rigidbody RigidBody;
        [HideInInspector] public float Speed;
        
        public void Init(Waypoint wayPoint, Rigidbody rb, bool canMove, float speed)
        {
            CurrentWaypoint = wayPoint;
            CanMove = canMove;
            RigidBody = rb;
            Speed = speed;
        }
    }
}