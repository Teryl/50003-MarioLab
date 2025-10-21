using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewIntGameEvent", menuName = "Game Events/Int Game Event")]
public class IntGameEvent : GameEvent<int>
{
    // This class inherits everything from GameEvent<int>
    // Raise, RegisterListener, UnregisterListener are all inherited
}