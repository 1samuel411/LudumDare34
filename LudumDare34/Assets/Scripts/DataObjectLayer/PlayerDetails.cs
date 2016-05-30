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
    }

    protected override void Initialize() {
        _coins = GetPropertyValueInt("Coins");
        _timeNeededHours = GetPropertyValueInt("TimeNeededHours");
        _timeUsed = GetPropertyValueInt("TimeUsed");
        _maxKills = GetPropertyValueInt("MaxKills");
        Debug.Log("Max Kills: " + _maxKills);
        _highScore = GetPropertyValueInt("HighScore");
    }

    private int _coins;
    private int _timeNeededHours;
    private int _timeUsed;
    private int _maxKills;
    private int _highScore;
    private DateTime _currentTime;

    public int Coins {
        get { return _coins; }
        set {
            if (value != _coins) {
                _coins = value;
                SyncPropertyChange("Coins");
            }
        }
    }

    public int TimeNeededHours {
        get { return _timeNeededHours; }
        set {
            if (value != _timeNeededHours) {
                _timeNeededHours = value;
                TimeUsed = value; //Reset the time used with the time Needed.
                PropertyValueChange("TimeNeededHours");
            }
        }
    }

    public int TimeUsed {
        get { return _timeUsed; }
        set {
            if (value != _timeUsed) {
                _timeUsed = value;
                PropertyValueChange("TimeUsed");
            }
        }
    }

    public int MaxKills {
        get { return _maxKills; }
        set {
            if (value != _maxKills) {
                _maxKills = value;
                PropertyValueChange("MaxKills");
            }
        }
    }

    public int HighScore {
        get { return _highScore; }
        set {
            if (HighScore != value) {
                _highScore = value;
                PropertyValueChange("HighScore");
            }
        }
    }

    public DateTime CurrentTime {
        get { return _currentTime; }
        set {
            if (CurrentTime != value) {
                _currentTime = value;
                PropertyValueChange("CurrentTime");
            }
        }
    }
}
