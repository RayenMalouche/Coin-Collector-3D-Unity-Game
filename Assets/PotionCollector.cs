using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCollector : MonoBehaviour
{
    public float speedBoost = 4f;
    public float boostDuration = 4f;
    public float rotationSpeed = 100f;
    
    void Update()
    {
        // Rotate the potion for visual effect
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Potion collected!");
            
            mvBal playerScript = other.GetComponent<mvBal>();
            if (playerScript != null)
            {
                playerScript.ApplySpeedBoost(speedBoost, boostDuration);
            }
            
            Destroy(gameObject);
        }
    }
}