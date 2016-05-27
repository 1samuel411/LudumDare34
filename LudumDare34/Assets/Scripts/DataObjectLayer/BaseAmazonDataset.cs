using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Amazon.CognitoSync.SyncManager;
using System.Reflection;
using Amazon.Runtime;
using UnityEngine;

namespace Assets.Scripts.DataObjectLayer {
    public class BaseAmazonDataset: INotifyPropertyChanged {

        protected Dataset _dataset;
        private bool _reSync;

        public SyncStatus isFirstSyncStatus;

        public BaseAmazonDataset(Dataset dataset) {
            _dataset = dataset;
            _reSync = true;
            isFirstSyncStatus = SyncStatus.Pending;
            PropertyChanged += OnPropertyChanged;
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
            Debug.LogException(e.Exception);
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
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (PropertyChanged != null) {
                Type classType = sender.GetType();
                string propName = e.PropertyName;
                string className = classType.Name;
                if(string.CompareOrdinal(className, _dataset.Metadata.DatasetName) != 0) {
                    //Can remove later on.
                    Debug.Log("Class Name does not match dataset Name! Class Name Must Match!");
                    Debug.Log("Class Name is: " + className);
                    return;
                }
                PropertyInfo property = classType.GetProperty(propName);
                if (property == null) {
                    Debug.Log("PROPERTY DOES NOT EXIST!! CHECK YOUR PROPERTY CHANGE!");
                    return;
                }
                string value = property.GetValue(sender, null).ToString();
                _dataset.Put(propName, value);
            }
        }

        protected virtual void PropertyValueChange(string propertyName) {
            Debug.LogFormat("Invoked Property Change with dataset {0}, and prop {1}",
                _dataset.Metadata.DatasetName, propertyName);
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            else
                Debug.Log("Property Change is not assigned in BaseAmazonDataset! with property " + propertyName);
        }
        #endregion

        public void SynchronizeData() {
            _dataset.SynchronizeAsync();
        }

        public void SynchronizeAndResync() {
            _reSync = true;
            SynchronizeData();
        }

        protected void SyncPropertyChange(string propertyName) {
            PropertyValueChange(propertyName);
            SynchronizeData();
        }

        protected string GetPropertyValue(string propertyName) {
            //string ret = propertyName.GetType().Name;
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
