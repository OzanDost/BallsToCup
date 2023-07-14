using TMPro;
using UnityEngine;

namespace UI.Misc
{
    public class ThreeDots : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dots;

        private float _timePerDot = 0.5f;
        private float _timer;
        private bool _canAnimate = false;
        private int _dotCount = 0;

        private void Awake()
        {
            dots.SetText("");
        }

        public void ToggleAnimation(bool toggle)
        {
            _canAnimate = toggle;
        }

        private void Update()
        {
            if (!_canAnimate)
                return;

            _timer += Time.deltaTime;
            if (_timer > _timePerDot)
            {
                _timer = 0;

                if (_dotCount >= 3)
                {
                    _dotCount = 0;
                    dots.SetText("");
                }
                else
                {
                    dots.SetText(dots.text + ".");
                    _dotCount++;
                }
            }
        }
    }
}