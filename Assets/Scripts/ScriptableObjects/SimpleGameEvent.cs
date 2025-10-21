using UnityEngine;

[CreateAssetMenu(fileName = "SimpleGameEvent", menuName = "ScriptableObjects/SimpleGameEvent", order = 3)]
public class SimpleGameEvent : GameEvent<Void>
{
    public void Raise() => Raise(new Void());
}