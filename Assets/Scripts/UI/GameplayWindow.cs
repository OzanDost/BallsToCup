using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameplayWindow : AWindowController
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();
            // pauseButton.onClick.AddListener(OnPauseButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnQuitButtonClicked()
        {
        }

        private void OnPauseButtonClicked()
        {
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
        }
    }
}