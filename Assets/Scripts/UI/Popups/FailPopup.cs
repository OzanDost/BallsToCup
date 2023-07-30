using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class FailPopup : Popup
    {
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;

        private void Awake()
        {
            _retryButton.onClick.AddListener(OnRetryButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnQuitButtonClicked()
        {
            EventDispatcher.LevelQuitRequested?.Invoke();
        }

        private void OnRetryButtonClicked()
        {
            EventDispatcher.LevelRetryRequested?.Invoke();
        }
    }
}