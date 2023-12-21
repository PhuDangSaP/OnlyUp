using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    private Vector3 cameraTarget;
    private Transform target;
    private Vector3 offset;
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - target.position;

    }
    private void Update()
    {
        transform.position = target.position + offset;
    }

}
