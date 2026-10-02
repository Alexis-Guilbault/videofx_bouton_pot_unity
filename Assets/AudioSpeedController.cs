using UnityEngine;


public class AudioSpeedController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
   

    public void SetSpeed(float value)
    {
        audioSource.pitch = value;
    }

}