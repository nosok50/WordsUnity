# Words (Слова) 🧩

> A 3D mobile word puzzle game inspired by Wordle mechanics, featuring a custom offline challenge system and native Android integration.

<img width="1269" height="600" alt="WordsScreen4 (1)" src="https://github.com/user-attachments/assets/81f58fae-fb7d-4d72-98f0-6ba16488f139" />

## About the Project
**Words** is a logic puzzle game where players have 6 attempts to guess a hidden word. The game provides visual feedback using a color-coded system to indicate correct letters and positions.

While the game is built in a 3D environment, it features a clean 2D UI optimized for mobile devices. The interface is currently in RU.

**Status:** Completed Prototype.

## Key Features
* **Classic Gameplay:** 6 attempts to guess the word with Green/Orange/Grey feedback.
* **Dynamic Keyboard:** The on-screen keyboard updates key colors in real-time based on the player's guesses.
  <img width="1269" height="600" alt="WordsScreen3 (1)" src="https://github.com/user-attachments/assets/232316e8-97ec-4524-bdfb-b8c3d7bcf61a" />
* **Offline Challenge System:** A unique feature that allows players to input a custom word, generate a cryptographic hash/code, and share it with friends. Friends can enter this code to play that specific word. **No internet connection required.**
  <img width="1269" height="600" alt="WordsScreen3 (2)" src="https://github.com/user-attachments/assets/33e6133a-1229-438f-b92e-b74735c995bc" />
* **Save System:** Progress is automatically saved using JSON serialization.
* **Native Android Integration:** Uses native Android keyboard and clipboard functionality for a seamless mobile experience.

## Tech Stack
* **Engine:** Unity 2022.3.62f3 LTS
* **Language:** C#
* **Core Packages:**
    * **New Input System:** For handling touch and input events.
    * **DOTween:** For smooth UI animations and game feel.
* **Data & System:**
    * **JsonUtility:** For local data persistence (save/load).
    * **AndroidJavaClass:** Used to bridge Unity with Android native features (Keyboard & Clipboard).

## Screenshots
<img width="1269" height="600" alt="WordsScreen1 (1)" src="https://github.com/user-attachments/assets/d546edca-636e-4970-b775-e18adbf60608" />

## How to Play
The game is designed for **Android devices**.
1.  **Guess the word:** Type a word using the on-screen keyboard.
2.  **Check hints:**
    * 🟩 **Green:** Correct letter in the correct spot.
    * 🟧 **Orange:** Correct letter but in the wrong spot.
    * ⬜ **Grey:** Letter is not in the word.
3.  **Create a Challenge:** Go to the menu, type a word to generate a code, and copy it to the clipboard to send to a friend.

## Installation
This is a source code repository. To run the project:
1.  Clone the repository.
2.  Open in Unity 2022.3.62f3 or later.
3.  Build for Android Platform.

---
*Created by nosok50*
