using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public int currentLevel = 1;
    public int maxLevel = 10;
    
    public GameObject enemyPrefab; // The enemy prefab to clone
    public GameObject losePanel;
    public GameObject levelCompletePanel; // NEW: Panel for level complete
    public GameObject player;
    
    public Vector3 playerStartPos = new Vector3(0, 1, 0);
    public Vector3 enemyStartPos = new Vector3(40, 1, 40);
    public float enemySpawnRadius = 8f; // Radius around player to spawn enemies
    public float baseEnemySpeed = 3f;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool gameOver = false;
    private PotionSpawner potionSpawner;

    void Start()
    {
        // Load saved level or start at level 1
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        score = 0;
        gameOver = false;
        
        Debug.Log("=== GAME STARTING - LEVEL " + currentLevel + " ===");
        
        // Hide panels at start
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
        
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
        
        // Find or create potion spawner
        potionSpawner = FindObjectOfType<PotionSpawner>();
        
        Time.timeScale = 1;
        
        SetupLevel();
        UpdateUI();
    }

    void SetupLevel()
    {
        Debug.Log("Setting up Level " + currentLevel);
        
        // Reset player
        if (player != null)
        {
            player.transform.position = playerStartPos;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }
            
            mvBal playerScript = player.GetComponent<mvBal>();
            if (playerScript != null)
            {
                playerScript.enabled = true;
            }
        }

        // Clear existing enemies
        ClearEnemies();
        
        // Spawn enemies based on level
        if (currentLevel >= 2)
        {
            int enemyCount = currentLevel - 1; // Level 2 = 1 enemy, Level 3 = 2 enemies, etc.
            SpawnEnemies(enemyCount);
        }
        
        // Start potion spawner
        if (potionSpawner != null)
        {
            potionSpawner.StartSpawning();
        }
        
        ResetCoins();
    }

    void SpawnEnemies(int count)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab not assigned!");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            // Spawn enemies around the player at random positions
            Vector2 randomCircle = Random.insideUnitCircle * enemySpawnRadius;
            Vector3 spawnPos = playerStartPos + new Vector3(randomCircle.x, 0, randomCircle.y);
            spawnPos.y = 1.5f;
            
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            
            // Setup enemy
            EnemyFollow enemyScript = enemy.GetComponent<EnemyFollow>();
            if (enemyScript != null)
            {
                enemyScript.enabled = true;
                enemyScript.player = player.transform;
                // Increase speed based on level
                enemyScript.speed = baseEnemySpeed + (currentLevel - 2);
                Debug.Log($"Enemy {i+1} spawned with speed: {enemyScript.speed}");
            }
            
            activeEnemies.Add(enemy);
        }
        
        Debug.Log($"Spawned {count} enemies for Level {currentLevel}");
    }
    
    void ClearEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

    public void AddScore(int points)
    {
        if (gameOver) return;
        
        score += points;
        UpdateUI();
        
        Debug.Log("Score: " + score + " / " + GetWinScore());
        
        if (score >= GetWinScore())
        {
            // Level complete!
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        gameOver = true;
        Debug.Log($"=== LEVEL {currentLevel} COMPLETE! ===");
        
        // Stop everything
        StopPlayer();
        StopEnemies();
        
        if (potionSpawner != null)
        {
            potionSpawner.StopSpawning();
        }
        
        // Show level complete panel
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        
        // Check if game won
        if (currentLevel >= maxLevel)
        {
            if (levelText != null)
            {
                levelText.text = "YOU WIN!";
            }
        }
    }
    
    public void NextLevel()
    {
        Debug.Log("Moving to next level...");
        
        if (currentLevel >= maxLevel)
        {
            // Game completed! Reset to level 1
            PlayerPrefs.SetInt("CurrentLevel", 1);
        }
        else
        {
            // Progress to next level
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    int GetWinScore()
    {
        // Level i requires i * 5 coins
        return currentLevel * 5;
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score + " / " + GetWinScore();
        }
        
        if (levelText != null)
        {
            levelText.text = "Level " + currentLevel;
        }
    }
    
    public void Lose()
    {
        if (gameOver) return;
        
        gameOver = true;
        Debug.Log("=== GAME OVER! ===");
        
        // Move back one level (but not below 1)
        currentLevel = Mathf.Max(1, currentLevel - 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        Debug.Log($"Dropping to Level {currentLevel}");
        
        // Stop everything
        StopPlayer();
        StopEnemies();
        
        if (potionSpawner != null)
        {
            potionSpawner.StopSpawning();
        }
        
        // Show lose panel
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }
    
    void StopPlayer()
    {
        if (player != null)
        {
            mvBal playerScript = player.GetComponent<mvBal>();
            if (playerScript != null)
            {
                playerScript.enabled = false;
            }
            
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
            }
        }
    }
    
    void StopEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                EnemyFollow enemyScript = enemy.GetComponent<EnemyFollow>();
                if (enemyScript != null)
                {
                    enemyScript.enabled = false;
                }
                
                Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    enemyRb.velocity = Vector3.zero;
                }
            }
        }
    }
    
    public void RestartGame()
    {
        Debug.Log($"=== RESTARTING AT LEVEL {currentLevel} ===");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void ResetCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var coin in coins)
        {
            Destroy(coin);
        }
        
        CoinSpawner spawner = FindObjectOfType<CoinSpawner>();
        if (spawner != null)
        {
            int coinsToSpawn = GetWinScore();
            spawner.SpawnCoins(coinsToSpawn);
            Debug.Log($"Spawned {coinsToSpawn} coins for Level {currentLevel}");
        }
    }
    
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        RestartGame();
    }
}