# 🗺️ Roadmap y Tracker de Proyecto: NIGMA (Visual Murdoku)
*Documento de Gestión de Proyecto (PM) - Actualizado: 01/08/2026 — Fase 4 completada*

Este documento actúa como la hoja de ruta central para el desarrollo de Nigma, el juego de deducción lógica visual.

## Fase 1: Pre-Producción y Diseño (Completada)
- [x] Brainstorming y Selección de Core Concept (Líneas de Visión + Físicas).
- [x] Definición de Nombre Oficial (Nigma).
- [x] Creación del Documento Maestro (GDD) y purgado de conceptos obsoletos.
- [x] **Diseño Lógico del Puzzle:** Definida la mecánica de "Escena Corrupta", progresión de muebles ópticos y condición de victoria.
- [x] **Diseño Visual de Interfaz (Mockup):** Generado primer arte conceptual estilo "Isometric Toy-Box / Claymation".

## Fase 2: Prototipo Técnico (Core Loop en Unity) - (Completada por IA, pendiente Setup Manual)
- [x] Configuración inicial de Unity (Repositorio, Settings Isométricos 3D/2D). *(Pendiente de que el usuario lo abra)*
- [x] Crear el "Grid Manager" (Lógica subyacente de casillas espaciales).
- [x] Crear el sistema Drag & Drop de personajes y mobiliario con animaciones físicas (Dangling).
- [x] Programar el sistema de Raycast de Línea de Visión (óptica) que valide si hay muros o espejos.
- [x] Programar el GameManager con 1 caso de prueba "Hardcodeado" y el botón "Resolver".

## Fase 3: Vertical Slice (Prototipo Jugable Completo) ✅
- [x] Conectar los scripts en el Editor de Unity (script automático `Phase3Setup.cs`).
- [x] Implementar la interfaz visual (UI) para el "Atestado Textual" y el Maletín (`UIManager.cs`).
- [x] Crear el sistema de inventario limitado por nivel (`LevelData.cs` con mirrors/sofas/cameras...).
- [x] Diseñar y hardcodear los primeros 3 puzzles reales (`PuzzleGenerator.cs` → Nivel 1-3 con atestados narrativos).
- [x] Sistema de Candado (Caja Fuerte Meta-puzzle) integrado en la UI (`SafeManager.cs`).
- [x] Integrar Jokers y sistema de Puntuación/Multiplicadores (`JokerManager.cs` con Lupa Antigua x3).

## Fase 4: Multijugador (Código de Sala) y Producción de Contenido ✅
- [x] Ampliar la biblioteca de Herramientas y Objetos Físicos (Cámaras, Plantas, Ventiladores, Lámparas).
  - Nuevo enum `FurnitureType.cs` con todos los tipos.
  - `VisionRaycaster.cs` reescrito con lógica por tipo: cono de cámara, transparencia de planta, radio de lámpara, desplazamiento de ventilador.
  - `LevelData.cs` ampliado con inventario Fase 4 y `multiplayerClueFragments`.
- [x] Integración de red (Network) usando Unity Relay + Netcode for GameObjects.
  - `NetworkBootstrapper.cs`: inicializa UGS + autenticación anónima.
  - `RelayManager.cs`: crea/une asignaciones Relay; configura UnityTransport.
- [x] Programar el Lobby con sistema de "Room Code" (Código de 4 letras para unirse).
  - `LobbyManager.cs`: crea/une salas Unity Lobby; heartbeat; polling de jugadores.
  - `LobbyUIController.cs`: UI de Lobby con reveal animado del código.
- [x] Programar el Modo **"Agencias Rivales"** (Carrera de velocidad sincrónica).
  - `AgenciasRivalesMode.cs`: timer local, penalización +30s, ranking via RPC.
- [x] Programar el Modo **"Policía Corrupto"** (Distribución de pistas asimétrica a diferentes clientes).
  - `PoliciaCorruptoMode.cs`: rol oculto, pistas falsas procedurales, votación via RPC.

## Fase 5: Monetización (Freemium), Pulido y Lanzamiento
- [ ] Implementar In-App Purchases (Pago único de 4.99€ para desbloquear Premium).
- [ ] Configurar los muros de pago (Campaña de 20 niveles y Caso Diario = Gratis; Roguelite Infinito y Multijugador = Premium).
- [ ] Game Feel y ASMR: Pulir los efectos de sonido táctiles (papel, sellos, piezas de madera).
- [ ] Compilación final (Builds) para Itch.io / Steam / Stores móviles.
