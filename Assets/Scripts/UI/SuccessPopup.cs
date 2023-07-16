using System;
using Coffee.UIExtensions;
using Data;
using DG.Tweening;
using ThirdParty;
using ThirdParty.uiframework.Window;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class SuccessPopup : AWindowController<SuccessWindowProperties>
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private TextMeshProUGUI difficultyText;

        [SerializeField] private UIParticle leftParticle;
        [SerializeField] private UIParticle rightParticle;

        [SerializeField] private RectTransform[] stars;

        private Tween _scoreTween;
        private Sequence _starSequence;


        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        protected override void On_UIOPen()
        {
            base.On_UIOPen();
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

            int startNumber = 0;
        }

        protected override void On_UIClose()
        {
            base.On_UIClose();

            _starSequence?.Kill(true);
            _scoreTween?.Kill(true);
        }

        private void OnContinueButtonClicked()
        {
            Signals.Get<PlayButtonClicked>().Dispatch();
        }
    }

    [Serializable]
    public class SuccessWindowProperties : WindowProperties
    {
    }
}