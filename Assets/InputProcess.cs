using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputProcess : MonoBehaviour
{
     [SerializeField] private Slider slider;
     [SerializeField] private AudioSpeedController audioSpeedController;
     [SerializeField] private GlitchController glitchController;
     [SerializeField] private ScreenBendAnimator screenBendAnimator;
    // Start is called before the first frame update
    void Start()
    {
        // Add listener for audio speed controller
        slider.onValueChanged.AddListener(audioSpeedController.SetSpeed);
        audioSpeedController.SetSpeed(slider.value);
        
        // Add listener for glitch controller
        slider.onValueChanged.AddListener(glitchController.SetGlitch);
        glitchController.SetGlitch(slider.value);

    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButtonDown(0) )
        {
           screenBendAnimator.TriggerBend();
        }
    }

    void OnDestroy()
    {
        // Remove the listeners when the object is destroyed to avoid memory leaks
        slider.onValueChanged.RemoveListener(audioSpeedController.SetSpeed);
        slider.onValueChanged.RemoveListener(glitchController.SetGlitch);
    }
}
