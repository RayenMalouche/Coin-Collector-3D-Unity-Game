using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab; // Assign your bronze_coin prefab here
    public float spawnY = 1f; // Adjust to be slightly above the plane
    public float spawnRange = 80f; // Range for x and z

    void Start()
    {
        SpawnCoins(70);
    }

    public void SpawnCoins(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnRange, spawnRange), 
                spawnY, 
                Random.Range(-spawnRange, spawnRange)
            );
            
            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);
            
            // AUTO-FIX: Ensure the coin is setup correctly
            Debug.Log("Setting up coin: " + coin.name);
            
            // Set tag
            coin.tag = "Coin";
            
            // Make sure collider exists and is a trigger
            Collider col = coin.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
                Debug.Log("Coin collider set to trigger");
            }
            else
            {
                // Add sphere collider if missing
                SphereCollider sphereCol = coin.AddComponent<SphereCollider>();
                sphereCol.isTrigger = true;
                sphereCol.radius = 0.5f;
                Debug.Log("Added sphere collider to coin");
            }
            
            // Add CoinCollector script if missing
            if (coin.GetComponent<CoinCollector>() == null)
            {
                coin.AddComponent<CoinCollector>();
                Debug.Log("Added CoinCollector script");
            }
            
            // Add CoinRotation script if missing
            if (coin.GetComponent<CoinRotation>() == null)
            {
                CoinRotation rotation = coin.AddComponent<CoinRotation>();
                rotation.rotationSpeed = 100f;
                Debug.Log("Added CoinRotation script");
            }
        }
        
        Debug.Log("Spawned " + count + " coins with auto-fix");
    }

    public void CoinCollected()
    {
        SpawnCoins(1);
    }
}