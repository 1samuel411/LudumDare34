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

        public SyncStatus isFirstSyncStatus;

        public BaseAmazonDataset(Dataset dataset) {
            _dataset = dataset;
            _dataset.SynchronizeOnConnectivity();
            isFirstSyncStatus = SyncStatus.Pending;
            PropertyChanged += OnPropertyChanged;
            _dataset.OnSyncSuccess += DatasetOnOnSyncSuccess;
            _dataset.OnSyncFailure += DatasetOnOnSyncFailure;
        }

        private void DatasetOnOnSyncFailure(object sender, SyncFailureEventArgs syncFailureEventArgs) {
            isFirstSyncStatus = SyncStatus.Failed;
        }

        private void DatasetOnOnSyncSuccess(object sender, SyncSuccessEventArgs syncSuccessEventArgs) {
            isFirstSyncStatus = SyncStatus.Success;
        }

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

        protected void SyncPropertyChange(string propertyName) {
            PropertyValueChange(propertyName);
            SynchronizeData();
        }

        protected string GetPropertyValue(object property) {
            string ret = property.GetType().Name;
            return _dataset.Get(ret);
        }

        protected int GetPropertyValueInt(object property) {
            int ret;
            string value = GetPropertyValue(property);
            ret = (string.IsNullOrEmpty(value)) ? 0 : Convert.ToInt32(value);
            return ret;
        }

        protected float GetPropertyValueFloat(object property) {
            float ret = 0.0f;
            string value = GetPropertyValue(property);
            if (!string.IsNullOrEmpty(value))
                ret = float.Parse(value);
            return ret;
        }

        protected DateTime GetPropertyValueDateTime(object property) {
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
