# 🕵️‍♂️ DOCUMENTO MAESTRO: NIGMA
*Fecha de actualización: 31 de Julio de 2026*
*Directorio de guardado: `C:\Users\Danie\Documents\GitHub\nigma\`*

Este documento consolida y detalla absolutamente todas las ideas clave, mecánicas y conceptos que hemos ido puliendo durante nuestras sesiones de diseño. Sirve como la **Biblia de Diseño (Design Bible)** del proyecto para que cualquier programación futura se base en estas reglas exactas.

---

## 1. VISIÓN GENERAL
- **Presupuesto y Equipo:** 0€, 2 personas (Tú y yo). Herramientas gratuitas (Unity, Blender, etc.).
- **Plataformas:** PC / Móvil (pensado para ser táctil y casual, pero con profundidad).
- **El Concepto Fusión:** Es un juego de deducción lógica estilo *Murdoku* (grid), con la locura de combos y sinergias matemáticas de *Balatro* (pero sin usar cartas), una progresión de niveles limpia sin muerte permanente severa (*Roguelite*), toques de misterio de un *Escape Room*, y diseñado para en el futuro escalar a una experiencia social de engaño estilo *Mario Party / Among Us*.
- **La Filosofía de Diseño:** "Simpleza técnica y jugable". Todo el juego debe ocurrir mayormente en menús 2D limpios, arrastrando elementos, con énfasis en la lógica y no en motores de físicas ni entornos 3D complejos.

---

## 2. MECÁNICA CENTRAL: LÍNEA DE VISIÓN (Puzle Óptico-Lógico)
Eres un detective frente a un tablero de 4x4 (o 3x3 en niveles iniciales). Hemos eliminado las reglas tradicionales y limitantes de los sudokus/murdokus. Aquí la deducción es física y espacial: **La Línea de Visión**.
1. **Los Elementos:** Tienes Personajes (víctimas, asesinos, testigos) y Objetos Físicos (Muros, Espejos, Cámaras).
2. **El Objetivo:** Colocar todo en el tablero para que las "Líneas de Visión" cuadren con las declaraciones (sin leer texto, todo visual).
3. **Las Mecánicas Ópticas:**
   - *Visión Directa:* `[Testigo] ➡️ [Arma]` (Deben estar en línea recta sin obstáculos).
   - *Bloqueo:* `[Viuda] ❌👁️ [Mayordomo]` (Debe haber un Muro entre ellos para bloquear la línea de visión).
   - *Refracción:* `[Perro] ➡️ [Espejo] ⬇️ [Arma]` (Los espejos doblan la visión 90º).
Este giro transforma el juego de un simple sudoku a un puzle espacial infinito, donde rebotar la visión y bloquear líneas crea una profundidad jugable inmensa.

---

## 3. EL TOQUE "BALATRO": SINERGIAS SIN CARTAS
Para eliminar el uso de "cartas" pero mantener la aleatoriedad matemática y las sinergias adictivas:
1. **El Maletín del Detective:** Aquí guardas tus objetos. No hay mazos.
2. **Las Evidencias (Pistas):** Objetos de un solo uso que encuentras o ganas al resolver un caso rápido.
3. **Las Herramientas (Los Jokers):** Objetos pasivos que modifican radicalmente cómo puntúas o cómo resuelves el puzzle.
   - *Ejemplo de Herramienta Pasiva:* "Lupa Antigua" (Toda pista relacionada con la cocina te da x2 de puntuación).
4. **La Sinergia (El Combo):** El núcleo adictivo del juego. Puedes arrastrar una Evidencia sobre una Herramienta para "forzar" el tablero.
   - *Ejemplo de Combo:* Juntar la evidencia "Huella Dactilar" con la herramienta "Testigo Ocular" revela automáticamente a un asesino, ahorrándote 5 minutos de deducción lógica y multiplicando tus puntos. ¡Romper el juego usando tu ingenio está permitido y recompensado!

---

## 4. EL TOQUE "ESCAPE ROOM": META-PUZZLES
Para evitar complicar el juego con entornos 3D, el "toque" de escape room ocurre directamente en la interfaz 2D:
- **La Caja Fuerte:** De vez en cuando, el juego te da una Herramienta legendaria, pero está dentro de una caja fuerte virtual con un candado de 3 o 4 dígitos.
- **La Solución:** Para obtener la combinación, debes observar tu tablero actual de deducción de una forma distinta. 
  - *Ejemplo:* La pista del candado dice: "A - B = Código". Donde A es "Número de personas en las esquinas" y B es "Número de armas descubiertas". 
- **Impacto:** Da la inmersión e ilusión de un Escape Room físico usando solo lógica, números y UI, sin coste de programación extra.

---

## 5. ESTRUCTURA DE LA PARTIDA Y PROGRESIÓN
- **Modo Freemium (Tutorial / Diario):** El juego base es gratuito. Incluye "El Caso del Día" (el mismo tablero para todos) y una Campaña Básica de 20 niveles "Hardcodeados" (hechos a mano para garantizar una curva de dificultad perfecta y enseñar las mecánicas ópticas de forma suave).
- **El Run Infinito (Premium):** Al pagar 4.99€, desbloqueas el núcleo duro (Roguelite). Resuelves salas generadas proceduralmente (mutaciones de tableros base para asegurar solución única), eligiendo herramientas para potenciar tu Maletín.

## 6. EL MODO MULTIJUGADOR (Código de Sala & Friend Pass)
Para jugar con amigos usamos un sistema de "Room Code" (estilo Among Us), muy barato de mantener en servidores. 
- **El Sistema "Friend Pass" (Viralidad):** Solo el Anfitrión necesita haber pagado el Premium de 4.99€. Los amigos pueden bajar la app gratuita, meter el código de sala y jugar gratis al multijugador.
1. **Agencias Rivales (Velocidad):** Varios jugadores reciben el mismo grid simultáneamente. El objetivo es resolverlo lo más rápido posible. Los errores de colocación penalizan.
2. **El Policía Corrupto (Party Game - Roles Ocultos):** El juego estrella para reuniones. Se genera un tablero gigante en los móviles de los 5 amigos. **Las pistas lógicas se reparten**. Tenéis que hablar en voz alta para resolver el caso juntos. Pero cuidado: a un jugador se le asignan pistas falsas en secreto (El Corrupto), y su objetivo es convencer sutilmente a los demás de colocar mal las piezas sin que sospechen de él.

---

## 8. DIRECCIÓN DE ARTE Y SONIDO
- **Cámara:** 2D, Vista Isométrica o Cenital (Top-Down).
- **Estética "Cozy Mystery" (Misterio Acogedor):** Colores cálidos de madera, flexos, lluvia cayendo en la ventana, tazas de café, fichas de sospechosos con fotos polaroid o dibujos caricaturescos limpios.
- **Sonido (Sensación Táctil - ASMR):** Poner una ficha en el tablero tiene que sonar a un golpe de sello en papel duro ("¡CLAC!"). Acompañado de música Jazz suave de fondo o Lofi Mystery.

---
*Nota de Desarrollo: Cualquier idea nueva, mecánica o recorte, lo iremos registrando y comentando en este documento o en archivos similares dentro de esta carpeta del repositorio.*
