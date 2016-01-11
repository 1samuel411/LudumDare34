using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnHandler: MonoBehaviour {
    private GameObject _spawnHandler;
    private int _spawnHandlerKey;
    private bool _isKeyAdded = false;
    private bool _invokedInstantiateAll = false;
    public GameObject spawnHandler { get { return _spawnHandler; } }
    public List<GameObject> spawnLocations; 
    public PoolManager poolManager;
    public int spawnHandlerKey {get { return _spawnHandlerKey; } }
    public int maxSpawnAmount;
    public int maxActiveUnits { get { return gameObject.GetComponentsInChildren<SpawnObject>(true).Count(); } }

    public void Awake() {
        spawnLocations = new List<GameObject>();
        poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        _spawnHandler = this.gameObject;
        maxSpawnAmount = 5;
    }

    public void AddKey(int key)
    {
        if (!_isKeyAdded) {
            _spawnHandlerKey = key;
            _isKeyAdded = true;
        }
    }

    public void AddSpawnerLocation(GameObject obj) {
        spawnLocations.Add(obj);
    }

    public void AddSpawnerLocation() {
        AddSpawnerLocation(this.gameObject);
    }

    public void SpawnObject(KeyValuePair<int, SpawnObject> spawnObj) {
        //Get All InactiveGameObjects from the SpawnHandler.
        IEnumerable<SpawnObject> spawnObjs = gameObject.GetComponentsInChildren<SpawnObject>(true)
                                                .Where(s => s.gameObject.activeSelf.Equals(false) &&
                                                    s.spawnObjectKey.Equals(spawnObj.Key));

        if (spawnObjs.Any()) {
            int num = UnityEngine.Random.Range(0, spawnLocations.Count);
            spawnObjs.First().ActivateObject(spawnLocations.ElementAt(num).transform.position);
        } else {
            Debug.Log("Could not find an inactive SpawnObject in collection, instantiating new object.");
            if(_invokedInstantiateAll)
                InstantiateObject(spawnObj.Value);
            else
                InstantiateAllObjects(spawnObj.Value);
        }
    }

    public void InstantiateObject(SpawnObject obj) {
            SpawnObject gObj = GameObject.Instantiate(obj, this.transform.position, Quaternion.identity) as SpawnObject;
            gObj.transform.SetParent(_spawnHandler.transform);
            gObj.SetSpawnObject(obj.spawnObjectKey, this);
            gObj.gameObject.SetActive(false);
    }

    public void InstantiateAllObjects(SpawnObject obj) {
        for (int i = 0; i < maxSpawnAmount; i++)
            InstantiateObject(obj);
        _invokedInstantiateAll = true;
    }
}
