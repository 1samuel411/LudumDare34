using UnityEngine;

[System.Serializable]
public class ShopItem {
    private int _timeBought;
    public string title;
    public float originalCost;
    public string desc;
    public Sprite icon;
    public int maxPurchases;
    public int timesBought { get { return _timeBought; } }
    public int multiplier;
    public bool iap = false;
    public ItemType itemType;

    public void SetTimesBought(int xBought) {
        _timeBought = xBought;
    }

    public enum ItemType {
        health,
        ammo,
        timer,
        damagePistol,
        damageBoost,
        Money
    }
}
