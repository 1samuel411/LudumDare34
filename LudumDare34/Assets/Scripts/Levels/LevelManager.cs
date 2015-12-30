using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    public Text curWaveText;
    public Entity[] enemies;
    public int wave = 0;
    public int enemyMultiplyer;
    public int enemiesInWave;
    public int currentEnemiesInWave;
    public int totalEnmiesInWave;
    public int specialRound;
    public int enemyLimit;
    public float spawnCooldownInterval;
    public Entity[] clouds;
    public float spawnCooldownIntervalClouds;

    private float cooldownTimer;
    private float cooldownTimerClouds;

    [System.Serializable]
    public struct Entity
    {
        public Transform[] spawnLocations;
        public GameObject prefab;
        // 1 is super common, 0 is super rare
        public float rarity;
    }

	void Awake ()
    {
        instance = this;
        StartCoroutine(SpawnClouds());
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
        for (int i = 0; i < enemiesInWave; i++)
        {
            cooldownTimer = Time.time + spawnCooldownInterval;
            while (Time.time < cooldownTimer)
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
            for (int x = 0; x < enemies.Length; x++)
            {
                if (selectedRarity < enemies[x].rarity)
                {
                    selectedEnemeyYet = true;
                    selectedEnemy = x;
                }
            }
            if (!selectedEnemeyYet)
            {
                int randomEnemy = Random.Range(0, enemies.Length);
                selectedEnemy = randomEnemy;
                selectedEnemeyYet = true;
            }

            // spawn enemy
            SpawnEnemy(selectedEnemy);
        }
    }

    public IEnumerator SpawnClouds()
    {
        cooldownTimerClouds = Time.time + spawnCooldownIntervalClouds;
        while (Time.time < cooldownTimerClouds)
        {
            yield return null;
        }

        int selectedCloud = 0;
        bool selectedCloudYet = false;
        float selectedRarity = Random.Range(0f, 1.0f);
        // Spawn enemy
        for (int x = 0; x < enemies.Length; x++)
        {
            if (selectedRarity < enemies[x].rarity)
            {
                selectedCloudYet = true;
                selectedCloud = x;
            }
        }
        if (!selectedCloudYet)
        {
            int randomEnemy = Random.Range(0, enemies.Length);
            selectedCloud = randomEnemy;
            selectedCloudYet = true;
        }

        // spawn enemy
        SpawnCloud(selectedCloud);
        StartCoroutine(SpawnClouds());
    }

    public void SpawnEnemy(int e)
    {
        GameObject enemySpawned = Instantiate(enemies[e].prefab, enemies[e].spawnLocations[Random.Range(0, enemies[e].spawnLocations.Length)].position, Quaternion.identity) as GameObject;
        BaseHealth enemySpawnedHealth = enemySpawned.GetComponent<BaseHealth>();
        enemySpawnedHealth.currentHealth = wave * 2;
    }

    public void SpawnCloud(int c)
    {
        GameObject cloudSpawned = Instantiate(clouds[c].prefab, clouds[c].spawnLocations[Random.Range(0, clouds[c].spawnLocations.Length)].position, Quaternion.identity) as GameObject;
    }

    //Need to load Weapon Types.
}