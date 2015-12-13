using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour
{

    void Start()
    {

    }

    public void Destruct()
    {
        GameObject.Destroy(gameObject);
    }
}
