using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int baseHealth = 10;
    private int totalEnemies = 0;
    private int killedEnemies = 0;
    private bool gameEnded = false;
    private bool allWavesCompleted = false;
    public GameObject gameOverCanvas;
    public GameObject victoryCanvas;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy()
    {
        totalEnemies++;
        Debug.Log($"Enemy spawned, total: {totalEnemies}");
    }

    public void EnemyKilled()
    {
        killedEnemies++;
        Debug.Log($"Enemy killed, total kills: {killedEnemies}/{totalEnemies}");

        CheckVictory();
    }

    public void EnemyReachedBase()
    {
        baseHealth--;
        Debug.Log($"Enemy reached base, remaining health: {baseHealth}");
        if (baseHealth <= 0)
        {
            GameOver(false);
        }
    }

    public void AllWavesComplete()
    {
        allWavesCompleted = true;
        CheckVictory(); // Trigger final victory check
    }

    void CheckVictory()
    {
        if (allWavesCompleted && killedEnemies >= totalEnemies)
        {
            GameOver(true);
        }
    }

    void GameOver(bool victory)
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;

        if (victory)
        {
            Debug.Log("Victory!");
            victoryCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("Game Over!");
            gameOverCanvas.SetActive(true);
        }
    }
}
