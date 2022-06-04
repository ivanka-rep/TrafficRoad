using System;
using System.Collections;
using System.Collections.Generic;
using TrafficRoad.Waypoints;
using UnityEngine;

public class PedestriansTest : MonoBehaviour
{
    [SerializeField] private GameObject m_pedestrian;
    void Start()
    {
        gameObject.GetComponent<WaypointRoute>().AddObjectToRoute(m_pedestrian, 50f);
    }

    private void FixedUpdate()
    {
        m_pedestrian.transform.position = new Vector3(m_pedestrian.transform.position.x, 0, m_pedestrian.transform.position.z);
    }
}
