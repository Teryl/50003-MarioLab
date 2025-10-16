using UnityEngine;
using System.Collections;

public class PoisonMushroom : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isCollected = false;

    [Header("Settings")]
    public bool boxCollectible = false;
    public float bounceForce = 5f;

    [Header("Poison Settings")]
    public float poisonDuration = 5f;
    public AudioClip poisonCollectSound;

    private bool isPoisonActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (!boxCollectible)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            rb.AddForce(new Vector2(0, bounceForce), ForceMode2D.Impulse);
        }
    }

    public void ResetMushroom()
    {
        isCollected = false;
        isPoisonActive = false;
        StopAllCoroutines();
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            if (audioSource != null && poisonCollectSound != null)
            {
                audioSource.PlayOneShot(poisonCollectSound);
            }
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            if (!isPoisonActive)
            {
                StartCoroutine(PoisonCountdown());
            }
        }

        if (other.CompareTag("Platform") && boxCollectible)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject, 5f);
        }
    }

    IEnumerator PoisonCountdown()
    {
        isPoisonActive = true;
        
        Debug.Log("Mario has been poisoned! Death in " + poisonDuration + " seconds...");
        yield return new WaitForSeconds(poisonDuration);
        if (GameManager.instance != null)
        {
            Debug.Log("Poison has killed Mario!");
            GameManager.instance.KillPlayer();
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
