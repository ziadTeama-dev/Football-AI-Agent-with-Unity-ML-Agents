# ⚽ AI Football Agent — Unity ML-Agents

A Unity ML-Agents project where an AI football agent learns how to move around the field, track the ball, interact with it, use a dash ability, and score goals.

The main goal of the project was to design a reward system that encourages useful football behavior instead of allowing the agent to exploit the environment.

---

## 🎮 Project Overview

The agent is trained using **Unity ML-Agents** and receives observations about:

- ⚽ Ball position and velocity
- 🤖 Enemy position and velocity
- 🥅 Opponent goal position
- 🏠 Agent's own goal position
- 🏃 Agent velocity
- 👀 Agent forward direction
- 🔄 Agent rotation

The agent can control:

- Forward/backward movement
- Left/right movement
- Rotation
- Dash

---

# 🎞️ GIF Demo

Add a GIF showing the trained agent playing football.

![Gameplay GIF](Recordings/Movie_001.gif)

**Suggested GIF content:**

- Agent approaching the ball
- Agent interacting with the ball
- Agent using dash
- Agent scoring a goal

---
## 🧠 Main Problems During Training & Solutions

### 1. Training Was Too Slow

#### Problem

At the beginning, the agent was mainly given a reward for scoring a goal.

This made the learning process very slow because scoring a goal is a relatively rare event. The agent had very little feedback while trying to discover how to reach the ball and interact with the game.

#### Solution

A small negative reward was added based on the distance between the agent and the ball.

```csharp
float distToBall = Vector3.Distance(transform.position, ball.position);
AddReward(-Mathf.Clamp(distToBall, 0f, 5f) * 0.0003f);
```

This gives the agent a continuous signal that encourages it to stay closer to the ball.

---

### 2. How to Make the Agent Look at the Ball?

#### Problem

The agent needed to pay attention to the ball, but simply giving a goal reward was not enough to encourage this behavior.

#### Solution

A reward based on the angle between the agent's forward direction and the direction toward the ball was introduced.

```csharp
Vector3 dirToBall = (ball.position - transform.position).normalized;
float lookDot = Vector3.Dot(transform.forward, dirToBall);

AddReward(Mathf.Max(0f, lookDot) * 0.01f);
```

The closer the agent's forward direction is to the ball, the higher the reward.

---

### 3. The Agent Kept Looking at the Ball

#### Problem

The previous solution created a new problem.

The agent learned that continuously looking at the ball was beneficial, so it could spend too much time focusing on the ball instead of actually interacting with it.

#### Solution

A reward was added for actually touching the ball.

```csharp
private void OnCollisionEnter(Collision collision)
{
    if (collision.collider.CompareTag("ball"))
    {
        AddReward(0.07f);
    }
}
```

This changes the objective from simply:

> "Look at the ball"

to:

> "Look at the ball and eventually interact with it."

---

### 4. The Agent Used Dash Too Often

#### Problem

The agent has a dash ability that can be useful in critical situations.

However, without a proper penalty, the agent could learn to use dash too frequently instead of learning when dash is actually useful.

#### Solution

A negative reward was applied every time the agent successfully performed a dash.

```csharp
if (dash == 1 && Time.time - lastDashTime > dashCooldown)
{
    rb.AddForce(transform.forward * dashForce, ForceMode.VelocityChange);
    lastDashTime = Time.time;

    AddReward(-0.05f);
}
```

A cooldown was also added:

```csharp
[SerializeField] private float dashCooldown = 1.5f;
```

This encourages the agent to save dash for situations where it provides a meaningful advantage.

### 🎯 Result

The reward system therefore encourages the agent to use dash strategically rather than constantly.

---

## 🏆 Reward System

The current reward system contains several components:

| Behavior | Reward |
|---|---:|
| ⚽ Score a goal | `+7.0` |
| 💥 Opponent loses | `-5.0` |
| 🏃 Touch the ball | `+0.07` |
| 👀 Look toward the ball | Up to `+0.01` |
| 🥅 Move ball toward goal | Based on alignment |
| 📍 Stay closer to ball | Small negative distance penalty |
| ⏱️ Spend time | `-0.005` per action |
| 💨 Use dash | `-0.05` |

The idea is to combine **sparse rewards** (scoring) with **dense rewards** (ball distance, looking at the ball, touching the ball, and ball movement).

---

## ⚙️ Dash System

The dash is implemented as a discrete action:

```csharp
int dash = actions.DiscreteActions[0];
```

When the agent chooses dash:

```csharp
rb.AddForce(
    transform.forward * dashForce,
    ForceMode.VelocityChange
);
```

The dash also has a cooldown:

```csharp
dashCooldown = 1.5f;
```

and a penalty:

```csharp
AddReward(-0.05f);
```

This combination helps prevent unnecessary dash usage.

---

## 🥅 Goal System

The `Goal_Is_hit` script detects when the ball enters either goal.

It tracks:

- Red team goals
- Blue team goals
- Total goals
- Goal effects

When a goal is scored, the game also triggers visual effects such as:

- 💥 Particles
- 📷 Camera shake
- 🐌 Slow motion
- 📝 Goal text

The winning agent receives:

```csharp
AddReward(7f);
```

while the opponent receives:

```csharp
enemyAgent.AddReward(-5f);
```

---

# 📸 Screenshots

## 🎮 Game Screenshot

![Game Screenshot](screenshots/gameplay.png)

---



# 📊 TensorBoard

TensorBoard can be used to visualize the training process and monitor how the agent improves over time.


![TensorBoard Training](screenshots/tensorboard/tensorboard.png)

- to open tensorboard
```
cd Football-AI-Agent-with-Unity-ML-Agents

<!-- then run tensorboard -->

tensorboard --logdir results

```

---


# 🏗️ Project Structure

```text
.
├── Assets/
│   ├── Scripts/
│   ├── Scenes/
│   ├── Prefabs/
│   └── ...
├── Logs/
├── Packages/
├── ProjectSettings/
├── results/
├── .vscode/
├── adversarial_agents.sln
├── adversarial_agents2.sln
├── Assembly-CSharp.csproj
└── README.md
```

---

# 🛠️ Technologies

- **Unity**
- **Unity ML-Agents**
- **C#**
- **TensorBoard**
- **Reinforcement Learning**

---

# 📥 Download Project

Download the complete Unity project:

- **i will uploaded when internet connection become stable incase who didn't find the project**

**[⬇️ Download the Project](YOUR_PROJECT_DOWNLOAD_LINK_HERE)**



---

# 🚀 How to Run

1. Clone or download the project.
2. Open the project using the compatible Unity version.
3. Open the main scene.
4. Make sure Unity ML-Agents is installed correctly.
5. Open the training configuration if you want to continue training.
6. Run the scene to test the agent.

---

# 🔬 Future Improvements

Possible improvements for the project:

- Add more advanced tactical rewards.
- Reward successful passes and dribbles.
- Improve opponent behavior.
- Add multiple agents.
- Introduce team-based strategies.
- Improve dash decision-making based on game state.
- Add better reward shaping for shooting and defending.
- Compare different ML-Agents training configurations.

---

## 👨‍💻 Project Goal

The main objective of this project is to experiment with **Reinforcement Learning in a real-time football environment** and understand how reward design affects the behavior learned by an AI agent.

A major part of the project was not only training the model, but also identifying unwanted behaviors and modifying the reward function to guide the agent toward more useful football behavior.

---

## 👨‍💻 Author

Ziad Abdelhaliem Teama