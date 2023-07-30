using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public int RotationSpeed => _rotationSpeed;
        public float CenterDistanceSensitivity => _centerDistanceSensitivity;
        public float MinInputThreshold => _minInputThreshold;

        [SerializeField] private int _rotationSpeed;

        [SerializeField] private float _centerDistanceSensitivity;

        [SerializeField] private float _minInputThreshold;


        #region Ball Settings

        public PhysicMaterial BallPhysicMaterial => _ballPhysicMaterial;
        public List<Material> BallMaterials => _ballMaterials;

        [SerializeField] PhysicMaterial _ballPhysicMaterial;

        [SerializeField] private List<Material> _ballMaterials = new();

        [SerializeField] private Rigidbody _ballRigidbody = new Rigidbody();

        #endregion
    }
}