using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerativeShaders : MonoBehaviour
{
    [SerializeField]
    Shader[] shaders = default;

    [SerializeField]
    Material material;

    int index = 0;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {

        if ( Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
        {
            index = (index + 1) % shaders.Length;
            material.shader = shaders[index];
        }
     }


}
