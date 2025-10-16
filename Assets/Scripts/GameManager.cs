using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Unity Events")]
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChanged;
    public UnityEvent<int> gameOver;

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
    public AudioSource menuSFX;

    private bool isDeathSequenceActive = false;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        UpdateScoreDisplay();

        Time.timeScale = 1.0f;

        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }

        // Invoke game start event - HUDManager will handle hiding game over UI
        gameStart?.Invoke();
    }

    public void OnEnemyDefeated()
    {
        AddScore(5);
    }

    public void OnPortalEnter(string sceneName)
    {
        // Load the specified scene in Single mode (replaces current scene)
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("OnPortalEnter called with empty scene name!");
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
        scoreChanged?.Invoke(score);
        // Debug.Log("Score: " + score);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    // Centralized method to kill the player (called by enemies, poison mushrooms, etc.)
    public void KillPlayer()
    {
        // Find the player in the current scene
        PlayerMovement player = FindPlayerInScene();
        if (player != null && player.isAlive)
        {
            Debug.Log("Player is being killed!");
            
            // Set player as dead
            player.isAlive = false;
            
            // Get rigidbody
            Rigidbody2D marioBody = player.GetComponent<Rigidbody2D>();
            if (marioBody != null)
            {
                // Stop all horizontal movement
                marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
                
                // Give death impulse (Mario jumps up when dying)
                marioBody.AddForce(Vector2.up * player.deathImpulse, ForceMode2D.Impulse);
            }
            
            // Play death animation
            if (player.marioAnimator != null)
            {
                player.marioAnimator.Play("mario_die");
            }
            
            // Play death sound
            if (player.marioAudio != null && player.marioDeath != null)
            {
                player.marioAudio.PlayOneShot(player.marioDeath);
            }
            
            // Start the death sequence (game over after delay)
            StartDeathSequence();
        }
    }

    public void StartDeathSequence()
    {
        if (!isDeathSequenceActive)
        {
            isDeathSequenceActive = true;

            // Stop background music immediately
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }

            // Find and disable camera movement in current scene
            CameraMovement cameraMovement = FindFirstObjectByType<CameraMovement>();
            if (cameraMovement != null)
            {
                cameraMovement.enabled = false;
            }

            // Start the death sequence coroutine
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
        // Debug.Log("Game Over!");

        Time.timeScale = 0.0f;

        // Invoke game over event - HUDManager will handle the UI
        gameOver?.Invoke(score);
    }

    // Helper method to find the player in the current scene
    private PlayerMovement FindPlayerInScene()
    {
        // Try to find by tag first (most reliable)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            return playerObject.GetComponent<PlayerMovement>();
        }

        // Fallback: search for PlayerMovement component in scene
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            return player;
        }

        return null;
    }

    public void RestartGame()
    {
        // Debug.Log("Restart!");

        // Stop all coroutines and reset death sequence flag
        StopAllCoroutines();
        isDeathSequenceActive = false;

        // Reset time scale first (important for animations and physics)
        Time.timeScale = 1.0f;

        // Find and reset camera in current scene
        CameraMovement cameraMovement = FindFirstObjectByType<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.enabled = true;
            cameraMovement.ResetCamera(new Vector3(0, 4.5f, -10));
        }

        // Find and reset the player in the current scene
        PlayerMovement currentPlayer = FindPlayerInScene();
        if (currentPlayer != null)
        {
            currentPlayer.ResetPlayer();
        }
        else
        {
            Debug.LogWarning("Could not find player in current scene to reset!");
        }

        // Reset all game objects
        ResetEnemies();
        ResetMysteryBoxes();
        ResetCollectibles();

        // Reset score
        score = 0;
        UpdateScoreDisplay();
        scoreChanged?.Invoke(score);

        // Invoke game restart event for any listeners
        gameRestart?.Invoke();

        // Restart background music
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
            backgroundMusic.Play();
        }
    }

    void ResetEnemies()
    {
        // Find all EnemyMovement components in the current scene
        EnemyMovement[] allEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        foreach (EnemyMovement enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
                enemy.ResetEnemy();
            }
        }
    }

    void ResetMysteryBoxes()
    {
        // Find all MysteryBox components in the current scene
        MysteryBox[] allMysteryBoxes = FindObjectsByType<MysteryBox>(FindObjectsSortMode.None);
        foreach (MysteryBox mysteryBox in allMysteryBoxes)
        {
            if (mysteryBox != null)
            {
                mysteryBox.ResetMysteryBox();
            }
        }
    }

    void ResetCollectibles()
    {
        // Find all CoinController components in the current scene
        CoinController[] allCoins = FindObjectsByType<CoinController>(FindObjectsSortMode.None);
        foreach (CoinController coin in allCoins)
        {
            if (coin != null)
            {
                coin.ResetCoin();
            }
        }

        foreach (PoisonMushroom mushroom in FindObjectsByType<PoisonMushroom>(FindObjectsSortMode.None))
        {
            if (mushroom != null)
            {
                mushroom.ResetMushroom();
            }
        }
    }

    public void RestartButtonCallback()
    {
        RestartGame();
        menuSFX?.Play();
    }
}
