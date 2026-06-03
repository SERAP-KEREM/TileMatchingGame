# Tile Matching Game — Unity Project

Welcome to **Tile Matching Game**, a Unity-based puzzle game where players eliminate groups of connected tiles to complete objectives and score points. Built with **Unity 2022.3.36f1 LTS**, this project follows **solid software architecture principles** and **design patterns** to ensure **scalability, modularity, and maintainability**.

![Gameplay](Docs/gameplay.png)

---

## Portfolio fork (maintained by [SERAP-KEREM](https://github.com/SERAP-KEREM))

This repository is a **personal portfolio fork** of the original project. It focuses on stability fixes and documentation while preserving the original gameplay and architecture.

### My contributions

| Area | Change |
|------|--------|
| **Level loading** | Fixed `IndexOutOfRangeException` when starting a level by setting board size before `ResetGame` / `StartGame` (`LevelManager.LoadLevel`) |
| **Board safety** | Hardened `GetTileAt`, `SetTileAt`, and `RemoveTileAt` when logical board size and the internal array are out of sync |
| **Build** | Removed unused `UnityEditor` import from runtime code (`LevelManager`) |
| **Level selection** | Corrected level index clamp in `SetLevel` |
| **Code quality** | Removed dead code in `GameHUD`, renamed score event handler for clarity, aligned `CollectColorTilesGoal` namespace with its folder |
| **Copy & docs** | Fixed typo in `MaxMovesGoal` UI text; updated Unity version, clone URL, and outdated README references |
| **UX** | Level goals refresh when gameplay starts (`PlayingState`); level buttons use stable indices instead of asset name parsing |
| **Performance** | Cached main camera in `GameplayController`; match finding uses iterative DFS instead of recursive |
| **Housekeeping** | Renamed `BoardModfier.cs` → `BoardModifier.cs`; event unsubscription in `GameHUD.OnDestroy`; safer `LevelButtonFactory` fields |
| **Goals UI** | Added `GoalsPanelView` with dedicated panel text, close button, and safe state transitions (`ShowGoalsState`) |
| **Input & pause** | Fixed Escape on start screen, Goals overlay, and pause resume via `LastState` |
| **HUD polish** | In-game Restart button, live goals summary after each move, wired `LevelText` / `GoalsSummaryText` |
| **Gameplay rule** | Minimum cluster size of 3 tiles to match (`AppConstants.MinimumMatchSize`) |

> Add your screenshot as `Docs/gameplay.png` (create the `Docs` folder if needed). Until then, the image link above may appear broken on GitHub.

### Thanks to the original author

This game was created by **[henritar](https://github.com/henritar)**. Thank you for sharing this project under the MIT license — it is an excellent example of MVC, design patterns, and clean Unity architecture. This fork keeps your core design intact and only adds maintenance and portfolio documentation on top.

Upstream repository: [henritar/TileMatchingGame](https://github.com/henritar/TileMatchingGame)

---

## 1. Core Systems

This game follows an **MVC-based architecture**, separating logic into **Model, View, and Controller**, while utilizing **event-driven systems** for efficient communication between components.

### Game Board and Tile System

- The board consists of a **grid of tiles**, each represented by a `Tile` instance.
- **Tile attributes** (color, sprite) are stored in `TileFlyweight`, implementing the **Flyweight pattern** to reduce memory usage.
- `Board` manages tile placement and updates, while `BoardLayoutCalculator` acts as an **Adapter**, converting board coordinates into world positions.
- `MatchFinder` detects groups of matching tiles (DFS) to trigger game logic events.
- `BoardModifier` handles **tile removal, gravity simulation, and refilling** new tiles through `TileFactory`.

### Game Objectives and Level System

- Objectives are defined using the **Strategy pattern**, allowing different goal types:
  - `CollectColorTilesGoal` → Remove a specific number of tiles of a certain color.
  - `CollectTilesPointsGoal` → Reach a target score by matching tiles.
  - `MaxMovesGoal` → Complete the level within a move limit.
- `LevelManager` loads levels dynamically using **ScriptableObjects**, while `LevelButtonFactory` generates level selection buttons.

### Game Flow and UI

- `GameManager` manages game states using the **State pattern**, transitioning between:
  - `PlayingState` → Active gameplay mode.
  - `PauseState` → Freezes all interactions.
  - `ShowGoalsState` → Displays level objectives.
  - `VictoryState` → Triggers when objectives are met.
  - `GameOverState` → Ends the level when conditions are not met.
- `GameHUD` dynamically updates **score, objectives, and game status** using **event-driven communication**.

### User Interaction and Input Handling

- `GameplayController` processes player actions (tap/click) and delegates them to `GameManager`, applying the **Command pattern** for structured input handling.
- `TileViewPool` implements **Object Pooling**, improving performance by reusing UI elements instead of constantly instantiating new ones.

---

## 2. Technical Design

### Design Patterns Used

The project integrates multiple **design patterns** to ensure clean architecture and maintainability:

- **Flyweight** → `TileFlyweight` reduces redundant sprite and color allocations.
- **Factory Method** → `TileFactory` and `LevelButtonFactory` dynamically generate new tiles and buttons.
- **Singleton** → `CoroutineRunner` provides a single coroutine host for board fill animations.
- **Strategy** → `IGoal` allows for different game objectives without modifying core logic. `IMatchFinder` can also be used as an example of Strategy for new match-finding algorithms.
- **State** → `GameManager` orchestrates game state transitions.
- **Observer** → Board events, score, and `GameHUD` use event-driven updates.
- **Adapter** → `CanvasAdapter` / `BoardLayoutCalculator` convert logical board coordinates into world positions.
- **Object Pooling** → `TileViewPool` optimizes UI performance by reusing elements.
- **Command (Partial)** → `GameplayController` structures input handling.

These patterns ensure that the project remains **modular, scalable, and easy to maintain**.

### Interfaces for Flexibility and Testability

To improve code maintainability, flexibility, and ease of testing, several key components implement interfaces:

- **`IBoard`** → Defines the contract for the game board, making it possible to swap implementations or mock it in tests.
- **`IMatchFinder`** → Abstracts the match-finding logic, allowing for different matching algorithms.
- **`IScoreManager`** → Provides a structured way to handle scoring logic.
- **`ITileFactory`** → Encapsulates the creation of tiles, making it possible to modify tile generation logic without affecting other components.

These interfaces facilitate unit testing by enabling dependency injection and reducing tight coupling between components.

---

## 3. Setup and Installation

### Prerequisites

- Unity **2022.3.37f1 LTS** or later (see `ProjectSettings/ProjectVersion.txt`).
- No additional packages or external dependencies are required.

### Installation Steps

1. **Clone this fork**:

   ```sh
   git clone https://github.com/SERAP-KEREM/TileMatchingGame.git
   ```

2. **Open the project in Unity Hub.**
3. Open `Assets/Scenes/MainScene.unity`.
4. **Run the game** by pressing Play in the Unity Editor.

---

## 4. License

This project is licensed under the **MIT License** — see [LICENSE](LICENSE).

Original work © [henritar](https://github.com/henritar). Portfolio maintenance © [SERAP-KEREM](https://github.com/SERAP-KEREM).
