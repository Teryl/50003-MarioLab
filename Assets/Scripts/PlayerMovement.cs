using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float maxSpeed;
    public float upSpeed;
    private bool onGround = true;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;


    void Start()
    {
        marioSprite = GetComponent<SpriteRenderer>();
        marioBody = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 30;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            onGround = true;
        }

        // if (col.gameObject.CompareTag("Ground")) onGroundState = true;

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

        // if (col.gameObject.CompareTag("Ground") ||
        // col.gameObject.CompareTag("Enemy") ||
        // col.gameObject.CompareTag("Obstacles"))
        // {
        //     onGround = false;
        //     onGroundState = false;
        // }

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
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGround = false;
        }

        // if (Input.GetKeyDown("space") && onGroundState)
        // {
        //     marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
        //     onGroundState = false;
        // }

    }
}
