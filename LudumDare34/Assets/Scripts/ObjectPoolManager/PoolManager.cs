using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public int currentActiveSpawns = 0;
    public List<SpawnObject> _spawnObjects;
    private List<SpawnHandler> _spawnHandlers;
    private int _miscHandlerKey;

    public List<SpawnHandler> SpawnHandlers { get { return _spawnHandlers; } }  //Needed an Immutable List of Spawn Handlers.

    public void Awake() {
        _spawnHandlers = new List<SpawnHandler>();
        _miscHandlerKey = CreateNewSpawnHandler();
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
        int handlerKey = _spawnHandlers.Count;
        return CreateNewSpawnHandler(spawnHandler, handlerKey, setPoolManagerParent);
    }

    /// <summary>
    /// Creates a new SpawnHandler from an existing object.
    /// </summary>
    /// <param name="spawnHandler">The SpawnHandler to Add.</param>
    /// <param name="spawnHandlerKey">The Reference Key for the Spawn Handler.</param>
    /// <param name="setPoolManagerParent">Should this spawnHandler be a child of the PoolManager?</param>
    /// <returns>The Spawn Handler Number.</returns>
    public int CreateNewSpawnHandler(GameObject spawnHandler, int spawnHandlerKey, bool setPoolManagerParent = false) {
        if(spawnHandler.GetComponent<SpawnHandler>() == null)
            spawnHandler.AddComponent<SpawnHandler>();   //Add Handler
        //Set the Parent of the SpawnHandler to the PoolManager.
        if(setPoolManagerParent)
            spawnHandler.transform.SetParent(this.transform);
        SpawnHandler handler = spawnHandler.GetComponent<SpawnHandler>();
        handler.SetSpawnHandler(spawnHandlerKey);
        _spawnHandlers.Add(handler);
        currentActiveSpawns++;
        return spawnHandlerKey;
    }

    #endregion

    //Add newly instantiated objects to the spawnPool, as misc.
    public void AddToSpawnPool(GameObject spawnObject) {
        AddToSpawnPool(spawnObject, _miscHandlerKey);
    }

    public void AddToSpawnPool(GameObject spawnObject, int spawnHandlerKey) {
        if(spawnObject.GetComponent<SpawnObject>() == null)
            spawnObject.AddComponent<SpawnObject>();
        SpawnObject sObj = spawnObject.GetComponent<SpawnObject>();
        SpawnHandler handler = _spawnHandlers.FirstOrDefault(s => s.spawnerKey == spawnHandlerKey);
        if(handler == null) 
            throw new UnityException("Missing SpawnHandler for SpawnPool");
        sObj.SetSpawnObject(spawnObject, spawnHandlerKey);
        _spawnObjects.Add(sObj);
    }

    public void Spawn(SpawnObject spawnObject) {
        SpawnObject sObj = _spawnObjects.FirstOrDefault(s => s.gameObject == spawnObject);
        if(sObj == null)
            throw new UnityException("SpawnObject does not Exist in Pool Manager!");
        //Get me a list of all Handlers that reference this "spawnerKey"
        IEnumerable<SpawnHandler> referenceHandlers = _spawnHandlers.Where(s => s.spawnerKey == sObj.spawnerKey);
        int randomNumber = UnityEngine.Random.Range(1, referenceHandlers.Count());
        SpawnHandler handler = referenceHandlers.ElementAt(randomNumber);
        handler.SpawnObject(sObj);
    }

    public void DeactivateObject(GameObject obj)
    {
        SpawnObject sObj = _spawnObjects.FirstOrDefault(s => s.gameObject == obj);
        if(sObj == null)
            throw new UnityException("SpawnObject does not exist in Pool Manager!");

    }
}

public enum enemySpawnerType {
    SkySpawner = 0,
    GroundSpawner = 1
}