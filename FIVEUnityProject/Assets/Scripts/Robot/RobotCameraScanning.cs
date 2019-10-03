using UnityEngine;

namespace FIVE.Robot
{
    public class RobotCameraScanning : MonoBehaviour
    {
        private Material scanningEffectMaterial;
        private MaterialPropertyBlock propBlock;
        void Awake()
        {
            propBlock = new MaterialPropertyBlock();

            scanningEffectMaterial = Resources.Load<Material>("Materials/Custom/ScanningEffectMaterial");
            scanningEffectMaterial = new Material(scanningEffectMaterial);
            scanningEffectMaterial.SetColor("_Color", Color.white);
        }
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {

            Graphics.Blit(src, dest, scanningEffectMaterial);
        }

        void Update()
        {
            scanningEffectMaterial.SetFloat("_Intensity", 1f);
            scanningEffectMaterial.SetFloat("_ElapsedTime", Time.realtimeSinceStartup * 4f);
        }
    }
}
