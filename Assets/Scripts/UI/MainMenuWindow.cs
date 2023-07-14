using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class MainMenuWindow : AWindowController
    {
        [SerializeField] private Button playButton;

        protected override void Awake()
        {
            base.Awake();
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            Signals.Get<PlayButtonClicked>().Dispatch();
        }
    }
}