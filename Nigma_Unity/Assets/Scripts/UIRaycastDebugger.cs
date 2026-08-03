using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Nigma.Editor
{
    /// <summary>
    /// Este script escanea en tiempo real debajo del ratón y muestra en consola
    /// qué objeto de UI está recibiendo realmente el clic. 
    /// Útil para detectar capas invisibles que bloquean botones.
    /// </summary>
    public class UIRaycastDebugger : MonoBehaviour
    {
        void Update()
        {
            if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = UnityEngine.InputSystem.Mouse.current.position.ReadValue()
                };

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)
                {
                    Debug.Log($"[UI Debugger] CLIC DETECTADO. El ratón ha tocado {results.Count} elementos de UI:");
                    for (int i = 0; i < results.Count; i++)
                    {
                        Debug.Log($"   Capa {i}: {results[i].gameObject.name} (en {results[i].gameObject.transform.parent?.name})");
                    }
                }
                else
                {
                    Debug.LogWarning("[UI Debugger] CLIC AL VACÍO: El EventSystem NO ha detectado ningún elemento de UI debajo del ratón. Comprueba GraphicRaycaster y EventSystem.");
                }
            }
        }
    }
}
