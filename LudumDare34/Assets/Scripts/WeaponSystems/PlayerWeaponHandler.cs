using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerWeaponHandler
{
    public List<BaseWeapon> weapons;
    public BaseWeapon startingWeapon;

    public PlayerWeaponHandler(BaseWeapon weapon) {
        startingWeapon = weapon;
    }

    public void PickedUpWeapon(BaseWeapon weapon) {
        weapons.Add(weapon);
    }

    public BaseWeapon GetNextWeapon(BaseWeapon weapon) {
        BaseWeapon baseWeapon;
        weapon.ActivateGun(false);  //deactivate the gun.
        if (weapons.Count > 0) {
            baseWeapon = weapons.FirstOrDefault();
            weapons.Remove(baseWeapon);
        } else {
            baseWeapon = startingWeapon;
        }

        return baseWeapon;
    }
}
