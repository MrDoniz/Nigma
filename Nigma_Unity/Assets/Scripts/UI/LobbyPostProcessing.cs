using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nigma.UI
{
    public class LobbyPostProcessing : MonoBehaviour
    {
        private Volume volume;

        private void Start()
        {
            SetupPostProcessing();
        }

        private void SetupPostProcessing()
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var uCamData = mainCam.GetComponent<UniversalAdditionalCameraData>();
                if (uCamData == null) uCamData = mainCam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                uCamData.renderPostProcessing = true;
            }

            volume = gameObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 100;

            VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            if (!profile.Has<DepthOfField>())
            {
                var dof = profile.Add<DepthOfField>(true);
                dof.active = true;
                dof.mode.Override(DepthOfFieldMode.Gaussian);
                dof.gaussianStart.Override(1f);
                dof.gaussianEnd.Override(15f);
                dof.gaussianMaxRadius.Override(1.5f);
            }
            
            if (!profile.Has<Vignette>())
            {
                var vignette = profile.Add<Vignette>(true);
                vignette.active = true;
                vignette.intensity.Override(0.35f);
                vignette.smoothness.Override(0.8f);
                vignette.color.Override(new Color(0.1f, 0.05f, 0.05f));
            }
        }
    }
}
