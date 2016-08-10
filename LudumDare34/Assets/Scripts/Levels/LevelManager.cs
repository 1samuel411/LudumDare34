using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SVGImporter;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{

    public int score;
    public Text scoreText;
    public Text coinsText;
    public static LevelManager instance;
    public GameObject gameInfoPrefab;
    public Slider levelBar;

    [HideInInspector]
    public PlayerController player;

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
    public int enemiesKilled;
    public int coins;
    private int _skySpawner;
    private int _bossSpawner;
    private int _groundSpawner;
    private int _weaponSpawner;
    public int _wave;
    public bool spawnEnemies;
    public bool spawnNextWave = false;
    public bool moveOn = true;

    public SVGImage[] wepImages;

    public GameObject coinObj;
    public GameObject explosionObj;
    public GameObject burnObj;
    public SpawnObject explosionSpawnObj;
    public SpawnObject burnSpawnObj;
    public SpawnObject coinsSpawnObj;

    public bool wepsEnabled = true;
    public List<int> boughtItems = new List<int>();
    public int ammoAddition;
    public float timeAddition;
    public int boostDamageAddition;
    public int pistolDamageAddition;
    public int healthAddition;

    public int maxLevel = 25;

    public AudioSource musicPlayer;

    private Transform bossSpawner;

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
	    coins = 0; //Int32.Parse(InfoManager.GetInfo("coins"));
        spawnNextWave = false;
	    poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        bossSpawner = GameObject.FindGameObjectWithTag("BossSpawner").transform;
        spawnObjects = new List<SpawnObject>();
        coinsSpawnObj = poolManager.AddToSpawnPool(coinObj);
        explosionSpawnObj = poolManager.AddToSpawnPool(explosionObj);
        burnSpawnObj = poolManager.AddToSpawnPool(burnObj);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        InitializeEnemySpawners();
        InitializeEffectSpawners();

        musicPlayer = GameObject.Find("moosic").GetComponent<AudioSource>();
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
        _bossSpawner = CreateSpawnHandlers(enemySpawnerType.BossSpawner.ToString(), sph);

        foreach (var enemy in enemies) {
            SpawnObject sObj = new SpawnObject();
            if (enemy.spawnerType == enemySpawnerType.GroundSpawner)
                sObj = poolManager.AddToSpawnPool(enemy.prefab, _groundSpawner);
            else if (enemy.spawnerType == enemySpawnerType.SkySpawner)
                sObj = poolManager.AddToSpawnPool(enemy.prefab, _skySpawner);
            else if (enemy.spawnerType == enemySpawnerType.BossSpawner)
                sObj = poolManager.AddToSpawnPool(enemy.prefab, _bossSpawner);
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
        coinsText.text = coins.ToString();
        if(spawnNextWave) {
            NextWave();
        }
        curWaveText.text = _wave.ToString();

        levelBar.value = _wave / 25.0f; 

        if (totalEnmiesInWave <= 0 && Time.time >= waveCooldownTimer && moveOn)
            spawnNextWave = true;
	}

    private void NextWave() {
        StartCoroutine(SpawnObjects());
    }

    int range, current;
    float randomEnemy;
    private IEnumerator SpawnObjects()
    {
        if (!spawnEnemies)
            yield return null;

        //while (Tutorial.instance.finishedTutorial == false)
        //{
        //    waveCooldownTimer = Time.time + 1;
        //    yield return null;
        //}
        _wave++;
        spawnNextWave = false;
        waveCooldownTimer = 99999999999;

        if (_wave == maxLevel)
        {
            SpawnEnemy(enemies.Count() - 1);
        }
        else
        {
            for (int l = 0; l < (_wave ^ 4); l++)
            {
                yield return new WaitForSeconds(enemySpawnTime);
                SpawnEnemy();
            }
        }
        waveCooldownTimer = Time.time + 2;
    }

    public void SpawnEnemy(int enemy = -1)
    {
        if (!spawnEnemies)
            return;
        if(enemy != -1)
        {
            bossSpawner.position = player.transform.position + new Vector3((player.transform.position.x < 0) ? 5 : -5, 15);
        }
        poolManager.Spawn(spawnObjects.ElementAt((enemy == -1) ? GetRandomEnemy() : enemy));
        totalEnmiesInWave++;
        Debug.Log("Spawning Object");
    }

    public void SpawnEnemyRegardless()
    {
        totalEnmiesInWave++;
        poolManager.Spawn(spawnObjects.ElementAt(GetRandomEnemy()));
    }

    public int GetRandomEnemy()
    {
        int newEnemy = 0;
        range = 0;
        for (int i = 0; i < enemies.Count() - 1; i++)
            range += enemies[i].rarity;

        randomEnemy = UnityEngine.Random.Range(0.0f, range);
        current = 0;
        for (int i = 0; i < enemies.Count() -1; i++)
        {
            current += enemies[i].rarity;
            if (randomEnemy < current)
            {
                if (_wave >= enemies[i].minWave)
                {
                    if(enemies[i].minWave == maxLevel)
                        newEnemy = GetRandomEnemy();
                    else
                        newEnemy = i;
                }
                else
                {
                    newEnemy = GetRandomEnemy();
                }
                break;
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

    public void LoadLevel(string levelName, bool keepInfo, bool fade = false)
    {
        if (keepInfo)
        {
            GameObject info = Instantiate(gameInfoPrefab) as GameObject;
            DontDestroyOnLoad(info);
            GameInfo gameInfo = info.GetComponent<GameInfo>();
            gameInfo.coins = coins;
            gameInfo.enemiesKilled = enemiesKilled;
            gameInfo.score = score;
            gameInfo.wave = _wave;
            gameInfo.lastLevel = SceneManager.GetActiveScene().name;
        }
        if (fade)
        {
            StartCoroutine(FadeOutAndLoad(levelName));
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
    }

    public void LoadMainMenuFromGame()
    {
        Popup.Create("Return to Menu?", "If you leave you will lose all data that has not been saved!", "Accept", "Return", false, CallbackLoadMenuFromGame);
    }

    void CallbackLoadMenuFromGame(Popup.ResponseTypes response)
    {
        if(response == Popup.ResponseTypes.Accepted)
            LoadLevel("mainMenu", false, true);
    }

    IEnumerator FadeOutAndLoad(string levelName)
    {
        CameraManager.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(levelName);
    }
    //Need to load Weapon Types.
}
public enum enemySpawnerType {
    SkySpawner = 0,
    GroundSpawner = 1,
    BossSpawner = 2
}