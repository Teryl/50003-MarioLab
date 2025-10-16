using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isCollected = false;

    [Header("Settings")]
    public bool boxCollectible = false;
    public float bounceForce = 5f;

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

    public void ResetCoin()
    {
        isCollected = false;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(1);
            }

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
        if (other.CompareTag("Platform") && boxCollectible)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject, 5f);
        }
    }
}
