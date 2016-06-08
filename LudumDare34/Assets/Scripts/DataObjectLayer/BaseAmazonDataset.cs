using System;
using System.Collections.Generic;
using Amazon.CognitoSync.SyncManager;
using UnityEngine;

namespace Assets.Scripts.DataObjectLayer {
    public class BaseAmazonDataset{

        protected Dataset _dataset;
        private bool _reSync;

        public SyncStatus isFirstSyncStatus;

        public BaseAmazonDataset(Dataset dataset) {
            _dataset = dataset;
            _reSync = true;
            isFirstSyncStatus = SyncStatus.Pending;
            _dataset.OnSyncSuccess += DatasetOnOnSyncSuccess;
            _dataset.OnSyncFailure += DatasetOnOnSyncFailure;
            _dataset.OnSyncConflict = this.HandleSyncConflict;
            _dataset.OnDatasetDeleted = this.HandleDatasetDeleted;
            _dataset.SynchronizeOnConnectivity();
        }

        protected virtual void Initialize() {
        }

        #region Dataset Handlers
        private void DatasetOnOnSyncSuccess(object sender, SyncSuccessEventArgs e) {
            var s = sender as Dataset;
            Debug.Log("Successfully synced for Dataset: " + s.Metadata);
            if (_reSync) {
                isFirstSyncStatus = SyncStatus.Success;
                Initialize();
                _reSync = false;
            }
        }

        private void DatasetOnOnSyncFailure(object sender, SyncFailureEventArgs e) {
            Dataset dataset = sender as Dataset;
            if(dataset.Metadata != null)
                Debug.Log("Sync Failed for dataset: " + dataset.Metadata.DatasetName);
            else
                Debug.Log("Sync Failed");
            Debug.Log("Exception Message " + e.Exception.Message);
            Debug.Log("RootException " + e.Exception.GetBaseException().Message);
            if(_reSync) {
                isFirstSyncStatus = SyncStatus.Failed;
                Initialize();
                _reSync = false;
            }
        }

        public virtual bool HandleSyncConflict(Dataset dataset, List<SyncConflict> conflicts) {
            if(dataset.Metadata != null) {
                Debug.LogWarningFormat("Sync Conflict {0}", dataset.Metadata.DatasetName);
            } else {
                Debug.LogWarning("Sync Conflict");
            }

            List<Record> resolvedRecords = new List<Record>();
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
        #endregion

        #region INotifyPropertyChanged

        protected virtual void PropertyValueChange(string propertyName, object value) {
            Debug.LogFormat("Invoked Property Change with dataset {0}, and prop {1}",
                _dataset.Metadata.DatasetName, propertyName);
            _dataset.Put(propertyName, value.ToString());
        }
        #endregion

        public void SynchronizeData() {
            _dataset.SynchronizeAsync();
        }

        public void SynchronizeAndResync() {
            _reSync = true;
            SynchronizeData();
        }

        protected void SyncPropertyChange(string propertyName, object value) {
            PropertyValueChange(propertyName, value);
            SynchronizeData();
        }

        protected string GetPropertyValue(string propertyName) {
            return _dataset.Get(propertyName);
        }

        protected int GetPropertyValueInt(string property) {
            int ret;
            string value = GetPropertyValue(property);
            ret = (string.IsNullOrEmpty(value)) ? 0 : Convert.ToInt32(value);
            return ret;
        }

        protected float GetPropertyValueFloat(string property) {
            float ret = 0.0f;
            string value = GetPropertyValue(property);
            if (!string.IsNullOrEmpty(value))
                ret = float.Parse(value);
            return ret;
        }

        protected DateTime GetPropertyValueDateTime(string property) {
            string value = GetPropertyValue(property);
            return Convert.ToDateTime(value);
        }
    }

    public enum SyncStatus {
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}
