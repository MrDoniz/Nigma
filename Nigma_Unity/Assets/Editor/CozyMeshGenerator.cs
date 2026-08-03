using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Nigma.Editor
{
    public class CozyMeshGenerator : EditorWindow
    {
        [MenuItem("Nigma/3. Generar Modelos 3D Cozy (Arte)", false, 3)]
        public static void GenerateCozyMeshes()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Models"))
                AssetDatabase.CreateFolder("Assets", "Models");

            Mesh pegDoll = GeneratePegDoll();
            SaveMesh(pegDoll, "CozyPegDoll");

            Mesh cozySofa = GenerateRoundedBox(new Vector3(1.2f, 0.5f, 0.6f), 0.15f);
            SaveMesh(cozySofa, "CozySofa");

            Mesh cozyBed = GenerateRoundedBox(new Vector3(1.0f, 0.3f, 1.6f), 0.1f);
            SaveMesh(cozyBed, "CozyBed");

            EditorUtility.DisplayDialog("Modelos Generados", "Se han creado los modelos 3D cozy en Assets/Models.", "OK");
        }

        private static void SaveMesh(Mesh mesh, string name)
        {
            string path = $"Assets/Models/{name}.asset";
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                existing.Clear();
                EditorUtility.CopySerialized(mesh, existing);
                AssetDatabase.SaveAssets();
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
        }

        // Generates a Peg Doll (Pawn) using a Lathe (Surface of Revolution)
        private static Mesh GeneratePegDoll()
        {
            // Profile curve (Height Y, Radius X)
            Vector2[] profile = new Vector2[]
            {
                new Vector2(0f, 0f),      // Bottom center
                new Vector2(0.35f, 0f),   // Bottom edge
                new Vector2(0.33f, 0.15f),
                new Vector2(0.28f, 0.35f),
                new Vector2(0.22f, 0.5f),
                new Vector2(0.1f, 0.62f), // Neck
                new Vector2(0.25f, 0.7f), // Chin
                new Vector2(0.3f, 0.85f), // Head middle
                new Vector2(0.25f, 0.98f),
                new Vector2(0.15f, 1.05f),
                new Vector2(0f, 1.08f)    // Top center
            };

            int segments = 24; // smooth roundness
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Generate vertices
            for (int p = 0; p < profile.Length; p++)
            {
                float radius = profile[p].x;
                float height = profile[p].y;

                for (int s = 0; s <= segments; s++)
                {
                    float angle = (float)s / segments * Mathf.PI * 2f;
                    float x = Mathf.Sin(angle) * radius;
                    float z = Mathf.Cos(angle) * radius;
                    verts.Add(new Vector3(x, height, z));
                    uvs.Add(new Vector2((float)s / segments, (float)p / (profile.Length - 1)));
                }
            }

            // Generate triangles
            for (int p = 0; p < profile.Length - 1; p++)
            {
                for (int s = 0; s < segments; s++)
                {
                    int current = p * (segments + 1) + s;
                    int next = current + segments + 1;

                    tris.Add(current);
                    tris.Add(next + 1);
                    tris.Add(next);

                    tris.Add(current);
                    tris.Add(current + 1);
                    tris.Add(next + 1);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "PegDoll";
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        // Generates a simple rounded box (bevel) by subdividing a sphere and flattening the sides
        private static Mesh GenerateRoundedBox(Vector3 size, float radius)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Mesh sphere = temp.GetComponent<MeshFilter>().sharedMesh;
            
            Vector3[] verts = sphere.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 v = verts[i];
                // Push sphere vertices outwards to form a rounded box
                float hx = (size.x * 0.5f) - radius; if (hx < 0) hx = 0;
                float hy = (size.y * 0.5f) - radius; if (hy < 0) hy = 0;
                float hz = (size.z * 0.5f) - radius; if (hz < 0) hz = 0;

                v *= radius; // scale sphere down to corner radius
                
                // Translate corners to box dimensions
                v.x += (verts[i].x > 0 ? hx : -hx);
                v.y += (verts[i].y > 0 ? hy : -hy);
                v.z += (verts[i].z > 0 ? hz : -hz);
                
                verts[i] = v;
            }

            Mesh box = new Mesh();
            box.name = "RoundedBox";
            box.vertices = verts;
            box.triangles = sphere.triangles;
            box.normals = sphere.normals;
            box.uv = sphere.uv;
            box.RecalculateNormals();
            box.RecalculateBounds();

            DestroyImmediate(temp);
            return box;
        }
    }
}
