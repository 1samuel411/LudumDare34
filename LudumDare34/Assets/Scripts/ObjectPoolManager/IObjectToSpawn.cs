using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPoolManager {
    public interface IObjectToSpawn<T>
    {
        void Test(T type);

        void Enable();

        void Disable();

        void RestartValues();
    }
}
