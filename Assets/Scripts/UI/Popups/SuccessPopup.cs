using Coffee.UIExtensions;
using DG.Tweening;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class SuccessPopup : Popup
    {
        [SerializeField] private Button continueButton;

        [SerializeField] private UIParticle leftParticle;
        [SerializeField] private UIParticle rightParticle;

        [SerializeField] private RectTransform[] stars;

        private Sequence _starSequence;


        private void Awake()
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            leftParticle.Play();
            rightParticle.Play();


            foreach (var star in stars)
            {
                star.gameObject.SetActive(false);
            }

            _starSequence = DOTween.Sequence();
            foreach (var star in stars)
            {
                _starSequence.Append(star.DOScale(1, 0.4f).From(2.3f).SetEase(Ease.InExpo)
                    .OnStart(() => star.gameObject.SetActive(true)));
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            _starSequence?.Kill(true);
        }

        private void OnContinueButtonClicked()
        {
            EventDispatcher.PlayButtonClicked?.Invoke();
        }
    }
}