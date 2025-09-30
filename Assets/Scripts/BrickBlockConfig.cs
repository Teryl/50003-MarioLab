using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrickBlockConfig : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpringJoint2D springJoint;
    private Vector2 originalPosition;
    private bool isBouncing = false;
    
    [Header("Bounce Settings")]
    [SerializeField] private float positionTolerance = 0.05f; // How close to original position before locking
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        originalPosition = transform.localPosition; // Use local position relative to parent
        
        // Make sure Y is locked initially
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | 
                        RigidbodyConstraints2D.FreezePositionY | 
                        RigidbodyConstraints2D.FreezeRotation;
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
        
        // Unlock Y position to allow spring to work
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | 
                        RigidbodyConstraints2D.FreezeRotation;
        
        // Wait a tiny bit for the spring to start moving upward
        yield return new WaitForSeconds(0.1f);
        
        // Now monitor until it returns to original Y position
        bool hasMovedUp = false;
        
        while (isBouncing)
        {
            float currentY = transform.localPosition.y;
            
            // Check if box has moved up from original position
            if (currentY > originalPosition.y + positionTolerance)
            {
                hasMovedUp = true;
            }
            
            // If it moved up and is now back at or below original position, stop it
            if (hasMovedUp && currentY <= originalPosition.y + positionTolerance)
            {
                // Snap to exact original position
                transform.localPosition = new Vector2(originalPosition.x, originalPosition.y);
                rb.linearVelocity = Vector2.zero;
                
                // Lock Y position again
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | 
                                RigidbodyConstraints2D.FreezePositionY | 
                                RigidbodyConstraints2D.FreezeRotation;
                
                isBouncing = false;
                break;
            }
            
            yield return new WaitForFixedUpdate(); // Check every physics frame
        }
    }
}
