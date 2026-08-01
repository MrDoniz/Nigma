# 💡 20 Ideas de Juego — Brainstorm Inicial

> **Referencia de influencias:**
> - 🔍 **Murdoku** → Grid de lógica + deducción + misterio narrativo
> - 🃏 **Balatro** → Sinergia de cartas, multiplicadores, construcción de "máquina" de puntuación
> - ⚰️ **Roguelike** → Runs procedurales, muerte permanente, desbloqueos meta-progresivos
> - 🎲 **Mario Party** → Mini-juegos, social, eventos aleatorios, adaptable a grupos

---

## 🟣 IDEA 1 — *CrimeRun*
### "Soy un detective que cada vez llega tarde a la escena del crimen"

**Concepto base:**
Un roguelike de deducción donde cada "run" eres un detective diferente que investiga un crimen generado proceduralmente. El crimen se resuelve mediante un grid estilo Murdoku (quién estaba dónde, cuándo y con qué), pero el mapa del crimen cambia en cada partida.

**Mecánicas clave:**
- El grid de lógica ES el campo de batalla. Colocas sospechosos en casillas.
- Tienes **Pistas** como cartas (estilo Balatro): cada pista es una carta que al "jugarla" elimina una posición del grid o te da info.
- Las pistas tienen **sinergias entre sí** (dos pistas de "localización" juntas revelan una casilla exacta, una pista "coartada" elimina una fila entera).
- Al final de cada escenario (como las Antres de Balatro), eliges una **habilidad permanente de detective**: "olfato de sangre" (revela siempre al culpable en la zona norte), "memoria fotográfica" (guarda el grid parcial del run anterior), etc.
- Si fallas el caso, el verdadero criminal huye y se convierte en un **boss del siguiente escenario**.

**Adaptabilidad:**
- Fácil: Grid 3×3, pistas visuales con iconos.
- Difícil: Grid 6×6, pistas con lógica compuesta, tiempo límite.

**Estética:** Noir oscuro, cámara cenital, paleta monocromática con detalles en rojo.

---

## 🟢 IDEA 2 — *Alchemist's Deck*
### "Cada ingrediente es una carta, cada poción es una combinación explosiva"

**Concepto base:**
Alquimista roguelike que usa un sistema de cartas estilo Balatro, donde las cartas son **ingredientes** y el objetivo es crear **pociones** que generen la mayor puntuación de "poder mágico". Cada run recorres una mazmorra procedural.

**Mecánicas clave:**
- Juegas N cartas de ingrediente para formar **recetas** (como manos de poker: "doble fuego", "trío de hierbas", "flush de agua").
- Los **artefactos** (Jokers de Balatro) son objetos mágicos que modifican tus multiplicadores: el Caldero Oxidado +2 Mult por cada ingrediente de tierra, el Orbe Etéreo ×3 si la receta tiene exactamente 5 cartas.
- Cada zona de la mazmorra tiene un **boss mágico** que requiere cierto tipo de poción para ser derrotado. No puedes atacar con magia de fuego al Dragón de Hielo si no tienes al menos 3 cartas de hielo.
- Grid de deducción al entrar a una habitación: un pequeño puzzle que si lo resuelves te da una **pista de qué ingrediente necesitarás en el boss**.

**Adaptabilidad:**
- Sistema de dificultad "temperatura del caldero": baja = más tiempo, pistas visuales; alta = ingredientes ocultos, recetas cambian a mitad del run.

**Estética:** Fantasía colorida, caricaturesco, estilo ilustración de libro antiguo.

---

## 🔵 IDEA 3 — *Sudoku Heist*
### "Planificamos el robo. El grid es el plano del edificio"

**Concepto base:**
Un juego de ladrones donde el "robo" se planifica mediante un puzzle de deducción lógica. El edificio es un grid donde debes colocar a tus ladrones en posiciones sin que ninguno coincida con las cámaras o guardias. Es un Murdoku invertido: tú eres el criminal.

**Mecánicas clave:**
- Cada run = un edificio nuevo generado proceduralmente.
- Tienes un **mazo de habilidades** (Balatro): cartas como "hackeo", "disfraz", "distracción". Al jugar combinaciones, desbloqueas casillas del grid o mueves guardias.
- Sinergias: "hackeo" + "cámara ciega" = toda una fila sin seguridad.
- Al completar el robo, ganas dinero para comprar mejores cartas en la "guarida" entre misiones.
- Modo multijugador asíncrono estilo Mario Party: cada jugador es un ladrón diferente y compiten por conseguir el mayor botín del mismo edificio.

**Adaptabilidad:**
- Modo cooperativo: todos los jugadores aportan cartas para resolver el grid juntos.
- Modo competitivo: turnos alternos, el que resuelve primero se lleva más botín.

**Estética:** Azul neón sobre negro, estética hacker retro, personajes pixel art caricaturescos.

---

## 🟡 IDEA 4 — *Cosmos Deduction*
### "El universo tiene un orden. Tú debes descubrirlo"

**Concepto base:**
Un juego de lógica espacial donde debes colocar **planetas, estrellas y agujeros negros** en un grid galáctico siguiendo las "leyes cósmicas" (las reglas del puzzle). Un roguelike donde cada sector del universo es un nuevo puzzle.

**Mecánicas clave:**
- Las "leyes cósmicas" son las pistas: "El agujero negro nunca está al norte de una estrella", "Los planetas habitados siempre tienen una luna adyacente".
- Tienes **cartas de tecnología** (Balatro): telescopios, sondas, mapas estelares. Cada carta te da información o te permite mover/colocar elementos en el grid.
- Al resolver un sector, ganas **recursos** para mejorar tu nave y desbloquear habilidades pasivas para runs futuras (meta-progresión).
- Evento aleatorio estilo Mario Party: "Tormenta de meteoritos" — algunas casillas del grid quedan bloqueadas aleatoriamente.

**Adaptabilidad:**
- Modo familia: grids 4×4, pistas ilustradas, sin tiempo.
- Modo experto: grids 8×8, leyes contradictorias que requieren razonamiento por eliminación avanzado.

**Estética:** Watercolor espacial, colores pastel cósmicos, soundtrack lo-fi ambiental.

---

## 🔴 IDEA 5 — *Spell Grid*
### "Cada hechizo ocupa una posición en el tablero mágico"

**Concepto base:**
Un mago que debe combinar hechizos en un grid para maximizar su poder antes de enfrentarse a un jefe. Las reglas de colocación en el grid son las de deducción (no puedes poner dos hechizos del mismo elemento en la misma fila), y el sistema de puntuación es Chips×Mult de Balatro.

**Mecánicas clave:**
- Tienes un grid 4×4. Debes colocar tus hechizos siguiendo las restricciones (Murdoku rules).
- Una vez colocados, el grid "se activa" y calcula tu poder: Chips (fuerza base de los hechizos) × Mult (bonus de posición y adyacencia).
- Los **glifos** (Jokers): modificadores que cambias entre rounds. "Glifo del Vacío" → ×2 si el centro del grid está vacío. "Glifo del Sol" → +10 Chips por cada hechizo de fuego en la fila superior.
- Cada dungeon tiene 3 floors con un boss al final. Permadeath en modo difícil.

**Adaptabilidad:**
- Modo práctica: ves los valores antes de colocar.
- Modo experto: valores ocultos hasta activar el grid.

**Estética:** Fantástico-medieval, runestones grabadas, paleta dorado y azul profundo.

---

## 🟠 IDEA 6 — *Murder at the Table* (Mario Party × Murdoku)
### "Hasta 4 jugadores. Solo uno es el asesino. Y ninguno lo sabe… todavía"

**Concepto base:**
Juego de mesa digital multijugador estilo Mario Party donde todos los jugadores recorren un tablero. Al caer en ciertas casillas, se activa un minijuego de deducción: un grid Murdoku que todos resuelven a la vez. El ganador del puzzle obtiene ventajas y **pistas** sobre quién es el asesino entre los jugadores (rol oculto).

**Mecánicas clave:**
- Un jugador es elegido secretamente como "el asesino" al inicio.
- Durante el tablero, el asesino puede **sabotear** los puzzles de los demás (cambia pistas, bloquea casillas).
- Los investigadores acumulan pistas de los minijuegos para hacer una acusación al final de la partida.
- Cada vuelta al tablero es más difícil: los puzzles escalan, las pistas del asesino son más ambiguas.
- El asesino gana si llega al final sin ser descubierto. Los investigadores ganan si lo acusan correctamente antes.

**Adaptabilidad:**
- 2-6 jugadores (local o en línea).
- Modo solitario: tú eres siempre el investigador, la IA es el asesino.

**Estética:** Mansión victoriana, colores cálidos sepia, animaciones de cartas exageradas y teatrales.

---

## 🟣 IDEA 7 — *Rune Rogue*
### "Las runas antiguas tienen un orden. Descúbrelo antes de morir"

**Concepto base:**
Un roguelike donde cada nivel es un templo antiguo con un **grid de runas** que debes descifrar (Murdoku) para abrir la puerta siguiente. Mientras resuelves el puzzle, criaturas se acercan. Tienes un mazo de **cartas de acción** (Balatro) para ralentizar las criaturas o ganar más tiempo.

**Mecánicas clave:**
- El puzzle (grid de runas) y el combate ocurren simultáneamente en tiempo real pausable.
- Cartas de acción: "Barrera de Luz" (congela una fila de enemigos 3 turnos), "Runa Revelada" (muestra una casilla del puzzle), "Explosión Arcana" (daña todos los enemigos pero borra 2 casillas ya resueltas del grid).
- Las sinergias de cartas también aplican: "Barrera" + "Revelada" jugadas juntas → revela toda una columna.
- Al completar el puzzle, matas a todos los enemigos restantes en la sala. Al completar el dungeon, ganas una reliquia permanente.

**Adaptabilidad:**
- Modo tranquilo: sin tiempo, resuelves el puzzle a tu ritmo.
- Modo caos: los enemigos avanzan rápido, tú decides cuándo pausar.

**Estética:** Pixel art oscuro, grietas de luz entre las runas, partículas mágicas flotantes.

---

## 🟢 IDEA 8 — *Gastro Logic*
### "Eres chef. Tu cocina es un grid. Tu receta es un puzzle"

**Concepto base:**
Un juego de cocina donde la cocina es un grid y cada plato se prepara colocando ingredientes en posiciones específicas siguiendo restricciones de sabor (ácido no va con ácido en la misma fila, etc.). Balatro meets Gordon Ramsay.

**Mecánicas clave:**
- Grid de cocina = puzzle de deducción. Los "clientes" te dan pistas sobre qué quieren: "El postre no puede estar junto al picante", "El umami debe estar en la esquina".
- Tus cartas son **técnicas culinarias**: flambeado (×2 al plato adyacente), marinado (+5 sabor a todo el row), presentación perfecta (si el grid está completo sin errores, +50% de propina).
- La propina = tu puntuación (Chips×Mult). Necesitas superar la "crítica del chef" para avanzar al siguiente restaurante.
- En modo Mario Party: 4 jugadores cocinan en el mismo grid, pero cada uno controla su sección. Si un jugador falla, contamina las casillas adyacentes del resto.

**Adaptabilidad:**
- Recetas de práctica para principiantes, 8 estrellas Michelin para expertos.

**Estética:** Top-down colorido, animaciones jugosas de comida, estilo Studio Ghibli (Mononoke meets Ratatouille).

---

## 🔵 IDEA 9 — *Architect of Lies*
### "Construyes ciudades. Pero cada ciudadano tiene un secreto"

**Concepto base:**
Construyes una ciudad colocando edificios en un grid (restricciones de zonificación = Murdoku rules). Cada edificio tiene un "habitante" con un secreto. Al completar una zona, se revela una historia: un misterio que se va desenredando a lo largo del roguelike.

**Mecánicas clave:**
- Cada edificio es una carta (Balatro): tiene un valor base (población) y un multiplicador (comercio, cultura).
- Las restricciones del grid: "La fábrica nunca puede estar al lado del parque", "Solo puede haber un banco por fila".
- Al completar cada distrito, se revela un fragmento de historia (la narrativa roguelike).
- Eventos aleatorios: incendios, festivales, escándalos que reorganizan el grid a mitad del puzzle.
- Meta-progresión: desbloqueas nuevos tipos de edificios para runs futuras.

**Adaptabilidad:**
- Modo relajado: sin eventos negativos, construyes a tu ritmo.
- Modo caótico: eventos frecuentes, ciudadanos con demandas irracionales.

**Estética:** Isométrico pastel, estilo acuarela, personajes con cabezas grandes y expresivas.

---

## 🟡 IDEA 10 — *HexWitch*
### "El aquelarre tiene reglas. Las brujas no"

**Concepto base:**
Un grid hexagonal (en lugar de cuadrado) donde colocas brujas, hechizos y criaturas siguiendo reglas de covens (grupos mágicos). Las conexiones hexagonales crean sinergias únicas (Balatro) porque cada casilla tiene 6 vecinos en lugar de 4.

**Mecánicas clave:**
- Las pistas no son lineales (norte/sur) sino direccionales en hexágono: "La bruja de fuego nunca puede estar al noroeste de la de agua".
- Sinergias de adyacencia: si 3 brujas del mismo tipo forman un triángulo hexagonal, su poder se ×3.
- Cartas especiales: **Pactos** (Jokers) que modifican las reglas del grid para esa run: "Los covens de 4 valen el doble", "Las brujas solitarias son inmunes al daño de pistas incorrectas".
- Roguelike: cada aquelarre (nivel) más difícil, con más restricciones y brujas hostiles que cambian de posición.

**Adaptabilidad:**
- Grid hexagonal pequeño (7 casillas) para principiantes, grande (37 casillas) para expertos.

**Estética:** Gótico colorido, flora oscura brillante, paleta violeta y verde lima.

---

## 🔴 IDEA 11 — *Constellation Run*
### "Las estrellas cuentan historias. Tú las conectas"

**Concepto base:**
Cada run es una constelación diferente que debes "descubrir" conectando estrellas en un grid. Las restricciones son las leyendas mitológicas (pistas narrativas) y las mecánicas de puntuación son las formaciones estelares (manos de poker espaciales: Nebulosa, Supernova, Galaxia).

**Mecánicas clave:**
- El grid de estrellas es el puzzle. Las pistas vienen dadas por mitos narrados (texto + voz).
- Cuando conectas estrellas, formas "manos": tres en línea = constelación menor, cinco en L = constelación mayor. Cada mano tiene Chips × Mult.
- **Cartas de Mito** (Jokers): "La Lira de Orfeo" → ×2 si tu constelación tiene exactamente 7 estrellas. "El Escudo de Perseo" → +20 Chips si no cometiste ningún error en ese nivel.
- Roguelike: recorres 12 constelaciones zodiacales. Si completas las 12, el universo "renace" en Nueva Partida+ con reglas más complejas.

**Adaptabilidad:**
- Modo historia: pistas narrativas largas y detalladas.
- Modo speed: solo ves el grid, las pistas son brevísimas, cronómetro activo.

**Estética:** Oscuro con partículas estrelladas brillantes, música orquestal majestuosa.

---

## 🟠 IDEA 12 — *Circus Rogue*
### "Cada acto del circo es un puzzle. El público no perdona"

**Concepto base:**
Eres el director de un circo ambulante roguelike. Cada show debes organizar los actos en un grid de escenario (restricciones de seguridad: el malabarista no puede estar en la misma fila que el lanzallamas). La puntuación del público = Chips × Mult de Balatro.

**Mecánicas clave:**
- Las cartas son **artistas**: cada artista tiene un valor base y sinergias con otros artistas.
- Sinergias: Acróbata + Trampolín = ×2. Mago + Conejo = +15 Chips. Payaso junto a cualquier artista = −5 Mult (el público se ríe, no aplaude).
- El grid de restricciones garantiza que el show sea "seguro" pero tú decides cómo maximizar el aplauso.
- Eventos estilo Mario Party entre shows: "Lluvia torrencial" (el show de afuera se cancela, pierdes un acto), "VIP entre el público" (el siguiente show vale doble).
- Meta-progresión: contratas artistas nuevos, mejoras tu carpa, desbloqueas ciudades.

**Adaptabilidad:**
- Modo niños: artistas simpáticos, sin restricciones complejas, puntuación siempre positiva.
- Modo director: restricciones duras, artistas con egos (condiciones especiales para jugarlos).

**Estética:** Art nouveau vintage, colores ámbar y rojo, tipografía de cartel antiguo.

---

## 🟣 IDEA 13 — *BioGrid*
### "El ADN tiene un código. Tú lo reescribes"

**Concepto base:**
Eres un científico en un laboratorio futurista. Cada run editas una secuencia de ADN usando un grid de bases nitrogenadas (A, T, G, C). Las restricciones son las reglas de apareamiento de bases (Murdoku biológico). Las combinaciones generan organismos con distintos poderes.

**Mecánicas clave:**
- El grid de bases = el puzzle de deducción. Las pistas son observaciones científicas: "El gen de resistencia nunca está en la misma hebra que el gen de toxicidad".
- Las **mutaciones** son los Jokers: modificadores de run que cambian el comportamiento del grid: "Mutación Jungla" → A y G pueden estar en la misma fila si hay una C entre ellas.
- El organismo resultante ataca al boss de cada nivel con el poder basado en tu puntuación Chips × Mult.
- Narrativa roguelike: cada run avanzas en el árbol evolutivo hasta crear el organismo definitivo.

**Adaptabilidad:**
- Modo educativo (perfecto para niños mayores): aprenden bases nitrogenadas reales de forma lúdica.
- Modo experto: secuencias de 20 bases con restricciones complejas.

**Estética:** Bioluminiscente, verde neón sobre negro, animaciones de células fluyendo.

---

## 🟢 IDEA 14 — *Jazz Collective*
### "La música es un grid. El jazz no tiene reglas... excepto las tuyas"

**Concepto base:**
Compones jazz colocando instrumentos y notas en un grid de compases. Las restricciones armónicas son las reglas del puzzle (un instrumento de viento nunca puede tocar en el mismo compás que otro viento sin percusión de soporte). La partitura resultante se toca en tiempo real y la puntuación es la reacción del público.

**Mecánicas clave:**
- Las cartas son **músicos**: cada músico tiene un rango de notas (Chips) y un estilo (Mult).
- Sinergias: Contrabajo + Batería = "ritmo base" (+20 Chips a todo). Piano + Trompeta = "dueto clásico" (×2 si están en filas adyacentes).
- Los **Jokers** son **géneros musicales**: "Bebop" (las sinergias de velocidad tienen ×3), "Bossa Nova" (las notas de viento valen el doble si el grid está completo).
- El grid se "toca" en tiempo real después de resolverlo: escuchas tu composición y ves la reacción del público.
- Roguelike: recorres diferentes clubs de jazz, cada uno con restricciones armónicas distintas.

**Adaptabilidad:**
- Modo melódico: el juego te sugiere qué músicos van bien juntos.
- Modo improvisación: ninguna sugerencia, solo las restricciones del grid.

**Estética:** Noir jazzístico, humo azul, luces amarillas cálidas, siluetas de músicos.

---

## 🔵 IDEA 15 — *Garden of Clues*
### "El jardín esconde la verdad. Las flores la cuentan"

**Concepto base:**
Un juego de puzzles relajante con alma roguelike. Eres un jardinero-detective que debe plantar flores en un jardín (grid) siguiendo las preferencias de las flores (Murdoku: las rosas no pueden estar junto a las orquídeas). Pero cada jardín esconde un misterio que se revela al completarlo.

**Mecánicas clave:**
- Muy accesible: las restricciones se explican con diálogos de las propias flores (personificadas, con humor).
- Las cartas son **herramientas de jardín**: "Abono Mágico" (+10 Chips a la fila), "Regadera Encantada" (×2 si completas el jardín sin errores), "Pala Sabia" (revela una casilla).
- La historia se entrelaza con cada jardín: cada jardín completado revela un capítulo de una historia de misterio cozy.
- Meta-progresión: desbloqueas nuevas flores y herramientas para runs futuras.
- Modo multiplayer: 2-4 jugadores tienen secciones del jardín conectadas. Lo que uno planta afecta las restricciones del vecino.

**Adaptabilidad:**
- El más accesible de todos. Perfecto para todas las edades.
- Modo difícil: flores con condiciones especiales ("la flor de luna solo puede plantarse en número par de columna").

**Estética:** Acuarela suave, colores pasteles saturados, vibe cottagecore, música de caja de música.

---

## 🟡 IDEA 16 — *Mech Assembly*
### "Construyes un robot. El grid es su cuerpo. Las piezas son sus poderes"

**Concepto base:**
Eres un ingeniero en un mundo post-apocalíptico. Cada run ensambles un mech diferente colocando piezas en un grid-cuerpo (torso, brazos, piernas). Las restricciones de ensamblaje son el puzzle (un motor de plasma no puede estar adyacente a un depósito de agua). La puntuación de batalla es Chips × Mult.

**Mecánicas clave:**
- Cada pieza del mech es una carta: tiene stats base (Chips) y habilidades especiales (Mult triggers).
- Sinergias: Motor Turbo + Booster = "velocidad máxima" (×3 Mult en el primer turno de batalla). Escudo + Regenerador = "armadura viva" (+5 Chips por turno).
- El grid-cuerpo también es el puzzle: las restricciones de ensamblaje garantizan que el mech sea "funcional" pero tú optimizas para máximo poder.
- Roguelike: recorres una wasteland con mechs enemigos. Cada victoria te da chatarra para mejorar piezas.
- Evento estilo Mario Party: "Lluvia ácida" → todas las piezas de metal pierden 10 Chips temporalmente; "Mercado negro" → puedes robar una pieza del mech de otro jugador (modo multijugador).

**Adaptabilidad:**
- Modo historia: el ensamblaje tiene pistas visuales claras.
- Modo ingeniería: solo ves especificaciones técnicas, sin ayudas visuales.

**Estética:** Dieselpunk industrial, paleta naranja óxido y gris metálico, animaciones de engranajes.

---

## 🔴 IDEA 17 — *Dream Weaver*
### "Cada sueño es un grid. Cada pesadilla, un puzzle sin resolver"

**Concepto base:**
Un juego surreal donde entras a los sueños de distintas personas para "arreglarlos". Cada sueño es un grid de símbolos oníricos que debes ordenar siguiendo la lógica emocional del soñador (las restricciones cambian según el estado de ánimo del sueño: en un sueño de angustia, los símbolos de "miedo" no pueden estar en el centro).

**Mecánicas clave:**
- Las restricciones del grid son emocionales: únicas, narrativas y extrañas (como los sueños).
- Las cartas son **arquetipos jungianos**: la Sombra, el Anima, el Héroe, el Trickster. Cada arquetipo modifica cómo se resuelve el grid.
- Sinergias: Héroe + Monstruo = "confrontación" (revela el símbolo central). Sombra + Luz = "integración" (×3 Mult pero pierdes una carta de tu mazo).
- Cada sueño resuelto añade un fragmento de historia sobre el soñador (narrativa profunda y emotiva).
- Roguelike: si una pesadilla no se resuelve a tiempo, el soñador despierta y pierdes el progreso.

**Adaptabilidad:**
- Modo contemplativo: sin tiempo, con música ambiental guiada.
- Modo pesadilla: los símbolos cambian aleatoriamente cada 30 segundos.

**Estética:** Surrealista, entre Salvador Dalí y Spirited Away, música ambiental etérea con voces distorsionadas.

---

## 🟠 IDEA 18 — *Tavern Brawl Logic*
### "4 héroes entran a la taberna. El grid decide quién sobrevive"

**Concepto base:**
La fusión más directa de Mario Party + Murdoku + Balatro. Hasta 4 jugadores en una taberna roguelike. Cada ronda del tablero, todos resuelven el mismo grid de deducción con información privada (cada jugador solo ve sus pistas, no las del resto). Luego, intercambian cartas de habilidad y negocian.

**Mecánicas clave:**
- El grid se resuelve con información parcial: para resolverlo completamente necesitas la info de los demás. Así que debes **negociar y traicionar** a los otros jugadores para obtener sus pistas.
- Las cartas son **habilidades de héroe**: el Ladrón puede robar una pista a otro jugador, el Bardo puede mostrar su pista a todos a cambio de 2 monedas, el Mago puede **falsificar** una pista y dársela a un rival.
- Al final de cada ronda, el que resolvió mejor el grid gana el "botín de la noche". El que peor lo hizo debe revelar una habilidad secreta.
- Roguelike: la taberna tiene pisos, cada piso más corrupto y con más trampas sociales.

**Adaptabilidad:**
- Modo cooperativo puro: todos comparten pistas libremente y ganan o pierden juntos.
- Modo traición: solo uno puede ganar y las pistas falsas son legales.

**Estética:** Fantasía medieval cómica, personajes exagerados, estilo Dungeons & Dragons caricaturesco.

---

## 🟣 IDEA 19 — *Time Detective*
### "El crimen ocurrió ayer. Mañana no habrá pistas. Hoy, eres el único que puede verlas"

**Concepto base:**
Un roguelike de detectives con mecánica de línea temporal. El grid no es espacial sino **temporal**: colocas eventos en "cuándo" ocurrieron (horas del día) y quién estaba presente. La deducción resuelve el crimen en el tiempo, no en el espacio.

**Mecánicas clave:**
- El grid es una línea de tiempo: filas = horas (6am a midnight), columnas = personas.
- Las pistas son temporales: "Clara estaba con alguien dos horas antes del crimen", "El mayordomo nunca estaba solo antes de las 10pm".
- Cartas de investigación (Balatro): "Testigo Ocular" (revela una celda exacta de la timeline), "Foto de Vigilancia" (revela toda una hora del grid), "Coartada Rota" (elimina una persona de una hora completa).
- Sinergias: "Testigo" + "Foto" jugadas en la misma ronda = revela 2 celdas + elimina a todos los inocentes de esa hora.
- Roguelike: cada caso (run) tiene más sospechosos, más horas, narrativa generada proceduralmente.

**Adaptabilidad:**
- Timeline 4 horas / 3 personas para novatos.
- Timeline 18 horas / 8 personas para detectives veteranos.

**Estética:** Fotografías en blanco y negro, escritorio de detective con papeles y cuerdas rojas, estética "true crime".

---

## 🟢 IDEA 20 — *The Cartographer's Curse*
### "El mapa miente. Pero las reglas del mundo no"

**Concepto base:**
La idea más épica de las 20. Eres un cartógrafo en un mundo de fantasía cuyas leyes físicas son reglas de deducción lógica. Para "mapear" cada región, debes colocar elementos del terreno en un grid siguiendo las leyes del mundo (el agua fluye hacia el sur → nunca habrá agua en el tile norte de una montaña). El mapa completo = el roguelike superado.

**Mecánicas clave:**
- Las leyes del mundo = las restricciones del grid. Son consistentes y el jugador las aprende con el tiempo, como aprender física en el juego.
- Las **cartas de cartografía** (Balatro): "Brújula Mágica" (+10 Chips por cada dirección cardinal correctamente asignada), "Leyenda Antigua" (×2 Mult si el grid tiene al menos un río), "Pergamino Rasgado" (revela la mitad de una fila).
- Sinergias de leyes: si descubres que "el fuego nunca está al este del bosque" y "el bosque siempre está al norte del lago", deduces indirectamente la posición del fuego respecto al lago.
- El mundo se va revelando narrativamente: cada región mapeada cuenta la historia de una civilización perdida.
- Meta-progresión: el mapa del mundo se completa a través de múltiples runs (roguelike). Cada run descubre nuevas leyes que persisten.

**Adaptabilidad:**
- Modo explorador: las leyes se explican explícitamente, con tutoriales narrativos.
- Modo leyenda: las leyes no se explican nunca, debes deducirlas tú mismo desde cero.

**Estética:** Mapas de fantasía ilustrados a mano (estilo Tolkien), paleta sepia y azul tinta, animaciones de tinta fluyendo en el grid.

---

## 📊 Tabla Comparativa

| # | Nombre | Género | Público | Social | Complejidad | Originalidad |
|---|--------|--------|---------|--------|-------------|--------------|
| 1 | CrimeRun | Deducción Roguelike | +12 | ⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| 2 | Alchemist's Deck | Cartas Roguelike | +10 | ⭐ | ⭐⭐ | ⭐⭐⭐ |
| 3 | Sudoku Heist | Puzzle Social | +12 | ⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐ |
| 4 | Cosmos Deduction | Puzzle Espacial | Todos | ⭐⭐ | ⭐⭐ | ⭐⭐⭐ |
| 5 | Spell Grid | Magia Puzzle | +10 | ⭐ | ⭐⭐⭐ | ⭐⭐⭐ |
| 6 | Murder at the Table | Party + Misterio | +12 | ⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| 7 | Rune Rogue | Acción Puzzle | +12 | ⭐ | ⭐⭐⭐ | ⭐⭐⭐ |
| 8 | Gastro Logic | Cocina Party | Todos | ⭐⭐⭐⭐ | ⭐ | ⭐⭐⭐⭐ |
| 9 | Architect of Lies | Ciudad Narrativa | +14 | ⭐ | ⭐⭐⭐ | ⭐⭐⭐ |
| 10 | HexWitch | Puzzle Hexagonal | +12 | ⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| 11 | Constellation Run | Puzzle Narrativo | Todos | ⭐ | ⭐⭐ | ⭐⭐⭐⭐ |
| 12 | Circus Rogue | Party Roguelike | Todos | ⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐ |
| 13 | BioGrid | Ciencia Puzzle | +12 | ⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| 14 | Jazz Collective | Música Puzzle | +10 | ⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| 15 | Garden of Clues | Cozy Misterio | Todos | ⭐⭐ | ⭐ | ⭐⭐⭐ |
| 16 | Mech Assembly | Sci-fi Roguelike | +12 | ⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐ |
| 17 | Dream Weaver | Surrealista | +14 | ⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| 18 | Tavern Brawl Logic | Party Social | +12 | ⭐⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| 19 | Time Detective | Deducción Temporal | +14 | ⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| 20 | The Cartographer's Curse | Épico Narrativo | +12 | ⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |

---

## ⭐ Top 3 Recomendados (visión de programador)

1. **🥇 #6 Murder at the Table** → Perfecta fusión de las 4 influencias. Social, escalable, con profundidad táctica y narrativa. El más completo para todos los públicos.
2. **🥈 #18 Tavern Brawl Logic** → Altísima rejugabilidad, mecánica social única, tensión natural entre cooperar y traicionar.
3. **🥉 #20 The Cartographer's Curse** → El más ambicioso y original. Gran potencial narrativo y de meta-progresión entre runs.

---
*Brainstorm generado el 31/07/2026. Pendiente de selección del concepto final.*
