using Data;
using DefaultNamespace;
using DG.Tweening;
using Game.Managers;
using Lofelt.NiceVibrations;
using ThirdParty;
using UnityEngine;

namespace Game
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
            Signals.Get<RequestGameplayInitialize>().AddListener(Initialize);
            Signals.Get<BallEnteredCup>().AddListener(OnBallEnteredCup);
            Signals.Get<BallFellOut>().AddListener(OnBallFellOut);
            Signals.Get<LevelQuitRequested>().AddListener(OnLevelQuitRequested);
            Signals.Get<FinishButtonClicked>().AddListener(FinishLevel);
            Signals.Get<LevelSuccess>().AddListener(OnLevelSuccess);
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

            Signals.Get<GameplayInitialized>().Dispatch(data);
            _inputController.Initialize(_tube);
        }

        private void OnBallEnteredCup()
        {
            _enteredBallCount++;

            if (_enteredBallCount >= _ballTargetCount)
            {
                Signals.Get<SufficientBallCountReached>().Dispatch();
            }

            CheckProcessedBalls();
            
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }

        private void OnBallFellOut()
        {
            _fellOutBallCount++;
            CheckProcessedBalls();

            if (!_levelFinished)
            {
                Signals.Get<RequestCameraShake>().Dispatch();
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
                        Signals.Get<LevelFailed>().Dispatch();
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

            DOVirtual.DelayedCall(1f, () => { Signals.Get<LevelSuccess>().Dispatch(); });
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