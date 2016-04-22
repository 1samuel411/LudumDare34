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
    public int spawnHandlerKey { get { return _spawnHandlerKey; } }
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
        if(!_isKeyAdded) {
            _spawnHandlerKey = key;
            _isKeyAdded = true;
            initialSpawnAmount = (handlerDetails.initialSpawnAmount > 0) ? handlerDetails.initialSpawnAmount : 5;
            overFlowMaxSpawnAmount = (handlerDetails.overflowMaxSpawnAmount > initialSpawnAmount)
                ? handlerDetails.overflowMaxSpawnAmount : initialSpawnAmount * 2;
            //overFlowMaxSpawnAmount = 150;
        }
    }

    public void AddSpawnerLocation(GameObject obj) {
        spawnLocations.Add(obj);
    }

    public void AddSpawnerLocation() {
        AddSpawnerLocation(this.gameObject);
    }

    public SpawnObject SpawnObject(KeyValuePair<int, SpawnObject> spawnObj, Transform location = null) {
        SpawnObject sObj = null;
        //Get All InactiveGameObjects from the SpawnHandler.
        IEnumerable<SpawnObject> spawnObjs = gameObject.GetComponentsInChildren<SpawnObject>(true)
                                                .Where(s => s.gameObject.activeSelf.Equals(false) &&
                                                    s.spawnObjectKey.Equals(spawnObj.Key));

        if(spawnObjs.Any()) {
            if(location == null) {
                int num = UnityEngine.Random.Range(0, spawnLocations.Count);
                //Vector3 pos = spawnLocations.ElementAt(num).transform.position;
                Transform trans = spawnLocations.ElementAt(num).transform;
                sObj = spawnObjs.First();
                sObj.ActivateObject(trans);
            } else {
                sObj = spawnObjs.First();
                sObj.ActivateObject(location);
            }
        } else if(overFlowMaxSpawnAmount > _curSpawnAmount) {
            if(_invokedInstantiateAll)
                InstantiateObject(spawnObj.Value);
            else
                InstantiateAllObjects(spawnObj.Value);
            if (overFlowMaxSpawnAmount == _curSpawnAmount)
                overFlowMaxSpawnAmount++;
            sObj = SpawnObject(spawnObj, location);
        } else {
        }
        return sObj;
    }

    public void InstantiateObject(SpawnObject obj) {
        SpawnObject gObj = GameObject.Instantiate(obj, this.transform.position, Quaternion.identity) as SpawnObject;
        gObj.transform.SetParent(_spawnHandler.transform);
        gObj.SetSpawnObject(obj.spawnObjectKey, this);
        gObj.DeactivateObject();
        _curSpawnAmount++;
    }

    public void InstantiateAllObjects(SpawnObject obj) {
        for(int i = 0;i < initialSpawnAmount;i++)
            InstantiateObject(obj);
        _invokedInstantiateAll = true;
    }
}
