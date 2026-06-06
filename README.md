# Tile Matching Game — Unity Project

Welcome to **Tile Matching Game**, a Unity puzzle game where players clear clusters of connected tiles to score points and complete level objectives. The project uses **MVC architecture**, **ScriptableObject levels**, and several classic design patterns for maintainability.

Built with **Unity 2022.3.36f1 LTS** (see `ProjectSettings/ProjectVersion.txt`).

## Gameplay video

https://github.com/user-attachments/assets/abe82f01-eb35-40ea-af29-3cf567a33c54

## Screenshots

<p align="center">
  <img src="https://github.com/SERAP-KEREM/TileMatchingGame/blob/main/GameImages/1.png?raw=true" alt="Game Screenshot 1" width="300">
  <img src="https://github.com/SERAP-KEREM/TileMatchingGame/blob/main/GameImages/2.png?raw=true" alt="Game Screenshot 2" width="300">
</p>

<p align="center">
  <img src="https://github.com/SERAP-KEREM/TileMatchingGame/blob/main/GameImages/3.png?raw=true" alt="Game Screenshot 3" width="300">
  <img src="https://github.com/SERAP-KEREM/TileMatchingGame/blob/main/GameImages/4.png?raw=true" alt="Game Screenshot 4" width="300">
</p>

---

## Table of contents

- [How to play](#how-to-play)
- [Portfolio fork](#portfolio-fork-maintained-by-serap-kerem)
- [Contributions summary](#contributions-summary)
- [Architecture](#architecture)
- [Input handling](#input-handling)
- [Project structure](#project-structure)
- [Setup](#setup)
- [Controls](#controls)
- [License](#license)

---

## How to play

1. Open **MainScene** and press **Play**.
2. On the start screen, pick a **level** (5 levels included).
3. **Click or tap** a tile to remove all **connected tiles of the same color** in that cluster.
4. A cluster must contain **at least 3 tiles** to be valid.
5. Tiles fall down and new tiles refill empty spaces.
6. Complete all **level goals** before failing any limit (moves, score, etc.).
7. Use **Goals** on the HUD to read full objectives; **Restart** replays the current level.
8. On **Victory**, use **Next Level** to continue — the game no longer auto-skips to the next level.

---

## Portfolio fork (maintained by [SERAP-KEREM](https://github.com/SERAP-KEREM))

- **Fork:** [SERAP-KEREM/TileMatchingGame](https://github.com/SERAP-KEREM/TileMatchingGame)
- **Upstream:** [henritar/TileMatchingGame](https://github.com/henritar/TileMatchingGame)

**Clone this fork:**

```sh
git clone https://github.com/SERAP-KEREM/TileMatchingGame.git
cd TileMatchingGame
```

Open the folder in **Unity Hub** → Unity **2022.3.36f1** (2022.3 LTS).

### Thanks to the original author

Created by **[henritar](https://github.com/henritar)** under the MIT license — a strong example of MVC, state machines, and clean Unity structure. This fork preserves that design and layers maintenance on top.

---

## Contributions summary

| Commit | Area | What changed |
|--------|------|----------------|
| `fix: rename BoardModfier to BoardModifier` | Housekeeping | Fixed typo in service class/filename; extended `IBoardModifier` for level teardown |
| `fix: harden board tile access with bounds checks` | Board safety | `GetTileAt` / `SetTileAt` / `RemoveTileAt` guard logical size vs internal array |
| `fix: correct level loading order and level selection` | Level loading | Board size before reset/start; safe index clamp; removed `UnityEditor` from runtime; `Level.GetDisplayName()`; stable level button indices |
| `fix: clear tile visuals when preparing a new level` | Stability | `TileViewPool.ReleaseAllTileViews`, stop fill coroutines, `PrepareNewLevel` / `ResetGame` split — fixes ghost/stacked boards |
| `refactor: optimize match finding and cache main camera` | Performance | Iterative DFS in `DFSMatchFinder`; inject `Camera` in `GameplayController` |
| `fix: improve goal types and stop auto-advancing on victory` | Goals / flow | `CollectColorTilesGoal` namespace; `MaxMovesGoal` copy/progress; remove dead `GoalManager` field; victory no longer calls `SetNextLevel()` on enter |
| `feat: rebuild goals UI and enhance in-game HUD` | UX | `GoalsPanelView`, goals overlay panel, `LevelText`, live goals summary, in-game **Restart**, min **3** tiles to match, pause/Escape fixes |
| `docs: update README for portfolio fork and contributions` | Docs | README, gameplay video, and `GameImages/` screenshots |

### Feature highlights (player-facing)

| Feature | Details |
|---------|---------|
| **Match rule** | Minimum **3** connected same-color tiles (`AppConstants.MinimumMatchSize`) |
| **HUD** | Score, level name, compact goals summary, Restart, Goals button |
| **Goals panel** | Full objective text; **Goals** opens overlay; **Close** or **Escape** closes it |
| **Pause** | **Escape** during play pauses; **Escape** again resumes via `LastState` |
| **Victory** | Stays on current level until **Next Level** is pressed |
| **Levels** | 5 ScriptableObject levels (`Level1` … `Level5`) |

---

## Architecture

### MVC + events

- **Model** — `Board`, `Tile`, goal strategies (`IGoal`), ScriptableObject `Level` data
- **View** — `GameHUD`, `GoalsPanelView`, `TileView` / `TileViewPool`, UI canvases
- **Controller** — `GameManager`, `LevelManager`, `GoalManager`, `GameplayController`, game states

Communication uses **events** (`OnScoreChanged`, `OnEndTurn`, board tile events) instead of tight coupling.

### Game states (`GameManager`)

Four states are registered at bootstrap. Goals use a **UI overlay** while gameplay stays in `PlayingState`.

| State / mechanism | Purpose |
|-------------------|---------|
| `PlayingState` | Active gameplay, music, HUD updates |
| `PauseState` | Freezes time (`Time.timeScale = 0`) |
| `VictoryState` | Level complete overlay |
| `GameOverState` | Failure overlay |
| `LastState` | Transition token — return to previous state (unpause) |
| Goals overlay | `GoalsPanelView` shown/hidden without leaving `PlayingState` |

### Goal types (Strategy)

- **`CollectColorTilesGoal`** — Clear N tiles of a given color
- **`CollectTilesPointsGoal`** — Reach a target score
- **`MaxMovesGoal`** — Finish within a move limit

### Design patterns

| Pattern | Usage |
|---------|--------|
| **Flyweight** | `TileFlyweight` — shared tile visuals |
| **Factory** | `TileFactory`, `LevelButtonFactory` |
| **State** | `GameManager` + `IGameState` implementations |
| **Strategy** | `IGoal`, `IMatchFinder` |
| **Observer** | Score, goals, board, HUD events |
| **Object pool** | `TileViewPool` |
| **Adapter** | `CanvasAdapter` — board coords → UI positions |
| **Singleton (scene)** | `CoroutineRunner` — board fill coroutines |

### Key interfaces

`IBoard`, `IMatchFinder`, `IScoreManager`, `ITileFactory`, `IGoalManager`, `IBoardModifier` — support swapping implementations and testing with mocks.

---

## Input handling

Tile and keyboard input are **not** read inside `MonoBehaviour.Update` on gameplay scripts. `GameInitializer` polls input from **`LateUpdate`** so UI buttons are processed first in the same frame.

### Flow

```
GameInitializer.LateUpdate()
  └─ GameplayController.ObserveClickHandler()
       ├─ Escape (KeyDown) → GameManager.OnPausePressed()
       ├─ Mouse button up   → Physics2D raycast → TileView → HandleTileClick()
       └─ Touch ended       → same raycast path
```

### Mouse and touch

| Input | When processed | Action |
|-------|----------------|--------|
| **Mouse** | `GetMouseButtonUp(0)` | Raycast from `Camera.main` screen position |
| **Touch** | `TouchPhase.Ended` | Same world-space raycast |
| **Escape** | `GetKeyDown` | Pause / resume / close goals overlay |

Using **button up** / **touch ended** (not press began) avoids the same click both firing a UI button and selecting a tile underneath.

### What blocks tile clicks

`GameplayController` skips tile input when:

1. **Goals overlay is open** — `GoalsPanelView.IsVisible` via `ConfigureGoalsOverlay`
2. **Pointer is over UI** — `EventSystem.current.IsPointerOverGameObject()`

`GameManager.HandleTileClick` then accepts clicks only in **`PlayingState`** (ignored during pause, victory, or game over).

### Escape priority

`OnPausePressed` order:

1. If goals overlay is open → close panel (`GoalsPanelView.Hide`), stay in `PlayingState`
2. If paused → `ChangeState(LastState)` to resume
3. If playing → `ChangeState(Paused)`

HUD **Goals** button calls `GoalsPanelView.Show()` directly; it does not toggle close (use **Close** or **Escape**).

---

## Project structure

```
Assets/
├── Scenes/
│   └── MainScene.unity              # Entry scene
├── ScriptableObjects/
│   ├── Level1.asset … Level5.asset
│   └── TileFlyweight_*.asset        # RED, BLUE, GREEN, YELLOW
├── Scripts/
│   ├── Runtime/TileMatchingGame/
│   │   ├── Controller/
│   │   │   ├── GameStates/          # Playing, Pause, Victory, GameOver
│   │   │   └── Interfaces/
│   │   ├── Model/                   # Board, Tile, goal strategies
│   │   ├── View/                    # GameHUD, GoalsPanelView, TileView
│   │   ├── Services/                # BoardModifier, DFSMatchFinder, pools
│   │   ├── Initializer/             # GameInitializer (scene bootstrap)
│   │   ├── ScriptableObjects/       # Level, TileFlyweight definitions
│   │   └── Utils/                   # AppConstants
│   └── Editor/                      # GoalSetupDrawer
├── Prefabs/
│   ├── ForefrontCanvas.prefab       # HUD, goals panel, overlays
│   ├── BackgroundCanvas.prefab
│   ├── GameInitializer.prefab
│   ├── SelectLevelButton.prefab
│   └── Tile.prefab
GameImages/
├── 1.png … 4.png                    # README screenshots
```

---

## Setup

### Prerequisites

- **Unity 2022.3.36f1 LTS** (recommended; 2022.3.x LTS should work)
- Git
- No extra packages required beyond the default project manifest

### Run locally

1. Clone the fork (URL above).
2. Unity Hub → **Add** → select project folder.
3. Open **`Assets/Scenes/MainScene.unity`**.
4. Press **Play**.
5. Assign **`GameInitializer`** references in the Inspector if opening a fresh scene instance (prefab/scene overrides should already wire HUD, goals panel, levels, audio).

### Adding media to the README

- **Screenshots:** save PNGs under **`GameImages/`** and reference them with `?raw=true` GitHub URLs (see top of this file).
- **Gameplay video:** upload to a GitHub issue/PR comment or release asset, then paste the bare `https://github.com/user-attachments/assets/...` URL on its own line in the README.

---

## Controls

| Input | Action |
|-------|--------|
| **Mouse click / tap** | Select tile cluster (≥ 3 tiles) on button up / touch release |
| **Goals** (HUD) | Open objectives overlay |
| **Close** (goals panel) | Close objectives overlay |
| **Restart** (HUD) | Restart current level |
| **Next Level** (Victory) | Load next level |
| **Escape** | Close goals when open; pause during play; resume when paused |

---

## License

MIT License — see [LICENSE](LICENSE).

Original work © [henritar](https://github.com/henritar). Portfolio maintenance © [SERAP-KEREM](https://github.com/SERAP-KEREM).
