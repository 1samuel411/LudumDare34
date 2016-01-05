using UnityEngine;
using System.Collections;
using System.Linq;
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
    public PoolManager poolManager;

    private float cooldownTimer;
    private int _skySpawner;
    private int _groundSpawner;


    [System.Serializable]
    public struct Enemy
    {
        public Transform[] spawnLocations;
        public GameObject prefab;
        // 1 is super common, 0 is super rare
        public float rarity;
        public enemySpawnerType spawnerType;
    }

	void Awake ()
    {
        instance = this;
	    poolManager = GameObject.FindGameObjectWithTag("Spawner").GetComponent<PoolManager>();

        InitializeSpawners();
    }

    private void InitializeSpawners() {
        _skySpawner = InitializeEnemySpawnHandlers(enemySpawnerType.SkySpawner.ToString());
        _groundSpawner = InitializeEnemySpawnHandlers(enemySpawnerType.GroundSpawner.ToString());

        foreach(var enemy in enemies) {
            if(enemy.spawnerType == enemySpawnerType.GroundSpawner)
                poolManager.AddToSpawnPool(enemy.prefab, _groundSpawner);
            else if(enemy.spawnerType == enemySpawnerType.SkySpawner)
                poolManager.AddToSpawnPool(enemy.prefab, _skySpawner);
        }
    }
	
    private int InitializeEnemySpawnHandlers(string tag) {
        GameObject[] spawnHandlers = GameObject.FindGameObjectsWithTag(tag);
        int value = -1;
        int cnt = 0;
        do {
            if(cnt == 0)
                value = poolManager.CreateNewSpawnHandler(spawnHandlers.First(), true);
            else
                poolManager.CreateNewSpawnHandler(spawnHandlers[cnt], value, true);
            cnt++;
        } while (spawnHandlers.Count() > cnt);
        return value;
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