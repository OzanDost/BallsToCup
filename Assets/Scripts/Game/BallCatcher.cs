using System.Collections.Generic;
using Data;
using ThirdParty;
using UnityEngine;

namespace DefaultNamespace
{
    public class BallCatcher : MonoBehaviour
    {
        private List<GameObject> _addedBalls = new();

        private void Start()
        {
            EventDispatcher.GameplayInitialized += OnGameplayInitialized;
        }

        private void OnGameplayInitialized(LevelData data)
        {
            _addedBalls = new List<GameObject>(data.BallCount);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_addedBalls.Contains(other.gameObject))
            {
                _addedBalls.Add(other.gameObject);
                
                EventDispatcher.BallEnteredCup?.Invoke();
            }
        }
    }
}