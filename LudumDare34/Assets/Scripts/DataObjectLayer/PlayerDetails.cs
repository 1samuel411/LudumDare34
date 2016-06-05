using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Amazon.CognitoSync.SyncManager;
using Assets.Scripts.DataObjectLayer;

public class PlayerDetails : BaseAmazonDataset {

    public PlayerDetails(Dataset dataset)
        : base(dataset) {
        _timeNeededHours = _timeUsed = 0;
        _coins = _maxKills = _highScore = 0;
        _unlockedLevels = 0;
    }

    protected override void Initialize() {
        _coins = GetPropertyValueInt("Coins");
        _timeNeededHours = GetPropertyValueInt("TimeNeededHours");
        _timeUsed = GetPropertyValueInt("TimeUsed");
        _maxKills = GetPropertyValueInt("MaxKills");
        _highScore = GetPropertyValueInt("HighScore");
        _unlockedLevels = GetPropertyValueInt("UnlockedLevels");
    }

    private int _coins;
    private int _timeNeededHours;
    private int _timeUsed;
    private int _maxKills;
    private int _highScore;
    private int _unlockedLevels;
    private DateTime _currentTime;

    public int Coins {
        get { return _coins; }
        set {
            if (value != _coins) {
                _coins = value;
                SyncPropertyChange("Coins", value);
            }
        }
    }

    public int TimeNeededHours {
        get { return _timeNeededHours; }
        set {
            if (value != _timeNeededHours) {
                _timeNeededHours = value;
                TimeUsed = value; //Reset the time used with the time Needed.
                PropertyValueChange("TimeNeededHours", value);
            }
        }
    }

    public int TimeUsed {
        get { return _timeUsed; }
        set {
            if (value != _timeUsed) {
                _timeUsed = value;
                PropertyValueChange("TimeUsed", value);
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

    public DateTime CurrentTime {
        get { return _currentTime; }
        set {
            if (CurrentTime != value) {
                _currentTime = value;
                PropertyValueChange("CurrentTime", value);
            }
        }
    }
}
