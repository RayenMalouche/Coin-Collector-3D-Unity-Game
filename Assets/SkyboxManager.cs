using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material skyboxMaterial;
    
    void Start()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
        
        // Optional: Enable fog for atmosphere
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.5f, 0.8f, 1f);
        RenderSettings.fogDensity = 0.02f;
    }
}