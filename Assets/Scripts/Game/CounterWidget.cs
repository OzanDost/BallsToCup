using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CounterWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _counterText;

        private Tween _scalePunch;
        private int _enteredBallCount;
        private int _ballTargetCount;

        private void Awake()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.GameplayInitialized += OnGameplayInitialized;
            EventDispatcher.Instance.BallEnteredCup += OnBallEnteredCup;
        }

        private void OnBallEnteredCup()
        {
            _enteredBallCount++;
            SetText();
        }

        private void OnGameplayInitialized(LevelData data)
        {
            _enteredBallCount = 0;
            _ballTargetCount = data.BallTargetCount;
            SetText();
        }

        private void SetText()
        {
            _counterText.SetText($"{_enteredBallCount}/{_ballTargetCount}");

            PunchTextScale();
        }

        private void PunchTextScale()
        {
            _scalePunch?.Kill(true);

            _scalePunch = _counterText.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f).OnComplete(() =>
            {
                _counterText.transform.localScale = Vector3.one;
            });
        }
    }
}