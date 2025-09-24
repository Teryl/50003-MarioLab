using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    // movement
    public float speed;
    public float maxSpeed;
    public float upSpeed;

    private bool onGround = true;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    // UI
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;

    // game over screen
    public GameObject gameplayUI;
    public GameObject gameOverMenu;
    public TextMeshProUGUI gameOverScoreText;

    // Sprites update
    public Sprite deathSprite;
    private Sprite originalSprite;

    // music
    public AudioSource themeAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip themeMusic;
    public AudioClip jumpSound;
    public AudioClip deathSound;

    void Start()
    {
        marioSprite = GetComponent<SpriteRenderer>();
        // Store original sprite
        originalSprite = marioSprite.sprite;
        marioBody = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 30;

        PlayThemeMusic();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            onGround = true;
        }

        if (col.gameObject.CompareTag("Enemy"))
        {
            // Check if player is above the enemy (landing on top)
            if (transform.position.y > col.transform.position.y)
            {
                onGround = true;
                onGroundState = true;
                Debug.Log("Landed on enemy - can jump!");
            }
            else
            {
                Debug.Log("Collided with goomba");
            }
        }

        if (col.gameObject.CompareTag("Obstacles"))
        {
            // Check if player is above the obstacle (landing on top)
            if (transform.position.y > col.transform.position.y)
            {
                onGround = true;
                onGroundState = true;
                Debug.Log("Landed on obstacle - can jump!");
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        // Only reset onGround if leaving ground or if leaving an enemy/obstacle you were on top of
        if (col.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }

        if ((col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Obstacles"))
        && transform.position.y > col.transform.position.y)
        {
            onGround = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            PlayDeathSound();
            StopThemeMusic();
            GameOver();
        }

    }

    void Update()
    {
        if (Keyboard.current[Key.A].isPressed && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Keyboard.current[Key.D].isPressed && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }


    void FixedUpdate() //Called 50 times per second
    {
        float moveHorizontal = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current[Key.A].isPressed)
            {
                moveHorizontal = -1f;
            }
            else if (Keyboard.current[Key.D].isPressed)
            {
                moveHorizontal = 1f;
            }
        }

        // Movement
        Vector2 movement = new Vector2(moveHorizontal, 0);
        if (Mathf.Abs(moveHorizontal) > 0)
        {
            if (marioBody.linearVelocity.magnitude < maxSpeed)
            {
                marioBody.AddForce(movement * speed);
            }
        }
        if (Keyboard.current[Key.A].wasReleasedThisFrame || Keyboard.current[Key.D].wasReleasedThisFrame)
        {
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // Jump
        if (Keyboard.current[Key.Space].wasPressedThisFrame && onGround)
        {
            // Play jump sound effect FIRST
            PlayJumpSound();

            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGround = false;
        }

    }

    private void GameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Pause the game
        Time.timeScale = 0.0f;

        // Change to death sprite
        marioSprite.sprite = deathSprite;

        // Wait 3 second
        yield return new WaitForSecondsRealtime(3f);

        // Pause the game
        Time.timeScale = 0.0f;

        // Update game over score
        gameOverScoreText.text = "Score: " + jumpOverGoomba.score.ToString();

        // Hide gameplay UI and show game over menu
        gameplayUI.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
        // Restart theme music
        PlayThemeMusic();
    }

    private void ResetGame()
    {
        // Hide the game over menu and show gameplay UI
        gameOverMenu.SetActive(false);
        gameplayUI.SetActive(true);

        // Reset sprite to original
        marioSprite.sprite = originalSprite;

        // reset position
        marioBody.transform.position = new Vector3(-80.0f, 2.0f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        jumpOverGoomba.score = 0;
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }

    }

    // ===== MUSIC =====
    private void PlayThemeMusic()
    {
        if (themeAudioSource != null && themeMusic != null)
        {
            themeAudioSource.clip = themeMusic;
            themeAudioSource.loop = true;  // Loop the theme music
            themeAudioSource.Play();
        }
    }

    private void StopThemeMusic()
    {
        if (themeAudioSource != null && themeAudioSource.isPlaying)
        {
            themeAudioSource.Stop();
        }
    }

    private void PlayJumpSound()
    {
        if (sfxAudioSource != null && jumpSound != null)
        {
            // Make sure the audio source is enabled and volume is up
            sfxAudioSource.volume = 1f;
            sfxAudioSource.PlayOneShot(jumpSound);
            Debug.Log("Jump sound played!");   // debug code
        }
        else
        {
            Debug.Log("Jump sound failed - sfxAudioSource: " + (sfxAudioSource != null) + ", jumpSound: " + (jumpSound != null));
        }
    }

    private void PlayDeathSound()
    {
        if (sfxAudioSource != null && deathSound != null)
        {
            // Use PlayOneShot so it can play even when time is paused
            sfxAudioSource.volume = 1f;
            sfxAudioSource.PlayOneShot(deathSound);
            Debug.Log("Death sound played!");
        }
        else
        {
            Debug.Log("Death sound failed - sfxAudioSource: " + (sfxAudioSource != null) + ", deathSound: " + (deathSound != null));
        }
    }
}
