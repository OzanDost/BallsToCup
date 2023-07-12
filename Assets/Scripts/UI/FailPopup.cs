using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class FailPopup : AWindowController
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnQuitButtonClicked()
        {
        }

        private void OnRetryButtonClicked()
        {
        }
    }
}