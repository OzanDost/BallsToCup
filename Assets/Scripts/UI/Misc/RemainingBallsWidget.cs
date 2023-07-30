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
        private int _totalBalls;

        private void Awake()
        {
            EventDispatcher.Instance.GameplayInitialized += OnGameplayInitialized;
            EventDispatcher.Instance.BallReleaseRequested += OnBallReleaseRequested;
        }

        private void OnGameplayInitialized(LevelData data)
        {
            _count = _totalBalls = data.BallCount;
            SetBallText();
        }

        private void OnBallReleaseRequested(Ball ball)
        {
            _count--;
            if (_count < 0) _count = 0;
            SetBallText();
        }

        private void SetBallText()
        {
            _remainingBallsText.text = $"Remaining Balls\n{_count}/{_totalBalls}";
        }
    }
}