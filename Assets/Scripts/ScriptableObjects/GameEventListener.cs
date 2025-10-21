using UnityEngine;
using UnityEngine.Events;

public class GameEventListener<T> : MonoBehaviour
{
    public GameEvent<T> Event;
    public UnityEvent<T> Response;

    private void OnEnable()
    {
        if (Event != null)
        {
            Event.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        if (Event != null)
        {
            Event.UnregisterListener(this);
        }
    }

    public void OnEventRaised(T data)
    {
        Response.Invoke(data);
    }
}
