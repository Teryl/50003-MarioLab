using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float maxSpeed;
    public float upSpeed;
    public float fallGravityMultiplier;
    public float lowJumpGravityPenalty;
    public float airTurnPenalty;
    private bool onGround = true;
    private float originalGravityScale;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        originalGravityScale = marioBody.gravityScale;
        Application.targetFrameRate = 30;
    }

    void Update()
    {
        if (Keyboard.current[Key.A].isPressed)
        {
            marioSprite.flipX = false;
        }
        else if (Keyboard.current[Key.D].isPressed)
        {
            marioSprite.flipX = true;
        }
    }

    void FixedUpdate() //Called 50 times per second
    {
        // Ground check
        if (Mathf.Abs(marioBody.linearVelocity.y) < 0.01f)
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }

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
        }

        Vector2 movement = new Vector2(moveHorizontal, 0);
        if (Mathf.Abs(moveHorizontal) > 0)
        {
            if (marioBody.linearVelocity.magnitude < maxSpeed)
            {
                if (!onGround)
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
        if (Keyboard.current[Key.Space].wasPressedThisFrame && onGround)
        {
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
        }
        if (marioBody.linearVelocity.y < -0.01f)
        {
            marioBody.gravityScale = fallGravityMultiplier * originalGravityScale;
        }
        else if (marioBody.linearVelocity.y > 0.01f && !Keyboard.current[Key.Space].isPressed)
        {
            marioBody.AddForce(Vector2.up * Physics2D.gravity.y * lowJumpGravityPenalty * marioBody.mass);
        }
        else
        {
            marioBody.gravityScale = originalGravityScale;
        }
    }
}
