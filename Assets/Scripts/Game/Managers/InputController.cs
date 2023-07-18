using DefaultNamespace;
using UnityEngine;

namespace Game.Managers
{
    public class InputController : MonoBehaviour
    {
        private float _rotationSpeed = 0.5f;
        private float _centerDistanceSensitivity = 1.0f;
        private float _minInputThreshold = 0.01f;

        private Vector2 _screenCenter;
        private Vector3 _initialPoint;

        private Transform _tube;
        private bool _initialized;

        private Vector2 _initialTouchPosition;
        private Quaternion _targetRotation;

        private void Start()
        {
            _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            _rotationSpeed = 150f;
            
            _rotationSpeed = ConfigHelper.GameConfig.RotationSpeed;
            _centerDistanceSensitivity = ConfigHelper.GameConfig.CenterDistanceSensitivity;
            _minInputThreshold = ConfigHelper.GameConfig.MinInputThreshold;
        }

        public void Initialize(Tube tube)
        {
            _initialized = true;
            _tube = tube.BowlContainer.transform;
        }

        private void Update()
        {
            if (!_initialized) return;
            if (InputHandler.GetMouseButtonDown())
            {
                _initialTouchPosition = InputHandler.GetMousePositionVector2();
            }

            if (InputHandler.GetMouseButton())
            {
                Vector2 currentTouchPosition = InputHandler.GetMousePositionVector2();
                Vector2 directionFromCenter = currentTouchPosition - _screenCenter;
                Vector2 previousDirectionFromCenter = _initialTouchPosition - _screenCenter;

                float angleDifference = Vector2.SignedAngle(previousDirectionFromCenter, directionFromCenter);

                if (Mathf.Abs(angleDifference) < _minInputThreshold) return;

                float distanceFactor = directionFromCenter.magnitude / _screenCenter.magnitude;

                float adjustedRotationSpeed = _rotationSpeed * Mathf.Pow(distanceFactor, _centerDistanceSensitivity);

                _tube.transform.rotation *= Quaternion.Euler(0f, 0f,
                    angleDifference * adjustedRotationSpeed * Time.deltaTime);

                _initialTouchPosition = currentTouchPosition;
            }
        }

        public void Reset()
        {
            _initialized = false;
        }
    }
}