# 🗺️ Roadmap y Tracker de Proyecto: NIGMA (Visual Murdoku)
*Documento de Gestión de Proyecto (PM) - Actualizado: 01/08/2026*

Este documento actúa como la hoja de ruta central para el desarrollo de Nigma, el juego de deducción lógica visual.

## Fase 1: Pre-Producción y Diseño (En Curso)
- [x] Brainstorming y Selección de Core Concept (Visual Murdoku + Balatro)
- [x] Definición de Nombre Oficial (Nigma)
- [x] Creación del Documento Maestro (GDD) y purgado de conceptos obsoletos
- [ ] **Diseño Lógico del Puzzle:** Determinar matemáticamente cómo generar y validar las pistas visuales (las reglas de "No adyacente", "Misma fila", etc.) para que los puzzles de 4x4 tengan solución única.
- [ ] **Diseño Visual de Interfaz (Mockup):** Diseñar conceptualmente cómo se ve la pantalla principal (Tablero, Pistas Visuales en un lateral, Maletín de Herramientas debajo) sin texto.

## Fase 2: Prototipo Técnico (Core Loop en Unity)
- [ ] Configuración inicial de Unity (Repositorio, .gitignore, Settings 2D vertical).
- [ ] Crear el "Grid Manager" (Lógica subyacente de casillas 4x4).
- [ ] Crear el sistema Drag & Drop de las fichas (Sospechosos/Iconos).
- [ ] Programar un intérprete visual de pistas (que el juego sepa evaluar si el jugador ha puesto al Perro al lado del Gato).
- [ ] Programar 1 caso de prueba "Hardcodeado" (fijo) para comprobar que el loop es divertido.

## Fase 3: Vertical Slice (Prototipo Jugable Completo)
- [ ] Sistema de Generación Procedural de Reglas (El cerebro del juego que crea casos matemáticamente perfectos).
- [ ] Interfaz de Usuario (UI) para el Maletín y los "Jokers/Herramientas" (que multiplican puntuación o revelan pistas).
- [ ] Sistema de Puntuación estilo Balatro (Chips x Mult) basado en el orden y velocidad de resolución.
- [ ] Sistema de Candado (Caja Fuerte Meta-puzzle) integrado.
- [ ] Arte temporal estilo "Cozy Mystery" (Misterio relajante, jazz, lluvia).

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
