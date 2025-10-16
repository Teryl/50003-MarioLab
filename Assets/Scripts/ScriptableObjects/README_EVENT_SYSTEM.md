# ScriptableObject Event System Guide

Since me and James are collaborating on this, I decided to ask AI for a quick draft on how this works, so we dont die in Lab 5. For internal use only.

Currently, just planning to use it to reset enemies on restart.

## Overview
This project now uses a ScriptableObject-based event system for decoupled communication between game systems. This is cleaner and more maintainable than directly calling methods or using UnityEvents everywhere.

## Architecture

### Core Components

1. **GameEvent.cs** (ScriptableObjects folder)
   - Simple event with no parameters
   - Used for: Game Start, Game Restart

2. **IntGameEvent.cs** (ScriptableObjects folder)
   - Event that passes an integer value
   - Used for: Score Changes, Game Over (with final score)

3. **GameEventListener.cs** (Scripts folder)
   - Component that listens to GameEvent ScriptableObjects
   - Responds with UnityEvents that you configure in the Inspector

4. **IntGameEventListener.cs** (Scripts folder)
   - Component that listens to IntGameEvent ScriptableObjects
   - Responds with UnityEvents that receive an integer value

## Setup Instructions

### Step 1: Create ScriptableObject Event Assets

In Unity Editor:

1. **Create Game Events:**
   - Right-click in Project window → Create → Game Events → Game Event
   - Create these events:
     - `OnGameStart` - Raised when game initializes
     - `OnGameRestart` - Raised when player hits restart button

2. **Create Int Game Events:**
   - Right-click in Project window → Create → Game Events → Int Game Event
   - Create these events:
     - `OnScoreChanged` - Raised when score changes (passes score value)
     - `OnGameOver` - Raised when game ends (passes final score)

3. **Recommended folder structure:**
   ```
   Assets/
     ScriptableObjects/
       Events/
         OnGameStart.asset
         OnGameRestart.asset
         OnScoreChanged.asset
         OnGameOver.asset
   ```

### Step 2: Assign Events to GameManager

1. Select the GameManager GameObject in your scene
2. In the Inspector, find the "ScriptableObject Events" section
3. Drag your created event assets into the appropriate slots:
   - `On Game Start` → OnGameStart.asset
   - `On Game Restart` → OnGameRestart.asset
   - `On Score Changed` → OnScoreChanged.asset
   - `On Game Over` → OnGameOver.asset

### Step 3: Set Up Listeners

#### Example: HUDManager listening to events

Instead of subscribing in code, you can now use GameEventListener components:

1. **Add GameEventListener to HUD GameObject:**
   - Select your HUD GameObject
   - Add Component → GameEventListener
   - Assign the `OnGameRestart` ScriptableObject to "Game Event"
   - In "Response", add a callback to your HUDManager's `OnGameRestart()` method

2. **Add IntGameEventListener for score changes:**
   - Add Component → IntGameEventListener
   - Assign the `OnScoreChanged` ScriptableObject to "Game Event"
   - In "Response", add a callback to your HUDManager's `UpdateScore(int)` method

3. **Add IntGameEventListener for game over:**
   - Add Component → IntGameEventListener
   - Assign the `OnGameOver` ScriptableObject to "Game Event"
   - In "Response", add a callback to your HUDManager's `ShowGameOver(int)` method

#### Example: Enemy spawner listening to restart

1. Select your Enemy container GameObject
2. Add Component → GameEventListener
3. Assign `OnGameRestart` event
4. In Response, add callback to reset all enemies

## Benefits of This Approach

### 1. **Decoupling**
- GameManager doesn't need references to HUDManager, enemies, etc.
- Systems don't know about each other - they just raise/listen to events
- Easier to add/remove features without breaking existing code

### 2. **Scene Independence**
- Events work across scene transitions automatically
- No need to find objects dynamically at runtime
- Each scene can have its own listeners without conflicts

### 3. **Designer-Friendly**
- Events can be wired up in the Inspector without code changes
- Easy to see what responds to what event
- No need to modify scripts to add new listeners

### 4. **Testability**
- Can raise events manually in Editor for testing
- Select any event asset and call `Raise()` from context menu
- Easy to debug event flow

### 5. **Reusability**
- Same event can trigger multiple responses
- Events can be reused across different projects
- Easy to create event templates

## Migration from Old System

The GameManager still has legacy UnityEvents for backward compatibility:
- `gameStart` (UnityEvent)
- `gameRestart` (UnityEvent)
- `scoreChanged` (UnityEvent<int>)
- `gameOver` (UnityEvent<int>)

You can migrate gradually:
1. Keep existing code using legacy events working
2. Add new features using ScriptableObject events
3. Slowly migrate old features to use listeners
4. Eventually remove legacy UnityEvents

## Advanced Usage

### Creating Custom Event Types

If you need events with different parameter types:

```csharp
// Example: StringGameEvent.cs
[CreateAssetMenu(fileName = "NewStringGameEvent", menuName = "Game Events/String Game Event")]
public class StringGameEvent : ScriptableObject
{
    private List<StringGameEventListener> listeners = new List<StringGameEventListener>();

    public void Raise(string value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    // Register/Unregister methods...
}
```

### Raising Events from Code

```csharp
// In any script that has a reference to the event
public GameEvent onPlayerDeath;

void Die()
{
    if (onPlayerDeath != null)
    {
        onPlayerDeath.Raise();
    }
}
```

### Multiple Listeners

One event can have multiple listeners:
- HUDManager listens to OnGameRestart → hides game over UI
- EnemySpawner listens to OnGameRestart → resets enemy positions
- PowerUpManager listens to OnGameRestart → respawns power-ups
- AudioManager listens to OnGameRestart → restarts background music

All without GameManager knowing about any of them!

## Debugging Tips

1. **Check listener registration:**
   - Select the event asset in Project window
   - Check how many listeners are registered in the Inspector

2. **Add debug logs:**
   - Modify GameEvent.Raise() to log when events are raised
   - Helps track event flow

3. **Event not firing?**
   - Make sure event asset is assigned in GameManager
   - Check that listeners are enabled
   - Verify OnEnable/OnDisable are being called

## Common Patterns

### Pattern 1: Global Game State Events
```
OnGameStart → Initialize all systems
OnGamePause → Pause all systems
OnGameResume → Resume all systems
OnGameRestart → Reset all systems
```

### Pattern 2: Entity Events
```
OnPlayerDeath → Show game over, stop enemies, play sound
OnEnemySpawn → Update enemy counter, check difficulty
OnItemCollected → Update UI, play sound, check achievements
```

### Pattern 3: UI Events
```
OnMenuOpen → Pause game, hide HUD
OnMenuClose → Resume game, show HUD
OnSettingsChanged → Apply new settings to all systems
```

## Conclusion

ScriptableObject events provide a professional, scalable architecture for game communication. They're used in many commercial games and are considered best practice in Unity development.

Start small, migrate gradually, and enjoy the benefits of clean, decoupled code! 🎮
