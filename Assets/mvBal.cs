using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mvBal : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 5f; // Jump power
    private Rigidbody rb;
    public float fallThreshold = -5f;
    private bool hasFallen = false;
    private bool isGrounded = true;

    // Speed boost variables
    private float originalSpeed;
    private bool isSpeedBoosted = false;
    private Coroutine speedBoostCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Add a Rigidbody to the Player!");
        }
        hasFallen = false;
        
        // Store original speed for speed boost functionality
        originalSpeed = speed;
    }

    void Update()
    {
        // Only allow movement if not fallen
        if (!hasFallen)
        {
            float horizontal = Input.GetAxis("Horizontal") * speed;
            float vertical = Input.GetAxis("Vertical") * speed;

            Vector3 move = new Vector3(horizontal, 0, vertical);
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            
            // Jump with space key
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                Debug.Log("Player jumped!");
            }
        }

        // Check for falling off
        if (transform.position.y < fallThreshold && !hasFallen)
        {
            hasFallen = true;
            Debug.Log("Player fell off the edge!");
            
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
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if landing on ground
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        // Stay grounded while on ground
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        // Leave ground
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }
    
    void OnEnable()
    {
        hasFallen = false;
        isGrounded = true;
    }

    // Speed boost method called by PotionCollector
    public void ApplySpeedBoost(float boostAmount, float duration)
    {
        // If already boosted, stop the current coroutine to reset the duration
        if (isSpeedBoosted && speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine(boostAmount, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float duration)
    {
        isSpeedBoosted = true;
        speed = originalSpeed + boostAmount; // Apply the boost

        Debug.Log($"Speed boosted! New speed: {speed}");

        yield return new WaitForSeconds(duration);

        // Revert to original speed
        speed = originalSpeed;
        isSpeedBoosted = false;
        Debug.Log("Speed boost ended.");
    }
}