using UnityEngine;
using UnityEngine.UI;

namespace Nigma.UI
{
    [RequireComponent(typeof(RawImage))]
    public class GradientBackground : MonoBehaviour
    {
        public Color topColor = new Color(1f, 0.95f, 0.9f);
        public Color bottomColor = new Color(0.9f, 0.6f, 0.4f);

        void Start()
        {
            ApplyGradient();
        }

        public void ApplyGradient()
        {
            RawImage img = GetComponent<RawImage>();
            
            // Create a 1x2 texture
            Texture2D tex = new Texture2D(1, 2, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            
            // Set pixels (Bottom is index 0, Top is index 1)
            tex.SetPixel(0, 0, bottomColor);
            tex.SetPixel(0, 1, topColor);
            tex.Apply();
            
            img.texture = tex;
        }
    }
}
