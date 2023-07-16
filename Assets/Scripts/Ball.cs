using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
    }
}