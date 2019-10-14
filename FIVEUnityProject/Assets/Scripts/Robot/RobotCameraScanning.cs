using UnityEngine;
using FIVE.EventSystem;
using static FIVE.Util;

namespace FIVE.Robot
{
    public class OnGlobalScanRequested : IEventType
    {
    }

    public class OnGlobalScanFinished : IEventType
    {
    }

    public class RobotCameraScanning : MonoBehaviour
    {
        private bool isScanning;
        private MaterialPropertyBlock propBlock;
        private Material scanningEffectMaterial;

        private float timer = 4f;

        void Awake()
        {
            propBlock = new MaterialPropertyBlock();

            scanningEffectMaterial = Resources.Load<Material>("Materials/Custom/ScanningEffectMaterial");
            scanningEffectMaterial = new Material(scanningEffectMaterial);
            scanningEffectMaterial.SetColor("_Color", Color.white);
            scanningEffectMaterial.SetFloat("_Intensity", 0);
            Subscribe<OnGlobalScanRequested>(DoScan);
        }

        private void DoScan()
        {
            isScanning = true;
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, scanningEffectMaterial);
        }

        void Update()
        {
            if (isScanning)
            {
                scanningEffectMaterial.SetFloat("_Intensity", timer / 2);
                scanningEffectMaterial.SetFloat("_ElapsedTime", Time.realtimeSinceStartup * 4f + 12f);
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = 4f;
                    isScanning = false;
                    this.RaiseEvent<OnGlobalScanFinished>();
                }
            }
        }
    }
}