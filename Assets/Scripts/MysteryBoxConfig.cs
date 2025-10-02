using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MysteryBoxConfig : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Vector2 originalPosition;
    private bool isBouncing = false;
    private bool hasBeenHit = false;
    
    [Header("Bounce Settings")]
    [SerializeField] private float positionTolerance = 0.05f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        originalPosition = transform.localPosition;
        
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void ResetMysteryBox()
    {
        hasBeenHit = false;
        isBouncing = false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBouncing && !hasBeenHit)
        {
            StartCoroutine(ActivateSpringBounce());
        }
    }
    
    IEnumerator ActivateSpringBounce()
    {
        isBouncing = true;
        hasBeenHit = true; // mark as used - won't bounce again

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        
        // unlock Y position to allow spring to work
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
                // snap to exact original position
                transform.localPosition = new Vector2(originalPosition.x, originalPosition.y);
                rb.linearVelocity = Vector2.zero;
                
                // lock Y position again
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
