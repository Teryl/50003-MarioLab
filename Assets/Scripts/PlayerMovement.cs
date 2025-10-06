using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Physics Parameters")]
    public float speed;
    public float maxSpeed;
    public float upSpeed;
    public float fallGravityMultiplier;
    public float lowJumpGravityPenalty;
    public float airTurnPenalty;
    public float deathImpulse = 15;

    private bool onGroundState = true;
    private bool faceRightState = true;
    private float originalGravityScale;

    [System.NonSerialized]
    public bool isAlive = true;
    private Vector3 startPosition;

    [Header("Component References")]
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;

    // Input System variables
    private float moveInput = 0f;
    private bool isJumpPressed = false;
    private bool isJumpHeld = false;

    public void ResetPlayer()
    {
        // reset position
        marioBody.transform.position = startPosition;
        // reset sprite direction
        marioSprite.flipX = true;
        faceRightState = true;
        // reset state
        onGroundState = true;
        isAlive = true;
        // reset animator
        marioAnimator.SetTrigger("gameRestart");
        marioAnimator.SetBool("onGround", onGroundState);
        // reset input
        moveInput = 0f;
        isJumpPressed = false;
        isJumpHeld = false;
    }

    void GiveDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        originalGravityScale = marioBody.gravityScale;
        Application.targetFrameRate = 60;
        startPosition = transform.position;
        marioAnimator.SetBool("onGround", onGroundState);
        faceRightState = true;
    }

    public void OnMove(InputValue input)
    {
        moveInput = input.Get<float>();
        Debug.Log("Moving");
    }

    public void OnJump(InputValue input)
    {
        isJumpPressed = true;
        Debug.Log("Jumping");
    }

    public void OnJumphold(InputValue input)
    {
        isJumpHeld = true;
        Debug.Log("Jump is held for some time.");
    }

    void Update()
    {
        if (moveInput < 0 && faceRightState)
        {
            marioSprite.flipX = false;
            faceRightState = false;
            if (marioBody.linearVelocity.x > 0.1f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
        else if (moveInput > 0 && !faceRightState)
        {
            marioSprite.flipX = true;
            faceRightState = true;
            if (marioBody.linearVelocity.x < -0.1f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
        
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    // Death check
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy") && isAlive)
        {
            // Start death sequence
            isAlive = false;
            marioAnimator.Play("mario_die");
            marioAudio.PlayOneShot(marioDeath);
            GiveDeathImpulse();

            // Start delayed game over through GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartDeathSequence();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform")) && !onGroundState)
        {
            // Check if collision is from the top
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // If the contact normal points upwards, it's a top collision
                if (contact.normal.y > 0.5f)
                {
                    onGroundState = true;
                    marioAnimator.SetBool("onGround", onGroundState);
                    break;
                }
            }
        }
    }

    public void restartButtonCallback()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    void FixedUpdate() // note: called 50 times per second
    {
        if (!isAlive) return;

        // movement
        Vector2 movement = new Vector2(moveInput, 0);
        if (Mathf.Abs(moveInput) > 0)
        {
            if (marioBody.linearVelocity.magnitude < maxSpeed)
            {
                if (!onGroundState)
                {
                    movement.x *= airTurnPenalty - 1;
                }
                marioBody.AddForce(movement * speed);
            }
        }
        else
        {
            // stop horizontal movement when no input
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // jump handling
        if (isJumpPressed && onGroundState && isAlive)
        {
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGroundState = false;
            // update the animator
            marioAnimator.SetBool("onGround", onGroundState);
        }

        isJumpPressed = false;

        // gravity setting
        if (marioBody.linearVelocity.y < -0.01f)
        {
            marioBody.gravityScale = fallGravityMultiplier * originalGravityScale;
        }
        else if (marioBody.linearVelocity.y > 0.01f && !isJumpHeld)
        {
            marioBody.AddForce(Vector2.up * Physics2D.gravity.y * lowJumpGravityPenalty * marioBody.mass);
        }
        else
        {
            marioBody.gravityScale = originalGravityScale;
        }

        // reset jump held state (at end of frame)
        if (marioBody.linearVelocity.y <= 0)
        {
            isJumpHeld = false;
        }
    }
}
