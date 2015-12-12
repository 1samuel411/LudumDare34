using UnityEngine;
using System.Collections;
using System.Configuration;

public class BaseHealth : IDamageable {

    public int currentHealth = 1;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (value > currentHealth)
                DestroyThisObject();
            else
                currentHealth = value;
        }
    }
	// Use this for initialization
	void Start () {
	
	}

    #region IDamageable Members

    public int DealDamage(int damage)
    {
        currentHealth -= damage;
        //Implement MissChance.

        return currentHealth;
    }

    public void DestroyThisObject() {
        
    }
    #endregion
}
