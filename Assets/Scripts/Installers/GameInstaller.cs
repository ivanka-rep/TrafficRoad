using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace TrafficRoad
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private CameraController m_cameraCtrl;
        [SerializeField] private GuiManager m_guiManager;
        [SerializeField] private Transform m_vehiclePoolsT;
        [SerializeField] private Transform m_pedestrianPoolsT;
        [SerializeField] private FxEffectsPools m_fxEffectsPools;
        
        public override void InstallBindings()
        { 
            Container.BindInstance(m_cameraCtrl).AsSingle();
            Container.BindInstance(m_guiManager).AsSingle();
            Container.BindInstance(m_fxEffectsPools).AsSingle();
            BindPoolList(m_vehiclePoolsT, "VehiclePools");
            BindPoolList(m_pedestrianPoolsT, "PedestrianPools");
        }
        
        private void BindPoolList(Transform poolsTransform, string id)
        {
            List<ObjectPool> poolsList = poolsTransform.GetComponents<ObjectPool>().ToList();
            Container.BindInstance(poolsList).WithId(id);
        }
    }

    [Serializable] public class Prefabs
    {
        public List<GameObject> turningVehicles;
    }
}