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
    
    [Header("Raycast Settings")]
    public float raycastDistance = 1f;
    public LayerMask obstacleLayerMask = -1;

    [System.NonSerialized]
    public Vector3 startPosition;
    
    [System.NonSerialized]
    public bool isDefeated = false;
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
        startPosition = transform.position;
        
        int groundLayer = LayerMask.NameToLayer("Ground");
        int obstacleLayer = LayerMask.NameToLayer("Obstacles");
        obstacleLayerMask = (1 << groundLayer) | (1 << obstacleLayer);
        
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

    bool CheckForObstacle()
    {
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance, obstacleLayerMask);
        return hit.collider != null;
    }

    void FixedUpdate()
    {
        bool hitObstacle = CheckForObstacle();
        bool reachedMaxDistance = (movingRight && transform.position.x >= originalX + maxDistance) || 
                                 (!movingRight && transform.position.x <= originalX - maxDistance);
        
        if (hitObstacle || reachedMaxDistance)
        {
            movingRight = !movingRight;
            ComputeVelocity();
        }
        
        MoveGoomba();
    }

    void OnDrawGizmos()
    {
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        Vector2 rayOrigin = transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, rayDirection * raycastDistance);
    }
}
