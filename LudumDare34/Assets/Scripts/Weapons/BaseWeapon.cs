﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseWeapon : BaseItem
{
    public int maxAmmo;
    public int minAmmo;
    public bool isGunActive;
    public bool isAutomatic;
    public bool CanShoot;
    public float weaponTriggerSpeed;   //Used for the wait Time between Shots.
    public WeaponEffects weaponEffect;
    public ProjectileType weaponType;
    public GameObject projectile;
    public GameObject bulletSpawnBox;
    protected IList<Projectile> _projectiles;
    private Projectile _projectile;

    public BaseWeapon() {
        bulletSpawnBox = this.gameObject;
        Initialize();
    }

    protected virtual void Initialize() { }

    public int DamagePerBullet() {
        return _projectile.damage;
    }

    protected IEnumerator SpawnBullets() {
        while (isGunActive) {
            GameObject newProjectileObject = GameObject.Instantiate(projectile, bulletSpawnBox.transform.position, (Quaternion) bulletSpawnBox.transform.rotation) as GameObject;
            _projectile = newProjectileObject.GetComponent<Projectile>();
            yield return new WaitForSeconds(weaponTriggerSpeed);
        }
    }

    public void ActivateGun(bool activateGun) {
        isGunActive = activateGun;
        if (activateGun) {
            StartCoroutine(SpawnBullets());
        }
    }
}

public enum WeaponEffects {
    Normal = 0,
    Fire = 1,
    Ice = 2,
}