using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class JumpOverGoomba : MonoBehaviour
{
    public Transform enemyLocation;
    private bool onGroundState = true;

    private bool countScoreState = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    void FixedUpdate()
    {
        onGroundState = onGroundCheck();

        if (Keyboard.current[Key.Space].isPressed && onGroundState)
        {
            countScoreState = true;
            onGroundState = false;
        }

        if (!onGroundState && countScoreState)
        {
            if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(1);
                }
                countScoreState = false;
            }
        }
    }
    private bool onGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGroundState = true;
            countScoreState = false;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.down * maxDistance, boxSize);
    }
}
