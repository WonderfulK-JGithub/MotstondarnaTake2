using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkyBox : MonoBehaviour
{


    [SerializeField, Range(0f, 2f)] float rotationSpeed = 1f;
    [Range(0f, 2f)] public float exposure = 1f;

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", rotationSpeed * Time.time);
        RenderSettings.skybox.SetFloat("_Exposure", exposure);

    }
}
