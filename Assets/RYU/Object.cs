using System;
using UnityEngine;

namespace RYU
{
    public class Object : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetMaterial(Material mat)
        {
            _meshRenderer.material = mat;
        }
        
    }
}