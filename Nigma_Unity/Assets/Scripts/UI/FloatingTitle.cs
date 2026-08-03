using UnityEngine;

namespace Nigma.UI
{
    public class FloatingTitle : MonoBehaviour
    {
        public float amplitude = 15f;
        public float speed = 1.5f;

        private RectTransform rectTransform;
        private Vector2 startPos;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            startPos = rectTransform.anchoredPosition;
        }

        void Update()
        {
            // Use unscaledTime so it works even if timeScale is 0
            float newY = startPos.y + Mathf.Sin(Time.unscaledTime * speed) * amplitude;
            rectTransform.anchoredPosition = new Vector2(startPos.x, newY);
        }
    }
}
