# 50.033 Super Mario Labs
This project is a collection of progressively complex labs that build Super Mario Bros. from the NES on Unity.

## Members
| Name         | Student ID   |
|--------------|--------------|
| James Bryan Budiono     | 1007150     |
| Mithunbalaji Mageswari Ganeshkumar  | 1007494     |

## Lab 5: Unity for Teenagers

> This README was written using AI, do refer to the actual report for a more concise description.

### Major Architectural Improvements

#### ScriptableObject-Based Game Architecture
- **Event System**: Implemented a decoupled communication system using ScriptableObjects
  - `GameEvent` and `IntGameEvent` for parameter-less and integer-based events
  - `GameEventListener` and `IntGameEventListener` components for Inspector-driven event handling
  - Events include: OnGameStart, OnGameRestart, OnScoreChanged, OnGameOver
  - **Benefits**: Scene-independent, designer-friendly, highly maintainable, and easily testable
  
#### Enhanced Enemy AI with Raycast Detection
- **Intelligent Obstacle Avoidance**: Goombas now use Physics2D raycasting to detect obstacles
  - Automatic detection of "Ground" and "Obstacles" layers
  - Direction switching based on raycast hits, overriding distance-based patrol
  - Configurable raycast distance and visual debugging with Gizmos
  - **Benefits**: More realistic enemy behavior, better level design flexibility

#### Centralized Game State Management
- **Singleton GameManager**: Robust game state control with DontDestroyOnLoad persistence
  - Centralized death handling (`KillPlayer()` method)
  - Dynamic scene object discovery (players, cameras, audio sources)
  - Comprehensive restart system with object state reset
  - **Benefits**: Consistent behavior across scene transitions, easier debugging

#### Event-Driven UI System
- **Decoupled HUD Management**: UI responds to game events rather than direct method calls
  - Score persistence across scene transitions
  - Automatic UI state management on game restart/death
  - Multiple event listeners support for extensible UI systems
  - **Benefits**: Modular UI components, easier to extend and maintain

### Technical Implementation Details

#### ScriptableObject Event Flow
```
GameManager.AddScore() → Raises OnScoreChanged → HUDManager.UpdateScore()
GameManager.KillPlayer() → Raises OnGameOver → HUDManager.ShowGameOver()
GameManager.RestartGame() → Raises OnGameRestart → All systems reset
```

#### Enemy AI Enhancement
```
EnemyMovement.CheckForObstacle() → Physics2D.Raycast() → Layer Detection → Direction Change
```

#### Comprehensive Reset System
- **Automatic Object Discovery**: Finds and resets enemies, collectibles, mystery boxes, poison mushrooms
- **Physics State Reset**: Clears velocities, positions, and animation states
- **Audio Management**: Stops/starts background music appropriately


### Architecture Documentation
- **[ScriptableObject Event System Guide](Assets/Scripts/ScriptableObjects/README_EVENT_SYSTEM.md)**: Comprehensive documentation of the event architecture implementation
- **Code Quality**: Removed all comments from source code for production-ready cleanliness while maintaining full functionality

### Key Design Patterns Implemented
1. **Singleton Pattern**: GameManager with DontDestroyOnLoad persistence
2. **Observer Pattern**: ScriptableObject events with multiple listeners
3. **Strategy Pattern**: Configurable enemy AI behavior via Inspector settings
4. **Factory Pattern**: Dynamic object discovery and instantiation
5. **State Management**: Centralized game state with event-driven updates

### Performance Optimizations
- **Efficient Raycasting**: Layered collision detection for enemy AI
- **Object Pooling**: Reset system reuses existing objects rather than destroy/instantiate
- **Event Batching**: Single event triggers multiple system responses
- **Scene Persistence**: Maintains game state across level transitions

## Videos
**[Lab 5 - Main Submission](https://tinyurl.com/sfhfjtrj)**
> Gameplay video is inside this link

**Lab 5 Features Demonstration**
> Updated gameplay showing enhanced enemy AI, ScriptableObject event system in action, and improved game state management

## Architecture Highlights

### Before vs After Lab 5
| Aspect | Lab 4 | Lab 5 |
|--------|-------|-------|
| **Event System** | Direct method calls, UnityEvents | ScriptableObject-based decoupled events |
| **Enemy AI** | Simple distance-based patrol | Raycast obstacle detection + distance fallback |
| **State Management** | Basic singleton | Comprehensive GameManager with scene persistence |
| **UI Coupling** | Tight coupling to GameManager | Event-driven, modular UI components |
| **Code Quality** | Commented development code | Production-ready, comment-free codebase |
| **Debugging** | Limited visual feedback | Gizmos, event tracing, comprehensive logging |

### Scalability Improvements
- **Modular Components**: Easy to add new listeners without modifying existing code
- **Scene Independence**: Events work seamlessly across level transitions  
- **Designer-Friendly**: Most gameplay behavior configurable via Inspector
- **Maintainable Codebase**: Clear separation of concerns and minimal dependencies

[Report Link](https://tinyurl.com/sfhfjtrj)

