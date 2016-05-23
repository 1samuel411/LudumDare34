using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

/* Used to create your own dataset handler script with your dataset.
 * The recommended method is to use the default "FSDataset" class, and 
 * override the steps you want to change.
 * */
namespace FS.SyncManager.CognitoSync {
    public interface IDatasetHandler {

        void HandleSyncSuccess(object sender, SyncSuccessEventArgs e);

        void HandleSyncFailure(object sender, SyncFailureEventArgs e);

        bool HandleSyncConflict(Dataset dataset, List<SyncConflict> conflicts);

        bool HandleDatasetDeleted(Dataset dataset);

        bool HandleDatasetMerged(Dataset localDataset, List<string> mergedDatasetNames);
    }
}