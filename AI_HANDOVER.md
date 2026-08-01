# 🤖 AI Handover Document (Nigma)

**To the next Antigravity AI Agent reading this:**

Welcome to the Nigma project! The user has spun up a new chat session to keep the context clean for the next development phase.

## 📌 Project Context
- **Name:** Nigma
- **Genre:** Casual Isometric Logic Puzzle (Visual Murdoku + Balatro Synergies)
- **Engine:** Unity (C#)
- **Core Loop:** The player reads a text riddle ("Atestado policial"), uses a limited inventory of draggable furniture (Mirrors, Sofas, etc.) to visualize physical lines of sight on a grid, and clicks "Solve" to answer the riddle based on their deductions.
- **Rules:** The physical design is set in stone. Read `Docs/Design/Documento_Maestro_Diseño.md` for the exact rules (furniture physics, score-based Jokers, text riddle mechanics).

## 🚀 Current Status (Where we left off)
- **Phase 1 (Design):** 100% COMPLETE.
- **Phase 2 (Prototyping):** I (the previous AI) have scaffolded the 4 core C# scripts (`GridManager.cs`, `VisionRaycaster.cs`, `DraggableObject.cs`, `GameManager.cs`) inside `Assets/Scripts/`.
- The user is currently installing Unity Hub and setting up the initial empty Unity project locally.

## 🎯 Your Immediate Mission (Phase 3)
Read the `Docs/PM/Roadmap_Nigma.md` file. You are starting **Phase 3: Vertical Slice**.

The user will likely tell you that they have opened Unity and assigned the scripts to objects. 
Your job is to:
1. Guide them through connecting the public variables in the Unity Editor if they need help.
2. Begin designing the UI architecture (Canvas) for the Text Riddle and the Maletín (Inventory).
3. Hardcode the first 3 actual levels/puzzles in `GameManager.cs` or a new `LevelData` structure.

**Do NOT change the game mechanics.** The design phase is closed and the user is very happy with it. Focus entirely on Unity C# implementation and UI building.

Good luck! 🕵️‍♂️
