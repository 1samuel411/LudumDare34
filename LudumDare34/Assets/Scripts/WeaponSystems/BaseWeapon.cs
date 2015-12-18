using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseWeapon : BaseItem
{
    public bool isGunActive;
    public bool isAutomatic;
    public bool CanShoot;
    public Vector2 pickupPosition;
    public float weaponTriggerSpeed;   //Used for the wait Time between Shots.
    public WeaponEffects weaponEffect;
    public ProjectileType weaponType;
    public GameObject projectile;
    public GameObject bulletSpawnBox;
    protected IList<Projectile> _projectiles;
    private Projectile _projectile;
    public WeaponAttributes weaponAttribute;

    public void Awake()
    {
        if(bulletSpawnBox == null)
            bulletSpawnBox =  this.gameObject;

        this.name = (string.IsNullOrEmpty(this.name)) ? gameObject.name : this.name;
        Initialize();
    }

    protected virtual void Initialize() { }

    public int DamagePerBullet() {
        return _projectile.damage;
    }

    protected IEnumerator SpawnBullets() {
        while (isGunActive) {
            //Used to decrement ammo in weaponAttributes.
            if (!weaponAttribute.coreWeapon && weaponAttribute.usingAmmo)
                weaponAttribute.currentAmmo -= 1;
            GameObject newProjectileObject = GameObject.Instantiate(projectile, bulletSpawnBox.transform.position, (Quaternion) bulletSpawnBox.transform.rotation) as GameObject;
            newProjectileObject.transform.localScale = new Vector3((PlayerController.instance.direction == 1) ? 1 : -1, 1);
            _projectile = newProjectileObject.GetComponent<Projectile>(); //may need to fix this?
            yield return new WaitForSeconds(weaponTriggerSpeed);
        }
    }

    public void ActivateGun(bool activateGun) {
        isGunActive = activateGun;
        gameObject.SetActive(activateGun);
        if (activateGun) {
            ResetWeaponAttributes();
            StartCoroutine(SpawnBullets());
            if (weaponAttribute.usingTimer) {
                weaponAttribute.checkTimer = true;
            }
        }
    }

    public void DestroyWeapon()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void ResetWeaponAttributes()
    {
        if (!weaponAttribute.coreWeapon) {
            weaponAttribute.currentMaxAmmo = weaponAttribute.maxAmountForAmmo;
            weaponAttribute.CurMaxAlottedTime = weaponAttribute.maxAmountForTime;
        }
    }

    public void DropWeapon()
    {
        //Awesome Drop Animation.
    }
}

public enum WeaponEffects {
    Normal = 0,
    Fire = 1,
    Ice = 2,
}