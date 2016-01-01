using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ObjectPoolManager {
    public class ObjectToSpawn : MonoBehaviour, IObjectToSpawn<CoreTimer>
    {
        #region IObjectToSpawn<CoreTimer> Members

        public void Test(CoreTimer type) {
            throw new NotImplementedException();
        }

        public void Enable() {
            throw new NotImplementedException();
        }

        public void Disable() {
            throw new NotImplementedException();
        }

        public void RestartValues() {
            throw new NotImplementedException();
        }

        #endregion

        protected void SpawnObject()
        {
            
        }
    }

    public class Test2 : ObjectToSpawn
    {
        private void Update()
        {

        }
    }
}
