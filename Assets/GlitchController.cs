using UnityEngine;
using UnityEngine.UI;

public class GlitchController : MonoBehaviour
{
    [SerializeField] private Kino.AnalogGlitch analogGlitch;

    [Header("Maximum Glitch Values")]
    [SerializeField, Range(0f, 1f)] private float maxScanLineJitter = 1f;
    [SerializeField, Range(0f, 1f)] private float maxVerticalJump = 1f;
    [SerializeField, Range(0f, 1f)] private float maxColorDrift = 1f;

    private void Start()
    {
        
    }

    public void SetGlitch(float value)
    {
        analogGlitch.scanLineJitter = value * value * maxScanLineJitter;
        analogGlitch.verticalJump = value * value *maxVerticalJump;
        analogGlitch.colorDrift = value * value * maxColorDrift;
    }

}