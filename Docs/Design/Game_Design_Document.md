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

### A. El Tablero Físico (La Escena Corrupta)
El bucle de juego principal transforma al jugador en un investigador activo:
- **La Llegada:** El nivel empieza con el grid (ej. 4x4) ya amueblado y con personajes colocados. Hay elementos estructurales FIJOS (paredes, pilares, ventanas) que no puedes mover. Esta disposición representa la "versión de los testimonios", pero está corrupta (ej. alguien afirma haber visto un mueble, pero hay un pilar en medio).
- **La Reconstrucción:** Tienes muebles infinitos (sillas, sillones, mesas altas, espejos). Debes arrastrar y mover este mobiliario por el tablero para que las líneas de visión físicas (óptica espacial) cuadren con la lógica de los testimonios.
- **La Resolución:** El juego no te dice cuándo terminas. Tú ganas la partida cuando, al recomponer la escena, logras identificar la casilla exacta del crimen (Víctima) y al único personaje cuyo testimonio era físicamente imposible (El Asesino).

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
- **Estética Visual (3D):** Isométrico 3D estilo "Maqueta de Juguete" (Toy-box / Claymation). Los personajes y muebles son modelos 3D con **dirección de mirada**. Es muy amigable y colorido. Las temáticas son **abiertas y variadas**: habrá niveles con personas "normales" (estilo Murdoku clásico), otros con animales, fantasía, etc.
- **Interfaz y Animaciones (Game Feel):** Táctil, de arrastrar y soltar (Drag & Drop). Extremadamente "jugoso". Seleccionar una pieza provoca un *pop* de rebote. Arrastrar a un personaje debe tener una animación cómica donde **lo levantas por la cabeza/cuello (scruff)** y su cuerpo/piernas cuelgan y se balancean con la inercia del movimiento.
- **Sonido:** Jazz suave, lo-fi de misterio, sonido de lluvia. Soltar una pieza suena a un satisfactorio golpe de madera ("¡CLAC!").
