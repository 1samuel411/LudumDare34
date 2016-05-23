using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.CognitoSync.SyncManager;
using Amazon.Runtime;
using UnityEngine;

namespace FS.SyncManager.CognitoSync {

	public class FSDatasetHandler : CognitoIdentitySync, IDatasetHandler {

	    public Dataset InitializeDataset(Dataset ds) {
            ds.OnSyncSuccess += this.HandleSyncSuccess;
            ds.OnSyncFailure += this.HandleSyncFailure;
            ds.OnSyncConflict = this.HandleSyncConflict;
            ds.OnDatasetDeleted = this.HandleDatasetDeleted;
            ds.OnDatasetMerged = this.HandleDatasetMerged;
            ds.SynchronizeOnConnectivity();
            return ds;
	    }

		public virtual void HandleSyncSuccess(object sender, SyncSuccessEventArgs e) {
		    var s = sender as Dataset;
			Debug.Log("Successfully synced for Dataset: " + s.Metadata);
		}

		public virtual void HandleSyncFailure(object sender, SyncFailureEventArgs e) {
			Dataset dataset = sender as Dataset;
			if(dataset.Metadata != null)
				Debug.Log("Sync Failed for dataset: " + dataset.Metadata.DatasetName);
			else
				Debug.Log("Sync Failed");
			Debug.LogException(e.Exception);
		}

        public virtual bool HandleSyncConflict(Dataset dataset, List<SyncConflict> conflicts) {
			if(dataset.Metadata != null) {
				Debug.LogWarningFormat("Sync Conflict {0}", dataset.Metadata.DatasetName);
			} else {
				Debug.LogWarning("Sync Conflict");
			}

			List<Amazon.CognitoSync.SyncManager.Record> resolvedRecords = new List <Amazon.CognitoSync.SyncManager.Record>();
			foreach(SyncConflict conflictRecord in conflicts) {
				resolvedRecords.Add(conflictRecord.ResolveWithRemoteRecord());
			}
			dataset.Resolve(resolvedRecords);
			return true;
		}

        public bool HandleDatasetDeleted(Dataset dataset) {
			Debug.Log(dataset.Metadata.DatasetName + " Dataset has been deleted");
			return true;
		}

        public bool HandleDatasetMerged(Dataset localDataset, List<string> mergedDatasetNames) {
			foreach(string name in mergedDatasetNames)
			{
				Dataset remoteDataset = cognitoSyncManager.OpenOrCreateDataset(name);
				EventHandler<SyncSuccessEventArgs> lambda;
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