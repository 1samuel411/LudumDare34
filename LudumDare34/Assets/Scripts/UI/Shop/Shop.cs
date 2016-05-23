using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Amazon.CognitoSync.SyncManager;

public class Shop : MonoBehaviour
{

    public Text selectedTitleText;
    public Text selectedCostText;
    public Text selectedDescText;
    public Image selectedIconImage;
    public Image selectedCoinsImage;

    public int selectedItem = -1;

    public Transform layout;

    public List<int> boughtItems = new List<int>();
    public Dataset purchasedItems;



    public Item[] items;
    public static Item[] itemDatabase;

    private List<ShopItem> shopItems = new List<ShopItem>();

    [System.Serializable]
    public struct Item
    {
        public string title;
        public int cost;
        public string desc;
        public Sprite icon;
        public bool repurchasable;
        public int maxPurchases;
        public int multiplyer;
        public ItemType itemType;
    }

    public enum ItemType
    {
        health, ammo, timer, damagePistol, damageBoost
    }

    void Start()
    {
        itemDatabase = items;
        //purchasedItems = CameraManager.instance.gameManager.syncManager.OpenOrCreateDataset("Purchases");
    }

    public void Open()
    {
        boughtItems = GetBoughtItems(InfoManager.GetInfo("bought"));
        Close();
        selectedItem = -1;
        layout.position = new Vector2(0, layout.position.y);
        for (int i = 0; i < items.Length; i++)
        {
            if (boughtItems.Contains(i) && !items[i].repurchasable)
                return;

            GameObject newShopItemObj = Instantiate(Resources.Load("ShopItem")) as GameObject;
            newShopItemObj.transform.SetParent(layout);
            newShopItemObj.transform.position = Vector3.zero;
            newShopItemObj.transform.localScale = Vector3.one;
            ShopItem newShopItem = newShopItemObj.GetComponent<ShopItem>();
            newShopItem.title = items[i].title;
            if (items[i].repurchasable)
            {
                int timesBought = 0;
                for (int x = 0; x < boughtItems.Count; x++)
                {
                    if (boughtItems[x] == i)
                        timesBought++;
                }
                newShopItem.cost = items[i].cost * ((timesBought == 0) ? 1 : (timesBought + 1));
                newShopItem.timesBought = timesBought;
                if (timesBought > items[i].maxPurchases)
                    newShopItem.maxed = true;
                else
                    newShopItem.maxed = false;
            }
            else
            {
                newShopItem.cost = items[i].cost;
            }
            newShopItem.desc = items[i].desc;
            newShopItem.icon = items[i].icon;
            newShopItem.index = i;
            newShopItem.selectedButton.onClick.AddListener(() => { SelectItem(newShopItem.index); });
            shopItems.Add(newShopItem);
        }
    }

    public void Close()
    {
        shopItems.Clear();
        for(int i = 0; i < layout.childCount; i++)
        {
            GameObject.Destroy(layout.GetChild(i).gameObject);
        }
    }

    public void SelectItem(int shopItem)
    {
        selectedItem = shopItem;
    }

    private bool purchasing;
    public void PurchaseItem()
    {
        if (!purchasing)
        {
            purchasing = true;
            Popup.Create("Are You Sure?", "Are you sure you want to purchase '" + items[selectedItem].title + "'?", "Yes", "No", false, CallbackPurchase);
        }
    }

    public void CallbackPurchase(Popup.ResponseTypes response)
    {
        purchasing = false;
        if(response == Popup.ResponseTypes.Accepted)
        {
            Item shopItem = items[selectedItem];
            Debug.Log("Purchasing item...");
            
            if (selectedItem != -1)
            {
                int timesBought = 0;
                for (int x = 0; x < boughtItems.Count; x++)
                {
                    if (boughtItems[x] == selectedItem)
                        timesBought++;
                }
                shopItem.cost = shopItem.cost * ((timesBought == 0) ? 1 : (timesBought + 1));

                Debug.Log(shopItem.cost);
                if (shopItem.cost > MainMenu.instance.Coins)
                {
                    Debug.Log("Not enough money!");
                    Popup.Create("Not Enough Coins", "You do not have enough coins to afford this item!", "Okay", "", true);
                    return;
                }
                Debug.Log("Purchased Items: " + shopItem.title);

                MainMenu.instance.Coins -= shopItem.cost;
                boughtItems.Add(selectedItem);

                for (int i = 0; i < shopItems.Count; i++)
                {
                    if (shopItems[i].index == selectedItem)
                        if (items[i].repurchasable)
                        {
                            timesBought = 0;
                            for (int x = 0; x < boughtItems.Count; x++)
                            {
                                if (boughtItems[x] == i)
                                    timesBought++;
                            }
                            shopItems[i].cost = items[i].cost * ((timesBought == 0) ? 1 : (timesBought+1));
                            if (timesBought > items[i].maxPurchases)
                                shopItems[i].maxed = true;
                            else
                                shopItems[i].maxed = false;
                        }
                        else
                        {
                            shopItems[i].cost = items[i].cost;
                        }

                    if (shopItems[i].index == selectedItem && !items[shopItems[i].index].repurchasable)
                    {
                        selectedItem = -1;
                        Destroy(shopItems[i].gameObject);
                    }
                }
                //Creates a CSV Of purchased Items.
                InfoManager.SetInfo("bought", GetBoughtItemsString(boughtItems.ToArray()));
                //InfoManager.SetInfo("coins", MainMenu.instance._coins.ToString());
            }
        }
    }

    //UnParses the CSV Line.
    public static List<int> GetBoughtItems(string items)
    {
        List<int> boughtItems = new List<int>();
        boughtItems.Clear();
        if (items != "")
        {
            string[] individualItems = items.Split(',');
            for (int i = 0; i < individualItems.Length; i++)
            {
                boughtItems.Add(Int32.Parse(individualItems[i]));
            }
        }
        return boughtItems;
    }
    
    //Creates a CSV.
    public static string GetBoughtItemsString(int[] items)
    {
        string itemsBoughtString = "";
        for(int i = 0; i < items.Length; i ++)
        {
            itemsBoughtString += ((i == 0) ? "" : ",") + items[i];
        }
        return itemsBoughtString;
    }

    void Update()
    {
        if(selectedItem != -1)
        {
            selectedTitleText.gameObject.SetActive(true);
            selectedCostText.gameObject.SetActive(true);
            selectedDescText.gameObject.SetActive(true);
            selectedCoinsImage.gameObject.SetActive(true);
            selectedIconImage.gameObject.SetActive(true);

            selectedTitleText.text = items[selectedItem].title;
            int cost = 0;
            if (items[selectedItem].repurchasable)
            {
                int timesBought = 0;
                for (int x = 0; x < boughtItems.Count; x++)
                {
                    if (boughtItems[x] == selectedItem)
                        timesBought++;
                }
                cost = items[selectedItem].cost * ((timesBought == 0) ? 1 : (timesBought + 1));
            }
            else
            {
                cost = items[selectedItem].cost;
            }
            selectedCostText.text = cost.ToString();
            selectedDescText.text = items[selectedItem].desc;
            selectedIconImage.sprite = items[selectedItem].icon;
        }
        else
        {
            selectedTitleText.gameObject.SetActive(false);
            selectedCostText.gameObject.SetActive(false);
            selectedDescText.gameObject.SetActive(false);
            selectedCoinsImage.gameObject.SetActive(false);
            selectedIconImage.gameObject.SetActive(false);
        }
    }
}
