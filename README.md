# treasure_hunt

## Architecture Overview

### Core Systems

#### 1. **Configuration**
- **GameConfig.cs**: ScriptableObject with game parameters

#### 2. **Chest System**
- **Chest.cs**: Core chest logic with async and cancellation support
- **ChestState.cs**: Enum for chest states
- Uses CancellationToken for proper async/await cancellation


#### 3. **Currency System**
- **CurrencyManager.cs**: Manages Currency persistence and generation
- **Reward.cs**:  reward data structure
- **RewardType.cs**: enum for reward types 
- add new reward types by extending the enum

#### 4. **Game State Management**
- **GameManager.cs**: Orchestrates all game systems
- **GameState.cs**: Enum for game states 
- Handles round , attempt , and win/loss conditions


## Key Design Decisions

### Async/Await with CancellationToken
The chest opening mechanism uses Task-based async operations with CancellationToken:

### Events
- Components communicate via C# events
- UI updates automatically on state changes

### FSM pattern
### ServiceLocator pattern




## Build Instructions

1. Open project in Unity 6000.0.59f2
2. File → Build Settings
3. Select target platform
4. Click "Build" or "Build and Ru