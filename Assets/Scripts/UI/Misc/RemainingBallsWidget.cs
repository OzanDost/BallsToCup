using System;
using Data;
using DefaultNamespace;
using ThirdParty;
using TMPro;
using UnityEngine;

namespace UI.Misc
{
    public class RemainingBallsWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _remainingBallsText;

        private int _count;

        private void Awake()
        {
            Signals.Get<GameplayInitialized>().AddListener(OnGameplayInitialized);
            Signals.Get<BallReleaseRequested>().AddListener(OnBallReleaseRequested);
        }

        private void OnGameplayInitialized(LevelData data)
        {
            _count = data.BallCount;
            SetBallText();
        }

        private void OnBallReleaseRequested(Ball ball)
        {
            _count--;
            SetBallText();
        }

        private void SetBallText()
        {
            _remainingBallsText.text = $"Remaining Balls\n{_count}/{_count}";
        }
    }
}