using System;
using DG.Tweening;
using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class FakeRewardedPopup : AWindowController<FakeRewardedPopupProperties>
    {
        [SerializeField] private Button finishRewarded;
        [SerializeField] private Button closeRewarded;
        [SerializeField] private GameObject failedToFinishText;

        private Tween _delayedCloseTween;

        protected override void Awake()
        {
            base.Awake();
            finishRewarded.onClick.AddListener(OnFinishRewarded);
            closeRewarded.onClick.AddListener(OnCloseRewarded);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            failedToFinishText.SetActive(false);
        }

        protected override void On_UIOPen()
        {
            base.On_UIOPen();
        }

        protected override void On_UIClose()
        {
            base.On_UIClose();
        }

        private void OnCloseRewarded()
        {
            failedToFinishText.SetActive(true);
            _delayedCloseTween = DOVirtual.DelayedCall(2f, () =>
            {
                CloseRequest?.Invoke(this);
                Properties.failedActionCallBack?.Invoke();
            });

        }

        private void OnFinishRewarded()
        {
            CloseRequest?.Invoke(this);
            Properties.successActionCallBack?.Invoke();
        }
    }

    [Serializable]
    public class FakeRewardedPopupProperties : WindowProperties
    {
        public Action successActionCallBack;
        public Action failedActionCallBack;

        public FakeRewardedPopupProperties(Action successActionCallBack, Action failedActionCallBack)
        {
            this.successActionCallBack = successActionCallBack;
            this.failedActionCallBack = failedActionCallBack;
        }
    }
}