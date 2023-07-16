using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private Rigidbody _rigidbody;
        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
        
        public void SetPhysicMaterial(PhysicMaterial physicMaterial)
        {
            _sphereCollider.material = physicMaterial;
        }

        public void SetRigidbodyConfigs(Rigidbody rigidbody)
        {
            _rigidbody.mass = rigidbody.mass;
            _rigidbody.drag = rigidbody.drag;
            _rigidbody.angularDrag = rigidbody.angularDrag;
            _rigidbody.useGravity = rigidbody.useGravity;
            _rigidbody.isKinematic = rigidbody.isKinematic;
            _rigidbody.interpolation = rigidbody.interpolation;
            _rigidbody.collisionDetectionMode = rigidbody.collisionDetectionMode;
            _rigidbody.constraints = rigidbody.constraints;
        }
    }
}