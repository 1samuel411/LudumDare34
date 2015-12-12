using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseWeapon : BaseItem
{
    public string name;
    public int maxAmmo;
    public int minAmmo;
    public bool isGunActive;
    public bool isAutomatic;
    public bool CanShoot;
    public float weaponTriggerSpeed;   //Used for the wait Time between Shots.
    public WeaponEffects weaponEffect;
    public ProjectileType weaponType;
    public Projectile projectile;
    public GameObject bulletSpawnBox;
    protected IList<Projectile> _projectiles;

    public BaseWeapon() {
        projectile = new Projectile(10);
        Initialize();
    }

    protected virtual void Initialize() { }

    public int DamagePerBullet() {
        return projectile.damage;
    }

    protected IEnumerator CreateBullets() {
        while (isGunActive) {
            GameObject.Instantiate(projectile, bulletSpawnBox.transform.position, (Quaternion) bulletSpawnBox.transform.rotation);
            yield return new WaitForSeconds(weaponTriggerSpeed);
        }
    }

    public void ActivateGun(bool activateGun) {
        if (activateGun) {
            StartCoroutine(CreateBullets());
        }
        isGunActive = activateGun;

    }
}

public enum WeaponEffects {
    Normal = 0,
    Fire = 1,
    Ice = 2,
}