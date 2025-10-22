using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/Countdown")]
public class CountdownDecision : Decision
{
    public float buffDuration;

    public override bool Decide(StateController controller)
    {
        bool elapsed = controller.CheckIfCountDownElapsed(buffDuration);
        return elapsed;
    }
}