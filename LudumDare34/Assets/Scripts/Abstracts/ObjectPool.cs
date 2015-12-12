using UnityEngine;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;

public class ObjectPool<T>
{
    public T genObj;

    public ObjectPool(T T)
    {
        genObj = T;
    }
}