using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Game
{
    public class BallOutsideHandler : MonoBehaviour
    {
        private List<GameObject> _collidedBalls = new();

        private void Start()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.GameplayInitialized += OnGameplayInitialized;
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
                EventDispatcher.Instance.BallFellOut?.Invoke();
            }
        }
    }
}