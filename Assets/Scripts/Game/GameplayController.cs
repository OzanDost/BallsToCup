using System;
using Data;
using DefaultNamespace;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private Transform _targetPos;

        private Tube _tube;
        private int _enteredBallCount;
        private int _ballTargetCount;

        private void Awake()
        {
            Signals.Get<RequestGameplayInitialize>().AddListener(Initialize);
        }

        public void Initialize(LevelData data)
        {
            ResetGameplay();
            LoadTube(data.Tube);

            _ballTargetCount = data.BallTargetCount;

            Signals.Get<GameplayInitialized>().Dispatch(data);
        }

        private void OnBallEnteredBowl(Ball ball)
        {
            ball.EnteredBowl -= OnBallEnteredBowl;

            _enteredBallCount++;

            if (_enteredBallCount >= _ballTargetCount)
            {
                //todo 
            }
        }


        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                _tube.BowlContainer.Rotate(Vector3.forward * (40 * Time.fixedDeltaTime));
            }

            if (Input.GetKey(KeyCode.D))
            {
                _tube.BowlContainer.Rotate(Vector3.forward * (-40 * Time.fixedDeltaTime));
            }
        }

        private void LoadTube(Tube tube)
        {
            _tube = Instantiate(tube, _targetPos.position, Quaternion.identity);
            var offset = _tube.transform.InverseTransformPoint(_tube.BallParent.position);
            _tube.BallParent.position = _targetPos.position + offset;
            foreach (var ball in _tube.Balls)
            {
                ball.EnteredBowl += OnBallEnteredBowl;
            }
        }

        private void ResetGameplay()
        {
            if (_tube != null)
            {
                foreach (var ball in _tube.Balls)
                {
                    ball.EnteredBowl -= OnBallEnteredBowl;
                }

                Destroy(_tube.gameObject);
            }

            _enteredBallCount = 0;
        }
    }
}