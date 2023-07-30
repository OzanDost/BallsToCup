using Data;
using DefaultNamespace;
using DG.Tweening;
using ThirdParty;
using UnityEngine;

namespace Game.Managers
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private Transform _targetPos;
        [SerializeField] private GameObject _gameArea;
        [SerializeField] private ParticleSystem[] _finishParticles;

        private Tube _tube;
        private int _totalBallCount;
        private int _enteredBallCount;
        private int _fellOutBallCount;
        private int _ballTargetCount;
        private bool _levelFinished;
        private Vector2 _previousTouchDirection;
        private float _rotationAngle;
        private bool _isGivingInput;


        private void Awake()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.RequestGameplayInitialize += Initialize;
            EventDispatcher.Instance.BallEnteredCup += OnBallEnteredCup;
            EventDispatcher.Instance.BallFellOut += OnBallFellOut;
            EventDispatcher.Instance.LevelQuitRequested += OnLevelQuitRequested;
            EventDispatcher.Instance.FinishButtonClicked += FinishLevel;
            EventDispatcher.Instance.LevelSuccess += OnLevelSuccess;
        }

        private void OnLevelSuccess()
        {
            _levelFinished = true;
        }

        private void OnLevelQuitRequested()
        {
            ResetGameplay();
        }


        private void Initialize(LevelData data)
        {
            ResetGameplay();
            LoadTube(data.Tube);
            _gameArea.SetActive(true);

            _ballTargetCount = data.BallTargetCount;
            _totalBallCount = data.BallCount;

            EventDispatcher.Instance.GameplayInitialized?.Invoke(data);
            
            _inputController.Initialize(_tube);
        }

        private void OnBallEnteredCup()
        {
            _enteredBallCount++;

            if (_enteredBallCount >= _ballTargetCount)
            {
                EventDispatcher.Instance.SufficientBallCountReached?.Invoke();
            }

            CheckProcessedBalls();
            
        }

        private void OnBallFellOut()
        {
            _fellOutBallCount++;
            CheckProcessedBalls();

            if (!_levelFinished)
            {
                EventDispatcher.Instance.CameraShakeRequested?.Invoke();
            }
        }


        private void CheckProcessedBalls()
        {
            if (_fellOutBallCount + _enteredBallCount >= _totalBallCount)
            {
                if (_enteredBallCount >= _ballTargetCount)
                {
                    if (!_levelFinished)
                    {
                        FinishLevel();
                    }
                }
                else
                {
                    if (!_levelFinished)
                    {
                        EventDispatcher.Instance.LevelFailed?.Invoke();
                        _levelFinished = true;
                    }
                }
            }
        }

        private void FinishLevel()
        {
            _levelFinished = true;
            foreach (var particle in _finishParticles)
            {
                particle.Play();
            }

            DOVirtual.DelayedCall(1f, () =>
            {
                EventDispatcher.Instance.LevelSuccess?.Invoke();
            });
        }

        private void LoadTube(Tube tube)
        {
            _tube = Instantiate(tube, _targetPos.position, Quaternion.identity);
            var offset = _tube.transform.InverseTransformPoint(_tube.BallParent.position);
            _tube.BallParent.position = _targetPos.position + offset;
        }

        private void ResetGameplay()
        {
            if (_tube != null)
            {
                Destroy(_tube.gameObject);
            }

            _enteredBallCount = 0;
            _fellOutBallCount = 0;
            _gameArea.SetActive(false);
            _levelFinished = false;

            _inputController.Reset();
            foreach (var finishParticle in _finishParticles)
            {
                finishParticle.Stop();
            }
        }
    }
}