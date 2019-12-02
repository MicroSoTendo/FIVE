using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.UI
{
    public class ScrollView3D : MonoBehaviour
    {
        public Camera originCamera;					// Camera that renders the whole UI
        public Camera targetCamera;                 // Camera to overlay over the RectTransform
        private RectTransform scrollRectTransform;	// Target RectTransform for size
        private void Awake()
        {
            scrollRectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!scrollRectTransform || !originCamera || !targetCamera)
            {
                return;
            }
            var corners = new Vector3[4];
            scrollRectTransform.GetWorldCorners(corners);
            //	1 ------- 2
            //	|    +    |
            //	0 ------- 3
            Vector3 BL = originCamera.WorldToScreenPoint(new Vector3(corners[0].x, corners[0].y, 0));
            Vector3 TL = originCamera.WorldToScreenPoint(new Vector3(corners[1].x, corners[1].y, 0));
            Vector3 TR = originCamera.WorldToScreenPoint(new Vector3(corners[2].x, corners[2].y, 0));
            Vector3 BR = originCamera.WorldToScreenPoint(new Vector3(corners[3].x, corners[3].y, 0));

            var finalRect = new Rect
            {
                x = BL.x / Screen.width,
                y = BL.y / Screen.height,
                width = (TR.x - TL.x) / Screen.width,
                height = (TR.y - BR.y) / Screen.height
            };
            targetCamera.rect = finalRect;
            targetCamera.orthographicSize = (corners[2].y - corners[3].y) / 2f;
        }
    }
}