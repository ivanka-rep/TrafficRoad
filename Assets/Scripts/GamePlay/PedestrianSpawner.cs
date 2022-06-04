using System;
using System.Collections;
using System.Collections.Generic;
using TrafficRoad.Waypoints;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TrafficRoad
{
    public class PedestrianSpawner : MonoBehaviour
    {
        [SerializeField] private WaypointRoute m_waypointRoute;
        [SerializeField] private Transform m_startPos;
        
        [Inject(Id = "PedestrianPools")] private List<ObjectPool> m_pedestrianPools;
        [Inject] private PedestrianSettings m_pedestrianSettings;

        private List<Pedestrian> m_pooledPedestrians;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            m_pooledPedestrians = new List<Pedestrian>();
            GetPedestriansFromPools();
            
            for (int i = 1; i <= m_pedestrianSettings.pedestriansOnStart; i++)
            {
                SpawnRandomPedestrian(i);
            }
            
            StartCoroutine(SpawnPedestriansCycle());
        }

        private IEnumerator SpawnPedestriansCycle()
        {
            float waitTime = Random.Range(m_pedestrianSettings.timeBetweenSpawnsMin, m_pedestrianSettings.timeBetweenSpawnsMax);
            yield return new WaitForSeconds(waitTime);
            
            SpawnRandomPedestrian();
            yield return SpawnPedestriansCycle();
        }

        private void SpawnRandomPedestrian(int onStartIdx = 0)
        {
            List<Pedestrian> availablePedestrians = m_pooledPedestrians.FindAll(p => p.gameObject.activeSelf == false);
            int startWaypointIdx = 0;
            int randPool = Random.Range(0, availablePedestrians.Count);
            Pedestrian pedestrian = availablePedestrians[randPool];
            
            if (onStartIdx == 0)
            {
                pedestrian.transform.position = m_startPos.position;
                pedestrian.transform.LookAt(m_waypointRoute.Waypoints[0].transform);
            }
            else
            {
                int distance = m_waypointRoute.Waypoints.Count / m_pedestrianSettings.pedestriansOnStart;
                int waypointIdx = onStartIdx * distance;

                if (m_waypointRoute.Waypoints[waypointIdx] != null)
                {
                    pedestrian.transform.position = m_waypointRoute.Waypoints[waypointIdx].transform.position;
                    pedestrian.transform.LookAt(m_waypointRoute.Waypoints[waypointIdx].NextWaypoint.transform);
                    startWaypointIdx = waypointIdx;
                }
                else return;
            }
            
            GameObject pedObj;
            (pedObj = pedestrian.gameObject).SetActive(true);
            m_waypointRoute.AddObjectToRoute(pedObj, m_pedestrianSettings.speed, startWaypointIdx);
        }
        

        private void GetPedestriansFromPools()
        {
            m_pedestrianPools.ForEach(pool =>
                { pool.PooledObjects.ForEach(obj => { m_pooledPedestrians.Add(obj.GetComponent<Pedestrian>()); }); });
        }
    }
    
    [Serializable] public class PedestrianSettings
    {
        public int pedestriansOnStart;
        public float speed;
        public float timeBetweenSpawnsMin;
        public float timeBetweenSpawnsMax;
    }
}
