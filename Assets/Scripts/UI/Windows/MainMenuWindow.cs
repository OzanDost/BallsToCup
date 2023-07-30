using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class MainMenuWindow : Window
    {
        [SerializeField] private Button _playButton;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            EventDispatcher.PlayButtonClicked?.Invoke();
        }
    }
}