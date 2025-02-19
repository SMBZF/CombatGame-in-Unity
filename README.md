# Bee Knight - A Remake of *White Lavender*  

This is a **combat-focused action game** where players can switch between **melee and ranged combat**, utilizing both a sword and a bow to fight enemies. This project is a **remake of *White Lavender***, with enhanced mechanics and additional gameplay elements.  

📺 **Gameplay Video**: [Watch on YouTube](https://www.youtube.com/watch?v=M97tmWkgadI)  
🎮 **Play the Game**: [Itch.io](https://smbzf.itch.io/bee-knight)  

[![Watch the video](https://img.youtube.com/vi/M97tmWkgadI/0.jpg)](https://www.youtube.com/watch?v=M97tmWkgadI)  

---

## **⚔️ Features & Enhancements**  

### 🔹 **Character Controller**  
✅ **Original**: Camera only rotated horizontally with the mouse; movement used `WASD`, with sprinting via `Shift + WASD`.  
✅ **Remake**: Same controls, but added **vertical camera adjustment**, making aiming with the bow smoother.  

### 🔹 **Melee Combat**  
✅ **Original**: Charged and normal sword attacks, enemy targeting (`Q`), and a kick (`X`).  
✅ **Remake**:  
- Implemented **combo attacks** (multiple hits when clicking rapidly).  
- Retained **charging, targeting, and kicking mechanics**.  

### 🔹 **Ranged Combat** (New Feature 🚀)  
❌ **Original**: No ranged combat.  
✅ **Remake**: Added a **bow for ranged attacks**, allowing precise long-range engagements.  

### 🔹 **Enemy AI**  
✅ **Original**: Different enemy types with varying attack behaviors.  
✅ **Remake**:  
- Implemented **basic AI detection & pursuit**.  
- Due to time constraints, **only one enemy type was developed**.  

### 🔹 **Health & Stamina System**  
✅ **Original**: Health, stamina, and mana bars.  
✅ **Remake**: Only **health and stamina** were implemented (no magic system).  

### 🔹 **Other Features**  
✅ **Sound Effects**: Added background music, attack sounds, and walking effects.  
✅ **Combat Visual Feedback**: Screen shake & impact animations for attacks.  
✅ **Gamepad Support**: Designed with keyboard & controller input compatibility.  

---

## **🔧 Technical Overview**  

### 🎮 **Core Systems Implemented**  
✔ **State Machine for Combat** - Handles attack wind-up, impact, and cooldown.  
✔ **Third-Person Controller** - Smooth movement & camera tracking.  
✔ **Bow & Arrow Physics** - Uses Bézier curves for natural arrow trajectory.  
✔ **Enemy AI System** - Detection, pursuit, and attack logic.  

### 🛠 **Tech Stack**  
- **Engine**: Unity  
- **Language**: C#  
- **Tools**: Git, Visual Studio  
- **Physics**: Rigidbody-based combat interactions  

---

## **💡 My Contributions**  
I independently **recreated core mechanics** from *White Lavender* while introducing **ranged combat** and improving gameplay smoothness. My key contributions include:  
- **Full Combat System Implementation** - Melee (sword combos) & ranged (bow shooting).  
- **Player Controller & Camera System** - Including third-person movement & vertical aiming.  
- **Enemy AI** - Developed enemy detection and pursuit behaviors.  
- **UI & Game Feel Enhancements** - Sound design, visual effects, and controls tuning.  
