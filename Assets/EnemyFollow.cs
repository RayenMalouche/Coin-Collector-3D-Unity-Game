using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float targetHeight = 1.5f; // Desired Y position
    public float heightReturnSpeed = 2f; // How fast to return to target height
    public float maxHeightDeviation = 0.5f; // Start correcting if more than this far from target
    private Rigidbody rb;
    private bool hasCollided = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hasCollided = false;
        
        // Constrain rotation on X and Z axes to prevent tipping
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Enemy found player: " + playerObj.name);
            }
            else
            {
                Debug.LogError("Enemy cannot find player with tag 'Player'");
            }
        }
        
        // Set initial height
        Vector3 pos = transform.position;
        pos.y = targetHeight;
        transform.position = pos;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Always correct height if deviating from target
        float currentHeight = transform.position.y;
        float heightDifference = targetHeight - currentHeight;
        
        // If height is significantly off, smoothly correct it
        float verticalVelocity = 0f;
        if (Mathf.Abs(heightDifference) > 0.1f)
        {
            verticalVelocity = Mathf.Clamp(heightDifference * heightReturnSpeed, -5f, 5f);
            Debug.Log($"Enemy correcting height: Current={currentHeight:F2}, Target={targetHeight}, VelY={verticalVelocity:F2}");
        }
        
        if (player != null && !hasCollided)
        {
            // Calculate horizontal direction to player
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0; // Only move horizontally
            Vector3 direction = toPlayer.normalized;
            
            // Set velocity with height correction
            rb.velocity = new Vector3(
                direction.x * speed, 
                verticalVelocity, 
                direction.z * speed
            );
            
            // Make enemy look at player (only Y-axis rotation)
            if (toPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer);
                targetRotation.x = 0;
                targetRotation.z = 0;
                transform.rotation = targetRotation;
            }
        }
        else
        {
            // Just correct height when not chasing
            rb.velocity = new Vector3(0, verticalVelocity, 0);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");
        
        // Only react to player collision, ignore obstacles
        if (collision.gameObject.CompareTag("Player") && !hasCollided)
        {
            hasCollided = true;
            Debug.Log("Enemy caught the player!");
            
            ScoreManager manager = FindObjectOfType<ScoreManager>();
            if (manager != null)
            {
                manager.Lose();
            }
            else
            {
                Debug.LogError("ScoreManager not found!");
            }
        }
        // For obstacles, just let physics and height correction handle it
    }
    
    void OnEnable()
    {
        hasCollided = false;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Reset to target height when enabled
        Vector3 pos = transform.position;
        pos.y = targetHeight;
        transform.position = pos;
    }
}