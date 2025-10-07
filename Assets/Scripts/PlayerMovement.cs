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
    public float doubleJumpMultiplier = 0.8f;
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

    // double jump mechanic
    private bool hasDoubleJumped = false;
    private bool justDoubleJumped = false; // disable low jump penalty right after double jump

    // movement tracking
    private bool moving = false;



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
        hasDoubleJumped = false;
        justDoubleJumped = false;
        moving = false;
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

    // note: these will be called by ActionManager
    public void OnJump()
    {
        isJumpPressed = true;
    }

    public void OnJumpHold()
    {
        isJumpHeld = true;
    }
    
    public void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }
    
    public void MoveCheck(int value)  // note: checks movement input and update the moving state
    {
        if (value == 0)
        {
            moving = false;
            moveInput = 0;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            moveInput = value;
        }
    }

    void Move(float value)
    {
        Vector2 movement = new Vector2(value, 0);
        if (Mathf.Abs(value) > 0)
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
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }
    }

    public void Jump() // jump from ground
    {
        if (isAlive && onGroundState)
        {
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGroundState = false;
            marioAnimator.SetBool("onGround", onGroundState);
            Debug.Log("Ground jump");
        }
    }

    public void JumpHold()  // hold jump
    {
        Debug.Log("Jump is held");
    }

    public void DoubleJump()
    {
        if (isAlive && !onGroundState && !hasDoubleJumped)
        {
            // reset vertical velocity
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0);

            Vector2 doubleJump = new Vector2(0, upSpeed * doubleJumpMultiplier);
            marioBody.AddForce(doubleJump, ForceMode2D.Impulse);
            hasDoubleJumped = true;
            justDoubleJumped = true;
            isJumpHeld = true; // treat double jump as if button held
            PlayJumpSound();
            Debug.Log("Double jump");
        }
    }

    void Update()
    {
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
                    hasDoubleJumped = false;
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
        if (moving)
        {
            Move(moveInput);
        }
        else
        {
            // stop horizontal movement when no input
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // note: jump handling is done through ActionManager (UnityEvents)
        if (isJumpPressed && isAlive)
        {
            // jump from ground
            if (onGroundState)
            {
                Jump();
            }
            // jump in air (haven't double jumped yet)
            else if (!hasDoubleJumped)
            {
                DoubleJump();
            }
        }
        isJumpPressed = false;

        // gravity setting
        if (marioBody.linearVelocity.y < -0.01f)
        {
            marioBody.gravityScale = fallGravityMultiplier * originalGravityScale;
            justDoubleJumped = false;
        }
        else if (marioBody.linearVelocity.y > 0.01f && !isJumpHeld && !justDoubleJumped)
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
