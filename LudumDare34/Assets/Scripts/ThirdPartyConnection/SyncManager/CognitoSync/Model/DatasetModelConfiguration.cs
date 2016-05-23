using UnityEngine;
using System;
using System.Linq;
using System.Collections;

//Not Used at this moment.
namespace FS.SyncManager.CognitoSync.Model {
    public class DatasetModelConfiguration<TEntity>
        where TEntity : class {

        private string _datasetName;

        public Type GetType() {
            return typeof (TEntity);
        }

        public string datasetName {
            get {
                //If the name is unassigned, use the name of the class as the dataset Name.
                if (string.IsNullOrEmpty(_datasetName))
                    _datasetName = GetType().Name;
                return _datasetName;
            }
        }

        public void AssignDatasetName(string datasetName) {
            if (!string.IsNullOrEmpty(datasetName))
                _datasetName = datasetName;
        }

        public void SynchronizeDataset() {
            var prop = GetType().GetProperties();
            foreach (var p in prop) {
                string keyName = p.Name;
                string value = p.GetValue(GetType(), null).ToString();
            }
        }
    }
}