using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrafficRoad
{
    public class RandomCarMaterial : MonoBehaviour
    {
        [SerializeField] private List<Material> m_sourceMaterials;
        [SerializeField] private List<MeshRenderer> m_carMeshes;

        private void Start()
        {
            Material randomMat = m_sourceMaterials[Random.Range(0, m_sourceMaterials.Count)];
            m_carMeshes.ForEach(mesh => { mesh.material = randomMat; });
        }
    }
}