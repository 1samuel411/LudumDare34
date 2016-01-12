using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    public Text curWaveText;
    public Enemy[] enemies;
    public PoolManager poolManager;
    public List<SpawnObject> spawnObjects;

    private float cooldownTimer;
    private int _skySpawner;
    private int _groundSpawner;
    private int _wave;
    public bool spawnNextWave = false;

    [System.Serializable]
    public struct Enemy
    {
        public GameObject prefab;
        // 1 is super common, 0 is super rare
        public float rarity;
        public enemySpawnerType spawnerType;
    }

	void Awake ()
    {
        instance = this;
	    poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        spawnObjects = new List<SpawnObject>();
        InitializeSpawners();
    }

    private void InitializeSpawners() {
        SpawnHandlerDetails sph = new SpawnHandlerDetails() {
            initialSpawnAmount = 5,
            overflowMaxSpawnAmount = 10,
            setPoolManagerParent = true
        };
        _skySpawner = CreateEnemySpawnHandlers(enemySpawnerType.SkySpawner.ToString(), sph);
        _groundSpawner = CreateEnemySpawnHandlers(enemySpawnerType.GroundSpawner.ToString(), sph);

        foreach(var enemy in enemies) {
            SpawnObject sObj = new SpawnObject();
            if (enemy.spawnerType == enemySpawnerType.GroundSpawner)
                sObj = poolManager.AddToSpawnPool(enemy.prefab, _groundSpawner);
            else if (enemy.spawnerType == enemySpawnerType.SkySpawner)
                sObj = poolManager.AddToSpawnPool(enemy.prefab, _skySpawner);
            spawnObjects.Add(sObj);
        }
        Debug.Log("Initialized was successful!");
    }
	
    private int CreateEnemySpawnHandlers(string tag, SpawnHandlerDetails shd) {
        GameObject[] spawnHandlers = GameObject.FindGameObjectsWithTag(tag);
        int value = -1;
        int cnt = 0;
        do {
            if(cnt == 0)
                value = poolManager.CreateNewSpawnHandler(spawnHandlers.First(), shd);
            else
                poolManager.AssignTransformToSpawnHandler(spawnHandlers[cnt], value);
            cnt++;
        } while (spawnHandlers.Count() > cnt);
        return value;
    }

	void Update () {
        if(spawnNextWave) {
            curWaveText.text = _wave.ToString();
            NextWave();
        }
	}

    private void NextWave() {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            spawnNextWave = false;
            int num = Random.Range(0, spawnObjects.Count());
            poolManager.Spawn(spawnObjects.ElementAt(num));
            yield return new WaitForSeconds(2.0f);
        }
    }
    //Need to load Weapon Types.
}
public enum enemySpawnerType {
    SkySpawner = 0,
    GroundSpawner = 1
}