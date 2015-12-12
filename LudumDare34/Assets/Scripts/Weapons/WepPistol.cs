using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class WepPistol : BaseWeapon 
{
    public WepPistol() {
        projectile = new Projectile(10);
    }

    protected override void Initialize()
    {
        this.name = "Pistol";
        this.maxAmmo = 0;
        this.minAmmo = 0;
        this.weaponEffect = WeaponEffects.Normal;
        this.weaponType = ProjectileType.ShortBullet;
        this.weaponTriggerSpeed = 0.4f;
        this.isAutomatic = true;
        this.CanShoot = true;
        this.isGunActive = false;
    }
}
