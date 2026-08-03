using UnityEngine;

namespace Nigma.Core
{
    public class Billboard : MonoBehaviour
    {
        private Camera mainCam;

        private void Start()
        {
            mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
                if (mainCam == null) return;
            }
            
            // Makes the sprite face the camera perfectly
            transform.rotation = mainCam.transform.rotation;
        }
    }
}
