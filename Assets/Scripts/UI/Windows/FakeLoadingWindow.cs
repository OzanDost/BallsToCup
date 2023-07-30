using DG.Tweening;
using UI.Core;
using UI.Misc;
using UnityEngine;

namespace UI.Windows
{
    public class FakeLoadingWindow : Window
    {
        [SerializeField] private ThreeDots _threeDots;

        private readonly float _fakeLoadDuration = 5f;

        public override void OnOpen()
        {
            base.OnOpen();
            _threeDots.ToggleAnimation(true);

            DOVirtual.DelayedCall(_fakeLoadDuration, LoadingCompleted);
        }

        private void LoadingCompleted()
        {
            CloseRequest?.Invoke(this);
            EventDispatcher.Instance.FakeLoadingFinished?.Invoke();
        }

        public override void OnClose()
        {
            base.OnClose();
            _threeDots.ToggleAnimation(false);
        }
    }
}