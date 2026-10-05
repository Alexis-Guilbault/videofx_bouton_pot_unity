//
// ScreenBend - Rubber band screen punch/pinch effect with spring physics
//
// Combines full-screen bending rendering with damped harmonic oscillator animation.
//

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/Screen Bend")]
public class ScreenBend : MonoBehaviour
{
    #region Parameters


    // Click/touch position in UV space (0-1, bottom-left origin)
    [SerializeField]
    private Vector2 bendPosition = new Vector2(0.5f, 0.5f);

    // Animation Settings
    [Header("Animation Physics")]
    [SerializeField, Range(10f, 300f)]
    private float _springConstant = 30f; // Stiffness of the spring (higher = snappier)

    [SerializeField, Range(0.01f, 0.99f)]
    private float _dampingRatio = 0.15f; // Lower = more wobbles (0.01 = infinite wobble, 0.99 = critically damped)
    #endregion

    #region Private Properties

    private Shader _shader;
    private Material _material;

    // Runtime Physics State
    private float _currentBend = 0f;
    private float _currentVelocity = 0f;
    private bool _isAnimating = false;

    #endregion

    #region Lifecycle & Initialization

    void Awake()
    {
        if (_shader == null)
        {
            _shader = Shader.Find("Effects/ScreenBend");

            if (_shader == null)
            {
                Debug.LogError(
                    "[ScreenBend] Shader 'Effects/ScreenBend' not found! Ensure the shader is in Assets/Effects/ScreenBend.shader or similar."
                );
            }
        }
    }

    void OnEnable()
    {
        CreateMaterialIfNeeded();
    }

    void OnDisable()
    {
        if (_material != null)
        {
            if (!Application.isPlaying)
                DestroyImmediate(_material);
            else
                Destroy(_material);

            _material = null;
        }
    }

    void OnDestroy()
    {
        if (_material != null)
        {
            DestroyImmediate(_material);
        }
    }

    void Update()
    {
        if (_isAnimating)
        {
            // Damped Harmonic Oscillator: F = -k*x - c*v
            // Target is always 0 (return to flat screen) unless manually overridden
            float target = 0f;
            float displacement = _currentBend - target;

            // Spring force pulls toward target
            float springForce = -_springConstant * displacement;

            // Damping force opposes velocity
            float dampingForce = -_dampingRatio * _currentVelocity;

            // Acceleration (mass assumed = 1)
            float acceleration = springForce + dampingForce;

            // Integrate
            _currentVelocity += acceleration * Time.deltaTime;
            _currentBend += _currentVelocity * Time.deltaTime;

            // Stop condition: nearly settled
            if (Mathf.Abs(_currentBend) < 0.005f && Mathf.Abs(_currentVelocity) < 0.01f)
            {
                _currentBend = 0f;
                _currentVelocity = 0f;
                _isAnimating = false;
            }
        }
    }

    #endregion

    #region Material Management

    void CreateMaterialIfNeeded()
    {
        if (_material == null && _shader != null)
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
    }

    #endregion

    #region Rendering

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CreateMaterialIfNeeded();

        if (_material != null)
        {
            // Update shader with current bend state (animated or manual)
            _material.SetFloat("_BendAmount", _currentBend);
            _material.SetVector(
                "_CenterPosition",
                new Vector4(bendPosition.x, bendPosition.y, 0, 0)
            );

            Graphics.Blit(source, destination, _material);
        }
        else
        {
            // Fallback: passthrough without effect
            Graphics.Blit(source, destination);
        }
    }

    #endregion

    #region Public Control Methods

    public void SetBendAmount(float value)
    {
        // Setting this manually stops any active animation and snaps to value
        StopAnimation();
        _currentBend = Mathf.Clamp(value, -1f, 1f);
    }

    /// <summary>
    /// Stops any ongoing animation and resets the bend to 0.
    /// </summary>
    public void StopAnimation()
    {
        _isAnimating = false;
        _currentBend = 0f;
        _currentVelocity = 0f;
    }

    /// <summary>
    /// Adds a "punch" impulse to the current bend.
    /// Positive values push the screen outward, negative inward.
    /// Starts/resumes animation if it was stopped.
    /// </summary>
    /// <param name="force">The magnitude of the punch (-1 to 1)</param>
    public void Punch(float force = 0.7f)
    {

        // Add force to current state (allow stacking if desired, though usually starts from 0)
        _currentBend += force;
        _currentBend = Mathf.Clamp(_currentBend, -1f, 1f);



        _isAnimating = true;
    }


    #endregion
}

/* //
// FullScreenBend - Rubber band screen punch effect
//
// Minimal controller with automatic shader loading
//

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/Screen Bend")]
public class ScreenBend : MonoBehaviour
{
    #region Parameters

    // Bend intensity (0 = no bend, 1 = maximum)
    [SerializeField, Range(-1f, 1f)]
    private float _bendAmount = 0f;

    public float bendAmount {
        get { return _bendAmount; }
        set { _bendAmount = value; }
    }

    // Click/touch position in UV space (0-1, bottom-left origin)
    [SerializeField]
    private Vector2 bendPosition = new Vector2(0.5f, 0.5f);

    public Vector2 centerPosition {
        get { return bendPosition; }
        set { bendPosition = value; }
    }

    #endregion

    #region Private Properties

  
    private Shader _shader;

    private Material _material;

    #endregion

    #region Shader Loading

    void Awake()
    {
        // Automatically load the shader if not assigned in inspector
        if (_shader == null)
        {
            _shader = Shader.Find("Effects/ScreenBend");
            
            if (_shader == null)
            {
                Debug.LogError("[ScreenBend] Shader 'Effects/ScreenBend' not found!");
            }
        }
    }

    #endregion

    #region Material Management

    void OnEnable()
    {
        CreateMaterialIfNeeded();
    }

    void OnDisable()
    {
        if (_material != null)
        {
            if (!Application.isPlaying)
                DestroyImmediate(_material);
            else
                Destroy(_material);
            
            _material = null;
        }
    }

    void OnDestroy()
    {
        if (_material != null)
        {
            DestroyImmediate(_material);
        }
    }

    void CreateMaterialIfNeeded()
    {
        if (_material == null && _shader != null)
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
    }

    #endregion

    #region Rendering

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CreateMaterialIfNeeded();

        if (_material != null)
        {
            // Pass current parameters to shader
            _material.SetFloat("_BendAmount", _bendAmount);
            _material.SetVector("_CenterPosition",
                new Vector4(bendPosition.x, bendPosition.y, 0, 0));

            Graphics.Blit(source, destination, _material);
        }
        else
        {
            // Fallback: passthrough without effect
            Graphics.Blit(source, destination);
        }
    }

    #endregion
} */
