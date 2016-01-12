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
    public int initialSpawnAmount;
    public int maxActiveUnits { get { return gameObject.GetComponentsInChildren<SpawnObject>(true).Count(); } }
    public int overFlowMaxSpawnAmount;
    private int _curSpawnAmount;

    public void Awake() {
        spawnLocations = new List<GameObject>();
        poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        _spawnHandler = this.gameObject;
        initialSpawnAmount = 5;
        _curSpawnAmount = 0;
    }

    public void AddDetails(SpawnHandlerDetails handlerDetails, int key) {
        if (!_isKeyAdded) {
            _spawnHandlerKey = key;
            _isKeyAdded = true;
            initialSpawnAmount = (handlerDetails.initialSpawnAmount > 0) ? handlerDetails.initialSpawnAmount : 5;
            overFlowMaxSpawnAmount = (handlerDetails.overflowMaxSpawnAmount > initialSpawnAmount)
                ? handlerDetails.overflowMaxSpawnAmount : initialSpawnAmount*2;
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
        if (overFlowMaxSpawnAmount >= _curSpawnAmount) {
            SpawnObject gObj = GameObject.Instantiate(obj, this.transform.position, Quaternion.identity) as SpawnObject;
            gObj.transform.SetParent(_spawnHandler.transform);
            gObj.SetSpawnObject(obj.spawnObjectKey, this);
            gObj.gameObject.SetActive(false);
            _curSpawnAmount++;
        }
    }

    public void InstantiateAllObjects(SpawnObject obj) {
        for (int i = 0; i < initialSpawnAmount; i++)
            InstantiateObject(obj);
        _invokedInstantiateAll = true;
    }
}
