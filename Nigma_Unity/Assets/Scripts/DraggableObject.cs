using UnityEngine;

namespace Nigma.Core
{
    [RequireComponent(typeof(Collider))]
    public class DraggableObject : MonoBehaviour
    {
        private GridManager gridManager;
        private GridManager.GridNode currentNode;
        
        private bool isDragging = false;
        private Vector3 startMousePos;
        private Vector3 startPos;
        private Camera mainCamera;
        
        [Header("Object Type")]
        [Tooltip("Defines the optical behavior of this object in the VisionRaycaster.")]
        public FurnitureType furnitureType = FurnitureType.Sofa;

        [Header("Character Settings")]
        [Tooltip("If true, this character/object is short and can pass through Plant line-of-sight.")]
        public bool isShortCharacter = false;

        [Header("Camera Settings (FurnitureType.Camera only)")]
        [Tooltip("Half-angle of the camera's vision cone in degrees.")]
        [Range(10f, 90f)]
        public float cameraVisionAngle = 45f;

        [Header("Fan Settings (FurnitureType.Fan only)")]
        [Tooltip("Direction the fan blows. Moves curtains tagged 'Curtain' in this direction.")]
        public Vector3 fanBlowDirection = Vector3.right;

        [Header("Game Feel Settings")]
        [Tooltip("How high the object lifts when dragged (simulating picking up by the scruff)")]
        public float liftHeight = 1.5f;
        [Tooltip("Speed of snapping to grid")]
        public float snapSpeed = 10f;
        [Tooltip("Tilt angle when dragged to simulate dangling")]
        public float dragTiltAmount = 15f;

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private Quaternion baseRotation;

        private void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            mainCamera = Camera.main;
            targetPosition = transform.position;
            baseRotation = transform.rotation;
            targetRotation = baseRotation;
        }

        private void Update()
        {
            if (isDragging)
            {
                // Follow mouse using raycast to ground plane
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.up * liftHeight);
                
                if (groundPlane.Raycast(ray, out float rayDistance))
                {
                    Vector3 hitPoint = ray.GetPoint(rayDistance);
                    targetPosition = new Vector3(hitPoint.x, liftHeight, hitPoint.z);
                    
                    // Simulate dangling/tilt based on movement direction
                    Vector3 moveDelta = targetPosition - transform.position;
                    float tiltX = moveDelta.z * dragTiltAmount; // Pitch
                    float tiltZ = -moveDelta.x * dragTiltAmount; // Roll
                    targetRotation = baseRotation * Quaternion.Euler(tiltX, 0, tiltZ);
                }
            }
            else
            {
                // Smooth snap to target position and return to base rotation
                targetRotation = baseRotation;
            }

            // Apply lerp for juicy movement
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * snapSpeed);
        }

        private void OnMouseDown()
        {
            if (gridManager == null) return;
            
            isDragging = true;
            
            // Un-occupy current node
            if (currentNode != null && currentNode.occupant == this)
            {
                currentNode.occupant = null;
            }
            
            // Pop effect (handled by lerp target change)
        }

        private void OnMouseUp()
        {
            isDragging = false;
            
            if (gridManager != null)
            {
                // Find nearest node
                GridManager.GridNode nearest = gridManager.GetNearestNode(transform.position);
                
                // If cell is occupied, just snap back to old position (simple logic for now)
                // A better approach would swap or search for nearest free cell.
                if (gridManager.IsCellOccupied(nearest))
                {
                    if (currentNode != null)
                    {
                        targetPosition = currentNode.worldPosition;
                        currentNode.occupant = this;
                    }
                }
                else
                {
                    // Snap to new cell
                    targetPosition = nearest.worldPosition;
                    nearest.occupant = this;
                    currentNode = nearest;
                    
                    // Trigger "Clac!" sound here
                    Debug.Log("Clac! Object placed at " + nearest.x + "," + nearest.y);
                }
            }
        }
    }
}
