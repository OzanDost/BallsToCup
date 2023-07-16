using System;
using System.Collections.Generic;
using Data;
using ThirdParty;
using UnityEngine;

namespace DefaultNamespace
{
    public class BallOutsideHandler : MonoBehaviour
    {
        private List<GameObject> _collidedBalls = new();

        private ASignal _ballFellOutSignal;

        private void Start()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            Signals.Get<GameplayInitialized>().AddListener(OnGameplayInitialized);
            _ballFellOutSignal = Signals.Get<BallFellOut>();
        }

        private void OnGameplayInitialized(LevelData data)
        {
            _collidedBalls = new List<GameObject>(data.BallCount);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_collidedBalls.Contains(other.gameObject))
            {
                _collidedBalls.Add(other.gameObject);
                _ballFellOutSignal?.Dispatch();
            }
        }
    }
}