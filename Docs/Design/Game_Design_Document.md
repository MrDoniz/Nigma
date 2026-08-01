# 🕵️‍♂️ Documento de Diseño del Juego (GDD)

## 1. Resumen del Juego
**Título Provisional:** *Nigma*
**Género:** Puzzle de Deducción / Roguelite Casual
**Plataforma:** PC / Móvil (Perfecto para pantallas táctiles y ratón)
**Público Objetivo:** Todos los públicos. Diseñado con la filosofía "fácil de aprender, difícil de dominar". Permite desde partidas diarias de 5 minutos en el bus, hasta sesiones largas de optimización de puntuaciones.
**Modelo:** Freemium (Caso Diario y Campaña Tutorial gratis. Desbloqueo Premium por 4.99€ para Roguelite infinito y Multijugador).

## 2. Concepto Principal (Core Concept)
> Eres un detective que resuelve crímenes autogenerados en un tablero (Grid). Cada partida es un caso distinto. No hay muerte permanente frustrante; el toque roguelike se centra en que cada nivel es independiente, con generación procedural, y tu éxito te permite desbloquear mejores herramientas y cosméticos para tu agencia de detectives.

El juego toma la estructura de **Línea de Visión (Puzles ópticos)**, añade las **sinergias explosivas y aleatoriedad de Balatro** (combinando herramientas y evidencias para multiplicar tu eficiencia), y se prepara para un futuro **social de engaño y negociación** inspirado en juegos de mesa.

## 3. Mecánicas Principales (Core Mechanics)

### A. El Tablero Físico (Líneas de Visión)
- El crimen se resuelve en un grid (ej. 4x4) usando lógica espacial y óptica, **sin pistas de texto**.
- Las reglas de lógica son visuales y físicas. Para resolver el puzle, conectas líneas de visión entre personajes.
  - *Ejemplo visual:* `[Ojo Abuela] ➡️ [Arma]` (La Abuela pudo ver el Arma en línea recta).
  - *Ejemplo visual:* `[Perro] ❌👁️ [Gato]` (El Perro NO pudo ver al Gato, por lo que debe haber un muro entre ellos).
- Para resolverlo, el jugador debe colocar no solo Sospechosos, sino **Objetos Físicos** en el grid: Muros (que bloquean la visión) y Espejos (que doblan la visión 90º).
- **Lenguaje Universal:** Al no haber texto ni reglas lógicas abstractas de "Murdoku", el puzle se vuelve físico, inmediato y con una profundidad increíble.

### B. El Toque Balatro (Aleatoriedad y Sinergias sin Cartas)
En lugar de un mazo de cartas, tienes un **Maletín de Detective**. Durante el caso, obtienes "Fichas de Evidencia" y "Herramientas".
- **Herramientas (Los Jokers):** Objetos pasivos que alteran las reglas a tu favor durante ese caso. 
  *Ejemplo:* "Lupa antigua" (Toda pista relacionada con la cocina te da el doble de puntos de deducción).
- **Evidencias (Sinergias):** Puedes combinar evidencias obtenidas para forzar el grid. 
  *Ejemplo:* Si combinas la ficha "Huella Dactilar" + "Testigo Ocular", la sinergia revela inmediatamente al culpable de una fila entera. Construir estas sinergias es lo que permite al jugador avanzado "romper" el juego y hacer puntuaciones masivas (Chips x Mult).
- **El Toque "Escape Room" (Meta-Puzzles):** Ocasionalmente encontrarás una "Caja Fuerte" que contiene una Herramienta muy potente, bloqueada por un candado de 3 dígitos. La combinación no está escondida por el escenario, sino que se deduce observando el propio tablero lógico que estás resolviendo (ej. "Nº de sospechosos en las esquinas" - "Casillas vacías en la fila superior"). Añade el subidón de un escape room sin complicar el desarrollo (es solo una interfaz UI de candado).

### C. El Toque Roguelite (La Estructura de la Partida)
- **El Caso (El Run):** Consiste en interrogar sospechosos en varias "Salas" o niveles consecutivos. Cada sala resuelta te da a elegir una nueva Herramienta o Evidencia.
- **Progresión entre Salas:** Vas construyendo tu "motor" de deducción (tu maletín).
- **Progresión Global:** Los puntos obtenidos sirven para mejorar tu Agencia de Detectives (cosméticos, nuevos modos de juego, o herramientas que pueden aparecer en futuros casos).

## 4. Modos de Juego y Progresión
- **El Caso del Día (Gratis):** Un puzzle idéntico para todo el mundo que cambia cada 24 horas. Ideal para enganchar al usuario casual.
- **Campaña Básica (Gratis):** 15-20 niveles que sirven de tutorial para introducir Muros, Espejos y mecánicas básicas de Línea de Visión.
- **Modo Roguelite Infinito (Premium):** Generación procedural infinita. Juegas *Runs* consecutivos comprando herramientas para tu maletín. Si fallas, empiezas de cero.

## 5. El Multijugador (Party Game / Código de Sala)
Para maximizar la viralidad y ahorrar costes de servidor, el multijugador funciona con un sistema de "Código de Sala" (estilo *Among Us* o *Jackbox*).
- **El Sistema "Friend Pass":** ¡Solo el Anfitrión necesita haber pagado el desbloqueo Premium! El Anfitrión crea la sala y recibe un código (ej. ABCD). Sus amigos, con la versión gratuita de la app, introducen el código y juegan gratis.
- **Agencias Rivales (Velocidad):** 2 a 4 jugadores reciben el mismo grid. El primero en cuadrar las líneas de visión gana. Los fallos penalizan con 10 segundos.
- **Policía Corrupto (Roles Ocultos):** El juego genera un grid gigante. **Las pistas se reparten entre los móviles de los jugadores**. Tenéis que hablar para resolverlo en equipo. El giro: Uno de los amigos es el "Corrupto" (tiene pistas falsas) y debe intentar sabotear el puzzle convenciendo a los demás de colocar mal las piezas sin ser descubierto.

## 6. Estilo Visual y Sonoro
- **Estilo Visual:** Isométrico o Top-Down 2D muy pulido. Paleta de colores cálida (madera, lámparas de escritorio, papel antiguo). Los personajes son carismáticos y exagerados.
- **Interfaz (UI):** Táctil, de arrastrar y soltar (Drag & Drop). Colocar un sospechoso en el tablero debe sentirse satisfactorio, con buenos efectos de sonido de papel y sellos.
- **Sonido:** Jazz suave, lo-fi de misterio, sonido de lluvia de fondo. Muy relajante para pensar.
