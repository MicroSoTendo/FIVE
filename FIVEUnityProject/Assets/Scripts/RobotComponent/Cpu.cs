using UnityEngine;

public class CPU : MonoBehaviour
{
    public int Speed; // instruction executed per frame
    public float PowerConsumption; // 0.0f ~ 1.0f;

    private void Start()
    {
        Speed = 1;
        PowerConsumption = 1.0f;
    }
}
