using System;
using Thumb;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class InputHandler : PlayerModule
    {
        
        private GameInput input;

        private void OnEnable()
        {
            if (input == null)
            {
                input = new GameInput();
            }
            
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void Update()
        {
            Controller.MoveInput = input.Player.Move.ReadValue<Vector2>();
            Controller.LookInput = input.Player.Look.ReadValue<Vector2>();
        }
    }
}