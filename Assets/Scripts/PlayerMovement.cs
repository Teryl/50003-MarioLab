using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float maxSpeed;
    public float upSpeed;
    private bool onGround = true;

    private Rigidbody2D marioBody;


    void Start() {
        marioBody = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 30;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground") {
            onGround = true;
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate() //Called 50 times per second
    {
        float moveHorizontal = 0f;
        if (Keyboard.current != null) {
            if (Keyboard.current[Key.A].isPressed) {
                moveHorizontal = -1f;
            } else if (Keyboard.current[Key.D].isPressed) {
                moveHorizontal = 1f;
            }
        }

        // Movement
        Vector2 movement = new Vector2(moveHorizontal, 0);
        if (Mathf.Abs(moveHorizontal) > 0) {
            if (marioBody.linearVelocity.magnitude < maxSpeed) {
                marioBody.AddForce(movement * speed);
            }
        }
        if (Keyboard.current[Key.A].wasReleasedThisFrame || Keyboard.current[Key.D].wasReleasedThisFrame) {
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // Jump
        if (Keyboard.current[Key.Space].wasPressedThisFrame && onGround) {
            Vector2 jump = new Vector2(0, upSpeed);
            marioBody.AddForce(jump, ForceMode2D.Impulse);
            onGround = false;
        }

    }
}
