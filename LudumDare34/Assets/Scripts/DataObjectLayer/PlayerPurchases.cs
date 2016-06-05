using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.CognitoSync.SyncManager;
using Assets.Scripts.DataObjectLayer;

public class PlayerPurchases : BaseAmazonDataset {
    public PlayerPurchases(Dataset dataset) 
        : base(dataset) {
        _bought = string.Empty;
    }

    protected override void Initialize() {
        _bought = GetPropertyValue("Bought");
    }

    private string _bought;

    //This one needs to change. it shouldn't be in CSV Format.
    public string Bought {
        get { return _bought; }
        set {
            if(_bought != value) {
                _bought = value;
                PropertyValueChange("Bought", value);
            }
        }
    }
}

