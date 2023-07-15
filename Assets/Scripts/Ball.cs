using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Ball : MonoBehaviour
    {
        public event Action<Ball> EnteredBowl;

        private void OnTriggerEnter(Collider other)
        {
            EnteredBowl?.Invoke(this);
        }
    }
}