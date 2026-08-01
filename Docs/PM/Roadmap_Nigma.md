# 🗺️ Roadmap y Tracker de Proyecto: NIGMA (Visual Murdoku)
*Documento de Gestión de Proyecto (PM) - Actualizado: 01/08/2026*

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

## Fase 3: Vertical Slice (Prototipo Jugable Completo)
- [ ] Conectar los scripts en el Editor de Unity (Asignar variables públicas, crear Prefabs de Muros y Espejos).
- [ ] Implementar la interfaz visual (UI) para el "Atestado Textual" y el Maletín.
- [ ] Crear el sistema de inventario limitado por nivel.
- [ ] Diseñar y hardcodear los primeros 3-5 puzzles reales para probar la curva de dificultad.
- [ ] Sistema de Candado (Caja Fuerte Meta-puzzle) integrado en la UI.
- [ ] Integrar Jokers y sistema de Puntuación/Multiplicadores (que alteren el meta-juego, no la física).

## Fase 4: Multijugador (Código de Sala) y Producción de Contenido
- [ ] Ampliar la biblioteca de Herramientas y Objetos Físicos (Espejos, Muros, Cámaras).
- [ ] Integración de red (Network) usando capa gratuita (ej. Unity Relay / Photon).
- [ ] Programar el Lobby con sistema de "Room Code" (Código de 4 letras para unirse).
- [ ] Programar el Modo **"Agencias Rivales"** (Carrera de velocidad sincrónica).
- [ ] Programar el Modo **"Policía Corrupto"** (Distribución de pistas asimétrica a diferentes clientes).

## Fase 5: Monetización (Freemium), Pulido y Lanzamiento
- [ ] Implementar In-App Purchases (Pago único de 4.99€ para desbloquear Premium).
- [ ] Configurar los muros de pago (Campaña de 20 niveles y Caso Diario = Gratis; Roguelite Infinito y Multijugador = Premium).
- [ ] Game Feel y ASMR: Pulir los efectos de sonido táctiles (papel, sellos, piezas de madera).
- [ ] Compilación final (Builds) para Itch.io / Steam / Stores móviles.
