using UnityEngine;
using UnityEngine.Events;

public class IntGameEventListener : MonoBehaviour
{
    public IntGameEvent gameEvent;
    public UnityEvent<int> response;

    private void OnEnable()
    {
        if (gameEvent != null)
        {
            gameEvent.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        if (gameEvent != null)
        {
            gameEvent.UnregisterListener(this);
        }
    }

    public void OnEventRaised(int value)
    {
        response?.Invoke(value);
    }
}
