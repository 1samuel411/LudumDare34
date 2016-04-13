using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public Item[] items;

    private List<ShopItem> shopItems = new List<ShopItem>();

    [System.Serializable]
    public struct Item
    {
        public string title;
        public int cost;
        public string desc;
        public Sprite icon;
        public bool repurshable;
    }

    public void Open()
    {
        GetBoughtItems(InfoManager.GetInfo("bought"));

        Close();
        selectedItem = -1;
        layout.position = new Vector2(0, layout.position.y);
        for (int i = 0; i < items.Length; i++)
        {
            if (!boughtItems.Contains(i) || items[i].repurshable)
            {
                GameObject newShopItemObj = Instantiate(Resources.Load("ShopItem")) as GameObject;
                newShopItemObj.transform.SetParent(layout);
                newShopItemObj.transform.position = Vector3.zero;
                newShopItemObj.transform.localScale = Vector3.one;
                ShopItem newShopItem = newShopItemObj.GetComponent<ShopItem>();
                newShopItem.title = items[i].title;
                if(items[i].repurshable)
                {
                    int timesBought = 0;
                    for(int x = 0; x < boughtItems.Count; x++)
                    {
                        if (boughtItems[x] == i)
                            timesBought++;
                    }
                    newShopItem.cost = items[i].cost * ((timesBought == 0) ? 1 : (timesBought + 1));
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

    public void PurchaseItem()
    {
        Popup.Create("Are You Sure?", "Are you sure you want to purchase '" + items[selectedItem].title + "'?", "Yes", "No", false, CallbackPurchase);
    }

    public void CallbackPurchase(Popup.ResponseTypes response)
    {
        if(response == Popup.ResponseTypes.Accepted)
        {
            Item shopItem = items[selectedItem];
            Debug.Log("Purchasing item...");
            if (selectedItem != -1)
            {
                if (shopItem.cost > MainMenu.instance.coins)
                {
                    Debug.Log("Not enough money!");
                    Popup.Create("Not Enough Coins", "You do not have enough coins to afford this item!", "Okay", "", true);
                    return;
                }
                Debug.Log("Purchased Items: " + shopItem.title);

                MainMenu.instance.coins -= shopItem.cost;
                boughtItems.Add(selectedItem);

                for (int i = 0; i < shopItems.Count; i++)
                {
                    if (shopItems[i].index == selectedItem)
                        if (items[i].repurshable)
                        {
                            int timesBought = 0;
                            for (int x = 0; x < boughtItems.Count; x++)
                            {
                                if (boughtItems[x] == i)
                                    timesBought++;
                            }
                            shopItems[i].cost = items[i].cost * ((timesBought == 0) ? 1 : (timesBought+1));
                        }
                        else
                        {
                            shopItems[i].cost = items[i].cost;
                        }

                    if (shopItems[i].index == selectedItem && !items[shopItems[i].index].repurshable)
                    {
                        selectedItem = -1;
                        Destroy(shopItems[i].gameObject);
                    }
                }
                InfoManager.SetInfo("bought", GetBoughtItemsString(boughtItems.ToArray()));
                InfoManager.SetInfo("coins", MainMenu.instance.coins.ToString());
            }
        }
    }

    public void GetBoughtItems(string items)
    {
        boughtItems.Clear();
        if (items != "")
        {
            string[] individualItems = items.Split(',');
            for (int i = 0; i < individualItems.Length; i++)
            {
                boughtItems.Add(Int32.Parse(individualItems[i]));
            }
        }
    }

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
            if (items[selectedItem].repurshable)
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
