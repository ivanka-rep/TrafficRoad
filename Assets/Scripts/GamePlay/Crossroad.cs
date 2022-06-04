using System;
using System.Collections;
using System.Collections.Generic;
using Features;
using TrafficRoad.Waypoints;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace TrafficRoad
{
    public class Crossroad : MonoBehaviour
    {
        [SerializeField] private TrafficRoute m_trafficRoute;
        [SerializeField] private WaypointRoute m_waypointRoute;
        [SerializeField] private Transform m_cameraCenterPoint;
        [SerializeField] private Transform m_roadEnd;
        [SerializeField] private bool m_isLeftCrossroad;
        [SerializeField] private bool m_isNewStreet;
        
        public bool IsNewStreet => m_isNewStreet;
        public bool IsLeftCrossroad => m_isLeftCrossroad;
        public TrafficRoute Route => m_trafficRoute;

        private List<Vehicle> m_vehicles;
        private GameManager m_gameManager;
        private float m_rangeBetweenCars = 8f;
        private float m_rangeToMainRoad = 2.15f;
        
        [Inject] private CameraController m_camCtrl;
        [Inject] private TrafficSettings m_trafficSettings;

        public void SpawnVehicles(List<GameObject> vehiclesList, GameTag turningVehicleTag)
        {
            m_gameManager = GameManager.s_instance;
            m_vehicles = new List<Vehicle>();
            Vector3 nextPos = m_roadEnd.position;
            
            for (int i = 0; i < vehiclesList.Count; i++)
            {
                    GameObject vehicle = Instantiate(vehiclesList[i]);
                    Vehicle vComp = vehicle.GetComponent<Vehicle>();
                    float rangeBetweenCars = i == 0 ? m_rangeToMainRoad : m_rangeBetweenCars;
                    Vector3 pos = CalculateNewCarPos(vComp, nextPos, rangeBetweenCars);
                    
                    vComp.IsTurningCar = true;
                    vComp.RegisterRoute(m_trafficRoute);
                    vComp.RigidBody.freezeRotation = true;
                    vehicle.GetComponent<GameTagReference>().GameTags.Add(turningVehicleTag);
                    vehicle.transform.position = new Vector3(pos.x, vehicle.transform.localScale.y / 2f, pos.z);
                    vehicle.transform.rotation = transform.rotation;
                    
                    nextPos = CalculateNewCarPos(vComp, nextPos, rangeBetweenCars);
                    m_vehicles.Add(vComp);
            }
            
            m_vehicles.ForEach(v => v.RigidBody.freezeRotation = false);
        }
        
         private void MoveNextCars() 
         {
             if (m_vehicles.Count == 0) { return; }
            
             Vehicle vehicle = m_vehicles[0];
             Vector3 newPos = CalculateNewCarPos(vehicle, m_roadEnd.position, m_rangeToMainRoad);
             Vector3 vehiclePos = vehicle.transform.position;

             var vehicleRef = vehicle;
             var newPosRef = newPos;
             StartCoroutine(vehicle.MoveTo(vehiclePos, newPos, 0.7f, () => 
             { 
                 vehicleRef.ActivateOutlines(true);
                 m_gameManager.ActivateControl(true);
                 m_gameManager.AnimatePointingArrow(newPosRef);
             }));
            
             for (int i = 1; i < m_vehicles.Count; i++)
             {
                 float oldVehicleScaleZ = m_vehicles[i - 1].transform.localScale.z;
                 Vector3 currentCarNewPos = m_isLeftCrossroad
                     ? newPos - new Vector3(oldVehicleScaleZ / 2f, 0, 0)
                     : newPos + new Vector3(oldVehicleScaleZ / 2f, 0, 0);
                
                 vehicle = m_vehicles[i];
                 vehiclePos = vehicle.transform.position;
                 newPos = CalculateNewCarPos(vehicle, currentCarNewPos, m_rangeBetweenCars);
                 StartCoroutine(vehicle.MoveTo(vehiclePos, newPos, 0.7f, 
                     () => { m_gameManager.ActivateControl(true); } ));
             }
         }
        
         private Vector3 CalculateNewCarPos(Vehicle vehicle, Vector3 nextPos, float range)
         {
             var vScale = vehicle.transform.localScale;
             Vector3 pos = m_isLeftCrossroad
                 ? nextPos - new Vector3(range + vScale.z / 2f, 0f, 0f)  // right crossroad
                 : nextPos + new Vector3(range + vScale.z / 2f, 0f, 0f); // left crossroad
             return new Vector3(pos.x, 0f, pos.z);
         }

         public void MoveCar()
        {
            m_waypointRoute.AddObjectToRoute(m_vehicles[0].gameObject, m_trafficSettings.speed);
            m_gameManager.DisablePointingArrow();
            m_vehicles.RemoveAt(0);
            MoveNextCars();
        }

         public void HighlightFirstCar()
         {
             m_vehicles[0].ActivateOutlines(true);
             m_gameManager.AnimatePointingArrow(m_vehicles[0].transform.position);
         }
         
        public void MoveCameraToCenter()
        {
            m_camCtrl.MoveCamera(m_cameraCenterPoint.position);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_roadEnd == null || m_roadEnd == null) return;
            Gizmos.color = Color.yellow;
            
            Vector3 from = m_roadEnd.position;
            float range = m_isLeftCrossroad ? -90f : 90f;
            Vector3 to = new Vector3(from.x + range, from.y, from.z);
            Gizmos.DrawLine(from, to );
        }
#endif
    }
}