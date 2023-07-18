using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public int RotationSpeed => _rotationSpeed;
        public float CenterDistanceSensitivity => _centerDistanceSensitivity;
        public float MinInputThreshold => _minInputThreshold;

        [TitleGroup("Input Settings")]
        [SerializeField] private int _rotationSpeed;

        [TitleGroup("Input Settings")]
        [SerializeField] private float _centerDistanceSensitivity;

        [TitleGroup("Input Settings")]
        [SerializeField] private float _minInputThreshold;


        #region Ball Settings

        public PhysicMaterial BallPhysicMaterial => _ballPhysicMaterial;
        public List<Material> BallMaterials => _ballMaterials;

        [TitleGroup("Ball Settings")]
        [InlineEditor(Expanded = true)]
        [SerializeField] PhysicMaterial _ballPhysicMaterial;

        [TitleGroup("Ball Settings")]
        [PreviewField]
        [ListDrawerSettings(ShowFoldout = false)]
        [SerializeField] private List<Material> _ballMaterials = new();
        
        [TitleGroup("Ball Settings")]
        [InlineEditor(Expanded = true)]
        [SerializeField] private Rigidbody _ballRigidbody = new Rigidbody();

        #endregion
    }
}