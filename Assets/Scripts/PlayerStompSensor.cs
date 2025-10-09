using UnityEngine;
using UnityEngine.Events;

public class PlayerStompSensor : MonoBehaviour
{

    [Header("Physics")]
    public float bounceForce = 10f;
    public float minDownwardVelocity = -1f;

    Rigidbody2D marioBody;

    void Start()
    {
        marioBody = GetComponentInParent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        if (transform.position.y < other.bounds.center.y) return;

        if (other.CompareTag("Enemy") && marioBody != null)
        {
            if (marioBody.linearVelocity.y < minDownwardVelocity)
            {
                var goomba = other.GetComponent<GoombaController>();
                if (goomba != null)
                {
                    goomba.TakeStomp();
                }
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0);
                Vector2 bounce = new Vector2(0, bounceForce);
                marioBody.AddForce(bounce, ForceMode2D.Impulse);
            }
        }
    }
}
