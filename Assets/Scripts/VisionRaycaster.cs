using System.Collections.Generic;
using UnityEngine;

namespace Nigma.Core
{
    [RequireComponent(typeof(LineRenderer))]
    public class VisionRaycaster : MonoBehaviour
    {
        [Header("Vision Settings")]
        public float maxVisionDistance = 20f;
        public int maxBounces = 3; // for mirrors
        public LayerMask obstacleLayer;
        
        private LineRenderer lineRenderer;
        
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            // Basic line renderer setup
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }

        private void Update()
        {
            CastVision();
        }

        private void CastVision()
        {
            List<Vector3> rayPoints = new List<Vector3>();
            
            // Start at character's eyes (approximate offset)
            Vector3 origin = transform.position + Vector3.up * 1.5f; 
            Vector3 direction = transform.forward;
            
            rayPoints.Add(origin);
            
            int bounces = 0;
            
            while (bounces <= maxBounces)
            {
                if (Physics.Raycast(origin, direction, out RaycastHit hit, maxVisionDistance, obstacleLayer))
                {
                    rayPoints.Add(hit.point);
                    
                    // Check if we hit a mirror
                    if (hit.collider.CompareTag("Mirror"))
                    {
                        // Reflect the ray
                        direction = Vector3.Reflect(direction, hit.normal);
                        // Flatten to horizontal plane to avoid weird angles
                        direction.y = 0; 
                        direction.Normalize();
                        
                        origin = hit.point + direction * 0.01f; // Offset to avoid self-collision
                        bounces++;
                    }
                    else if (hit.collider.CompareTag("Plant"))
                    {
                        // Plants block tall humans but we can add logic later 
                        // to let short characters pass the raycast through.
                        break;
                    }
                    else
                    {
                        // Hit a solid object (Wall, Sofa, Character)
                        // This character "sees" whatever it hit.
                        DraggableObject seenObject = hit.collider.GetComponent<DraggableObject>();
                        if (seenObject != null)
                        {
                            // We could trigger an event here "I see [Character]"
                        }
                        break;
                    }
                }
                else
                {
                    // Hit nothing, ray goes to max distance
                    rayPoints.Add(origin + direction * maxVisionDistance);
                    break;
                }
            }
            
            // Update visualizer
            lineRenderer.positionCount = rayPoints.Count;
            lineRenderer.SetPositions(rayPoints.ToArray());
        }
    }
}
