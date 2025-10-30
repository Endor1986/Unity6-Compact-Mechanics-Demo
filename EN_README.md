<p align="right">
  <a href="./DE_README.md">🇩🇪 Deutsche Version</a>
</p>

<p align="center">
  <img alt="Unity" src="https://img.shields.io/badge/Unity-6%2B-000000?logo=unity&logoColor=white">
  <img alt="HDRP" src="https://img.shields.io/badge/Render_Pipeline-HDRP-222222?logo=unity&logoColor=white">
  <img alt="Language" src="https://img.shields.io/badge/Language-C%23-178600?logo=csharp&logoColor=white">
  <img alt="License" src="https://img.shields.io/badge/License-MIT-lightgrey.svg">
</p>

# 🎮 Unity6 Compact Mechanics Demo

A compact **learning project** built in **Unity 6 (HDRP)**, showcasing essential **gameplay mechanics**.  
Originally created as part of an early *Dark Continent* prototype, this system was later refactored into a **side-scroller setup**  
to demonstrate its mechanics in a simpler, more modular and reusable form.

---

## ✨ Features

- **Player movement & jumping** (simple physics-based controller)  
- **Pressure plate interactions** (activates linked objects)  
- **Elevator system** (moves between two defined points)  
- **Automatic door opening**  
- Cleanly structured, modular C# scripts – ideal for learning and prototyping

---

## 🚀 Quickstart

### 1️⃣ Import & Setup
1. Open **Unity 6 (6000.2.6f2)** – recommended HDRP project  
2. Copy the folders `Scripts/`, `Assets/`, and `Docs/` into your Unity project  
3. Attach the four included scripts to their corresponding objects  
4. Hit ▶️ **Play** to test the system

### 2️⃣ Scene Overview (Quick Summary)
| Object | Script | Description |
|---------|---------|-------------|
| Player | `ThirdPersonControllerInputSystem` | Handles movement, sprinting, and jumping (used sideways here) |
| Elevator | `ElevatorSimple` | Moves up and down between two waypoints |
| Door | `DoorMoverOnce` | Moves once (e.g., opens upward) when triggered |
| Pressure Plate | `PressurePlateSimpleOnce` | Starts the elevator and opens the door when stepped on |

> Detailed setup steps and screenshots are included in the **Docs/** folder (`EN` + `DE`).

---

## 📦 Requirements

- **Unity 6.0.2 (HDRP)** or newer  
- Optional: **ProBuilder** for quick level design  
- Optional: Works with **URP** or **Built-in Pipeline** as well  

---

## 🧱 Included Files

**Scripts**  
`ThirdPersonControllerInputSystem.cs`  
`ElevatorSimple.cs`  
`DoorMoverOnce.cs`  
`PressurePlateSimpleOnce.cs`

**Example Assets**  
`Door (with frame)`  
`Elevator Platform`  
`Pressure Plate`

---

## 📘 Documentation

Full setup guide available in the [`Docs/`](Docs) folder  
(in **English** and **German**):

- 🎮 (EN) Unity6 Compact Mechanics Demo.pdf  
- 🎮 (DE) Unity6 Compact Mechanics Demo.pdf  

Includes:
- Scene setup (Player, Elevator, Door, Plate)  
- ProBuilder workflow tips  
- Expansion ideas and best practices  

---

## 🔧 Recommended Tools

| Tool | Description |
|------|--------------|
| 🧱 **ProBuilder** | Build test levels quickly and easily |
| 🎨 **Standard Materials** | Simple shaders for clear visibility |
| 💡 **HDRP Lighting** | Realistic materials and lighting |

---

## 🧠 Expansion Ideas

- 🔊 **Sound effects** – Add sounds for doors and elevator movement  
- 💡 **Light feedback** – e.g., a lamp turns green when a plate is activated  
- 🧩 **Multi-triggers** – Two plates must be pressed simultaneously to open a door  
- 🕹️ **Controller support** – via Unity’s Input System package  
- 📦 **Prefab kit** – Turn all components into reusable prefabs  

---

## 🧰 Project Structure

```text
Unity6-Compact-Mechanics-Demo/
├── README.md
├── README_EN.md
├── Docs/
│   ├── 🎮 (EN) Unity6 Compact Mechanics Demo.pdf
│   └── 🎮 (DE) Unity6 Compact Mechanics Demo.pdf
├── Scripts/
│   ├── ThirdPersonControllerInputSystem.cs
│   ├── ElevatorSimple.cs
│   ├── DoorMoverOnce.cs
│   └── PressurePlateSimpleOnce.cs
└── Assets/
    ├── Door/
    ├── ElevatorPlatform/
    └── PressurePlate/
```

---

## ✅ Project Status

- Fully functional and documented demo  
- Free to use and modify  
- Perfect for Unity beginners or mechanic prototypes  

---

## 👤 Author & Contact

**Phillip Kley**  
🌐 [epiccodemakers.eu](https://epiccodemakers.eu)  
📧 [p.kley86@web.de](mailto:p.kley86@web.de)  
💾 [GitHub: Endor1986](https://github.com/Endor1986)  

---

## 📜 License

**MIT License** – see `LICENSE`.  
This project is free to use, modify, and redistribute for learning purposes.