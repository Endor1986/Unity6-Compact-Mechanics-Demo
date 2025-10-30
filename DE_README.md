<p align="right">
  <a href="./EN_README.md">🇬🇧 English Version</a>
</p>

<p align="center">
  <img alt="Unity" src="https://img.shields.io/badge/Unity-6%2B-000000?logo=unity&logoColor=white">
  <img alt="HDRP" src="https://img.shields.io/badge/Render_Pipeline-HDRP-222222?logo=unity&logoColor=white">
  <img alt="Language" src="https://img.shields.io/badge/Sprache-C%23-178600?logo=csharp&logoColor=white">
  <img alt="License" src="https://img.shields.io/badge/Lizenz-MIT-lightgrey.svg">
</p>


# 🎮 Unity6 Compact Mechanics Demo

Ein kompaktes **Lernprojekt** in **Unity 6 (HDRP)**, das grundlegende **Gameplay-Mechaniken** zeigt.  
Ursprünglich als Teil eines *Dark Continent*-Prototyps gedacht, wurde es zu einem **Side-Scroller** umgebaut,  
um die Mechaniken einfacher, klarer und wiederverwendbar zu machen.

---

## ✨ Funktionen

- **Spielerbewegung & Sprung** (einfacher Physik-Controller)  
- **Druckplatten-Interaktion** (aktiviert verbundene Objekte)  
- **Fahrstuhlsystem** (pendelt zwischen zwei Punkten)  
- **Automatisches Türöffnen**  
- Sauber strukturierte, modulare C#-Skripte – ideal für Lernprojekte und Prototypen

---

## 🚀 Schnellstart

### 1️⃣ Import & Einrichtung
1. Öffne **Unity 6 (6000.2.6f2)** – empfohlenes HDRP-Projekt  
2. Kopiere die Ordner `Scripts/`, `Assets/` und `Docs/` in dein Projekt  
3. Füge die vier enthaltenen Skripte den entsprechenden Objekten hinzu  
4. Starte die Szene ▶️ und teste das System

### 2️⃣ Szenenaufbau (Kurzüberblick)
| Objekt | Script | Beschreibung |
|---------|---------|-------------|
| Spieler | `ThirdPersonControllerInputSystem` | Seitliche Bewegung, Sprinten, Springen |
| Fahrstuhl | `ElevatorSimple` | Bewegt sich zwischen oberem und unterem Punkt |
| Tür | `DoorMoverOnce` | Öffnet sich nach oben, wenn ausgelöst |
| Druckplatte | `PressurePlateSimpleOnce` | Startet Fahrstuhl + öffnet Tür |

> Detaillierte Schritte und Screenshots findest du im **Docs/**-Ordner (`DE` + `EN`).

---

## 📦 Voraussetzungen

- **Unity 6.0.2 (HDRP)** oder neuer  
- Optional: **ProBuilder** (zum schnellen Levelbau)  
- Optional: **URP** oder **Built-in Pipeline** ebenfalls kompatibel  

---

## 🧱 Enthaltene Dateien

**Skripte**  
`ThirdPersonControllerInputSystem.cs`  
`ElevatorSimple.cs`  
`DoorMoverOnce.cs`  
`PressurePlateSimpleOnce.cs`

**Beispiel-Assets**  
`Door (mit Rahmen)`  
`Elevator Platform`  
`Pressure Plate`

---

## 📘 Dokumentation

Ausführliche Anleitung im Ordner [`Docs/`](Docs)  
(in **deutscher** und **englischer** Version):

- 🎮 (DE) Unity6 Compact Mechanics Demo.pdf  
- 🎮 (EN) Unity6 Compact Mechanics Demo.pdf  

Inhalt:
- Szenenaufbau (Spieler, Fahrstuhl, Tür, Platte)  
- Tipps für ProBuilder  
- Erweiterungen und Lernhinweise  

---

## 🔧 Empfohlene Tools

| Tool | Beschreibung |
|------|---------------|
| 🧱 **ProBuilder** | Zum schnellen Erstellen von Testleveln |
| 🎨 **Standard-Materialien** | Klare Farben und Shader für gute Sichtbarkeit |
| 💡 **HDRP-Lighting** | Realistische Beleuchtung und Schatten |

---

## 🧠 Erweiterungen & Ideen

- 🔊 **Soundeffekte** – z. B. für Tür- und Fahrstuhlbewegungen  
- 💡 **Licht-Feedback** – Lampe leuchtet grün, wenn Platte aktiv ist  
- 🧩 **Mehrfach-Trigger** – zwei Platten müssen gleichzeitig gedrückt werden  
- 🕹️ **Controller-Unterstützung** – mit Unity Input System  
- 📦 **Prefab-Baukasten** – Komponenten exportieren und wiederverwenden  

---

## 🧰 Projektstruktur

```
Unity6-Compact-Mechanics-Demo/
├── README.md
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

---

## ✅ Projektstatus

- Voll funktionsfähige und dokumentierte Demo  
- Frei nutzbar und anpassbar  
- Perfekt für Unity-Einsteiger oder Mechanik-Prototypen  

```

---

## 👤 Autor & Kontakt

**Phillip Kley**  
🌐 [epiccodemakers.eu](https://epiccodemakers.eu)  
📧 [p.kley86@web.de](mailto:p.kley86@web.de)  
💾 [GitHub: Endor1986](https://github.com/Endor1986)  

---

## 📜 Lizenz

**MIT-Lizenz** – siehe `LICENSE`.  
Dieses Projekt darf zu Lernzwecken frei verwendet, verändert und weitergegeben werden.
