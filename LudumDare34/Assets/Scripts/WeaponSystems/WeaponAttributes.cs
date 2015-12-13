using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponAttributes {
    #region Handles Ammo Settings
    private int _currentAmmo;
    private int _currentMaxAmmo;

    public int currentMaxAmmo
    {
        get { return _currentMaxAmmo; }
        set
        {
            if (value >= maxAllowedAmmo)
                _currentMaxAmmo = maxAllowedAmmo;
            else
            {
                _currentMaxAmmo += value;
                _currentAmmo = _currentMaxAmmo;
            }
        }
    }

    public int currentAmmo
    {
        get { return _currentAmmo; }
        set { _currentAmmo = value; }
    }
    #endregion

    #region Timer Settings

    private float _currentMaxTime;
    private float _currentAlottedTime;

    public float CurMaxAlottedTime
    {
        get { return _currentMaxTime; }
        set
        {
            if(value >= maxAmountForTime)
                _currentMaxTime = maxAmountForTime;
            else
            {
                _currentMaxTime += value;
                _currentAlottedTime = _currentMaxTime;
            }
        }
    }

    public float CurAlottedTime {
        get { return _currentAlottedTime; }
        set { _currentAlottedTime = value; }
    }
    #endregion

    #region Set Radio Buttons for Ammo and Timer
    public bool UsingAmmo {
        get { return usingAmmo; }
        set {
            if (!coreWeapon) {
                usingAmmo = value;
                usingTimer = (!usingAmmo);
            }
            else
                usingAmmo = usingTimer = false;
        }
    }

    public bool UsingTimer {
        get { return usingTimer; }
        set {
            if (coreWeapon == false) {
                usingTimer = value;
                usingAmmo = (!usingTimer);
            }
            else
                usingAmmo = usingTimer = false;
        }
    }
    #endregion

    public bool usingAmmo;
    public bool usingTimer;
    public bool coreWeapon;

    public float maxAmountForTime;
    public int maxAllowedAmmo;

    public float TimeToAddPerPickUp;
    public int ammoToAddPerPickUp;

    protected void AddWeaponTime()
    {
        if (!coreWeapon)
        {
            if (usingTimer)
                CurMaxAlottedTime += TimeToAddPerPickUp;
            else
            {
                Debug.Log("This is set to use Ammo only!");
            }
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

    public void AddTimeOrAmmo()
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
            if (usingAmmo && currentAmmo > 0)
                bOk = true;
            else if (usingTimer && CurAlottedTime > 0.01f)
                bOk = true;
        } else
            bOk = true;

        return bOk;
    }
}

