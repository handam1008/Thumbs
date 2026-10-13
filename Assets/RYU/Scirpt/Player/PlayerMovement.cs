using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class PlayerMovement : PlayerModule
    {
        [SerializeField] private float maxSpeed = 12f;

        private void Update()
        {
            Vector3 motion = new();

            motion += Controller.MoveInput.x * Root.right;
            motion += Controller.MoveInput.y * Root.forward;
 
            // SimpleMove는 초당 속도를 받고 중력을 자동 적용한다 (점프 없으니 이걸로 충분)
            controller.SimpleMove(motion * maxSpeed);
        }
    }
}