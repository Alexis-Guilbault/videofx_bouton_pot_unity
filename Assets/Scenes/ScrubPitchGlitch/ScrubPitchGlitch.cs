using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Slider
using UnityEngine.Video; // For VideoClip and VideoPlayer

public class ScrubPitchGlitch : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] audioClips;

    [Header("Video")]
    [SerializeField]
    private VideoFrameScrubber videoFrameScrubber;

    [SerializeField]
    private VideoClip[] videoClips;

    // Start is called before the first frame update
    [Header("Glitch")]
    [SerializeField]
    private Kino.AnalogGlitch analogGlitch;

    [SerializeField, Range(0f, 1f)]
    private float maxScanLineJitter = 1f;

    [SerializeField, Range(0f, 1f)]
    private float maxVerticalJump = 1f;

    [SerializeField, Range(0f, 1f)]
    private float maxColorDrift = 1f;

    private int index = 0;

    // Set the video frame and the audio pitch based on value (0.0f to 1.0f)
    public void Set(float value)
    {
        // Set audio pitch
        audioSource.pitch = value;

        // Set video frame
        videoFrameScrubber.SetProgression(value);

        // Set glitch effect
        analogGlitch.scanLineJitter = value * value * maxScanLineJitter;
        analogGlitch.verticalJump = value * value * maxVerticalJump;
        analogGlitch.colorDrift = value * value * maxColorDrift;
    }

    void Start()
    {
        // Add listener for audio speed controller
        slider.onValueChanged.AddListener(Set);
        Set(0);
        slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            index = (index + 1);

            audioSource.clip = audioClips[index % audioClips.Length];
            videoFrameScrubber.SetVideoClip(videoClips[index % videoClips.Length]);
            audioSource.Play();
        }
    }

    void OnDestroy()
    {
        // Remove the listeners when the object is destroyed to avoid memory leaks
        slider.onValueChanged.RemoveListener(Set);
    }
}
