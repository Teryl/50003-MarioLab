using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset;
    public float smoothTime = 0.25f;
    public Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    [Header("Camera Boundaries")]
    public Transform startLimit;
    public Transform endLimit;
    public bool useBoundaries = true; // Enable/disable boundaries (enabled by defaut)

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Apply boundary clamping if enabled
            if (useBoundaries)
            {
                // Calculate viewport half width in real-time
                Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
                float viewportHalfWidth = Mathf.Abs(bottomLeft.x - this.transform.position.x);

                float minX = (startLimit != null) ? startLimit.position.x + viewportHalfWidth : float.MinValue;
                float maxX = (endLimit != null) ? endLimit.position.x - viewportHalfWidth : float.MaxValue;

                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            }

            transform.position = smoothedPosition;
        }

    }

    public void ResetCamera(Vector3 position)
    {
        transform.position = position;
        velocity = Vector3.zero; // Clear velocity to prevent smooth damp issues
    }
}
