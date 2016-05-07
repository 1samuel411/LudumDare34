using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour
{

    public bool descruct = true;
    private SpawnObject spawnObj;

    public Transform parent;

    void Start()
    {
        spawnObj = GetComponent<SpawnObject>();
    }

    void Update()
    {
        if(parent)
        {
            transform.position = parent.position;
        }
    }

    public void Destruct()
    {
        if (descruct)
            GameObject.Destroy(gameObject);
        else
        {
            if (spawnObj)
                spawnObj.DeactivateObject();
        }
    }
}
