using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrickBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpringJoint2D springJoint;
    private AudioSource audioSource;
    private Vector2 originalPosition;
    private bool isBouncing = false;

    [Header("Bounce Settings")]
    // how close to original position before locking
    [SerializeField] private float positionTolerance = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        audioSource = GetComponent<AudioSource>();
        originalPosition = transform.localPosition;

        // y locked at first
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBouncing)
        {
            StartCoroutine(ActivateSpringBounce());
        }
    }

    IEnumerator ActivateSpringBounce()
    {
        isBouncing = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        // unlock y (to bounce)
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(0.1f);

        bool hasMovedUp = false;

        while (isBouncing)
        {
            float currentY = transform.localPosition.y;

            if (currentY > originalPosition.y + positionTolerance)
            {
                hasMovedUp = true;
            }

            if (hasMovedUp && currentY <= originalPosition.y + positionTolerance)
            {
                // snap to original position
                transform.localPosition = new Vector2(originalPosition.x, originalPosition.y);
                rb.linearVelocity = Vector2.zero;

                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                isBouncing = false;
                break;
            }

            if (currentY < originalPosition.y - positionTolerance)
            {
                transform.localPosition = new Vector2(originalPosition.x, originalPosition.y);
                rb.linearVelocity = Vector2.zero;

                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

                isBouncing = false;
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
