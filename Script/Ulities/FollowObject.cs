using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothness;
    private Vector3 base_offset;
    private void Start()
    {
        base_offset = transform.position - target.position;
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + base_offset, Time.deltaTime* smoothness);
    }
}