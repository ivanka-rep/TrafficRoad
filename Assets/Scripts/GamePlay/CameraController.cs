using UnityEngine;

namespace TrafficRoad
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float m_offsetZ;
        [SerializeField] private float m_smoothness = 0.15f;

        private bool m_canMove = false;
        private Vector3 m_newCamPos;
        
        private void FixedUpdate()
        {
            if (false == m_canMove) return;
            transform.position = Vector3.Lerp(transform.position, m_newCamPos, m_smoothness);
        }

        public void MoveCamera(Vector3 to)
        {
            Vector3 oldPos = transform.position;
            m_newCamPos = new Vector3(to.x, oldPos.y, to.z - m_offsetZ);
            m_canMove = true;
        }
        
    }
}