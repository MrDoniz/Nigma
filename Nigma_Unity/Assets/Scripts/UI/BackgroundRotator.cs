using UnityEngine;

namespace Nigma.UI
{
    public class BackgroundRotator : MonoBehaviour
    {
        public float rotationSpeed = -5f; // Grados por segundo

        private void Update()
        {
            if (Camera.main != null)
            {
                // En lugar de rotar el diorama, rotamos la cámara a su alrededor.
                // Esto garantiza que el mapa siempre esté 100% centrado y encuadrado
                Camera.main.transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
                Camera.main.transform.LookAt(transform.position);
            }
        }
    }
}
