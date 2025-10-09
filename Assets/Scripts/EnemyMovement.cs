using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    private float originalX;
    public float maxDistance = 5f;
    public float speed = 2f;
    private bool movingRight = true;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    [System.NonSerialized]
    public Vector3 startPosition;
    public bool isDefeated = false;
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
        startPosition = transform.position;
        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2((movingRight ? 1 : -1) * maxDistance / speed, 0);
    }
    public void ResetEnemy()
    {
        transform.position = startPosition;
        movingRight = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
        ComputeVelocity();
        isDefeated = false;
    }
    void MoveGoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (movingRight && transform.position.x >= originalX + maxDistance)
        {
            movingRight = false;
            ComputeVelocity();
        }
        else if (!movingRight && transform.position.x <= originalX - maxDistance)
        {
            movingRight = true;
            ComputeVelocity();
        }
        MoveGoomba();
    }

}
