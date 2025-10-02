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

    [Header("Mystery Box References")]
    public GameObject mysteryBoxes;
    
    [Header("Death Sequence")]
    [SerializeField] private float deathSequenceDelay = 2.0f; // Time to wait after death impulse before game over
    
    [Header ("Camera")]
    public Transform gameCamera;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    
    private bool isDeathSequenceActive = false;
    
    // Singleton pattern for easy access
    public static GameManager Instance;
    
    void Awake()
    {
        // Singleton setup
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
        // Initialize score display
        UpdateScoreDisplay();

        // Make sure game over UI is hidden
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Set normal time scale
        Time.timeScale = 1.0f;

        // Play background music
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

            // stop background music
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }

            // freeze camera
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
        // wait for the player to bounce up and fall back down
        yield return new WaitForSeconds(deathSequenceDelay);
        
        GameOver();
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over!");
        
        // stop the game
        Time.timeScale = 0.0f;
        
        // update final score display
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score.ToString();
        }
        
        // show gameover UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        // clear score text during game over
        if (scoreText != null)
        {
            scoreText.text = "";
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart!");

        // Stop any active death sequence
        StopAllCoroutines();
        isDeathSequenceActive = false;

        // Reset time scale
        Time.timeScale = 1.0f;

        // Reset score
        score = 0;
        UpdateScoreDisplay();

        // Reset player position and state
        if (playerMovement != null)
        {
            playerMovement.ResetPlayer();
        }

        // reset enemies
        ResetEnemies();

        // reset mystery box
        ResetMysteryBoxes();

        // hide game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // reset camera position
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
        
        // restart background music
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
            // Get all MysteryBoxConfig components in children (recursive search)
            MysteryBoxConfig[] allMysteryBoxes = mysteryBoxes.GetComponentsInChildren<MysteryBoxConfig>();
            
            foreach (MysteryBoxConfig mysteryBox in allMysteryBoxes)
            {
                if (mysteryBox != null)
                {
                    mysteryBox.ResetMysteryBox();
                }
            }
        }
    }
    
    // Public method for restart button callback
    public void RestartButtonCallback()
    {
        RestartGame();
    }
}
