using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class Shop : MonoBehaviour {
    public Text selectedTitleText;
    public Text selectedCostText;
    public Text selectedDescText;
    public Image selectedIconImage;
    public Image selectedCoinsImage;

    public int selectedItem = -1;
    protected bool purchasing;

    public Transform layout;

    public List<int> boughtItems = new List<int>();
    public ShopItem[] items;
    public static ShopItem[] itemDatabase;

    private List<GameShopItem> shopItems = new List<GameShopItem>();

    public GameShopItem shopItem;

    void Start() {
        itemDatabase = items;
        SelectItem(selectedItem);
    }

    public void Open()
    {
        boughtItems = GetBoughtItems(GameManager.instance.playerPurchases.Bought);
        Debug.Log("boughtItems: " + boughtItems.Count);
        Close();
        selectedItem = -1;
        layout.position = new Vector2(0, layout.position.y);
        if (items.Any()) {
            for (int i = 0; i < items.Length; i++) {
                GameObject newShopItemObj = Instantiate(Resources.Load("GameShopItem")) as GameObject;
                newShopItemObj.transform.SetParent(layout);
                newShopItemObj.transform.position = Vector3.zero;
                newShopItemObj.transform.localScale = Vector3.one;
                GameShopItem newShopItem = newShopItemObj.GetComponent<GameShopItem>();
                newShopItem.SetIndex(i);
                newShopItem.SetShopItem(items[i]);
                //Might be a problem, this means we can never change the order of the shop items.
                int timesBought = boughtItems.Count(s => s == newShopItem.index);
                newShopItem.SetTimesBought(timesBought);
                newShopItem.selectedButton.onClick.AddListener(() => { SelectItem(newShopItem.index); });
                shopItems.Add(newShopItem);
            }
        }
    }

    public void Close() {
        shopItems.Clear();
        for(int i = 0; i < layout.childCount; i++)
            Destroy(layout.GetChild(i).gameObject);
    }

    public void SelectItem(int item) {
        selectedItem = item;
        if(selectedItem != -1) {
            selectedTitleText.gameObject.SetActive(true);
            selectedCostText.gameObject.SetActive(true);
            selectedDescText.gameObject.SetActive(true);
            selectedCoinsImage.gameObject.SetActive(true);
            selectedIconImage.gameObject.SetActive(true);

            shopItem = shopItems.First(s => s.index == selectedItem);
            selectedTitleText.text = items[selectedItem].title;
            selectedCostText.text = shopItem.cost.ToString();
            selectedDescText.text = items[selectedItem].desc;
            selectedIconImage.sprite = items[selectedItem].icon;
        } else {
            selectedTitleText.gameObject.SetActive(false);
            selectedCostText.gameObject.SetActive(false);
            selectedDescText.gameObject.SetActive(false);
            selectedCoinsImage.gameObject.SetActive(false);
            selectedIconImage.gameObject.SetActive(false);
        }
    }

    public void PurchaseItem()
    {
        if (!purchasing)
        {
            purchasing = true;
            Debug.Log("selected Item purchase: " + shopItem.shopItem.title);
            Popup.Create("Are You Sure?", "Are you sure you want to purchase '" + items[selectedItem].title + "'?", "Yes", "No", false, CallbackPurchase);
        }
    }

    public virtual void CallbackPurchase(Popup.ResponseTypes response)
    {
        purchasing = false;
        if(response == Popup.ResponseTypes.Accepted)
        {
            Debug.Log("Purchasing item...");
            if(shopItem != null) {
                if(shopItem.cost > GameManager.instance.playerDetails.Coins) {
                    Debug.Log("Not enough money!");
                    Popup.Create("Not Enough Coins", "You do not have enough coins to afford this item!", "Okay", "", true);
                    return;
                }
                Debug.Log("Purchased Items: " + shopItem.shopItem.title);
                //can this item be bought?
                if (shopItem.isRepurchasable) {
                    GameManager.instance.playerDetails.Coins -= shopItem.cost;
                    boughtItems.Add(shopItem.index);
                    int xBought = shopItem.shopItem.timesBought + 1;
                    shopItem.SetTimesBought(xBought);
                    //Creates a CSV Of purchased Items.
                    GameManager.instance.playerPurchases.Bought = GetBoughtItemsString(boughtItems.ToArray());
                    GameManager.instance.playerPurchases.SynchronizeData();
                }
                selectedItem = -1;
            }
        }
    }

    //UnParses the CSV Line.
    public static List<int> GetBoughtItems(string items) {
        List<int> boughtItems = new List<int>();
        boughtItems.Clear();
        if (!string.IsNullOrEmpty(items)) {
            boughtItems = items.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToInt32(s.Trim())).ToList();
        }
        return boughtItems;
    }
    
    //Creates a CSV.
    public static string GetBoughtItemsString(int[] items) {
        string itemsBoughtString = string.Empty;
        for(int i = 0; i < items.Length; i ++) {
            itemsBoughtString += ((i == 0) ? string.Empty : ",") + items[i];
        }
        return itemsBoughtString;
    }
}
