using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnObject: MonoBehaviour {
    private int _spawnerKey = 0;
    private GameObject _spawn;
    public GameObject spawn { get { return _spawn; } }
    public int spawnerKey { get { return _spawnerKey; } }

    public void SetSpawnObject(GameObject obj, int spawnerKey) {
        _spawn = obj;
        _spawnerKey = spawnerKey;
    }

    public void SetSpawnObject(int spawnerKey) {
        SetSpawnObject(this.gameObject, spawnerKey);
    }

    public void DeactivateObject()
    {
        if (_spawn.gameObject.activeSelf)
        {
            //_spawn.transform.parent == 
        }
    }
}