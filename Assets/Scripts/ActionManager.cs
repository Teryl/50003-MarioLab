
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent jump;
    public UnityEvent jumpHold;
    public UnityEvent doubleJump;
    public UnityEvent<int> moveCheck;

    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("[ActionManager] JumpHold");
            jumpHold.Invoke();
        }
    }

    // called twice, when pressed and unpressed
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("[ActionManager] Jump");
            jump.Invoke();
        }
    }
    // for the double jump
    public void OnDoublejumpAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("[ActionManager] DoubleJump");
            doubleJump.Invoke();
        }
    }

    // called twice, when pressed and unpressed
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
            Debug.Log($"[ActionManager] Moving - direction: {faceRight}");
            moveCheck.Invoke(faceRight);
        }
        if (context.canceled)
        {
            moveCheck.Invoke(0);
            Debug.Log("[ActionManager] Move stopped");
        }
    }
}
