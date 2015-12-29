using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    public Text curWaveText;
    public Enemy[] enemies;
    public int wave = 0;
    public int enemyMultiplyer;
    public int enemiesInWave;
    public int currentEnemiesInWave;
    public int totalEnmiesInWave;
    public int specialRound;
    public int enemyLimit;
    public float spawnCooldownInterval;

    private float cooldownTimer;

    [System.Serializable]
    public struct Enemy
    {
        public Transform[] spawnLocations;
        public GameObject prefab;
        // 1 is super common, 0 is super rare
        public float rarity;
    }

	void Awake ()
    {
        instance = this;
	}
	
	void Update ()
    {
        curWaveText.text = wave.ToString();
        if(currentEnemiesInWave <= 0 && totalEnmiesInWave <= 0)
        {
            NextWave();
        }
	}   

    public void NextWave()
    {
        wave++;
        if (wave <= 0 && specialRound <= 0)
        {
            if (wave % specialRound == 0)
            {
                // special wave
            }
        }
        else
        {
            // Reg wave
            totalEnmiesInWave = 0;
            enemiesInWave = wave * enemyMultiplyer;
            enemyLimit = enemiesInWave / 2;
            currentEnemiesInWave = enemiesInWave;
            StartCoroutine(SpawnEnemies());
        }
    }

    public IEnumerator SpawnEnemies()
    {
        for(int i = 0; i < enemiesInWave; i++)
        {
            cooldownTimer = Time.time + spawnCooldownInterval;
            while(Time.time < cooldownTimer)
            {
                yield return null;
            }

            while (totalEnmiesInWave >= enemyLimit)
                yield return null;

            totalEnmiesInWave++;
            currentEnemiesInWave--;

            int selectedEnemy = 0;
            bool selectedEnemeyYet = false;
            float selectedRarity = Random.Range(0f, 1.0f);
            // Spawn enemy
            for(int x = 0; x < enemies.Length; x++)
            {
                if (selectedRarity < enemies[x].rarity)
                {
                    selectedEnemeyYet = true;
                    selectedEnemy = x;
                }
            }
            if(!selectedEnemeyYet)
            {
                int randomEnemy = Random.Range(0, enemies.Length);
                selectedEnemy = randomEnemy;
                selectedEnemeyYet = true;
            }

            // spawn enemy
            SpawnEnemy(selectedEnemy);
        }
    }

    public void SpawnEnemy(int e)
    {
        GameObject enemySpawned = Instantiate(enemies[e].prefab, enemies[e].spawnLocations[Random.Range(0, enemies[e].spawnLocations.Length)].position, Quaternion.identity) as GameObject;
        BaseHealth enemySpawnedHealth = enemySpawned.GetComponent<BaseHealth>();
        enemySpawnedHealth.currentHealth = wave * 2;
    }

    //Need to load Weapon Types.
}