using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnHandler: MonoBehaviour {
    private GameObject _spawnHandler;
    public GameObject spawnHandler { get { return _spawnHandler; } }
    public List<GameObject> spawnLocations; 
    public PoolManager poolManager;
    public int maxSpawnAmount;
    public int maxActiveUnits { get { return gameObject.GetComponentsInChildren<SpawnObject>(true).Count(); } }

    void Awake() {
        poolManager = this.gameObject.transform.parent.GetComponent<PoolManager>();
        _spawnHandler = this.gameObject;
    }

    public void AddSpawnerLocation(GameObject obj) {
        spawnLocations.Add(obj);
    }

    public void AddSpawnerLocation() {
        AddSpawnerLocation(this.gameObject);
    }

    public void SpawnObject(SpawnObject obj) {
        //Get All InactiveGameObjects from the SpawnHandler.
        IEnumerable<SpawnObject> spawnObjs = gameObject.GetComponentsInChildren<SpawnObject>(true)
                                                .Where(s => s.gameObject.activeSelf.Equals(false) &&
                                                s.gameObject.Equals(obj.gameObject));

        if (spawnObjs.Any()) {
            int num = UnityEngine.Random.Range(1, spawnLocations.Count);
            spawnObjs.First().ActivateObject(spawnLocations.ElementAt(num).transform.position);
        } else {
            Debug.Log("Could not find an inactive SpawnObject in collection, instantiating new object.");
            InstantiateObject(obj);
        }
    }

    public void InstantiateObject(SpawnObject obj) {
            GameObject.Instantiate(obj, this.transform.position, Quaternion.identity);
            obj.gameObject.transform.SetParent(this.transform);
            obj.gameObject.SetActive(false);
    }

    public void InstantiateAll(SpawnObject obj) {
        for (int i = 0; i < maxSpawnAmount; i++)
            InstantiateObject(obj);
    }
}
