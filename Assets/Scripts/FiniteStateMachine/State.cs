using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/State")]
public class State : ScriptableObject
{
    public Action[] setupActions;
    public Action[] actions;
    public EventAction[] eventTriggeredActions;
    public Action[] exitActions;
    public Transition[] transitions;

    // For visualization at the Scene
    public Color sceneGizmoColor = Color.grey;

    /********************************/
    /* REGULAR METHODS */
    /********************************/
    public void UpdateState(StateController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    protected void DoActions(StateController controller)
    {
        if (actions == null) return;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] != null)
                actions[i].Act(controller);
        }
    }

    public void DoSetupActions(StateController controller)
    {
        if (setupActions == null) return;
        for (int i = 0; i < setupActions.Length; i++)
        {
            if (setupActions[i] != null)
                setupActions[i].Act(controller);
        }
    }

    public void DoExitActions(StateController controller)
    {
        if (exitActions == null) return;
        for (int i = 0; i < exitActions.Length; i++)
        {
            if (exitActions[i] != null)
                exitActions[i].Act(controller);
        }
    }

    public void DoEventTriggeredActions(StateController controller, ActionType type = ActionType.Default)
    {
        if (eventTriggeredActions == null) return;
        foreach (EventAction eventTriggeredAction in eventTriggeredActions)
        {
            if (eventTriggeredAction.type == type && eventTriggeredAction.action != null)
            {
                eventTriggeredAction.action.Act(controller);
            }
        }
    }

    protected void CheckTransitions(StateController controller)
    {
        if (transitions == null || transitions.Length == 0) return;

        controller.transitionStateChanged = false; // Reset

        for (int i = 0; i < transitions.Length; ++i)
        {
            if (controller.transitionStateChanged)
            {
                break;
            }

            if (transitions[i].decision == null) continue;

            bool decisionSucceeded = transitions[i].decision.Decide(controller);

            if (decisionSucceeded && transitions[i].trueState != null)
            {
                controller.TransitionToState(transitions[i].trueState);
            }
            else if (!decisionSucceeded && transitions[i].falseState != null)
            {
                controller.TransitionToState(transitions[i].falseState);
            }
        }
    }
}