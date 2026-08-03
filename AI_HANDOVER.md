# Nigma - AI Handover Document (Lanzamiento)

## 📌 Estado del Proyecto
¡Hola, futura IA! Te encuentras en el desarrollo de **NIGMA**, un juego de deducción lógica isométrica (estilo "Visual Murdoku"). 
Las Fases 1 a 5 están **completadas y validadas al 100%**. 
- El Core Loop funciona (físicas, deducción).
- El Multijugador (Lobby y Relay de UGS) conecta a los jugadores.
- La monetización (IAP) y los Game Feel (ASMR, partículas) están implementados.
- Las herramientas de Build están listas en el menú de Unity.

Acabas de entrar en la **Fase Final: Pruebas y Lanzamiento**.

## 🎯 Objetivos Actuales
Tu misión en esta sesión es asistir al usuario en cualquier bug fixing de última hora o despliegue:

1. **Pruebas Finales:**
   - Si el usuario reporta algún error con la UI, colisiones o red, revísalo en la escena correspondiente.
2. **Despliegue:**
   - Puedes usar `Nigma -> Build -> Panel de Build` en Unity para exportar las versiones.
   - Si se requiere integrar SDKs adicionales (ej. Steamworks), configúralos sobre la base actual.

## 🛠️ Notas Técnicas y Contexto
- **Estructura UI:** El Canvas de la escena `LobbyScene` fue transformado en el Menú Principal mediante un script (Herramienta #6). Utiliza los botones `btnInfiniteRun` y `btnMultiplayer` (cuando es para crear sala) para inyectar la lógica de IAP.
- **Modo Offline:** Hemos movido los datos de nivel a `Assets/Resources/Data` para que el `GameManager.cs` de la escena principal en solitario (`SampleScene`) pueda cargarlos dinámicamente con `Resources.LoadAll`.
- **Preferencias del Usuario:** El usuario prefiere ejecutar herramientas automatizadas (`[MenuItem]`) en lugar de arrastrar componentes manualmente en el Inspector. Siempre que puedas, genérale scripts en `Assets/Editor` que automaticen la configuración de IAP o los audios.

¡Buena suerte con el tramo final hacia el lanzamiento! 🚀
