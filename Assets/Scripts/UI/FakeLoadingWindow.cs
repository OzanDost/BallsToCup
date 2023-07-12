using ThirdParty;
using ThirdParty.uiframework.Window;
using UI.Misc;
using UnityEngine;

namespace UI.Windows
{
    public class FakeLoadingWindow : AWindowController
    {
        [SerializeField] private ThreeDots threeDots;

        private readonly float _fakeLoadDuration = 5f;

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            threeDots.ToggleAnimation(true);
            Invoke(nameof(LoadingCompleted), _fakeLoadDuration);
        }

        private void LoadingCompleted()
        {
            CloseRequest?.Invoke(this);
        }

        public override void UI_Close()
        {
            base.UI_Close();
            threeDots.ToggleAnimation(false);
        }
    }
}