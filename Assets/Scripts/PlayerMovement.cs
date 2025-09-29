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

    [Header ("Component References")]
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;

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
    }

    void GiveDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
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

    void Update()
    {
        if (Keyboard.current[Key.A].isPressed && faceRightState)
        {
            marioSprite.flipX = false;
            faceRightState = false;
            if (marioBody.linearVelocity.x > 0.1f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
        else if (Keyboard.current[Key.D].isPressed && !faceRightState)
        {
            marioSprite.flipX = true;
            faceRightState = true;
            if (marioBody.linearVelocity.x < -0.1f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
        else if (Keyboard.current[Key.A].isPressed && Keyboard.current[Key.D].isPressed)
        {
            marioSprite.flipX = true;
            faceRightState = true;
        }
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    // Death check
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy") && isAlive)
        {
            isAlive = false;
            GiveDeathImpulse();
            marioAnimator.Play("mario_die");
            marioAudio.PlayOneShot(marioDeath);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
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
    void FixedUpdate() //Called 50 times per second
    {
        // Movement
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
            else
            {
                moveHorizontal = 0f;
            }
        }

        Vector2 movement = new Vector2(moveHorizontal, 0);
        if (Mathf.Abs(moveHorizontal) > 0)
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
        if (Keyboard.current[Key.A].wasReleasedThisFrame || Keyboard.current[Key.D].wasReleasedThisFrame)
        {
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // Jump with low jump penalty
        if (Keyboard.current[Key.Space].isPressed && onGroundState)
        {
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGroundState = false;
            // update the animator
            marioAnimator.SetBool("onGround", onGroundState);
        }
        if (marioBody.linearVelocity.y < -0.01f)
        {
            marioBody.gravityScale = fallGravityMultiplier * originalGravityScale;
        } else if (marioBody.linearVelocity.y > 0.01f && !Keyboard.current[Key.Space].isPressed)
        {
            marioBody.AddForce(Vector2.up * Physics2D.gravity.y * lowJumpGravityPenalty * marioBody.mass);
        } else
        {
            marioBody.gravityScale = originalGravityScale;
        }
    }
}
