using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SVGImporter;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public int score;
    public Text scoreText;
    public static LevelManager instance;

    [HideInInspector]
    public BaseEntity player;

    public Image healthImage;

    public GameObject weaponUIList;
    public GameObject weaponUIElement;

    public Text curWaveText;
    public Enemy[] enemies;
    public PoolManager poolManager;
    public List<SpawnObject> spawnObjects;
    public List<SpawnObject> spawnEffects;
    public GameObject[] effects;
    public int totalEnmiesInWave;

    public float enemySpawnTime;
    public float waveCooldownTimer;
    private int _skySpawner;
    private int _groundSpawner;
    private int _weaponSpawner;
    public int _wave;
    public bool spawnNextWave = false;

    public SVGImage[] wepImages;

    [System.Serializable]
    public struct Enemy
    {
        public GameObject prefab;
        public int rarity;
        public int minWave;
        public enemySpawnerType spawnerType;
    }

	void Awake ()
    {
        instance = this;
        spawnNextWave = true;
	    poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        spawnObjects = new List<SpawnObject>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseEntity>();
        InitializeEnemySpawners();
        InitializeEffectSpawners();
    }

    void Start() {
        //InitializeWeaponSpawner();
        NextWave();
    }

    #region SpawnInitializer

    //private void InitializeWeaponSpawner() {
    //    SpawnHandlerDetails sph = new SpawnHandlerDetails() {
    //        initialSpawnAmount = 1,
    //        overflowMaxSpawnAmount = 12,
    //        setPoolManagerParent = true
    //    };
    //    string tag = "WeaponSpawner";

    //    GameObject obj = new GameObject(tag);
    //    obj.tag = tag;
    //    _weaponSpawner = CreateSpawnHandlers(tag, sph);
    //    Debug.Log("Triggered with #" + _weaponSpawner);


    //    foreach (var wep in weapons) {
    //        AddWeaponToSpawnPool(wep.gameObject);
    //    }
    //}

    //private void AddWeaponToSpawnPool(GameObject weapon) {
    //    weaponSpawnObjects.Add(poolManager.AddToSpawnPool(weapon, _weaponSpawner));
    //}

    private void InitializeEnemySpawners() {
        SpawnHandlerDetails sph = new SpawnHandlerDetails() {
            initialSpawnAmount = 5,
            overflowMaxSpawnAmount = 10,
            setPoolManagerParent = true
        };
        _skySpawner = CreateSpawnHandlers(enemySpawnerType.SkySpawner.ToString(), sph);
        _groundSpawner = CreateSpawnHandlers(enemySpawnerType.GroundSpawner.ToString(), sph);

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

    private void InitializeEffectSpawners()
    {
        for(int i = 0; i < effects.Length; i ++)
        {
            SpawnObject sObj = new SpawnObject();
            sObj = poolManager.AddToSpawnPool(effects[i], false);
            spawnEffects.Add(sObj);
        }
    }
	
    private int CreateSpawnHandlers(string tag, SpawnHandlerDetails shd) {
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
    #endregion

    void Update () {
        scoreText.text = score.ToString();
        if(spawnNextWave) {
            curWaveText.text = _wave.ToString();
            NextWave();
        }

        if (totalEnmiesInWave <= 0 && Time.time >= waveCooldownTimer)
            spawnNextWave = true;
	}

    private void NextWave() {
        StartCoroutine(SpawnObjects());
    }

    int range, current;
    float randomEnemy;
    private IEnumerator SpawnObjects()
    {
        _wave++;
        spawnNextWave = false;
        waveCooldownTimer = 99999999999;

        for (int l = 0; l < (_wave^4); l++)
        {
            yield return new WaitForSeconds(enemySpawnTime);
            SpawnEnemy();
        }
        waveCooldownTimer = Time.time + 2;
    }

    public void SpawnEnemy()
    {
        poolManager.Spawn(spawnObjects.ElementAt(GetRandomEnemy()));
        totalEnmiesInWave++;
    }

    public int GetRandomEnemy()
    {
        int newEnemy = 0;
        range = 0;
        for (int i = 0; i < enemies.Count(); i++)
            range += enemies[i].rarity;

        randomEnemy = Random.Range(0.0f, range);
        current = 0;
        for (int i = 0; i < enemies.Count(); i++)
        {
            current += enemies[i].rarity;
            if (randomEnemy < current)
            {
                if (_wave >= enemies[i].minWave)
                {
                    newEnemy = i;
                }
                else
                {
                    newEnemy = GetRandomEnemy();
                }
            }
        }
        return newEnemy;
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
public enum enemySpawnerType {
    SkySpawner = 0,
    GroundSpawner = 1
}