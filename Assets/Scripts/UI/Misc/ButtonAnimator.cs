using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Misc
{
    public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float _animationSpeed = 3f;
        [SerializeField] private float _subtractedScale = 0.1f;

        private Tween _animationTween;
        private float _defaultScale;
        private bool _defaultScaleSet;
        private bool _isPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_defaultScaleSet)
            {
                _defaultScale = transform.localScale.x;
                _defaultScaleSet = true;
            }

            _animationTween?.Kill();
            _animationTween = transform.DOScale(_defaultScale - _subtractedScale, _animationSpeed).SetSpeedBased()
                .SetEase(Ease.Linear);

            _isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isPressed) return;
            _isPressed = false;
            _animationTween?.Kill();
            _animationTween = transform.DOScale(_defaultScale, _animationSpeed).SetSpeedBased().SetEase(Ease.Linear);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isPressed) return;
            _isPressed = false;
            _animationTween?.Kill();
            _animationTween = transform.DOScale(_defaultScale, _animationSpeed).SetSpeedBased().SetEase(Ease.Linear);
        }
    }
}