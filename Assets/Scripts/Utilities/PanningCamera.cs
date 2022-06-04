using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Features
{
    public class PanningCamera: MonoBehaviour
    {
        private Camera m_cam;
        private Vector3 m_touchStart;
        private float groundZ = 0; 
        
        private void Awake()
        {
            m_cam = gameObject.GetComponent<Camera>();
            m_touchStart = Vector3.zero;
        }

        private void Update ()
        {
            if (Input.GetMouseButtonDown(0)) { m_touchStart = GetWorldPosition(groundZ); }
            
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = m_touchStart - GetWorldPosition(groundZ);
                m_cam.transform.position += direction;
            }
        }

        private Vector3 GetWorldPosition(float z)
        {
            Ray mousePos = m_cam.ScreenPointToRay(Input.mousePosition);
            Plane ground = new Plane(Vector3.down, new Vector3(0,0, z));
            ground.Raycast(mousePos, out float distance);
            return mousePos.GetPoint(distance);
        }

    }

}