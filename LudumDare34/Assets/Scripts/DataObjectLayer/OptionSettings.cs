using UnityEngine;
using System.Collections;
using Amazon.CognitoSync.SyncManager;
using Assets.Scripts.DataObjectLayer;

public class OptionSettings : BaseAmazonDataset {
    /* NOTE: This is a unique class, as it will function as a local cache
     * settings, and not be sync'd to AWS. the reason is that options
     * are related to a unique device, not game settings.
     * For this reason, It will not sync to AWS.
     */
    public OptionSettings(Dataset dataset) 
        : base(dataset) {
    }

    protected override void Initialize() {
        _voiceVolume = GetPropertyValueFloat("VoiceVolume");
        _musicVolume = GetPropertyValueFloat("MusicVolume");
        _effectVolume = GetPropertyValueFloat("EffectVolume");
    }

    private float _voiceVolume;
    private float _musicVolume;
    private float _effectVolume;
    private bool _voiceEnabled;
    private bool _musicEnabled;
    private bool _effectEnabled;

    public float  VoiceVolume {
        get { return _voiceVolume; }
        set {
            if (value != _voiceVolume) {
                _voiceVolume = value;
                PropertyValueChange("VoiceVolume", value);
            }
        }
    }

    public float MusicVolume {
        get { return _musicVolume; }
        set {
            if (value != _musicVolume) {
                _musicVolume = value;
                PropertyValueChange("MusicVolume", value);
            }
        }
    }

    public float EffectVolume {
        get { return _effectVolume; }
        set {
            if (value != _effectVolume) {
                _effectVolume = value;
                PropertyValueChange("EffectVolume", value);
            }
        }
    }

    public bool VoiceEnabled {
        get { return _voiceEnabled; }
        set {
            if (value != _voiceEnabled) {
                _voiceEnabled = value;
                PropertyValueChange("VoiceEnabled", value);
            }
        }
    }

    public bool MusicEnabled {
        get { return _musicEnabled; }
        set {
            if (value != _musicEnabled) {
                _musicEnabled = value;
                PropertyValueChange("MusicEnabled", value);
            }
        }
    }

    public bool EffectEnabled {
        get { return _effectEnabled; }
        set {
            if (value != _effectEnabled) {
                _effectEnabled = value;
                PropertyValueChange("EffectEnabled", value);
            }
        }
    }
}
