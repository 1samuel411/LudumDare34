using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SVGImporter;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    public Image healthImage;

    public GameObject weaponUIList;
    public GameObject weaponUIElement;

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
                for (int y = 0; y < enemies.Length; y++)
                {
                    if (selectedRarity < enemies[x].rarity && enemies[y].rarity > enemies[x].rarity)
                    {
                        selectedEnemeyYet = true;
                        selectedEnemy = x;
                    }
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
        for (int x = 0; x < clouds.Length; x++)
        {
            for (int y = 0; y < clouds.Length; y++)
            {
                if (selectedRarity < clouds[x].rarity && clouds[y].rarity > clouds[x].rarity)
                {
                    selectedCloudYet = true;
                    selectedCloud = x;
                }
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
        if(enemySpawnedHealth.healthChanged)
            enemySpawnedHealth.currentHealth = wave * 2;
    }

    public void SpawnCloud(int c)
    {
        GameObject cloudSpawned = Instantiate(clouds[c].prefab, clouds[c].spawnLocations[Random.Range(0, clouds[c].spawnLocations.Length)].position, Quaternion.identity) as GameObject;
    }

    public void SpawnWeaponUI(SVGAsset weaponAsset, float scale, BaseWeapon reference, float rotation = 110.0f)
    {
        GameObject weaponUISpawned = Instantiate(weaponUIElement, Vector2.zero, Quaternion.identity) as GameObject;
        weaponUISpawned.transform.SetParent(weaponUIList.transform);
        WeaponElement weaponElement = weaponUISpawned.GetComponent<WeaponElement>();
        weaponElement.reference = reference;
        weaponElement.scale = scale;
        weaponElement.rotation = rotation;
        weaponElement.targetImage = weaponAsset;

        if (reference.name == "Pistol")
        {
            weaponUISpawned.transform.SetAsFirstSibling();
        }
        else
            weaponUISpawned.transform.SetSiblingIndex(1);
    }

    //Need to load Weapon Types.
}