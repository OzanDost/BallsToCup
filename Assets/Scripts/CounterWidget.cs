using System;
using Data;
using DG.Tweening;
using ThirdParty;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
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
            Signals.Get<GameplayInitialized>().AddListener(OnGameplayInitialized);
            Signals.Get<BallEnteredCup>().AddListener(OnBallEnteredCup);
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
        }

        private void SetText()
        {
            _counterText.SetText($"{_enteredBallCount}/{_ballTargetCount}");

            _scalePunch?.Kill(true);
            _scalePunch = _counterText.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f);
        }
    }
}