using System;
using UnityEngine;

namespace FIVE.CameraSystem
{
    public class NoiseScreen : MonoBehaviour
    {
        private Material material;
        private int width;
        private int height;
        private void Awake()
        {
            var shader = Resources.Load<Shader>("Shaders/NoiseScreen");
            material = new Material(shader);
            material.color = Color.white;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest,  material);
        }

        private void Update()
        {
        }
    }
}
