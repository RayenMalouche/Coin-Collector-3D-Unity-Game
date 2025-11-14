using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Coin triggered by: " + other.gameObject.name + " with tag: " + other.tag);
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin collected by player!");
            
            ScoreManager manager = FindObjectOfType<ScoreManager>();
            if (manager != null)
            {
                manager.AddScore(1);
            }
            else
            {
                Debug.LogError("ScoreManager not found!");
            }
            
            CoinSpawner spawner = FindObjectOfType<CoinSpawner>();
            if (spawner != null)
            {
                spawner.CoinCollected();
            }
            
            Destroy(gameObject);
        }
    }
}