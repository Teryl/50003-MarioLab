using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverUI;
    public TextMeshProUGUI finalScoreText;

    void Awake()
    {
        // Subscribe to GameManager events
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.AddListener(GameStart);
            GameManager.instance.gameRestart.AddListener(OnGameRestart);
            GameManager.instance.scoreChanged.AddListener(UpdateScore);
            GameManager.instance.gameOver.AddListener(ShowGameOver);
        }
    }

    void Start()
    {
        // Find UI elements if not assigned in inspector
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        }
        if (gameOverUI == null)
        {
            gameOverUI = GameObject.Find("GameOverUI");
        }
        if (finalScoreText == null)
        {
            finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<TextMeshProUGUI>();
        }

        // Update score display with current score from GameManager
        // This is important for scene transitions where the score should persist
        if (GameManager.instance != null)
        {
            UpdateScore(GameManager.instance.score);
        }
    }

    public void GameStart()
    {
        // Hide game over UI and update score display
        HideGameOver();
        if (GameManager.instance != null)
        {
            UpdateScore(GameManager.instance.score);
        }
    }

    public void OnGameRestart()
    {
        // Hide game over UI and reset score display
        HideGameOver();
        if (GameManager.instance != null)
        {
            UpdateScore(GameManager.instance.score);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + finalScore.ToString();
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

    public void HideGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from game events to prevent memory leaks
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.RemoveListener(GameStart);
            GameManager.instance.gameRestart.RemoveListener(OnGameRestart);
            GameManager.instance.scoreChanged.RemoveListener(UpdateScore);
            GameManager.instance.gameOver.RemoveListener(ShowGameOver);
        }
    }
}