using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverUI;
    public TextMeshProUGUI finalScoreText;

    [Header("ScriptableObject Variables")]
    public IntVariable gameScore;

    // void Awake()
    // {
    //     if (GameManager.instance != null)
    //     {
    //         GameManager.instance.gameStart.AddListener(GameStart);
    //         GameManager.instance.gameRestart.AddListener(OnGameRestart);
    //         GameManager.instance.scoreChanged.AddListener(UpdateScore);
    //         GameManager.instance.gameOver.AddListener(ShowGameOver);
    //     }
    // }

    void Start()
    {
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
        // if (GameManager.instance != null)
        // {
        //     UpdateScore(GameManager.instance.score);
        // }
        HideGameOver();
        if (gameScore != null)
        {
            UpdateScore(gameScore.Value);
        }
    }

    public void OnGameStart()
    {
        HideGameOver();
        if (gameScore != null)
        {
            UpdateScore(gameScore.Value);
        }
    }

    public void OnGameRestart()
    {
        HideGameOver();
        if (scoreText != null)
        {
            scoreText.enabled = true;
        }
        if (gameScore != null)
        {
            UpdateScore(gameScore.Value);
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

    // void OnDestroy()
    // {
    //     if (GameManager.instance != null)
    //     {
    //         GameManager.instance.gameStart.RemoveListener(GameStart);
    //         GameManager.instance.gameRestart.RemoveListener(OnGameRestart);
    //         GameManager.instance.scoreChanged.RemoveListener(UpdateScore);
    //         GameManager.instance.gameOver.RemoveListener(ShowGameOver);
    //     }
    // }
}