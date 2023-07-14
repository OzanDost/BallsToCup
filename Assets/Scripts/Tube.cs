using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Tube : MonoBehaviour
    {
        [SerializeField] private Transform _bowlContainer;
        [SerializeField] private Bowl _bowl;
        [SerializeField] private List<Ball> _balls;
        [SerializeField] private Transform _ballParent;

        public Transform BowlContainer => _bowlContainer;
        public Bowl Bowl => _bowl;
        public List<Ball> Balls => _balls;
        public Transform BallParent => _ballParent;
        public void AssignBalls(List<Ball> balls)
        {
            _balls = balls;
        }
    }
}