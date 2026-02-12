# Game Developer Technical Assessment: The Bouncing Orb
This project implements the required 2D physics-based orb game with scoring, reset obstacle, and basic UI as described in the assessment brief.

## 1. Game Engine and Language
- Game Engine: Unity (2D)
- Recommended Version: Unity 2021 LTS or later
- Programming Language: C#

## 2. Project Structure

- `Assets/Scenes/MainScene.unity`  
  Main gameplay scene containing the orb, ground, obstacles, UI, and managers.
  
- `Assets/Scripts/`
  - `PlayerOrbController.cs` – Handles orb movement (A/D or Arrow keys) and jump (Space), using Rigidbody2D physics.
  - `OrbCollisionHandler.cs` – Detects collisions with ground and reset obstacle, updates score, and plays bounce sound.
  - `ScoreManager.cs` – Manages the score value, UI text display, and global score access.
  - `RoadSpawner.cs` (optional) – Spawns ground segments ahead of the player for continuous road.
  - `RandomObstacleSpawner.cs` (optional) – Spawns reset obstacles ahead of the player, with configurable spacing and height.

- `Assets/Prefabs/`
  - `Orb.prefab` – Player orb with Rigidbody2D, CircleCollider2D, and scripts attached.
  - `GroundSegment.prefab` – Ground piece with BoxCollider2D and static body.
  - `ObstaclePrefab.prefab` – Airborne rectangular obstacle (ResetObstacle).

- `Assets/Materials/`
  - `OrbBounce.mat` – Physics 2D Material (bounciness ~0.6).

- `Assets/UI/`
  - `ScoreText` – TextMeshProUGUI element showing current score.

## 3. How to Open and Run the Project

1. Install Unity (2021 LTS or later recommended).
2. Clone or unzip the project folder to your local machine.
3. Open Unity Hub.
4. Click **Open** and select the project folder.
5. After Unity loads, open the scene:
   - `Assets/Scenes/MainScene.unity`
6. Press the **Play** button in the Unity Editor to start the game.

## 4. Controls

- **Move Left**: `A` or Left Arrow
- **Move Right**: `D` or Right Arrow
- **Jump**: `Space`  
  (Jump works only when the orb is grounded on the floor.)

## 5. Implemented Features (Assessment Mapping)

### Part 1: Core Mechanics

- **2D Project & Camera**  
  The game uses a 2D Unity project with a simple main scene and default camera.

- **Environment (Ground)**  
  A static ground platform using `BoxCollider2D` and static `Rigidbody2D` acts as the floor.

- **Player Orb**
  - Circular sprite representing the player.
  - `Rigidbody2D` (Dynamic) so it is affected by gravity.
  - `CircleCollider2D` with Physics 2D Material for bounciness.

- **Movement**  
  `PlayerOrbController.cs` applies horizontal acceleration based on `Horizontal` input (A/D or Arrow keys), with a clamped maximum horizontal speed.

- **Bouncing Collision**  
  Orb uses a Physics 2D Material with a moderate restitution (bounciness 0.5–0.8) to bounce realistically off the ground.

- **Restricted Jump**  
  Jump is only allowed when the orb is grounded, detected via an overlap circle at a ground check point under the orb.

### Part 2: Feature Enhancement & Logic

- **Score Tracking**
  - `ScoreManager.cs` holds a persistent integer `Score`.
  - Score is displayed via a UI TextMeshPro element (`ScoreText`).

- **Scoring Mechanic**
  - Every time the orb collides with the ground, `OrbCollisionHandler.cs` calls `ScoreManager.AddBouncePoint()` to increment the score.

- **Reset Obstacle**
  - A static rectangular obstacle is placed in the air above the ground.
  - Tagged as `ResetObstacle`.
  - When the orb collides with this obstacle, `ScoreManager.ResetScore()` is called immediately.

- **Optional Enhancements**
  - Road spawning to create a continuous ground.
  - Random or spaced obstacle spawning along the X axis at a fixed or random Y height.
  - Bounce sound effect played when the orb hits the ground via an AudioSource on the orb.

### Part 3: Submission & Code Quality

- **Code Quality**
  - Scripts are separated by responsibility (movement, collisions, scoring, spawning).
  - Meaningful names are used for variables, methods, and components.
  - Comments explain non-trivial logic such as ground checks and spawn rules.

- **Project Organization**
  - Clear separation into `Scenes`, `Scripts`, `Prefabs`, `Materials`, and `UI` folders.
  - All required assets for running the test are contained in the project.

## 6. How to Customize

- Adjust orb movement and jump:
  - In `PlayerOrbController`, tweak `moveAcceleration`, `maxHorizontalSpeed`, and `jumpForce`.
- Adjust bounciness:
  - Modify `OrbBounce` Physics 2D Material bounciness.
- Adjust obstacle frequency and height:
  - In `RandomObstacleSpawner`, edit `minXSpacing`, `maxXSpacing`, `minY`, and `maxY`.
- Change reset behavior:
  - In `ScoreManager.ResetAll()` or button callback, add or remove logic such as resetting orb position.

---

This `README.md` should satisfy the assessment’s requirement: it documents the engine, language, how to open and run the project, and lists all controls and implemented features clearly.
