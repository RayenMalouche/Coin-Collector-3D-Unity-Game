using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSetup : MonoBehaviour
{
    public Material groundMaterial;
    public GameObject groundPlane;
    public Texture2D groundTexture;
    
    void Start()
    {
        SetupGround();
        AddEnvironmentProps();
    }
    
    void SetupGround()
    {
        if (groundPlane != null)
        {
            // Scale the ground to be larger
            groundPlane.transform.localScale = new Vector3(20, 1, 20);
            
            // Apply material
            Renderer groundRenderer = groundPlane.GetComponent<Renderer>();
            if (groundRenderer != null && groundMaterial != null)
            {
                groundRenderer.material = groundMaterial;
            }
            
            // Ensure it has collider
            if (groundPlane.GetComponent<Collider>() == null)
            {
                groundPlane.AddComponent<BoxCollider>();
            }
            
            groundPlane.tag = "Ground";
            groundPlane.layer = LayerMask.NameToLayer("Ground");
        }
    }
    
    void AddEnvironmentProps()
    {
        // Add some environmental objects
        AddBoundaryWalls();
        AddScatteredRocks();
    }
    
    void AddBoundaryWalls()
    {
        float groundSize = 200f; // 20 scale * 10 unit plane
        
        // Create four walls around the play area
        CreateWall(new Vector3(0, 2, groundSize/2), new Vector3(groundSize, 4, 1)); // North
        CreateWall(new Vector3(0, 2, -groundSize/2), new Vector3(groundSize, 4, 1)); // South
        CreateWall(new Vector3(groundSize/2, 2, 0), new Vector3(1, 4, groundSize)); // East
        CreateWall(new Vector3(-groundSize/2, 2, 0), new Vector3(1, 4, groundSize)); // West
    }
    
    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.name = "BoundaryWall";
        
        // Make it visually distinct
        Renderer renderer = wall.GetComponent<Renderer>();
        renderer.material.color = new Color(0.3f, 0.3f, 0.3f);
        
        // Ensure it has collision
        wall.AddComponent<Rigidbody>();
        Rigidbody rb = wall.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Walls don't move
    }
    
    void AddScatteredRocks()
    {
        // Add some random rocks for visual interest
        for (int i = 0; i < 10; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-80f, 80f),
                0.5f,
                Random.Range(-80f, 80f)
            );
            
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.transform.position = position;
            rock.transform.localScale = Vector3.one * Random.Range(0.5f, 2f);
            rock.name = "EnvironmentRock";
            
            Renderer renderer = rock.GetComponent<Renderer>();
            renderer.material.color = new Color(0.4f, 0.4f, 0.4f);
            
            // Make rocks static (no physics)
            rock.isStatic = true;
        }
    }
}