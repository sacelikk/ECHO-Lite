# ECHO-Lite — Q-Learning in Unity

ECHO-Lite is a 2D gridworld reinforcement learning project developed in Unity using C#. The project explores how an autonomous agent can learn to navigate an environment, avoid obstacles, and reach a target using **Q-Learning**.

## Overview

The agent learns through repeated training episodes, receiving rewards based on its interactions with the environment. Over time, it improves its policy to reach the goal more consistently.

## Features

- **Q-Learning:** Tabular reinforcement learning for autonomous navigation.
- **Epsilon-Greedy Exploration:** Balances exploration and exploitation during training.
- **Configurable Hyperparameters:** Adjustable learning rate, discount factor, and exploration decay.
- **Obstacle Avoidance:** Navigates a grid containing blocked cells.
- **Visual Training:** Observe the agent's learning process directly in Unity.
- **Training Statistics:** Tracks episodes, successful runs, success rate, and training steps.
- **Learned Path Visualization:** Displays the path selected by the trained agent.

## Technologies

- Unity
- C#
- Reinforcement Learning
- Q-Learning
- Git and GitHub

## How It Works

1. The environment is generated as a 2D grid with a starting position, obstacles, and a goal.
2. The agent explores the environment and selects actions using an epsilon-greedy strategy.
3. The Q-table is updated using rewards and estimated future rewards.
4. Exploration decreases over successive episodes as the agent learns.
5. After training, the agent attempts to follow its learned policy toward the goal.

The Q-Learning update rule is:

\[
Q(s,a) \leftarrow Q(s,a) + \alpha \left[r + \gamma \max_{a'}Q(s',a') - Q(s,a)\right]
\]

Where:
- \(s\): Current state
- \(a\): Selected action
- \(r\): Reward received
- \(\alpha\): Learning rate
- \(\gamma\): Discount factor
- \(s'\): Next state

## Getting Started

1. Clone this repository:

   ```bash
   git clone https://github.com/sacelikk/ECHO-Lite.git
   ```

2. Open Unity Hub.
3. Select **Add project from disk** and choose the cloned project folder.
4. Open the project with a compatible Unity Editor version.
5. Open the main scene and press **Play** to observe the agent's training.

## Project Structure

```text
ECHO-Lite/
├── Assets/
│   └── Scripts/
│       ├── GridManager.cs
│       └── QLearningAgent.cs
├── Packages/
├── ProjectSettings/
├── .gitignore
└── README.md
```

## Future Improvements

- Compare different hyperparameter configurations.
- Visualize cumulative rewards and learning curves.
- Evaluate performance across multiple training runs.
- Experiment with different reward functions and exploration strategies.
- Extend the environment with larger grids and more complex obstacles.

## Author

**Samet Alım Çelik**

Computer Engineering Student | Erciyes University

GitHub: [@sacelikk](https://github.com/sacelikk)

---

*This project is an educational implementation of reinforcement learning in a Unity environment.*
