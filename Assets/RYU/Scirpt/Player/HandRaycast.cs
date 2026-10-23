using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class HandRaycast : PlayerModule
    {
        [SerializeField] private LayerMask whatIsElectric;
        [SerializeField] private Camera cam;
        [SerializeField] private Material glowMat;
        private Object _current;
        public Object Current => _current; // 지금 바라보고 있는 전선 (없으면 null)
        private void Update()
        {
            Raycast();
        }

        public void Raycast()
        {
            Object target = null;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, 10f, whatIsElectric))
                hitInfo.collider.TryGetComponent(out target);

            if (target == _current) return;                   

            if (_current != null) _current.RollBackMaterial(); 
            if (target != null) target.SetMaterial(glowMat);        
            _current = target;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Controller.LookInput);
        }
    }
}