using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnHandler: MonoBehaviour {
    private int _spawnerKey;
    private GameObject _spawnHandler;
    public GameObject spawnHandler { get { return _spawnHandler; } }
    public int spawnerKey { get { return _spawnerKey; } }
    public SpawnerSettings handlerSettings;
    public PoolManager poolManager;

    void Awake() {
        poolManager = this.gameObject.transform.parent.GetComponent<PoolManager>();
    }

    public void SetSpawnHandler(GameObject obj, int key) {
        _spawnHandler = obj;
        _spawnerKey = key;
    }

    public void SetSpawnHandler(int key) {
        SetSpawnHandler(this.gameObject, key);
    }

    public void SpawnObject(SpawnObject obj) {
        SpawnObject[] spawnObj = gameObject.GetComponentsInChildren<SpawnObject>(true);
        IEnumerable<SpawnObject> inactiveSpawnObj = spawnObj.Where(s => s.gameObject.activeSelf.Equals(false));

        if(inactiveSpawnObj.Count() > 0) {
            SpawnObject sObj = inactiveSpawnObj.First();
            sObj.ActivateObject();
            handlerSettings.currentActiveUnits++;
            handlerSettings.currentDeactiveUnits--;
        } else {
            handlerSettings.maxSpawnAmount++;
            Instantiate(obj, this.transform.position, Quaternion.identity);
        }
    }

    private void InstantiateObject(SpawnObject obj) {
        for (int i = 0; i <= handlerSettings.maxSpawnAmount; i++) {
            GameObject.Instantiate(obj, this.transform.position, Quaternion.identity);
            obj.gameObject.SetActive(false);
            obj.gameObject.transform.SetParent(this.transform);
        }
        
    }
}
