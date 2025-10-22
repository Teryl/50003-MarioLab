using UnityEngine;
using System.Collections;

public class PoisonMushroom : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isCollected = false;

    [Header("Movement")]
    public float moveSpeed = 2f;
    private int direction = 1;

    [Header("Poison Settings")]
    public float poisonDuration = 5f;
    public AudioClip poisonCollectSound;
    public State invincibleSmallMarioState;

    private Vector3 startPosition;
    private bool startPositionSet = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (!startPositionSet)
        {
            startPosition = transform.position;
            startPositionSet = true;
        }
    }
    
    void FixedUpdate()
    {
        if (!isCollected && rb != null)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            if (audioSource != null && poisonCollectSound != null)
            {
                Debug.Log("Mario touches the Mushroom!");
                audioSource.PlayOneShot(poisonCollectSound);
            }

            MarioStateController marioStateController = other.GetComponent<MarioStateController>();
            if (marioStateController == null)
            {
                marioStateController = other.GetComponentInParent<MarioStateController>();
            }

            if (marioStateController != null)
            {
                Debug.Log("Mario now has Cancer that is killing him!");
                marioStateController.SetPowerup(PowerupType.Damage);

               if (invincibleSmallMarioState != null)
                {
                    marioStateController.TransitionToState(invincibleSmallMarioState);
                }
            }

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    direction *= -1;
                    break;
                }
            }
        }
    }

    public void ResetMushroom()
    {
        isCollected = false;

        if (startPositionSet)
        {
            transform.position = startPosition;
        }

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        direction = 1;
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
