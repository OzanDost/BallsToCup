using Data;
using DG.Tweening;
using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameplayWindow : AWindowController
    {
        [SerializeField] private Button _finishButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private RectTransform _finishButtonRect;

        private Tween _finishButtonTween;
        private float _finishButtonTargetX;

        protected override void Awake()
        {
            base.Awake();
            _finishButtonTargetX = -_finishButtonRect.rect.width - 20;
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            _finishButton.onClick.AddListener(OnFinishButtonClicked);

            Signals.Get<SufficientBallCountReached>().AddListener(OnSufficientBallCountReached);
            Signals.Get<GameplayInitialized>().AddListener(OnGameplayInitialized);
            Signals.Get<LevelSuccess>().AddListener(OnLevelSuccess);
        }

        private void OnLevelSuccess()
        {
            ToggleFinishButton(false);
        }

        private void OnGameplayInitialized(LevelData data)
        {
            _finishButtonTween?.Kill();
            _finishButtonRect.anchoredPosition = new(0, _finishButtonRect.anchoredPosition.y);
        }

        private void OnFinishButtonClicked()
        {
            Signals.Get<FinishButtonClicked>().Dispatch();
            ToggleFinishButton(false);
        }

        private void OnSufficientBallCountReached()
        {
            ToggleFinishButton(true);
        }

        private void ToggleFinishButton(bool value)
        {
            _finishButtonTween?.Kill(true);

            if (value)
            {
                _finishButtonTween?.Kill(true);
                _finishButtonTween = _finishButtonRect.DOAnchorPosX(_finishButtonTargetX, 0.3f).SetEase(Ease.Linear);
            }
            else
            {
                _finishButtonTween = _finishButtonRect.DOAnchorPosX(0, 0.15f)
                    .SetEase(Ease.Linear);
            }
        }

        private void OnQuitButtonClicked()
        {
            Signals.Get<LevelQuitRequested>().Dispatch();
        }
    }
}