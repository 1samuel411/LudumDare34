using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ThirdPartyConnection.SyncManager {
    public interface ISyncAcessor {
        SyncInitializer syncInitializer { get; }

        /// <summary>
        /// In charge of Initializing Datasets for First Sync.
        /// This class can be implemented in a singleton class such as a 
        /// Game Manager, and must be initialized on Awake.
        /// </summary>
        /// <param name="action">an Action to continue with if any.
        /// The developer is responsible in handling the null action if any.</param>
        /// <returns></returns>
        IEnumerator InitializeDatasets(IEnumerator action = null);

        /// <summary>
        /// Resyncs all datasets from AWS, and the local cache.
        /// </summary>
        void ResyncDatasets();
    }
}
