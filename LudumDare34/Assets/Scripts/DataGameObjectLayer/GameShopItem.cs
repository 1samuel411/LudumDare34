using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameShopItem : MonoBehaviour {

    public Text selectedCostText;
    public Text selectedTitleText;
    public Image selectedIconImage;
    public Button selectedButton;

    private int _index;
    public int index { get { return _index; } }
    public ShopItem shopItem;
    public int multiplierAddition { get { return shopItem.multiplier * shopItem.timesBought; } }
    public bool isRepurchasable { get { return (shopItem.timesBought < shopItem.maxPurchases); } }

    public float cost {
        get {
            return shopItem.originalCost *
                    ((shopItem.iap) ? 1 : 
                        ((shopItem.timesBought == 0.0f) ? 1 : 
                            (shopItem.timesBought + 1.0f)));
        }
    }

    public void SetIndex(int index) {
        _index = index;
    }

    public void SetTimesBought(int xBought) {
        shopItem.SetTimesBought(xBought);
    }

    public void SetShopItem(ShopItem xShopItem) {
        shopItem = xShopItem;
        selectedCostText.text = cost.ToString();
        selectedTitleText.text = shopItem.title;
        selectedIconImage.overrideSprite = shopItem.icon;
    }
}
