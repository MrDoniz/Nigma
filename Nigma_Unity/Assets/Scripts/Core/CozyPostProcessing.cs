using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace Nigma.Core
{
    public class CozyPostProcessing : MonoBehaviour
    {
        void Start()
        {
            // Intentar aplicar post-processing si URP esta disponible
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                // Configurar camara mediante reflection para evitar errores si no hay URP
                var uCamData = mainCam.GetComponent("UniversalAdditionalCameraData");
                if (uCamData != null)
                {
                    var prop = uCamData.GetType().GetProperty("renderPostProcessing");
                    if (prop != null) prop.SetValue(uCamData, true);
                }
            }

            // Crear un volumen global
            var volumeObj = new GameObject("GlobalVolume_Cozy");
            volumeObj.transform.SetParent(transform);
            var volume = volumeObj.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            // Intentar anadir Bloom y Vignette por reflection para evitar fallos de dependencias
            AddEffect(profile, "UnityEngine.Rendering.Universal.Bloom", out object bloom);
            if (bloom != null)
            {
                SetOverride(bloom, "intensity", 1.5f);
                SetOverride(bloom, "scatter", 0.7f);
                SetOverride(bloom, "tint", new Color(1f, 0.9f, 0.7f)); // Calido
            }

            AddEffect(profile, "UnityEngine.Rendering.Universal.Vignette", out object vignette);
            if (vignette != null)
            {
                SetOverride(vignette, "intensity", 0.45f);
                SetOverride(vignette, "smoothness", 0.8f);
                SetOverride(vignette, "color", new Color(0.1f, 0.05f, 0.0f)); // Marron oscuro
            }

            AddEffect(profile, "UnityEngine.Rendering.Universal.ColorAdjustments", out object ca);
            if (ca != null)
            {
                SetOverride(ca, "postExposure", 0.5f);
                SetOverride(ca, "contrast", 15f);
                SetOverride(ca, "saturation", 10f);
            }
        }

        private void AddEffect(VolumeProfile profile, string typeName, out object effectObj)
        {
            effectObj = null;
            System.Type type = System.Type.GetType(typeName + ", Unity.RenderPipelines.Universal.Runtime");
            if (type != null)
            {
                var method = profile.GetType().GetMethod("Add", new System.Type[] { typeof(bool) });
                if (method != null)
                {
                    // VolumeProfile.Add<T>(bool overrides) no es tan fácil de llamar por reflection porque es genérico.
                    // En su lugar usamos Add(Type type, bool overrides)
                    var addTypeMethod = profile.GetType().GetMethod("Add", new System.Type[] { typeof(System.Type), typeof(bool) });
                    if (addTypeMethod != null)
                    {
                        effectObj = addTypeMethod.Invoke(profile, new object[] { type, true });
                    }
                }
            }
        }

        private void SetOverride(object effect, string paramName, object value)
        {
            var field = effect.GetType().GetField(paramName);
            if (field != null)
            {
                var param = field.GetValue(effect);
                if (param != null)
                {
                    var prop = param.GetType().GetProperty("value");
                    if (prop != null) prop.SetValue(param, value);
                    
                    var over = param.GetType().GetProperty("overrideState");
                    if (over != null) over.SetValue(param, true);
                }
            }
        }
    }
}
