using System;
using UnityEngine;

namespace RYU
{
    public class Object : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private Material _material;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetMaterial(Material mat)
        {
            _material = _meshRenderer.material;
            _meshRenderer.material = mat;
        }
        public void RollBackMaterial()
        {
            _meshRenderer.material = _material;
        }
        
    }
}