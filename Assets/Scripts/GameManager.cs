using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("ScriptableObject Events")]
    public GameEvent onGameStart;
    public GameEvent onGameRestart;
    public IntGameEvent onScoreChanged;
    public IntGameEvent onGameOver;

    [Header("Legacy Unity Events (for backward compatibility)")]
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

        if (onGameStart != null)
        {
            onGameStart.Raise();
        }
        gameStart?.Invoke();
    }

    public void OnEnemyDefeated()
    {
        AddScore(5);
    }

    public void OnPortalEnter(string sceneName)
    {
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
        if (onScoreChanged != null)
        {
            onScoreChanged.Raise(score);
        }
        scoreChanged?.Invoke(score);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void KillPlayer()
    {
        PlayerMovement player = FindPlayerInScene();
        if (player != null && player.isAlive)
        {
            Debug.Log("Player is being killed!");
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }
            player.isAlive = false;
            Rigidbody2D marioBody = player.GetComponent<Rigidbody2D>();
            if (marioBody != null)
            {
                marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
                marioBody.AddForce(Vector2.up * player.deathImpulse, ForceMode2D.Impulse);
            }
            if (player.marioAnimator != null)
            {
                player.marioAnimator.Play("mario_die");
            }
            if (player.marioAudio != null && player.marioDeath != null)
            {
                player.marioAudio.PlayOneShot(player.marioDeath);
            }
            StartDeathSequence();
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
            CameraMovement cameraMovement = FindFirstObjectByType<CameraMovement>();
            if (cameraMovement != null)
            {
                cameraMovement.enabled = false;
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
        Time.timeScale = 0.0f;
        if (onGameOver != null)
        {
            onGameOver.Raise(score);
        }
        gameOver?.Invoke(score);
    }

    private PlayerMovement FindPlayerInScene()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            return playerObject.GetComponent<PlayerMovement>();
        }
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            return player;
        }
        return null;
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        isDeathSequenceActive = false;
        Time.timeScale = 1.0f;
        CameraMovement cameraMovement = FindFirstObjectByType<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.enabled = true;
            cameraMovement.ResetCamera(new Vector3(0, 4.5f, -10));
        }
        PlayerMovement currentPlayer = FindPlayerInScene();
        if (currentPlayer != null)
        {
            currentPlayer.ResetPlayer();
        }
        else
        {
            Debug.LogWarning("Could not find player in current scene to reset!");
        }
        ResetEnemies();
        ResetMysteryBoxes();
        ResetCollectibles();
        score = 0;
        UpdateScoreDisplay();
        if (onScoreChanged != null)
        {
            onScoreChanged.Raise(score);
        }
        scoreChanged?.Invoke(score);
        if (onGameRestart != null)
        {
            onGameRestart.Raise();
        }
        gameRestart?.Invoke();
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
            backgroundMusic.Play();
        }
    }

    void ResetEnemies()
    {
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
        AudioSource currentMenuSFX = FindMenuSFXInScene();
        if (currentMenuSFX != null)
        {
            currentMenuSFX.Play();
        }
    }

    private AudioSource FindMenuSFXInScene()
    {
        GameObject menuSFXObject = GameObject.Find("MenuSFX");
        if (menuSFXObject != null)
        {
            AudioSource audio = menuSFXObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                return audio;
            }
        }
        GameObject taggedObject = GameObject.FindGameObjectWithTag("MenuSFX");
        if (taggedObject != null)
        {
            return taggedObject.GetComponent<AudioSource>();
        }
        return null;
    }
}
