using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ObjectPoolManager {
    public class BasePool : MonoBehaviour
    {
        public List<ObjectToSpawn> objectsToSpawn;
        public void Awake() {
            objectsToSpawn = new List<IObjectToSpawn>();
        }

        void 
    }
}
