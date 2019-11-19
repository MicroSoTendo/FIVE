using TMPro;
using UnityEngine;

public class CameraControlled : MonoBehaviour
{
    private TextMeshPro text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = name;
        text.ForceMeshUpdate();
        SetText(false);
    }

    private void OnPreRender()
    {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("FaceCamera"))
        {
            o.transform.LookAt(o.transform.position + transform.rotation * Vector3.forward, transform.rotation * Vector3.up);
        }
        SetText();
    }

    private void OnPostRender()
    {
        SetText(false);
    }

    private void OnDisable()
    {
        SetText(false);
    }

    private void SetText(bool e = true)
    {
        if (text != null)
        {
            text.enabled = e;
        }
    }

    public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0));
    private Texture2D _texture;
    private float _alpha, _time;

    public void ResetFade()
    {
        _alpha = 1;
        _time = 0;
    }

    private void OnGUI()
    {
        if (_alpha > 0 && Event.current.type.Equals(EventType.Repaint))
        {
            if (_texture == null)
            {
                _texture = new Texture2D(1, 1);
            }

            _texture.SetPixel(0, 0, new Color(0, 0, 0, _alpha));
            _texture.Apply();

            _time += Time.deltaTime;
            _alpha = FadeCurve.Evaluate(_time);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture, ScaleMode.StretchToFill);
        }
    }
}