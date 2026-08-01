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

## 2. MECÁNICA CENTRAL: LA ESCENA CORRUPTA (Líneas de Visión)
Eres un detective investigando la escena de un crimen. La jugabilidad ya no consiste en empezar de cero, sino en corregir una habitación manipulada.
1. **Los Elementos Estructurales (Fijos):** La habitación ya tiene paredes, ventanas y pilares fijos que no puedes mover. Son las "reglas físicas" inamovibles del nivel.
2. **El Inventario Limitado:** Para evitar el caos en la pantalla del móvil, el jugador no tiene piezas infinitas. Recibe un inventario muy específico y limitado para cada nivel (ej. "En este caso dispones de 2 sillas y 1 espejo").
3. **El Enigma (Texto Inicial):** Cada nivel arranca con una breve descripción textual o "Atestado Policial" que plantea las reglas lógicas y la pregunta final (Ej: "¿Dónde se encontraba la víctima?" o "¿Dónde escondieron el botín?"). 
4. **La Reconstrucción (Scratchpad Físico):** El jugador usa el tablero y mueve los muebles como una herramienta de apoyo mental para deducir quién miente y cómo cuadran las líneas de visión. No hay "autocorrección" en tiempo real.
5. **El Botón 'Resolver' (Condición de Victoria):** El juego no te avisa automáticamente. Da igual cómo hayas dejado los muebles desordenados por la habitación; cuando crees saber la respuesta, pulsas el botón "Resolver" y seleccionas la casilla exacta que responde a la pregunta del Enigma. Si aciertas la deducción, ganas.
6. **Catálogo de Mobiliario (Progresión):** A medida que avanza la campaña, se desbloquean mecánicas ópticas más complejas:
   - *Sofás / Estanterías:* Bloquean la visión completamente.
   - *Espejos de pie:* Reflejan la línea de visión 90 grados.
   - *Lámparas de pie:* En niveles oscuros, revelan lo que esté en su haz de luz.
   - *Plantas de interior:* Bloquean la visión humana, pero permiten ver a través a personajes bajos (mascotas, robots).
   - *Ventiladores:* Desplazan cortinas o humo en una dirección específica.

---

## 3. EL TOQUE "BALATRO": SINERGIAS SIN CARTAS
Para eliminar el uso de "cartas" pero mantener la aleatoriedad matemática y las sinergias adictivas:
1. **El Maletín del Detective:** Aquí guardas tus objetos. No hay mazos.
2. **Las Evidencias (Pistas):** Objetos de un solo uso que encuentras o ganas al resolver un caso rápido.
3. **Las Herramientas (Los Jokers de Puntuación):** Objetos pasivos que modifican radicalmente tu meta-progresión o puntuación, **no las reglas físicas del tablero**.
   - *Ejemplo de Herramienta Pasiva:* "Lupa Antigua" (Si resuelves el caso en menos de 2 minutos, tu puntuación final se multiplica x3).
4. **La Sinergia (El Combo):** El núcleo adictivo del meta-juego. Puedes combinar evidencias obtenidas para maximizar tu recompensa al terminar el nivel.
   - *Ejemplo de Combo:* Juntar la evidencia "Huella Dactilar" con la herramienta "Libreta de Notas" hace que al acertar la solución, ganes monedas premium adicionales para la Agencia, permitiéndote escalar en los modos Roguelite.

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
- **Progresión Inicial (100% Hardcodeado):** Para garantizar que el juego es divertido y lógicamente perfecto, **todos los puzles de la versión inicial estarán creados a mano**. La generación procedural infinita se descarta para el lanzamiento base, priorizando la calidad sobre la cantidad.
- **El Run Infinito (Premium - Expansión Futura):** Al pagar 4.99€, desbloqueas paquetes de niveles premium (handcrafted) y nuevos modos de juego, con la posibilidad futura de añadir tableros generados proceduralmente una vez la fórmula base esté validada.

## 6. EL MODO MULTIJUGADOR (Código de Sala & Friend Pass)
Para jugar con amigos usamos un sistema de "Room Code" (estilo Among Us), muy barato de mantener en servidores. 
- **El Sistema "Friend Pass" (Viralidad):** Solo el Anfitrión necesita haber pagado el Premium de 4.99€. Los amigos pueden bajar la app gratuita, meter el código de sala y jugar gratis al multijugador.
1. **Agencias Rivales (Velocidad):** Varios jugadores reciben el mismo grid simultáneamente. El objetivo es resolverlo lo más rápido posible. Los errores de colocación penalizan.
2. **El Policía Corrupto (Party Game - Roles Ocultos):** El juego estrella para reuniones. Se genera un tablero gigante en los móviles de los 5 amigos. **Las pistas lógicas se reparten**. Tenéis que hablar en voz alta para resolver el caso juntos. Pero cuidado: a un jugador se le asignan pistas falsas en secreto (El Corrupto), y su objetivo es convencer sutilmente a los demás de colocar mal las piezas sin que sospechen de él.

---

## 8. DIRECCIÓN DE ARTE Y SONIDO
- **Cámara y Modelos:** Vista Isométrica 3D. Tanto los personajes como los muebles deben ser modelos 3D reales, lo que permite que tengan **dirección (hacia dónde miran)**, algo vital para las mecánicas de línea de visión.
- **Estética "Isometric Toy-Box" (Maqueta / Claymation):** El nivel parece una preciosa maqueta táctil (estilo madera pulida, arcilla o plastilina). Ideal para que lo genere una IA de forma hiper-consistente. Es colorido y apto para todas las edades.
- **Temáticas de Personajes (Multiverso):** No nos cerramos a un solo estilo. Habrá niveles de todo tipo. Desde personas "normales" (estilo Murdoku clásico), hasta animales antropomórficos, fantasía o ciencia ficción, permitiendo una variedad infinita de "casos" y cosméticos.
- **Interacción y "Game Feel":** La interfaz debe ser extremadamente jugosa (juicy). 
  - Al seleccionar un personaje o mueble, debe tener una pequeña animación de "pop" o rebote.
  - Al arrastrar a una persona, la animación debe simular que lo estás **levantando por la cabeza o el cuello (scruff)**, dejando que su cuerpecito y piernas cuelguen y se balanceen hacia los lados por la inercia del movimiento.
- **Sonido (Sensación Táctil - ASMR):** Soltar un mueble o personaje en el tablero tiene que sonar a un golpe de madera hueca o pieza de ajedrez ("¡CLAC!"). Acompañado de música Jazz suave de fondo.

---
*Nota de Desarrollo: Cualquier idea nueva, mecánica o recorte, lo iremos registrando y comentando en este documento o en archivos similares dentro de esta carpeta del repositorio.*
