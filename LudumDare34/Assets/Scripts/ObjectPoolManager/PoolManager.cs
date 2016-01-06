using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public int currentActiveSpawns = 0;
    public List<SpawnObject> _spawnObjects;
    private List<SpawnHandler> _spawnHandlers;
    private int keys;

    public Dictionary<int, SpawnHandler> allSpawnHandlers; 

    public List<SpawnHandler> SpawnHandlers { get { return _spawnHandlers; } }  //Needed an Immutable List of Spawn Handlers.

    public void Awake() {
        _spawnHandlers = new List<SpawnHandler>();
        keys = CreateNewSpawnHandler();
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
        handler.AddSpawnerLocation();
        allSpawnHandlers.Add(keys++,handler);
        return keys;
    }

    public void AssignTransformToSpawnHandler(GameObject spawnLocation, int spawnHandlerKey) {
        allSpawnHandlers.FirstOrDefault(s => s.Key == spawnHandlerKey)
            .Value.AddSpawnerLocation(spawnLocation);
    }
    #endregion

    //Add newly instantiated objects to the spawnPool, as misc.
    public void AddToSpawnPool(GameObject spawnObject) {
        AddToSpawnPool(spawnObject, 0);
    }

    public void AddToSpawnPool(GameObject spawnObject, int spawnHandlerKey) {
        SpawnObject spawnObj = spawnObject.GetComponent<SpawnObject>();
        if (spawnObj == null)
            spawnObject.AddComponent<SpawnObject>();
        KeyValuePair<int, SpawnHandler> handler = allSpawnHandlers.FirstOrDefault(s => s.Key == spawnHandlerKey);
        if(handler.Value == null)
            throw new UnityException("Missing SpawnHandler for SpawnPool");
        spawnObj.SetSpawnObject(spawnObject, spawnHandlerKey);
        _spawnObjects.Add(spawnObj);
    }

    public void Spawn(SpawnObject spawnObject) {
        SpawnObject sObj = _spawnObjects.FirstOrDefault(s => s.gameObject == spawnObject);
        if(sObj == null)
            throw new UnityException("SpawnObject does not Exist in Pool Manager!");
        //Get me a list of all Handlers that reference this "spawnerKey"
        KeyValuePair<int, SpawnHandler> referenceHandlers = allSpawnHandlers.FirstOrDefault(s => s.Key == sObj.spawnerKey);
        referenceHandlers.Value.SpawnObject(spawnObject);
    }

    public void DeactivateObject(GameObject obj)
    {
        SpawnObject sObj = _spawnObjects.FirstOrDefault(s => s.gameObject == obj);
        if(sObj == null)
            throw new UnityException("SpawnObject does not exist in Pool Manager!");
        sObj.DeactivateObject();
    }
}

public enum enemySpawnerType {
    SkySpawner = 0,
    GroundSpawner = 1
}