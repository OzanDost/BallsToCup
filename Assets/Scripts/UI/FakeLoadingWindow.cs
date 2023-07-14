using DG.Tweening;
using ThirdParty;
using ThirdParty.uiframework.Window;
using UI.Misc;
using UnityEngine;

namespace UI
{
    public class FakeLoadingWindow : AWindowController
    {
        [SerializeField] private ThreeDots threeDots;

        private readonly float _fakeLoadDuration = 5f;

        protected override void On_UIOPen()
        {
            base.On_UIOPen();
            threeDots.ToggleAnimation(true);

            DOVirtual.DelayedCall(_fakeLoadDuration, LoadingCompleted);
        }

        private void LoadingCompleted()
        {
            CloseRequest?.Invoke(this);
            Signals.Get<FakeLoadingFinished>().Dispatch();
        }

        public override void UI_Close()
        {
            base.UI_Close();
            threeDots.ToggleAnimation(false);
        }
    }
}