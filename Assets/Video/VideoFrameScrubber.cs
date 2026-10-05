using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoFrameScrubber : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    private ulong currentFrame = 0;
    private ulong frameCount = 0;

    private void Prepare()
    {
        videoPlayer.Prepare();
    }

    public void SetVideoClip(VideoClip clip)
    {
        videoPlayer.playOnAwake = true;
        videoPlayer.clip = clip;
        Prepare();
    }

    private void Start()
    {
        

        videoPlayer.prepareCompleted += OnVideoPrepared;

        Prepare();
    }

    private void OnVideoPrepared(VideoPlayer player)
    {
        videoPlayer.playOnAwake = false;
        
        Debug.Log($"Video prepared. Total frames: {videoPlayer.frameCount}");

        float previousProgression = 0f;
        if (frameCount > 0)
        {
            previousProgression = (float)currentFrame / (float)frameCount;
        }

        frameCount = videoPlayer.frameCount;
        // Set the initial frame to 0
        SetProgression(previousProgression);
    }

    public void SetProgression(float value)
    {
        if (!videoPlayer.isPrepared)
            return;

        ulong frame = (ulong)(value * (videoPlayer.frameCount - 1));
        if (frame != currentFrame)
        {
            currentFrame = frame;
            // if (debugMode)
            //     Debug.Log($"Setting frame to {frame} (value: {value})");
            videoPlayer.frame = (long)frame;
        }
    }

    private void OnDestroy() { }
}
