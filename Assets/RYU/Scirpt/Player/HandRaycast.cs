using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class HandRaycast : PlayerModule
    {
        public void Raycast()
        {
            var col = Physics.Raycast(transform.position, Controller.LookInput, 10f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Controller.LookInput);
        }
    }
}