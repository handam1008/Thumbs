using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class HandRaycast : PlayerModule
    {
        [SerializeField] private LayerMask whatIsElectric;
        [SerializeField] private Camera cam;
        [SerializeField] private Material glowMat;
        [SerializeField] private Material normalMat;
        private Object _current;
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

            if (_current != null) _current.SetMaterial(normalMat); 
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