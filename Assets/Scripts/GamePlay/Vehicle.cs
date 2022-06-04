using System;
using System.Collections;
using System.Collections.Generic;
using cakeslice;
using Features;
using UnityEngine;
using MoreMountains.NiceVibrations;
    
namespace TrafficRoad
{
    public class Vehicle : MonoBehaviour
    {
        [SerializeField] private int m_turnScore;
        [SerializeField] private bool m_isEmergencyVehicle;
        [SerializeField] private Transform m_backwardPoint;
        [SerializeField] private List<Outline> m_outlines;
        
        private GameManager m_gameManager;
        private TrafficRoute m_trafficRoute;
        private Rigidbody m_rigidBody;
        private GameTagReference m_gameTagReference;
        private bool m_isTurningCar = false;

        public Rigidbody RigidBody => m_rigidBody;
        public GameTagReference GameTagRef => m_gameTagReference;
        public bool IsEmergencyVehicle => m_isEmergencyVehicle;
        public bool IsTurningCar { get => m_isTurningCar; set => m_isTurningCar = value; }

        private void Awake()
        {
            m_gameManager = GameManager.s_instance;
            m_gameTagReference = gameObject.GetComponent<GameTagReference>();
            m_rigidBody = gameObject.GetComponent<Rigidbody>();
        }

        public void RegisterRoute(TrafficRoute route)
        {
            m_trafficRoute = route;
        }

        public void ActivateOutlines(bool value)
        {
            m_outlines.ForEach(outline => { outline.enabled = value; });
        }

        private bool IsEmergencyVehicleBehind()
        {
            //TODO: Disable late ?
            
            Transform car = transform;
            float carLength = gameObject.GetComponent<MeshRenderer>().bounds.size.z;
            Vector3 direction = m_trafficRoute.transform.forward;
            direction.z = -direction.z; direction.y = car.forward.y;
            if (!Physics.Raycast(m_backwardPoint.position, direction , out var hit, carLength * 2f )) return false;
            // Debug.Log("hit collider = " + hit.collider);

            Vehicle vehicleBehind = hit.collider.gameObject.GetComponent<Vehicle>();
            return vehicleBehind != null && vehicleBehind.IsEmergencyVehicle;
        }
        
        public IEnumerator MoveTo(Vector3 from, Vector3 to, float overTime, Action onEnd = null)
        {
            float startTime = Time.time;
            while(Time.time < startTime + overTime)
            {
                transform.position = Vector3.Lerp(from, to, (Time.time - startTime) / overTime);
                yield return null;
            }
            onEnd?.Invoke();
        }
        
        private void OnCollisionEnter(Collision col)
        {
            GameTagReference gameTag = col.gameObject.GetComponent<GameTagReference>();
            if (gameTag == null) return;
            if (!gameTag.ExistsTagName("Pedestrian") && !gameTag.ExistsTagName("Vehicle")) return;
            
            if (m_isTurningCar)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                m_isTurningCar = false;
                m_gameManager.AddScore(-(m_turnScore / 2), false);
                m_gameManager.AnimateScoreAddition(-(m_turnScore / 2), col.contacts[0].point);
                m_gameManager.SetNextCar();
                m_gameManager.PlayFxEffect(EffectType.Explosion, col.contacts[0].point);
                Destroy(gameObject);
            }
            else m_trafficRoute.MoveCarToPool(this);
            
            if (gameTag.ExistsTagName("Pedestrian")) {col.gameObject.SetActive(false);}
        }

        private void OnTriggerEnter(Collider col)
        {
            GameTagReference gameTag = col.gameObject.GetComponent<GameTagReference>();
            if (gameTag == null) return;
            
            if (gameTag.ExistsTagName("Finish"))
            { m_trafficRoute.MoveCarToPool(this); }

            if (gameTag.ExistsTagName("ReachWaypoint") && m_isTurningCar)
            { 
                m_isTurningCar = false;
                Vector3 pos = transform.position;
                transform.position = new Vector3(m_trafficRoute.transform.position.x, pos.y, pos.z );
                int turnScore = IsEmergencyVehicleBehind() ? -(m_turnScore / 2) : m_turnScore;
                m_gameManager.AddScore(turnScore, true);
                m_gameManager.AnimateScoreAddition(turnScore, transform.position);
                m_gameTagReference.GameTags.Remove(m_gameTagReference.GameTags.Find(gTag => // remove turning car tag
                    gTag.GameTagName == m_gameManager.TurningVehicleTag.GameTagName));
                m_trafficRoute.RegisterCar(this);
                m_gameManager.SetNextCar();
                
                ActivateOutlines(false);
            }
        }
        
        
    }
}