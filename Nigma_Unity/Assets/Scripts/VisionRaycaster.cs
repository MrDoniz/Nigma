using System.Collections.Generic;
using UnityEngine;

namespace Nigma.Core
{
    /// <summary>
    /// Gestiona la lógica de línea de visión de un personaje u objeto.
    /// Soporta los siguientes comportamientos ópticos (Fase 2-4):
    ///   - Mirror   → Reflexión de rayo 90°
    ///   - Plant    → Bloquea personajes altos; transparente para isShortCharacter
    ///   - Camera   → Visión en cono (no lineal); dibuja el cono con LineRenderer
    ///   - Lamp     → Activa en niveles isLightRequired; ilumina un radio esférico
    ///   - Fan      → Mueve objetos "Curtain" en fanBlowDirection
    ///   - Wall/Sofa/Solid → Bloqueo total del rayo
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class VisionRaycaster : MonoBehaviour
    {
        [Header("Vision Settings")]
        public float maxVisionDistance = 20f;
        public int maxBounces = 3;       // For mirrors
        public LayerMask obstacleLayer;

        [Header("Lamp Settings")]
        [Tooltip("Radius of light emitted when this object is a Lamp.")]
        public float lampLightRadius = 5f;
        [Tooltip("Color of the lamp's light cone visualization.")]
        public Color lampLightColor = new Color(1f, 0.9f, 0.5f, 0.4f);

        // ── Private State ───────────────────────────────────────────────────
        private LineRenderer lineRenderer;
        private DraggableObject myDraggable;
        private LevelData currentLevelData;  // injected by GameManager at level load

        // ── Fan State ────────────────────────────────────────────────────────
        // Curtains are kept in their displaced position while the fan is active.
        private List<Transform> displacedCurtains = new List<Transform>();
        private bool fanActivated = false;

        // ── Lamp State ───────────────────────────────────────────────────────
        private SphereCollider lampTrigger;   // Created at runtime if this is a Lamp

        // ────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            myDraggable = GetComponent<DraggableObject>();
        }

        private void Start()
        {
            if (myDraggable != null && myDraggable.furnitureType == FurnitureType.Lamp)
            {
                InitializeLamp();
            }

            if (myDraggable != null && myDraggable.furnitureType == FurnitureType.Fan)
            {
                ActivateFan();
            }
        }

        private void Update()
        {
            if (myDraggable == null) return;

            switch (myDraggable.furnitureType)
            {
                case FurnitureType.Character:
                    CastLinearVision();
                    break;

                case FurnitureType.Camera:
                    DrawCameraConeFrustum();
                    break;

                case FurnitureType.Lamp:
                    // Lamp logic is trigger-based (InitializeLamp), nothing to draw per frame.
                    break;

                case FurnitureType.Fan:
                    // Fan blows continuously; curtains are displaced in Start/ActivateFan.
                    break;

                default:
                    // Mirrors, Sofas, Walls, Plants don't emit their own vision ray.
                    lineRenderer.positionCount = 0;
                    break;
            }
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Public API

        /// <summary>
        /// Called by GameManager when a new level is loaded, so VisionRaycaster
        /// can check isLightRequired, etc.
        /// </summary>
        public void SetLevelData(LevelData level)
        {
            currentLevelData = level;
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Linear Vision (Characters & Mirrors)

        /// <summary>
        /// Standard line-of-sight ray with mirror bouncing, plant logic, and
        /// optional lamp-only visibility check.
        /// </summary>
        private void CastLinearVision()
        {
            List<Vector3> rayPoints = new List<Vector3>();

            // Eye position (approx torso height for isometric clarity)
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 direction = transform.forward;

            rayPoints.Add(origin);

            int bounces = 0;

            while (bounces <= maxBounces)
            {
                if (Physics.Raycast(origin, direction, out RaycastHit hit, maxVisionDistance, obstacleLayer))
                {
                    rayPoints.Add(hit.point);

                    DraggableObject hitObject = hit.collider.GetComponent<DraggableObject>();

                    // ── Mirror ────────────────────────────────────────────
                    if (hitObject != null && hitObject.furnitureType == FurnitureType.Mirror)
                    {
                        direction = Vector3.Reflect(direction, hit.normal);
                        direction.y = 0;
                        direction.Normalize();
                        origin = hit.point + direction * 0.01f;
                        bounces++;
                        continue;
                    }

                    // ── Plant ─────────────────────────────────────────────
                    if (hitObject != null && hitObject.furnitureType == FurnitureType.Plant)
                    {
                        bool iAmShort = myDraggable != null && myDraggable.isShortCharacter;
                        if (iAmShort)
                        {
                            // Short character sees through the plant — keep going
                            origin = hit.point + direction * 0.05f;
                            // Don't increment bounces, it's transparency not reflection
                            continue;
                        }
                        else
                        {
                            // Tall character is blocked by plant
                            break;
                        }
                    }

                    // ── Lamp illumination check ────────────────────────────
                    if (currentLevelData != null && currentLevelData.isLightRequired)
                    {
                        // If light is required, check the hit object is in lamp range
                        if (hitObject != null && !IsObjectLit(hit.collider.transform.position))
                        {
                            Debug.Log($"[VisionRaycaster] {hitObject.name} is in darkness — not visible.");
                            break;
                        }
                    }

                    // ── Solid obstacle (Wall, Sofa, Character, Camera) ────
                    if (hitObject != null)
                    {
                        Debug.Log($"[VisionRaycaster] {gameObject.name} sees: {hitObject.name} ({hitObject.furnitureType})");
                    }
                    break;
                }
                else
                {
                    // Open air — ray travels to max distance
                    rayPoints.Add(origin + direction * maxVisionDistance);
                    break;
                }
            }

            lineRenderer.positionCount = rayPoints.Count;
            lineRenderer.SetPositions(rayPoints.ToArray());
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Camera — Cone Vision

        /// <summary>
        /// Draws a fan-shaped frustum using LineRenderer to visualize the camera's
        /// cone of vision. The camera "sees" anything inside the cone that isn't
        /// blocked by a solid obstacle.
        ///
        /// NOTE: This is a visualization-only method; actual detection uses Physics.OverlapSphere
        /// + angle check (see CheckCameraDetection).
        /// </summary>
        private void DrawCameraConeFrustum()
        {
            if (myDraggable == null) return;

            float halfAngle = myDraggable.cameraVisionAngle;
            int segments = 20;
            List<Vector3> conePoints = new List<Vector3>();

            Vector3 origin = transform.position + Vector3.up * 1f;
            conePoints.Add(origin);

            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
                Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxVisionDistance, obstacleLayer))
                {
                    conePoints.Add(hit.point);
                }
                else
                {
                    conePoints.Add(origin + dir * maxVisionDistance);
                }
            }

            // Close the cone back to origin
            conePoints.Add(origin);

            lineRenderer.positionCount = conePoints.Count;
            lineRenderer.SetPositions(conePoints.ToArray());
            lineRenderer.startColor = new Color(1f, 0f, 0f, 0.8f);   // Red = camera zone
            lineRenderer.endColor   = new Color(1f, 0f, 0f, 0.1f);
        }

        /// <summary>
        /// Returns true if a given world position falls inside this camera's vision cone
        /// AND has line-of-sight (not blocked by a solid obstacle).
        /// Called by GameManager to check if a character is "caught on camera".
        /// </summary>
        public bool CheckCameraDetection(Vector3 targetWorldPos)
        {
            if (myDraggable == null || myDraggable.furnitureType != FurnitureType.Camera) return false;

            Vector3 origin  = transform.position + Vector3.up * 1f;
            Vector3 toTarget = (targetWorldPos - origin).normalized;
            float angle = Vector3.Angle(transform.forward, toTarget);

            if (angle > myDraggable.cameraVisionAngle) return false; // Outside cone

            float dist = Vector3.Distance(origin, targetWorldPos);
            return !Physics.Raycast(origin, toTarget, dist, obstacleLayer); // No obstruction
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Lamp — Area Light

        /// <summary>
        /// Sets up a SphereCollider trigger on this Lamp object.
        /// Any DraggableObject entering the trigger is marked as "lit".
        /// GameManager queries IsObjectLit() to determine visibility in dark levels.
        /// </summary>
        private void InitializeLamp()
        {
            lampTrigger = gameObject.AddComponent<SphereCollider>();
            lampTrigger.isTrigger = true;
            lampTrigger.radius = lampLightRadius;

            // Visual cue: enable a point light if one exists on this GameObject
            Light light = GetComponentInChildren<Light>();
            if (light != null)
            {
                light.enabled = true;
                light.range = lampLightRadius;
                light.color = lampLightColor;
                light.intensity = 1.5f;
            }
        }

        /// <summary>
        /// Returns true if the given world position is inside any active Lamp's radius.
        /// Called during linear vision cast when isLightRequired is true.
        /// </summary>
        public static bool IsObjectLit(Vector3 worldPos)
        {
            // Find all active lamps in the scene and check their radius
            VisionRaycaster[] allRaycasters = FindObjectsOfType<VisionRaycaster>();
            foreach (var rc in allRaycasters)
            {
                if (rc.myDraggable != null && rc.myDraggable.furnitureType == FurnitureType.Lamp)
                {
                    float dist = Vector3.Distance(rc.transform.position, worldPos);
                    if (dist <= rc.lampLightRadius) return true;
                }
            }
            return false;
        }

        #endregion

        // ────────────────────────────────────────────────────────────────────
        #region Fan — Curtain Displacement

        /// <summary>
        /// On activation, finds all curtains (tagged "Curtain") within a radius and
        /// displaces them in the fanBlowDirection, opening/closing lines of sight.
        /// The displacement is proportional to distance (closer curtains move more).
        /// </summary>
        private void ActivateFan()
        {
            if (myDraggable == null) return;
            if (fanActivated) return;

            fanActivated = true;
            float fanRadius = 6f;

            Collider[] nearby = Physics.OverlapSphere(transform.position, fanRadius, obstacleLayer);
            foreach (Collider col in nearby)
            {
                if (col.CompareTag("Curtain"))
                {
                    float dist = Vector3.Distance(transform.position, col.transform.position);
                    float displacement = Mathf.Lerp(1.5f, 0.3f, dist / fanRadius);
                    col.transform.position += myDraggable.fanBlowDirection.normalized * displacement;
                    displacedCurtains.Add(col.transform);
                    Debug.Log($"[Fan] Displaced curtain '{col.name}' by {displacement:F2}m.");
                }
            }
        }

        /// <summary>
        /// When the fan is moved/removed, resets all curtains to their original positions.
        /// Called automatically when the DraggableObject is picked up.
        /// </summary>
        public void DeactivateFan()
        {
            // Reset curtain positions (snap back)
            if (myDraggable == null) return;

            foreach (Transform curtain in displacedCurtains)
            {
                if (curtain != null)
                {
                    curtain.position -= myDraggable.fanBlowDirection.normalized * 0.9f;
                }
            }
            displacedCurtains.Clear();
            fanActivated = false;
        }

        #endregion
    }
}
