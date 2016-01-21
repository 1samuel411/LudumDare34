using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class SpawnObject: MonoBehaviour
{
    private int _spawnObjectKey;
    public int spawnObjectKey {get { return _spawnObjectKey; } }
    private SpawnHandler _spawnHandler;
    public SpawnHandler spawnHandler { get { return _spawnHandler; } }

    public void SetSpawnObject(int spawnObjectKey, SpawnHandler handler) {
        _spawnObjectKey = spawnObjectKey;
        _spawnHandler = handler;
    }

    public void DeactivateObject() {
        this.gameObject.SetActive(false);
    }

    public void ActivateObject(Vector3 startingLocation)
    {
        //We're giving it a start position and re-invoking "awake" to "fake" an instantiation
        this.gameObject.transform.position = startingLocation;
        this.gameObject.SetActive(true);
        //Need to Update this to "fake" an instantiation
        //SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
    }
}