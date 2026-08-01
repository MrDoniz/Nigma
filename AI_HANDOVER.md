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
- **Phase 1 (Design):** ✅ 100% COMPLETE.
- **Phase 2 (Prototyping):** ✅ 100% COMPLETE.
- **Phase 3 (Vertical Slice):** ✅ 100% COMPLETE.
- **Phase 4 (Multiplayer & Content Production):** ✅ 100% COMPLETE (scripts written; pending Editor setup by user).

### Phase 4 scripts written (all in `Nigma_Unity/Assets/Scripts/`):

| File | Purpose |
|---|---|
| `FurnitureType.cs` | Enum for all furniture types (Wall, Mirror, Sofa, Camera, Lamp, Plant, Fan, Character) |
| `LevelData.cs` | Extended with Camera/Lamp/Plant/Fan inventory, `multiplayerClueFragments`, `isLightRequired` |
| `DraggableObject.cs` | Extended with `furnitureType`, `isShortCharacter`, `cameraVisionAngle`, `fanBlowDirection` |
| `VisionRaycaster.cs` | Rewritten: Camera cone, Plant transparency, Lamp radius, Fan curtain displacement |
| `UIManager.cs` | Expanded: `UpdateInventoryFull()` for all 6 object types, `DecrementCounter()` |
| `Networking/NetworkBootstrapper.cs` | UGS init + anonymous sign-in |
| `Networking/RelayManager.cs` | Unity Relay: create/join allocations + configure UnityTransport |
| `Networking/LobbyManager.cs` | Room Code system (4 letters), Unity Lobby create/join/heartbeat |
| `Networking/LobbyUIController.cs` | Lobby UI: Host panel, Client join panel, mode selector, animated code reveal |
| `GameModes/AgenciasRivalesMode.cs` | Speed-race mode: local timer, +30s penalty, ServerRpc ranking |
| `GameModes/PoliciaCorruptoMode.cs` | Asymmetric mode: secret Corrupto role, clue distribution, vote tallying |

### ⚠️ Pending user manual steps (required before testing multiplayer):
1. **Unity Dashboard:** [dashboard.unity.com](https://dashboard.unity.com) → link project "Nigma" → enable **Relay**, **Lobby**, **Authentication**.
2. **Unity Editor:** Edit → Project Settings → Services → paste Project ID.
3. **Package Manager:** install `com.unity.netcode.gameobjects`, `com.unity.services.relay`, `com.unity.services.lobby`, `com.unity.services.authentication`.
4. **Define symbols:** Add `UNITY_NETCODE` and `UNITY_SERVICES_RELAY` and `UNITY_SERVICES_LOBBY` and `UNITY_SERVICES_CORE` to **Player Settings → Scripting Define Symbols**.
5. **Scene setup:** Add `NetworkBootstrapper`, `RelayManager`, `LobbyManager` and `LobbyUIController` GameObjects/components to the Lobby scene.

## 🎯 Your Immediate Mission (Phase 5)
Read `Docs/PM/Roadmap_Nigma.md`. You are starting **Phase 5: Monetización, Pulido y Lanzamiento**.

Your job is to:
1. **In-App Purchases:** Implement the `PremiumManager.cs` with a 4.99€ IAP unlock using Unity IAP (`com.unity.purchasing`).
2. **Paywall:** Lock the Roguelite Infinite mode and Multiplayer behind `PremiumManager.IsPremiumUnlocked`. The Friend Pass (host only needs premium) is already designed — implement it.
3. **Game Feel & ASMR:** Add `AudioManager.cs` with ASMR-style sound events: wood "CLAC" on placement, paper rustling in UI, background jazz loop.
4. **Build:** Set up build profiles for PC (Steam/Itch.io) and mobile (iOS/Android).

**Note:** Always check with the user before integrating any third-party store SDKs (Steam, Play Store, etc.) as they require developer accounts and certificates.

Good luck! 🕵️‍♂️
