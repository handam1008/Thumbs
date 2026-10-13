using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class PlayerController : MonoBehaviour
    {
        public CharacterController controller {get; private set;}

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        #region Input Binding
        
        public Vector2 MoveInput { get; set; }
        
        public Vector2 LookInput { get; set; }

        #endregion
    }
}