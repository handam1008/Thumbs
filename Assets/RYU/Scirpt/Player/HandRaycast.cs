using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class HandRaycast : PlayerModule
    {
        [SerializeField] private LayerMask whatIsElectric;
        [SerializeField] private Camera cam;
        private void Update()
        {
            Raycast();
        }

        public void Raycast()
        {
            var col = Physics.Raycast(cam.transform.position, cam.transform.forward, 10f, whatIsElectric);
            Debug.DrawRay(transform.position, Controller.LookInput, Color.red);
            Debug.Log(col);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Controller.LookInput);
        }
    }
}