using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.Interactive
{
    internal class RobotCursor : MonoBehaviour
    {
        private Texture2D cursorTexture;
        void Awake()
        {
            cursorTexture = Resources.Load<Texture2D>("Textures/UI/Cursor");
            Cursor.visible = true;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform objectHit = hit.transform;
                Pickable p = objectHit.gameObject.GetComponent<Pickable>();
                if (Input.GetMouseButtonDown(1))
                {
                    Debug.Log(objectHit.gameObject);
                    GameObject.Destroy(objectHit.gameObject);
                }
            }
        }
    }
}
