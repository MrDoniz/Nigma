namespace Nigma.Core
{
    /// <summary>
    /// Enum centralizado con todos los tipos de mobiliario del tablero de Nigma.
    /// Cada tipo tiene un comportamiento óptico distinto en el VisionRaycaster.
    /// </summary>
    public enum FurnitureType
    {
        // ─── Estructurales ──────────────────────────────────────────────────
        /// <summary>Pared fija. Bloquea completamente la línea de visión.</summary>
        Wall,

        // ─── Inventario Básico (Fase 2-3) ────────────────────────────────────
        /// <summary>Sofá / Estantería. Bloquea la visión completamente.</summary>
        Sofa,

        /// <summary>Espejo de pie. Refleja la línea de visión 90 grados.</summary>
        Mirror,

        // ─── Inventario Avanzado (Fase 4) ────────────────────────────────────
        /// <summary>
        /// Cámara de vigilancia. Emite una zona de visión en cono (ángulo configurable).
        /// No bloquea la visión ajena; solo define la zona que ella misma "ve".
        /// </summary>
        Camera,

        /// <summary>
        /// Lámpara de pie. En niveles con <c>isLightRequired = true</c>, solo los objetos
        /// dentro de su radio de luz son visibles para los personajes.
        /// </summary>
        Lamp,

        /// <summary>
        /// Planta de interior. Bloquea la visión de personajes altos (<c>isShortCharacter = false</c>)
        /// pero es transparente para personajes bajos o mascotas (<c>isShortCharacter = true</c>).
        /// </summary>
        Plant,

        /// <summary>
        /// Ventilador. Desplaza cortinas (objetos con tag "Curtain") en una dirección,
        /// abriendo o cerrando líneas de visión de forma dinámica.
        /// </summary>
        Fan,

        // ─── Personajes ──────────────────────────────────────────────────────
        /// <summary>Personaje jugador u NPC. Emite su propia línea de visión (VisionRaycaster).</summary>
        Character
    }
}
