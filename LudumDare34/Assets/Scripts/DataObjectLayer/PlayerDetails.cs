using System;
using Amazon.CognitoSync.SyncManager;
using Assets.Scripts.DataObjectLayer;

public class PlayerDetails : BaseAmazonDataset {

    public PlayerDetails(Dataset dataset)
        : base(dataset) {
        _timeNeeded = new DateTime();
        _coins = 0;
        _maxKills = 0;
        _highScore = 0;
        _unlockedLevels = 0;
    }

    protected override void Initialize() {
        _coins = GetPropertyValueInt("Coins");
        _timeNeeded = GetPropertyValueDateTime("TimeNeeded");
        _maxKills = GetPropertyValueInt("MaxKills");
        _highScore = GetPropertyValueInt("HighScore");
        _unlockedLevels = GetPropertyValueInt("UnlockedLevels");
    }

    private int _coins;
    private DateTime _timeNeeded;
    private int _maxKills;
    private int _highScore;
    private int _unlockedLevels;

    public int Coins {
        get { return _coins; }
        set {
            if (value != _coins) {
                _coins = value;
                SyncPropertyChange("Coins", value);
            }
        }
    }

    public DateTime TimeNeeded
    {
        get { return _timeNeeded; }
        set
        {
            if (TimeNeeded != value)
            {
                _timeNeeded = value;
                PropertyValueChange("TimeNeeded", value);
            }
        }
    }

    public int MaxKills {
        get { return _maxKills; }
        set {
            if (value != _maxKills) {
                _maxKills = value;
                PropertyValueChange("MaxKills", value);
            }
        }
    }

    public int HighScore {
        get { return _highScore; }
        set {
            if (HighScore != value) {
                _highScore = value;
                PropertyValueChange("HighScore", value);
            }
        }
    }

    public int UnlockedLevels {
        get { return _unlockedLevels; }
        set {
            if (_unlockedLevels != value) {
                _unlockedLevels = value;
                SyncPropertyChange("UnlockedLevels", value);
            }
        }
    }
}
