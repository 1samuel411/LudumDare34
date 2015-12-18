using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerWeaponHandler : MonoBehaviour
{
    public List<string> weaponsOwned;

    public void AddFirstWeapon(ref BaseWeapon firstWeapon) {
        weaponsOwned.Add(firstWeapon.name);
        firstWeapon.ActivateGun(true);
    }

    public void PickedUpWeapon(BaseWeapon weapon) {
        //Check the weapon doesn't have a parent
        if (weapon.gameObject.transform.parent == null)
        {
            //Check if already Owned!
            string storedWeapons = weaponsOwned.FirstOrDefault(w => string.CompareOrdinal(w, weapon.name) == 0);
            if (string.IsNullOrEmpty(storedWeapons)) {
                //Adding new Weapon!
                Destroy(weapon.GetComponent(typeof(BoxCollider2D)));
                Destroy(weapon.GetComponent(typeof(Rigidbody2D)));
                weapon.transform.parent = this.transform;
                weapon.ActivateGun(false);
                weapon.ResetWeaponAttributes();
                weaponsOwned.Add(weapon.name);
            } else { 
                //Add Weapon Ammo or Time!!
                BaseWeapon[] ownedWeapons = gameObject.GetComponentsInChildren<BaseWeapon>(true);
                BaseWeapon matchingWeapon = ownedWeapons.First(w => string.CompareOrdinal(w.name, weapon.name) == 0);
                matchingWeapon.weaponAttribute.AddTimeOrAmmo();
                weapon.DestroyWeapon(); //------------Weapon is DESTROYED
            }
        } else {
            Debug.Log("Weapon belongs to a parent!");
            //do not pick up weapon.
        }

    }

    /// <summary>
    /// Give it the current Gun to deactivate, and it will give you the nextGun.
    /// </summary>
    /// <param name="weapon">The Current Gun Equipped.</param>
    /// <returns></returns>
    public BaseWeapon GetNextWeapon(BaseWeapon weapon) {
        BaseWeapon baseWeapon;
        weapon.ActivateGun(false);  //deactivate the current gun.
        BaseWeapon[] ownedWeapons = gameObject.GetComponentsInChildren<BaseWeapon>(true);
        IEnumerable<BaseWeapon> availableWeapons = ownedWeapons.Where(w => w.weaponAttribute.CheckIfWeaponAvailable() == true);
        if (availableWeapons.Count() > 1) // check that we have more then just the core weapon.
        {
            //Add an order by, to prioritize next available weapon by strength.
            IEnumerable<BaseWeapon> nonCoreWeapons = ownedWeapons.Where(w => w.weaponAttribute.coreWeapon == false);
            baseWeapon = (nonCoreWeapons.Count() > 0) ? nonCoreWeapons.First() : availableWeapons.First();
        }
        else
            baseWeapon = availableWeapons.First();

        baseWeapon.ActivateGun(true);
        return baseWeapon;
    }
}
