//
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
    private Vector2 _centerPosition = new Vector2(0.5f, 0.5f);

    public Vector2 centerPosition {
        get { return _centerPosition; }
        set { _centerPosition = value; }
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
                new Vector4(_centerPosition.x, _centerPosition.y, 0, 0));

            Graphics.Blit(source, destination, _material);
        }
        else
        {
            // Fallback: passthrough without effect
            Graphics.Blit(source, destination);
        }
    }

    #endregion
}