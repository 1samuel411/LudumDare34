using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class PoolManager : MonoBehaviour
{
    public int currentActiveSpawns = 0;
    private int _keys;
    private int _spawnObjectsKey;

    public Dictionary<int, SpawnHandler> allSpawnHandlers;
    public Dictionary<int, SpawnObject> allSPawnObjects; 

    public void Awake() {
        allSPawnObjects = new Dictionary<int, SpawnObject>();
        allSpawnHandlers = new Dictionary<int, SpawnHandler>();
        _keys = CreateNewSpawnHandler();
        _spawnObjectsKey = 0;
    }

    #region SpawnHandler
    protected int CreateNewSpawnHandler() {
        GameObject obj = new GameObject("MiscSpawnHandler");
        return CreateNewSpawnHandler(obj, true);
    }

    /// <summary>
    /// Creates a new SpawnHandler from an existing object.
    /// </summary>
    /// <param name="spawnHandler">The SpawnHandler to Add.</param>
    /// <param name="setPoolManagerParent">Should this spawnHandler be a child of the PoolManager?</param>
    /// <returns>The Spawn Handler Number.</returns>
    public int CreateNewSpawnHandler(GameObject spawnHandler, bool setPoolManagerParent = false) {
        SpawnHandler handler = spawnHandler.GetComponent<SpawnHandler>();
        if (handler == null) {
            spawnHandler.AddComponent<SpawnHandler>();
            return CreateNewSpawnHandler(spawnHandler, setPoolManagerParent);
        }
        if(setPoolManagerParent)
            spawnHandler.transform.SetParent(this.transform);
        int value = ++_keys;
        handler.AddSpawnerLocation();
        handler.AddKey(value);
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
        return AddToSpawnPool(spawnObject, 0);
    }

    public SpawnObject AddToSpawnPool(GameObject spawnObject, int spawnHandlerKey) {
        SpawnObject spawnObj = spawnObject.GetComponent<SpawnObject>();
        if (spawnObj == null)
            spawnObject.AddComponent<SpawnObject>();
        KeyValuePair<int, SpawnHandler> handler = allSpawnHandlers.FirstOrDefault(s => s.Key == spawnHandlerKey);
        if(handler.Value == null)
            throw new UnityException("Missing SpawnHandler for SpawnPool");
        int value = ++_spawnObjectsKey;
        spawnObj.SetSpawnObject(value, handler.Value);
        allSPawnObjects.Add(value, spawnObj);
        return spawnObj;
    }

    public void Spawn(SpawnObject spawnObject) {
        KeyValuePair<int,SpawnObject> sObj = allSPawnObjects.FirstOrDefault(s => s.Key.Equals(spawnObject.spawnObjectKey));
        if(sObj.Equals(default(KeyValuePair<int, SpawnObject>)))
            throw new UnityException("SpawnObject does not Exist in Pool Manager!");
        //Get me a list of all Handlers that reference this "spawnerKey"
        KeyValuePair<int, SpawnHandler> referenceHandlers = allSpawnHandlers.FirstOrDefault(s => s.Value.spawnHandlerKey == sObj.Value.spawnHandler.spawnHandlerKey);
        referenceHandlers.Value.SpawnObject(sObj);
    }

    public void DeactivateObject(SpawnObject obj)
    {
        KeyValuePair<int, SpawnObject> sObj = allSPawnObjects.FirstOrDefault(s => s.Value.spawnObjectKey == obj.spawnObjectKey);
        if(sObj.Equals(default(KeyValuePair<int, SpawnObject>)))
            throw new UnityException("SpawnObject does not exist in Pool Manager!");
        obj.DeactivateObject();
    }

    public int GetSpawnObjectKey(SpawnObject obj) {
        int key = -1;

        return key;
    }

    public static void DeactivateObjects(SpawnObject obj)
    {
        PoolManager pm = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
        pm.DeactivateObject(obj);

    }
}

public enum enemySpawnerType {
    SkySpawner = 0,
    GroundSpawner = 1
}