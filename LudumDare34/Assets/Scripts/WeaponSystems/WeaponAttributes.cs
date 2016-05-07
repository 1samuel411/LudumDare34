using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponAttributes {
    #region Handles Ammo Settings
    public int _currentAmmo;
    private int _currentMaxAmmo;

    public int currentMaxAmmo
    {
        get { return _currentMaxAmmo; }
        set
        {
            _currentMaxAmmo = value;
            _currentAmmo = _currentMaxAmmo;
        }
    }

    public int currentAmmo
    {
        get { return _currentAmmo; }
        set { _currentAmmo = value; }
    }
    #endregion

    #region Timer Settings

    public bool checkTimer = false;
    private float _currentMaxTime;
    public float _currentTime;

    public float CurMaxAlottedTime
    {
        get { return _currentMaxTime; }
        set
        {
            _currentMaxTime = value;
            _currentTime = _currentMaxTime;
        }
    }

    public float CurAlottedTime {
        get { return _currentTime; }
        set { _currentTime = value;  }
    }
    #endregion

    public bool usingAmmo; 
    public bool usingTimer;
    public bool coreWeapon;

    public float maxAmountForTime;  //this is the set time cap by the developer
    public float originalMaxAmountForTime;
    public int maxAmountForAmmo;    //this is the set ammo cap by the developer.
    public int originalMaxAmountForAmmo;

    public float TimeToAddPerPickUp;//this is the amount of time to be added per pickup
    public int ammoToAddPerPickUp;  //this is the amount of ammo to be added per pickup

    protected void AddWeaponTime()
    {
        if (!coreWeapon)
        {
            if (usingTimer)
                CurMaxAlottedTime += TimeToAddPerPickUp;
            else
                Debug.Log("This is set to use Ammo only!");
        } else 
            Debug.Log("This is a Core Weapon! Timer is not needed.");
    }

    protected void AddAmmoToMax()
    {
        if (!coreWeapon)
        {
            if(usingAmmo)
                currentMaxAmmo += ammoToAddPerPickUp;
            else
                Debug.Log("This is set to use Time only!");
        } else
            Debug.Log("This is a Core Weapon! Ammo is not needed.");
    }

    public void AddTimeOrAmmo(int ammoAddition = 0, float timeAddition = 0)
    {
        if (!coreWeapon)
        {
            if(usingAmmo && ammoToAddPerPickUp != 0)
                AddAmmoToMax();
            else if (usingTimer && TimeToAddPerPickUp > 0.01f)
                AddWeaponTime();
            else
                Debug.Log("Forgot to adjust WeaponAttribute settings!");
        } else
            Debug.Log("This is a CoreWeapon! Cannot Add Time");
    }

    public bool CheckIfWeaponAvailable()
    {
        bool bOk = false;
        if (!coreWeapon) {
            //Debug.Log(string.Format("Current Ammo: {0}, Current Timer {1}", currentAmmo, _currentTime));
            if (usingAmmo && currentAmmo > 0)
                bOk = true;
            else if(usingTimer && _currentTime > 0.01f) {
                _currentTime -= Time.deltaTime;
                bOk = true;
            }
        } else
            bOk = true;

        return bOk;
    }
}

