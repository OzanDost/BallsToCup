using DG.Tweening;
using UnityEngine;

namespace Game.Managers
{
    public class CameraController : MonoBehaviour
    {
        private Tween _shakeTween;
        private bool _isShaking;
        private readonly Vector3 _shakeVector = new Vector3(1, 1, 0);

        private void Awake()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            EventDispatcher.CameraShakeRequested += OnRequestCameraShake;
        }

        private void OnRequestCameraShake()
        {
            if (_isShaking) return;
            _isShaking = true;
            _shakeTween = transform.DOShakePosition(0.1f, _shakeVector * 0.05f, 15).OnComplete(() =>
            {
                _isShaking = false;
            });
        }
    }
}