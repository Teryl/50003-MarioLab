using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset;
    public float smoothTime = 0.25f;
    public Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null) {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        
    }   
}
