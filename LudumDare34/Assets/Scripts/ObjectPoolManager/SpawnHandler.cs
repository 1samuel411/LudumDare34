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
    public SpawnHandlerSettings handlerSettings;

    public void SetSpawnHandler(GameObject obj, int key) {
        _spawnHandler = obj;
        _spawnerKey = key;
    }

    public void SetSpawnHandler(int key) {
        SetSpawnHandler(this.gameObject, key);
    }

    public void SpawnObject(SpawnObject obj) {
        SpawnObject[] spawnObjects = gameObject.GetComponentsInChildren<SpawnObject>(true);
        SpawnObject deactiveObject = spawnObjects.FirstOrDefault(s => !s.gameObject.activeSelf);
        if (deactiveObject != null) {
            deactiveObject.gameObject.SetActive(true);

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
