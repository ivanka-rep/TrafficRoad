using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TrafficRoad
{
    public class TrafficRoute : MonoBehaviour
    {
        [SerializeField] private bool m_enableTraffic = false;
        [SerializeField] private Transform m_startPos;
        [SerializeField] private Transform m_finishPos;

        [Inject(Id = "VehiclePools")] private List<ObjectPool> m_vehiclePools;
        [Inject] private TrafficSettings m_trafficSettings;
        
        private List<Vehicle> m_pooledVehicles;
        private List<Vehicle> m_activeVehicles;
        private bool m_createEmpty;
        
        
        private void Start()
        {
            m_activeVehicles = new List<Vehicle>();
            m_pooledVehicles = new List<Vehicle>();
            GetVehiclesFromPools();
            Init();
        }

        private void FixedUpdate()
        {
            VehiclesDriving();
        }

        private void Init()
        {
            // Spawning some count of cars on route at start.
            for (int carNum = 0; carNum < m_trafficSettings.startCarsOnRouteCount; carNum++)
            { SpawnRandomVehicle(carNum); }
            
            StartCoroutine( GameManager.DelayAction(1f,() =>
            {
                // Traffic cycles on route.
                StartCoroutine( VehicleSpawnCycle());
                StartCoroutine( EmptySpawn());
            }));
        }

        private void GetVehiclesFromPools()
        {
            m_vehiclePools.ForEach(pool =>
            { pool.PooledObjects.ForEach(obj => { m_pooledVehicles.Add(obj.GetComponent<Vehicle>()); }); });
        }
        
        private IEnumerator VehicleSpawnCycle()
        {
            SpawnRandomVehicle();
            
            float timeBetweenSpawns = Random.Range(m_trafficSettings.timeBetweenSpawnsMin, m_trafficSettings.timeBetweenSpawnsMax);
            if (m_createEmpty) { timeBetweenSpawns += m_trafficSettings.timeForEmpty;  m_createEmpty = false; }
            
            yield return new WaitForSeconds(timeBetweenSpawns);
            yield return VehicleSpawnCycle();
        }

        private IEnumerator EmptySpawn()
        {
            m_createEmpty = true;
            yield return new WaitForSeconds(Random.Range(3f, 5f));
            yield return EmptySpawn();
        }

        private void SpawnRandomVehicle(int startCarNum = -1)
        {
            if (!m_enableTraffic && startCarNum == -1) return;
            
            List<Vehicle> availableVehicles = m_pooledVehicles.FindAll(veh => false == veh.gameObject.activeSelf);
            int rand = Random.Range(0, availableVehicles.Count);
            Vehicle pooledVehicle = availableVehicles[rand];
            
            pooledVehicle.gameObject.SetActive(true);
            RegisterCar(pooledVehicle);
            
            Vector3 startPos = m_startPos.position;
            Vector3 finishPos = m_finishPos.position;
            
            if (startCarNum == -1)
                pooledVehicle.transform.position = new Vector3(startPos.x, 0f, startPos.z);
            else
            {
                float distance = Vector3.Distance(startPos, finishPos);
                float zPos = distance / m_trafficSettings.startCarsOnRouteCount * startCarNum;
                pooledVehicle.transform.position = new Vector3(startPos.x, 0f, zPos);
            }
        }

        public void RegisterCar(Vehicle vehicle)
        {
            vehicle.RegisterRoute(this);
            m_activeVehicles.Add(vehicle);
        }
        
        private void VehiclesDriving()
        {
            if (!m_enableTraffic) {m_activeVehicles.ForEach(veh => veh.RigidBody.velocity = Vector3.zero); return;}
            
            m_activeVehicles.ForEach(car =>
            {
                if (car.gameObject.activeSelf)
                {
                    Transform carTransform;
                    (carTransform = car.transform).LookAt(m_finishPos);
                    car.transform.position = new Vector3(transform.position.x, carTransform.position.y, carTransform.position.z);
                    car.RigidBody.velocity = carTransform.forward * (m_trafficSettings.speed * Time.fixedDeltaTime * 6f);
                }
            });
        }
        
        public void MoveCarToPool(Vehicle vehicle)
        {
            m_activeVehicles.Remove(vehicle);
            vehicle.gameObject.SetActive(false);
        }

        public void EnableTraffic(bool value) { m_enableTraffic = value; }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_startPos == null || m_finishPos == null) return;
            
            Vector3 startPos = m_startPos.position;
            Vector3 finishPos = m_finishPos.position;
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, finishPos);
            Gizmos.DrawCube(startPos, Vector3.one);
            Gizmos.DrawCube(finishPos, Vector3.one);
        }
#endif
    }
    
    [Serializable] public class TrafficSettings
    {
        public float speed;
        public float timeBetweenSpawnsMin;
        public float timeBetweenSpawnsMax; 
        public float timeForEmpty;
        public int startCarsOnRouteCount;
    }
    
}