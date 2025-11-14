using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public GameObject potionPrefab; // Assign your potion prefab
    public float spawnInterval = 10f; // Spawn every 10 seconds
    public float spawnY = 1f;
    public float spawnRange = 8f;
    private bool isSpawning = false;
    
    void Start()
    {
        StartSpawning();
    }
    
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnPotionRoutine());
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
        
        // Destroy all existing potions
        GameObject[] potions = GameObject.FindGameObjectsWithTag("Potion");
        foreach (var potion in potions)
        {
            Destroy(potion);
        }
    }
    
    IEnumerator SpawnPotionRoutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            // Remove old potion if exists
            GameObject[] oldPotions = GameObject.FindGameObjectsWithTag("Potion");
            foreach (var oldPotion in oldPotions)
            {
                Destroy(oldPotion);
            }
            
            // Spawn new potion at random position
            Vector3 pos = new Vector3(
                Random.Range(-spawnRange, spawnRange),
                spawnY,
                Random.Range(-spawnRange, spawnRange)
            );
            
            GameObject potion = Instantiate(potionPrefab, pos, Quaternion.identity);
            
            // Auto-setup potion
            potion.tag = "Potion";
            
            // Ensure trigger collider
            Collider col = potion.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                SphereCollider sphere = potion.AddComponent<SphereCollider>();
                sphere.isTrigger = true;
                sphere.radius = 0.5f;
            }
            
            // Add collector script if missing
            if (potion.GetComponent<PotionCollector>() == null)
            {
                potion.AddComponent<PotionCollector>();
            }
            
            Debug.Log("Potion spawned at: " + pos);
        }
    }
}