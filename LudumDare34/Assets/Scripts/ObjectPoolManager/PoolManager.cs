﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class PoolManager : MonoBehaviour
{
    public int currentActiveSpawns = 0;
    private int _keys;
    private int _miscSpawnHandler;
    private int _spawnObjectsKey;

    public Dictionary<int, SpawnHandler> allSpawnHandlers;
    public Dictionary<int, SpawnObject> allSPawnObjects; 

    public void Awake() {
        allSPawnObjects = new Dictionary<int, SpawnObject>();
        allSpawnHandlers = new Dictionary<int, SpawnHandler>();
        _keys = CreateNewSpawnHandler();
        _miscSpawnHandler = _keys;
        _spawnObjectsKey = 0;
    }

    #region SpawnHandler
    protected int CreateNewSpawnHandler() {
        GameObject obj = new GameObject("MiscSpawnHandler");
        SpawnHandlerDetails sph = new SpawnHandlerDetails() {
            initialSpawnAmount = 5,
            overflowMaxSpawnAmount = 10,
            setPoolManagerParent = true
        };
        return CreateNewSpawnHandler(obj, sph);
    }

    /// <summary>
    /// Creates a new SpawnHandler from an existing object.
    /// </summary>
    /// <param name="spawnHandler">The SpawnHandler to Add.</param>
    /// <param name="setPoolManagerParent">Should this spawnHandler be a child of the PoolManager?</param>
    /// <returns>The Spawn Handler Number.</returns>
    public int CreateNewSpawnHandler(GameObject spawnHandler, SpawnHandlerDetails handlerDetails) {
        SpawnHandler handler = spawnHandler.GetComponent<SpawnHandler>();
        if (handler == null) {
            spawnHandler.AddComponent<SpawnHandler>();
            return CreateNewSpawnHandler(spawnHandler, handlerDetails);
        }
        if(handlerDetails.setPoolManagerParent)
            spawnHandler.transform.SetParent(this.transform);
        int value = ++_keys;
        handler.AddSpawnerLocation();
        handler.AddDetails(handlerDetails, value);
        allSpawnHandlers.Add(value,handler);
        return value;
    }

    public void AssignTransformToSpawnHandler(GameObject spawnLocation, int spawnHandlerKey) {
        allSpawnHandlers.FirstOrDefault(s => s.Key == spawnHandlerKey)
            .Value.AddSpawnerLocation(spawnLocation);
    }
    #endregion

    //Add newly instantiated objects to the spawnPool, as misc.
    public SpawnObject AddToSpawnPool(GameObject spawnObject) {
        return AddToSpawnPool(spawnObject, _miscSpawnHandler);
    }

    public SpawnObject AddToSpawnPool(GameObject spawnObject, int spawnHandlerKey) {
        SpawnObject spawnObj = spawnObject.GetComponent<SpawnObject>();
        if (spawnObj == null)
            spawnObj = spawnObject.AddComponent<SpawnObject>();
        KeyValuePair<int, SpawnHandler> handler = allSpawnHandlers.FirstOrDefault(s => s.Key == spawnHandlerKey);
        if(handler.Value == null)
            throw new UnityException("Missing SpawnHandler for SpawnPool");
        //Check if the object is already in the pool with a matching handler.
        if (allSPawnObjects.Any()) {
            Dictionary<int, SpawnObject> spawnObjs = allSPawnObjects.Where(s => s.Value.spawnHandler.spawnHandlerKey.Equals(spawnHandlerKey)).ToDictionary(v => v.Key, v => v.Value);
            if (spawnObjs.Any()) {
                KeyValuePair<int, SpawnObject> sObj = spawnObjs.FirstOrDefault(s => s.Value.name.Contains(spawnObject.name.Trim()));
                //if the object exists, with a handler, return the existing object.
                if (!sObj.Equals(default(KeyValuePair<int, SpawnObject>))) {
                    return sObj.Value;
                }
            }
        }
        int value = ++_spawnObjectsKey;
        spawnObj.SetSpawnObject(value, handler.Value);
        allSPawnObjects.Add(value, spawnObj);
        return spawnObj;
    }

    public void Spawn(SpawnObject spawnObject) {
        SpawnAt(spawnObject, null);
    }

    //Spawn this object @ location.
    public void SpawnAt(SpawnObject spawnObject, Transform location) {
        KeyValuePair<int, SpawnObject> sObj = allSPawnObjects.FirstOrDefault(s =>
                                        s.Key.Equals(spawnObject.spawnObjectKey));
        if(sObj.Equals(default(KeyValuePair<int, SpawnObject>)))
            throw new UnityException("SpawnObject does not Exist in Pool Manager!");
        //Get me a list of all Handlers that reference this "spawnerKey"
        KeyValuePair<int, SpawnHandler> referenceHandlers = allSpawnHandlers.FirstOrDefault(s => s.Value.spawnHandlerKey == sObj.Value.spawnHandler.spawnHandlerKey);
        referenceHandlers.Value.SpawnObject(sObj, location);
    }

#region deactivatingObjects
    public void DeactivateObject(SpawnObject obj) {
        obj.DeactivateObject();
    }

    public static void DeactivateObjects(SpawnObject obj) {
        PoolManager pm = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        Debug.Log("Deactivate Print Value: " + obj.spawnObjectKey);
        pm.DeactivateObject(obj);
    }
#endregion
}