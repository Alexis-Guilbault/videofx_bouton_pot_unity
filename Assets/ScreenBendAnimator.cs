using UnityEngine;

public class ScreenBendAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField, Range(-1f, 1f)]
    private float _targetBend = -0.7f;

    [Header("Physics Settings")]
    [SerializeField, Range(10f, 300f)]
    private float _springConstant = 30f;  // Stiffness of the spring
    
    [SerializeField, Range(0.9f, 0.999999f)]
    private float _dampingRatio = 0.15f;  // Lower = more wobbles (0 = infinite wobble)

        [SerializeField] private ScreenBend screenBend;

    [Header("Runtime")]

    private float currentBend = 0f;
    private float currentVelocity = 0f;
    private bool isAnimating = false;

    void Awake()
    {
  
    }

    void Update()
    {
        if (isAnimating)
        {
            // Damped harmonic oscillator (proper spring physics)
            // F = -k*x - c*v  (Hooke's law + viscous damping)
            float target = 0f;
            float displacement = currentBend - target;
            
            // Spring force pulls toward target
            float springForce = -_springConstant * displacement;
            
            // Damping force opposes velocity (reduces wobble over time)
            float dampingForce = -_dampingRatio * currentVelocity;
            
            // Total acceleration (assuming mass = 1 for simplicity)
            float acceleration = springForce + dampingForce;
            
            // Integrate: velocity += acceleration * dt
            currentVelocity += acceleration * Time.deltaTime;
            
            // Integrate: position += velocity * dt
            currentBend += currentVelocity * Time.deltaTime;
            
            // Stop when nearly settled (higher thresholds = faster stop)
            if (Mathf.Abs(currentBend) < 0.01f && Mathf.Abs(currentVelocity) < 0.02f)
            {
                currentBend = 0f;
                currentVelocity = 0f;
                isAnimating = false;
            }
            
            if (screenBend != null)
            {
                screenBend.bendAmount = currentBend;
            }
        }
    }

    public void TriggerBend()
    {
        currentBend = _targetBend;  // Instant snap to target
        currentVelocity = 0f;
        isAnimating = true;
        
        if (screenBend != null)
        {
            screenBend.bendAmount = currentBend;
        }
    }

    public void TriggerBendAt(float targetBend)
    {
        _targetBend = targetBend;
        TriggerBend();
    }

    public void StopBend()
    {
        isAnimating = false;
        currentBend = 0f;
        currentVelocity = 0f;
        
        if (screenBend != null)
        {
            screenBend.bendAmount = 0f;
        }
    }


}