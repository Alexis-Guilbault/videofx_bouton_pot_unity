using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoFrameScrubber : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Slider slider;

    public bool debugMode = false;

    private long currentFrame = 0;

    private void Start()
    {
        videoPlayer.playOnAwake = false;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;

        slider.onValueChanged.AddListener(SetFrame);
    }

    private void OnVideoPrepared(VideoPlayer player)
    {
        if (debugMode)
            Debug.Log($"Video prepared. Total frames: {videoPlayer.frameCount}");

        // Slider represents 0% → 100% of the video.
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

        SetFrame(0f);
    }

    private void SetFrame(float value)
    {
        if (!videoPlayer.isPrepared)
            return;

        long frame = (long)(value * (videoPlayer.frameCount - 1));
        if (frame != currentFrame)
        {
            currentFrame = frame;
            if (debugMode)
                Debug.Log($"Setting frame to {frame} (value: {value})");
            videoPlayer.frame = frame;
        }

    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(SetFrame);
    }
}