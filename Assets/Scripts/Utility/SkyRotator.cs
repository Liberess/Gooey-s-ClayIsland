using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotator : MonoBehaviour
{
    private void Update()
    {
        try
        {

        }
        catch
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.5f);
        }
    }
}