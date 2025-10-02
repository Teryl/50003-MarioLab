using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MysteryBox : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Vector2 originalPosition;
    private bool isUsed = false;

    [Header("References")]
    public Animator boxAnimator;
    public GameObject itemPrefab;
    private GameObject activeCollectible;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        originalPosition = transform.localPosition;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void ResetMysteryBox()
    {
        isUsed = false;
        boxAnimator.SetBool("isUsed", false);
    }

    void PlayHitSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isUsed)
        {
            Vector2 hitDirection = collision.contacts[0].normal;

            if (hitDirection.y > 0.5f)
            {
                isUsed = true;
                boxAnimator.SetBool("isUsed", true);
                if (itemPrefab != null)
                {
                    Vector3 spawnPosition = transform.position + Vector3.up;
                    activeCollectible = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
                    activeCollectible.GetComponent<CoinController>().boxCollectible = true;
                }

            }
        }
    }
}
