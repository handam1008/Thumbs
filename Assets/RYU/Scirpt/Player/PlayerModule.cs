using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class PlayerModule : MonoBehaviour
    {
        protected PlayerController Controller;
        
        public CharacterController controller => Controller.controller;

        public Transform Root => Controller.transform;

        #region Unity Methods

        protected virtual void OnValidate()
        {
            ResolveDependencies();
        }

        protected virtual void Awake()
        {
            ResolveDependencies();
        }
        
        

        #endregion

        protected virtual void ResolveDependencies()
        {
            Controller = GetComponentInParent<PlayerController>();
        }
    }
}