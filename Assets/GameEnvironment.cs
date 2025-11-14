using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvironment : MonoBehaviour
{
    [Header("Environment Settings")]
    public Material skyboxMaterial;
    public Material groundMaterial;
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public int numberOfTrees = 20;
    public int numberOfRocks = 15;
    public float environmentRadius = 80f;
    
    [Header("Lighting")]
    public Light directionalLight;
    public Color ambientLight = new Color(0.4f, 0.4f, 0.4f);
    
    void Start()
    {
        SetupLighting();
        SetupSkybox();
        CreateTerrain();
        PopulateEnvironment();
    }
    
    void SetupLighting()
    {
        // Set up directional light (sun)
        if (directionalLight != null)
        {
            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            directionalLight.color = new Color(1f, 0.95f, 0.8f);
            directionalLight.intensity = 1f;
        }
        
        // Set ambient lighting
        RenderSettings.ambientLight = ambientLight;
        RenderSettings.ambientIntensity = 1f;
    }
    
    void SetupSkybox()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
        
        // Enable fog for depth
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.015f;
        RenderSettings.fogColor = new Color(0.7f, 0.8f, 1f);
    }
    
    void CreateTerrain()
    {
        // Create ground plane
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(20, 1, 20);
        ground.transform.position = Vector3.zero;
        
        // Setup ground material and physics
        Renderer groundRenderer = ground.GetComponent<Renderer>();
        if (groundMaterial != null)
        {
            groundRenderer.material = groundMaterial;
        }
        else
        {
            groundRenderer.material.color = new Color(0.2f, 0.6f, 0.2f); // Green grass
        }
        
        ground.tag = "Ground";
        ground.layer = LayerMask.NameToLayer("Ground");
        
        // Add some texture variation
        AddGroundDetails(ground);
    }
    
    void AddGroundDetails(GameObject ground)
    {
        // This could be expanded with actual texture blending
        // For now, we'll just add some color variation
        Renderer renderer = ground.GetComponent<Renderer>();
        renderer.material.mainTextureScale = new Vector2(10, 10);
    }
    
    void PopulateEnvironment()
    {
        // Create boundary
        CreateBoundary();
        
        // Add trees if prefab exists
        if (treePrefab != null)
        {
            for (int i = 0; i < numberOfTrees; i++)
            {
                Vector3 position = GetRandomEnvironmentPosition();
                GameObject tree = Instantiate(treePrefab, position, Quaternion.identity);
                tree.name = "EnvironmentTree_" + i;
                tree.transform.SetParent(this.transform);
            }
        }
        
        // Add rocks if prefab exists
        if (rockPrefab != null)
        {
            for (int i = 0; i < numberOfRocks; i++)
            {
                Vector3 position = GetRandomEnvironmentPosition();
                GameObject rock = Instantiate(rockPrefab, position, Quaternion.identity);
                rock.name = "EnvironmentRock_" + i;
                rock.transform.SetParent(this.transform);
                
                // Randomize rock scale and rotation
                float scale = Random.Range(0.5f, 1.5f);
                rock.transform.localScale = Vector3.one * scale;
                rock.transform.rotation = Random.rotation;
            }
        }
        
        // Add some decorative objects
        AddDecorativeObjects();
    }
    
    Vector3 GetRandomEnvironmentPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * environmentRadius;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }
    
    void CreateBoundary()
    {
        float boundaryHeight = 3f;
        float boundaryThickness = 1f;
        
        // Create four boundary walls
        string[] directions = { "North", "South", "East", "West" };
        Vector3[] positions = {
            new Vector3(0, boundaryHeight/2, environmentRadius),
            new Vector3(0, boundaryHeight/2, -environmentRadius),
            new Vector3(environmentRadius, boundaryHeight/2, 0),
            new Vector3(-environmentRadius, boundaryHeight/2, 0)
        };
        Vector3[] scales = {
            new Vector3(environmentRadius * 2, boundaryHeight, boundaryThickness),
            new Vector3(environmentRadius * 2, boundaryHeight, boundaryThickness),
            new Vector3(boundaryThickness, boundaryHeight, environmentRadius * 2),
            new Vector3(boundaryThickness, boundaryHeight, environmentRadius * 2)
        };
        
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "BoundaryWall_" + directions[i];
            wall.transform.position = positions[i];
            wall.transform.localScale = scales[i];
            
            // Visual setup
            Renderer renderer = wall.GetComponent<Renderer>();
            renderer.material.color = new Color(0.7f, 0.7f, 0.7f);
            
            // Physics setup
            Rigidbody rb = wall.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            wall.transform.SetParent(this.transform);
        }
    }
    
    void AddDecorativeObjects()
    {
        // Add some simple decorative objects (cubes as placeholders)
        for (int i = 0; i < 8; i++)
        {
            Vector3 position = GetRandomEnvironmentPosition();
            
            GameObject decoration = GameObject.CreatePrimitive(PrimitiveType.Cube);
            decoration.name = "Decoration_" + i;
            decoration.transform.position = position;
            decoration.transform.localScale = new Vector3(2f, 0.5f, 2f);
            
            Renderer renderer = decoration.GetComponent<Renderer>();
            renderer.material.color = new Color(0.8f, 0.6f, 0.2f);
            
            decoration.isStatic = true;
            decoration.transform.SetParent(this.transform);
        }
    }
}