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
    
    public void GameOver()
    {
        Debug.Log("Game Over!");
        
        // Stop the game
        Time.timeScale = 0.0f;
        
        // Update final score display
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score.ToString();
        }
        
        // Show game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        // Clear score text during game over
        if (scoreText != null)
        {
            scoreText.text = "";
        }
        
        // Trigger death animation if player exists
        if (playerMovement != null && playerMovement.marioAnimator != null)
        {
            playerMovement.marioAnimator.SetTrigger("onDeath");
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("Restart!");
        
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
        
        // Reset enemies
        ResetEnemies();
        
        // Hide game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
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
    
    // Public method for restart button callback
    public void RestartButtonCallback()
    {
        RestartGame();
    }
}
