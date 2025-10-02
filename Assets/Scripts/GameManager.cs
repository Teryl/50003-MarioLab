using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Score System")]
    public TextMeshProUGUI scoreText;
    public int score = 0;

    [Header("Game Over")]
    public GameObject gameOverUI;
    public TextMeshProUGUI finalScoreText;

    [Header("Player References")]
    public PlayerMovement playerMovement;
    public Transform playerStartPosition;

    [Header("Enemy References")]
    public GameObject enemies;

    [Header("Mystery Boxes")]
    public GameObject mysteryBoxes;

    [Header("Collectibles")]
    public GameObject collectibles;

    [Header("Death Sequence")]
    [SerializeField] private float deathSequenceDelay = 2.0f;

    [Header("Camera")]
    public Transform gameCamera;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private bool isDeathSequenceActive = false;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        UpdateScoreDisplay();

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        Time.timeScale = 1.0f;

        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }

    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
        Debug.Log("Score: " + score);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void StartDeathSequence()
    {
        if (!isDeathSequenceActive)
        {
            isDeathSequenceActive = true;

            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }

            if (gameCamera != null)
            {
                CameraMovement cameraMovement = gameCamera.GetComponent<CameraMovement>();
                if (cameraMovement != null)
                {
                    cameraMovement.enabled = false;
                }
            }

            StartCoroutine(DeathSequenceCoroutine());
        }
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        yield return new WaitForSeconds(deathSequenceDelay);

        GameOver();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        Time.timeScale = 0.0f;

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score.ToString();
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        if (scoreText != null)
        {
            scoreText.text = "";
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart!");

        StopAllCoroutines();
        isDeathSequenceActive = false;

        Time.timeScale = 1.0f;

        score = 0;
        UpdateScoreDisplay();

        if (playerMovement != null)
        {
            playerMovement.ResetPlayer();
        }

        ResetEnemies();

        ResetMysteryBoxes();

        ResetCollectibles();

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        if (gameCamera != null)
        {
            CameraMovement cameraMovement = gameCamera.GetComponent<CameraMovement>();
            if (cameraMovement != null)
            {
                cameraMovement.enabled = true;
                cameraMovement.ResetCamera(new Vector3(0, 4.5f, -10));
            }
            else
            {
                gameCamera.position = new Vector3(0, 4.5f, -10);
            }
        }

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
            backgroundMusic.Play();
        }
    }

    void ResetEnemies()
    {
        if (enemies != null)
        {
            foreach (Transform eachChild in enemies.transform)
            {
                EnemyMovement enemyMovement = eachChild.GetComponent<EnemyMovement>();
                if (enemyMovement != null)
                {
                    eachChild.transform.localPosition = enemyMovement.startPosition;
                }
            }
        }
    }

    void ResetMysteryBoxes()
    {
        if (mysteryBoxes != null)
        {
            MysteryBox[] allMysteryBoxes = mysteryBoxes.GetComponentsInChildren<MysteryBox>();

            foreach (MysteryBox mysteryBox in allMysteryBoxes)
            {
                if (mysteryBox != null)
                {
                    mysteryBox.ResetMysteryBox();
                }
            }
        }
    }

    void ResetCollectibles()
    {
        if (collectibles != null)
        {
            foreach (Transform eachChild in collectibles.transform)
            {
                CoinController coinController = eachChild.GetComponent<CoinController>();
                if (coinController != null)
                {
                    Destroy(coinController.gameObject);
                }
            }
        }
        // also respawn stationary coins
        CoinController[] allCoins = collectibles.GetComponentsInChildren<CoinController>();
        foreach (CoinController coin in allCoins)
        {
            if (coin != null && !coin.boxCollectible)
            {
                coin.gameObject.SetActive(true);
                coin.GetComponent<SpriteRenderer>().enabled = true;
                coin.GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    public void RestartButtonCallback()
    {
        RestartGame();
    }
}
