using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.CognitoSync.SyncManager;
using UnityEngine;

namespace FS.SyncManager.CognitoSync {

    public class FSDatasetHandler: SyncManager {

	    public Dataset InitializeDataset(Dataset ds) {
            ds.OnDatasetMerged = this.HandleDatasetMerged;
            ds.SynchronizeOnConnectivity();
            return ds;
	    }

        public bool HandleDatasetMerged(Dataset localDataset, List<string> mergedDatasetNames) {
            Debug.Log("Handling Dataset Merge");
			foreach(string name in mergedDatasetNames) {
				Dataset remoteDataset = cognitoSyncManager.OpenOrCreateDataset(name);
                Debug.Log("Remote Dataset Name: " + remoteDataset.Metadata.DatasetName);
				EventHandler<SyncSuccessEventArgs> lambda = null;
				lambda = (object sender, SyncSuccessEventArgs e) => {
					IList<Record> existingValues = localDataset.Records;
					IList<Record> newValues = remoteDataset.Records;

					localDataset.Resolve(ResolveMergedRecords(existingValues.ToList(), newValues.ToList()));

					remoteDataset.Delete(); //delete local dataset.
					remoteDataset.OnSyncSuccess -= lambda;
					remoteDataset.OnSyncSuccess += (object s2, SyncSuccessEventArgs e2) => {
						localDataset.SynchronizeAsync();
					};
					remoteDataset.SynchronizeAsync();
				};
				remoteDataset.OnSyncSuccess += lambda;
				remoteDataset.SynchronizeAsync();
			}
			//returning true allows the Synchronize to continue. 
			//False stops it.
			return false;
		}

		protected virtual List<Record> ResolveMergedRecords(List<Record> localRecords, List<Record> remoteRecords) {
			return remoteRecords;
		}
	}
}