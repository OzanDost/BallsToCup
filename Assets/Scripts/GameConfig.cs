using UnityEngine;

namespace DefaultNamespace
{
    public class GameConfig : ScriptableObject
    {
        public int RotationSpeed => _rotationSpeed;
        public float CenterDistanceSensitivity => _centerDistanceSensitivity;
        public float MinInputThreshold => _minInputThreshold;

        [SerializeField] private int _rotationSpeed;
        [SerializeField] private float _centerDistanceSensitivity;
        [SerializeField] private float _minInputThreshold;
    }
}