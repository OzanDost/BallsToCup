using System.Collections.Generic;
using ThirdParty;
using UnityEngine;

namespace DefaultNamespace
{
    public class Tube : MonoBehaviour
    {
        [SerializeField] private Transform _bowlContainer;
        [SerializeField] private Bowl _bowl;
        [SerializeField] private List<Ball> _balls;
        [SerializeField] private Transform _ballParent;
        [SerializeField] private Transform _releasedBalls;

        public Transform BowlContainer => _bowlContainer;
        public Bowl Bowl => _bowl;
        public List<Ball> Balls => _balls;
        public Transform BallParent => _ballParent;

        public void AssignBalls(List<Ball> balls)
        {
            _balls = balls;
        }

        private void Start()
        {
            Signals.Get<BallReleaseRequested>().AddListener(OnBallReleaseRequested);
        }

        private void OnBallReleaseRequested(Ball ball)
        {
            ball.transform.SetParent(_releasedBalls, true);
        }
    }
}