using DefaultNamespace;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(BoxCollider))]
    public class BallReleaser : MonoBehaviour
    {
        public BoxCollider BoxCollider => _boxCollider;
        [SerializeField] private BoxCollider _boxCollider;

        private void OnTriggerEnter(Collider other)
        {
            var ball = other.GetComponent<Ball>();
            EventDispatcher.BallReleaseRequested?.Invoke(ball);
        }
    }
}