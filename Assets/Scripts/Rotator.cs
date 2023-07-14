using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Rotator : MonoBehaviour
    {
        private void FixedUpdate()
        {
            transform.Rotate(Vector3.forward, 20 * Time.fixedDeltaTime);
        }
    }
}