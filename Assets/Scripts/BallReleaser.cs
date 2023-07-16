using ThirdParty;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(BoxCollider))]
    public class BallReleaser : MonoBehaviour
    {
        public BoxCollider BoxCollider => _boxCollider;
        [SerializeField] private BoxCollider _boxCollider;

        private void OnTriggerEnter(Collider other)
        {
            var ball = other.GetComponent<Ball>();
            Signals.Get<BallReleaseRequested>().Dispatch(ball);
        }
    }
}