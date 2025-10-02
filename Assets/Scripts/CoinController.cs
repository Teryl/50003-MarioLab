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
        Debug.Log("CoinController script is active.");
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
            }

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
                StartCoroutine(DisableAfterSound());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        if (other.CompareTag("Platform") && boxCollectible)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject, 2f);
        }
    }
    
    IEnumerator DisableAfterSound()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        gameObject.SetActive(false);
    }
}
