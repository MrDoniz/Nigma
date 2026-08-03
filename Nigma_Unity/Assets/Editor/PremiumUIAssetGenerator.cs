using System.IO;
using UnityEngine;
using UnityEditor;

namespace Nigma.Editor
{
    public class PremiumUIAssetGenerator
    {
        [MenuItem("Nigma/Herramientas Avanzadas/Generar Arte UI (Redondeado)", false, 100)]
        public static void GenerateAssets()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/UI"))
                AssetDatabase.CreateFolder("Assets/Resources", "UI");

            string path = "Assets/Resources/UI/RoundedRect.png";
            GenerateRoundedRect(path, 256, 256, 64);
            
            string pathWood = "Assets/Resources/UI/WoodenRect.png";
            GenerateWoodenRect(pathWood, 512, 128, 64);
            
            AssetDatabase.Refresh();

            // Configurar el importer como Sprite para RoundedRect
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = new Vector4(64, 64, 64, 64); // Slicing
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }

            // Configurar el importer para la UI generada por IA
            string aiPath = "Assets/Resources/UI/LaytonButtonAI.jpg";
            TextureImporter aiImporter = AssetImporter.GetAtPath(aiPath) as TextureImporter;
            if (aiImporter != null)
            {
                aiImporter.textureType = TextureImporterType.Sprite;
                aiImporter.spriteImportMode = SpriteImportMode.Single;
                aiImporter.spriteBorder = new Vector4(120, 120, 120, 120); // Slicing para proteger los bordes steampunk
                aiImporter.mipmapEnabled = false;
                aiImporter.filterMode = FilterMode.Bilinear;
                aiImporter.SaveAndReimport();
            }

            // Configurar el importer como Sprite para WoodenRect
            TextureImporter importerWood = AssetImporter.GetAtPath(pathWood) as TextureImporter;
            if (importerWood != null)
            {
                importerWood.textureType = TextureImporterType.Sprite;
                importerWood.spriteImportMode = SpriteImportMode.Single;
                importerWood.spriteBorder = new Vector4(64, 64, 64, 64); // Slicing
                importerWood.alphaIsTransparency = true;
                importerWood.mipmapEnabled = false;
                importerWood.filterMode = FilterMode.Bilinear;
                importerWood.SaveAndReimport();
            }

            Debug.Log("[PremiumUI] Texturas RoundedRect y WoodenRect generadas con éxito.");
        }

        private static void GenerateRoundedRect(string path, int width, int height, float radius)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];

            Vector2 center = new Vector2(width / 2f, height / 2f);
            float innerWidth = width - radius * 2f;
            float innerHeight = height - radius * 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center.x) - innerWidth / 2f);
                    float dy = Mathf.Max(0f, Mathf.Abs(y - center.y) - innerHeight / 2f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = Mathf.Clamp01(radius - dist); 
                    pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        private static void GenerateWoodenRect(string path, int width, int height, float radius)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];

            Vector2 center = new Vector2(width / 2f, height / 2f);
            float innerWidth = width - radius * 2f;
            float innerHeight = height - radius * 2f;

            Color darkWood = new Color(0.12f, 0.05f, 0.02f, 1f); // Borde caoba muy oscuro
            Color topWood = new Color(0.35f, 0.15f, 0.08f, 1f); // Centro caoba rojizo
            Color bottomWood = new Color(0.2f, 0.08f, 0.04f, 1f); // Sombra caoba profunda

            for (int y = 0; y < height; y++)
            {
                // Gradient for the inner wood
                float t = (float)y / height;
                Color innerColor = Color.Lerp(bottomWood, topWood, t);

                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Max(0f, Mathf.Abs(x - center.x) - innerWidth / 2f);
                    float dy = Mathf.Max(0f, Mathf.Abs(y - center.y) - innerHeight / 2f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    float alpha = Mathf.Clamp01(radius - dist);
                    
                    // Borde tallado suave
                    float borderDist = radius - dist; // Va de 0 (borde exterior) a radius (centro)
                    float borderThickness = 16f;
                    float borderBlend = Mathf.Clamp01(borderDist / borderThickness);
                    
                    // Añadir un bisel (bevel) brillante sutil en la parte superior
                    float bevel = (y > height - radius && dist < radius) ? 0.1f * Mathf.Clamp01((height - y) / radius) : 0f;

                    Color finalColor = Color.Lerp(darkWood, innerColor, borderBlend);

                    // Estilo Profesor Layton: Ribete dorado/bronce interior
                    if (borderDist > 5f && borderDist < 11f) 
                    {
                        Color gold = new Color(0.85f, 0.68f, 0.35f, 1f);
                        // Mezcla suave para antialiasing
                        float goldBlend = Mathf.Clamp01(1f - Mathf.Abs(borderDist - 8f) / 3f);
                        finalColor = Color.Lerp(finalColor, gold, goldBlend * 0.95f);
                    }
                    else if (borderDist >= 11f && borderDist < 14f)
                    {
                        // Sombra interior sutil del ribete dorado
                        float shadowBlend = Mathf.Clamp01(1f - Mathf.Abs(borderDist - 12.5f) / 1.5f);
                        finalColor = Color.Lerp(finalColor, darkWood, shadowBlend * 0.6f);
                    }

                    finalColor.r += bevel; finalColor.g += bevel; finalColor.b += bevel;
                    finalColor.a = alpha;

                    pixels[y * width + x] = finalColor;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }
    }
}
