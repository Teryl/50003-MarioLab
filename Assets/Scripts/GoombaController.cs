using UnityEngine;
using UnityEngine.Events;

public class GoombaController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    private float defeatDuration = 0.5f;
    public UnityEvent onDeath;
    private EnemyMovement enemyMovement;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        onDeath.AddListener(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().OnEnemyDefeated);
    }   

    public void TakeStomp()
    {
        if (!enemyMovement.isDefeated)
        {
            Debug.Log("Goomba stomped!");
            enemyMovement.isDefeated = true;
            animator.SetTrigger("onDefeat");

            if (audioSource != null)
            {
                audioSource.Play();
            }

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            Invoke("DisableGoomba", defeatDuration);
            onDeath?.Invoke();
        }
    }

    private void DisableGoomba()
    {
        gameObject.SetActive(false);
    }
}
