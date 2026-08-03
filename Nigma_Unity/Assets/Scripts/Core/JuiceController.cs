using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Nigma.Core
{
    public class CameraRotator : MonoBehaviour
    {
        public Transform target;
        public float speed = 5f;

        void Update()
        {
            if (target != null)
            {
                transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
                transform.LookAt(target);
            }
            else 
            {
                // Si no hay target, simplemente rota sobre si misma (panoramica)
                transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
            }
        }
    }

    public class ButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 originalScale;
        public float hoverScale = 1.05f;
        public float pressScale = 0.95f;
        public float animationSpeed = 15f;
        private Vector3 targetScale;

        void Start()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }

        void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = originalScale * pressScale;
            AudioManager.Instance?.PlayUIClick();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
        }
    }

    public class CharacterBreathing : MonoBehaviour
    {
        public float breathSpeed = 2f;
        public float breathAmount = 0.05f;
        private Vector3 startScale;
        private float offset;

        void Start()
        {
            startScale = transform.localScale;
            offset = Random.Range(0f, 10f); // Para que no respiren sincronizados
        }

        void Update()
        {
            float wave = Mathf.Sin((Time.time + offset) * breathSpeed) * breathAmount;
            transform.localScale = new Vector3(startScale.x, startScale.y + wave, startScale.z);
        }
    }

    public class PopInAnimation : MonoBehaviour
    {
        public float duration = 0.5f;
        private Vector3 finalScale;
        private float timer = 0f;
        private bool hasPopped = false;

        void Start()
        {
            finalScale = transform.localScale;
            transform.localScale = Vector3.zero;
            // Delay aleatorio para el efecto cascada
            timer = -Random.Range(0f, 0.4f); 
        }

        void Update()
        {
            if (timer < duration)
            {
                timer += Time.deltaTime;
                if (timer > 0f)
                {
                    if (!hasPopped)
                    {
                        hasPopped = true;
                        AudioManager.Instance?.PlayWoodClac();
                    }
                    
                    float t = timer / duration;
                    // Elastic ease out
                    float p = 0.3f;
                    float scaleFactor = Mathf.Pow(2, -10 * t) * Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p) + 1;
                    transform.localScale = finalScale * scaleFactor;
                }
            }
            else
            {
                transform.localScale = finalScale;
                enabled = false;
            }
        }
    }
}
