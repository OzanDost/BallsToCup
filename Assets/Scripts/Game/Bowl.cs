using UnityEngine;

public class Bowl : MonoBehaviour
{
    public Transform Center => center;
    public Transform Entry => entry;
    public MeshCollider MeshCollider => _meshCollider;

    [SerializeField] private Transform center;
    [SerializeField] private Transform entry;
    [SerializeField] private MeshCollider _meshCollider;
}