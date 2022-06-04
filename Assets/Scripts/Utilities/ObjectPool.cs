using UnityEngine;
using System.Collections.Generic;

namespace TrafficRoad
{
   public class ObjectPool : MonoBehaviour
   {
      [SerializeField] private GameObject m_objectToPool;
      [SerializeField] private int m_amountToPool;
      
      private Transform m_parent;
      
      private List<GameObject> m_pooledObjects;
      public List<GameObject> PooledObjects => m_pooledObjects;

      private void Awake()
      {
         m_parent = this.transform;
         m_pooledObjects = new List<GameObject>();
         for (int i = 0; i < m_amountToPool; i++)
         {
            GameObject tmp = Instantiate(m_objectToPool, m_parent);
            tmp.SetActive(false);
            m_pooledObjects.Add(tmp);
         }
      }

      public GameObject GetPooledObject()
      {
         for (int i = 0; i < m_amountToPool; i++)
         {
            if (!m_pooledObjects[i].activeSelf)
            {
               return m_pooledObjects[i];
            }
         }
         return null;
      }
   }
   
}