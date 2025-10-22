using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/KillPlayer")]
public class KillPlayerAction : Action
{
    public override void Act(StateController controller)
    {
        // disable the FSM
        controller.isActive = false;
        
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.KillPlayer();
        }
    }
}