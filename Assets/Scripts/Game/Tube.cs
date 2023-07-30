using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Game
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
            EventDispatcher.BallReleaseRequested += OnBallReleaseRequested;
        }

        private void OnBallReleaseRequested(Ball ball)
        {
            ball.transform.SetParent(_releasedBalls, true);
        }
    }
}