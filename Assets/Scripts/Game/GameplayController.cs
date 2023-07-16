using Data;
using DefaultNamespace;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private Transform _targetPos;
        [SerializeField] private GameObject _gameArea;
        [SerializeField] private float _rotationSpeed = 1f;

        private Tube _tube;
        private int _totalBallCount;
        private int _enteredBallCount;
        private int _fellOutBallCount;
        private int _ballTargetCount;
        private bool _levelFinished;
        private Vector2 _previousTouchDirection;
        private float _rotationAngle;
        private bool _isGivingInput;

        private bool _initialized;

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
            _initialized = true;
            _inputController.Initialize(data.Tube);
        }

        private void OnBallEnteredCup()
        {
            _enteredBallCount++;

            if (_enteredBallCount >= _ballTargetCount)
            {
                Signals.Get<SufficientBallCountReached>().Dispatch();
            }

            CheckProcessedBalls();
        }

        private void OnBallFellOut()
        {
            _fellOutBallCount++;
            CheckProcessedBalls();
        }


        private void CheckProcessedBalls()
        {
            if (_fellOutBallCount + _enteredBallCount >= _totalBallCount)
            {
                if (_enteredBallCount >= _ballTargetCount)
                {
                    if (!_levelFinished)
                    {
                        Signals.Get<LevelSuccess>().Dispatch();
                    }
                }
                else
                {
                    Signals.Get<LevelFailed>().Dispatch();
                }
            }
        }

        private void Update()
        {
            if (!_initialized) return;
#if UNITY_EDITOR

            if (Input.GetKey(KeyCode.A))
            {
                _tube.BowlContainer.Rotate(Vector3.forward * (_rotationSpeed * Time.fixedDeltaTime));
            }

            if (Input.GetKey(KeyCode.D))
            {
                _tube.BowlContainer.Rotate(Vector3.forward * (-_rotationSpeed * Time.fixedDeltaTime));
            }
#endif
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
            _initialized = false;

            _inputController.Reset();
        }
    }
}