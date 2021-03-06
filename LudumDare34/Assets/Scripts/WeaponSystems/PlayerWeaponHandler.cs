﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class PlayerWeaponHandler : MonoBehaviour
{
    public List<string> weaponsOwned;
    public bool pickingUpWep;
    public int ammoAddition;
    public float timeAddition;

    public void AddFirstWeapon(ref BaseWeapon firstWeapon)
    {
        weaponsOwned.Add(firstWeapon.name);
        firstWeapon.ActivateGun(true);
    }

    public void PickedUpWeapon(BaseWeapon weapon)
    {
        //Check the weapon doesn't have a parent
        if(weapon.gameObject.transform.parent == null || 
            string.CompareOrdinal(weapon.gameObject.transform.parent.tag, "SpawnHandler") == 0) {
            // Add effect
            CameraManager.ShakeScreen(2, 1.5f);
            CameraManager.ZoomIn(8, 4.5f, 4, 0.3f, transform.position, 5, 1);
            StartCoroutine(UIPickup(weapon));
            StartCoroutine(UISpin(weapon));

            bool alreadyOwned = false;
            //Check if already Owned!
            for(int i = 0; i < weaponsOwned.Count; i++) {
                if(weaponsOwned[i] == weapon.name)
                    alreadyOwned = true;
            }
            if (!alreadyOwned) {
                //Adding new Weapon!
                BoxCollider2D[] collider = weapon.GetComponents<BoxCollider2D>();
                for (int i = 0; i < collider.Length; i++)
                    collider[i].enabled = false;

                Destroy(weapon.GetComponent(typeof(Rigidbody2D)));
                weapon.transform.parent = this.transform;
                weapon.transform.localPosition = weapon.pickupPosition;
                weapon.transform.GetChild(1).transform.localScale = new Vector3(-weapon.pickupScale, weapon.pickupScale, weapon.pickupScale);
                weapon.ActivateGun(false);
                weapon.ResetWeaponAttributes();
                weaponsOwned.Add(weapon.name);
            } else {
                Debug.Log("we own it?");
                //Add Weapon Ammo or Time!!
                BaseWeapon[] ownedWeapons = gameObject.GetComponentsInChildren<BaseWeapon>(true);
                BaseWeapon matchingWeapon = ownedWeapons.First(w => string.CompareOrdinal(w.name, weapon.name) == 0);
                matchingWeapon.weaponAttribute.AddTimeOrAmmo();
                matchingWeapon.ResetWeaponAttributes();
                weapon.DestroyWeapon(); //------------Weapon is DESTROYED
            }
        }
        else {
            Debug.Log("Weapon belongs to a parent!");
            //do not pick up weapon.
        }
    }

    IEnumerator UISpin(BaseWeapon wep)
    {
        if (pickingUpWep)
        {
            while (pickingUpWep)
            {
                Vector3 rotation;
                rotation = wep.weapon_background.rectTransform.localEulerAngles;
                rotation.z += 50 * Time.deltaTime;
                wep.weapon_background.rectTransform.localEulerAngles = rotation;
                yield return null;
            }
        }
    }

    IEnumerator UIPickup(BaseWeapon wep)
    {
        if(wep.weapon_image)
            wep.weapon_image.gameObject.SetActive(true);
        if(wep.weapon_background)
            wep.weapon_background.gameObject.SetActive(true);
        pickingUpWep = true;

        float alpha = 0;
        Color newColorWep = wep.weapon_image.color;
        Color newColorBg = wep.weapon_background.color;
        while (alpha < 0.7f)
        {
            alpha += 8 * Time.deltaTime;

            newColorBg.a = alpha;
            newColorWep.a = alpha;
            wep.weapon_image.color = newColorWep;
            wep.weapon_background.color = newColorBg;
            
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while(alpha > 0)
        {
            alpha -= 8 * Time.deltaTime;

            newColorBg.a = alpha;
            newColorWep.a = alpha;
            wep.weapon_image.color = newColorWep;
            wep.weapon_background.color = newColorBg;

            yield return null;
        }

        pickingUpWep = false;
        wep.weapon_image.gameObject.SetActive(false);
        wep.weapon_background.gameObject.SetActive(false);
    }

    /// <summary>
    /// Give it the current Gun to deactivate, and it will give you the nextGun.
    /// </summary>
    /// <param name="weapon">The Current Gun Equipped.</param>
    /// <returns></returns>
    public BaseWeapon GetNextWeapon(BaseWeapon weapon) {
        // clear images
        for(int i = 0; i < LevelManager.instance.weaponUIList.transform.childCount; i ++)
        {
            Destroy(LevelManager.instance.weaponUIList.transform.GetChild(i).gameObject);
        }

        BaseWeapon baseWeapon;
        weapon.ActivateGun(false);  //deactivate the current gun.
        weapon.wepEnabled = true;
        BaseWeapon[] ownedWeapons = gameObject.GetComponentsInChildren<BaseWeapon>(true);
        IEnumerable<BaseWeapon> availableWeapons = ownedWeapons.Where(w => w.weaponAttribute.CheckIfWeaponAvailable() == true);
        if (availableWeapons.Count() > 1) // check that we have more then just the core weapon.
        {
            //Add an order by, to prioritize next available weapon by strength.
            IEnumerable<BaseWeapon> nonCoreWeapons = availableWeapons.Where(w => w.weaponAttribute.coreWeapon == false);
            baseWeapon = (nonCoreWeapons.Count() > 0) ? nonCoreWeapons.First() : availableWeapons.First();
        } else {
            baseWeapon = availableWeapons.First();
        }

        // Make ui image 
        for(int i = 0; i < availableWeapons.Count(); i++)
        {
            LevelManager.instance.SpawnWeaponUI(availableWeapons.ElementAt(i).weapon_asset, availableWeapons.ElementAt(i).scale, availableWeapons.ElementAt(i));
        }

        baseWeapon.ActivateGun(true);
        return baseWeapon;
    }

    private PlayerController controller;
    private float toggleTimer;
    private float toggleTime = 0.4f;
    public void Update()
    {
        if (!controller)
            controller = LevelManager.instance.player.GetComponent<PlayerController>();
        if (Input.GetKeyDown(controller.toggleWeaponKey) || TouchController.controller.GetTouchUp(TouchLocations.Down, 250, 120))
        {
            toggleTimer = toggleTime + Time.time;
            Tutorial.instance.toggled = true;
            LevelManager.instance.wepsEnabled = !LevelManager.instance.wepsEnabled;
        }
    }
}
