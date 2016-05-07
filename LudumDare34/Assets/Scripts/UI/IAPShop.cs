using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Purchasing;

public class IAPShop : MonoBehaviour, IStoreListener
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
    public static Item[] itemDatabase;

    private List<ShopItem> shopItems = new List<ShopItem>();

    [System.Serializable]
    public struct Item
    {
        public string title;
        public float cost;
        public int coinsAdded;
        public string desc;
        public Sprite icon;
        public bool repurshable;
        public int maxPurchases;
        public int multiplyer;
        public ItemType itemType;
    }

    public enum ItemType
    {
        health, ammo, timer, damagePistol, damageBoost, money
    }

    private static IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider;

    private static string kProductIDFifty = "fifty";
    private static string kProductIDFiveHundred = "fiveHundred";
    private static string kProductIDTwoThousand = "twoThousand";

    private static string kProductNameAppleFifty = "com.blazewolf.beargame.purchases.coins.50";             // Apple App Store identifier for the consumable product.
    private static string kProductNameAppleFiveHundred = "com.blazewolf.beargame.purchases.coins.500";             // Apple App Store identifier for the consumable product.
    private static string kProductNameAppleTwoThousand= "com.blazewolf.beargame.purchases.coins.2000";             // Apple App Store identifier for the consumable product.

    private static string kProductNameGooglePlayFifty = "com.blazewolf.beargame.purchases.coins.50";        // Google Play Store identifier for the consumable product.
    private static string kProductNameGooglePlayFiveHundred = "com.blazewolf.beargame.purchases.coins.500";        // Google Play Store identifier for the consumable product.
    private static string kProductNameGooglePlayTwoThousand = "com.blazewolf.beargame.purchases.coins.2000";        // Google Play Store identifier for the consumable product.

    void Awake()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
        builder.AddProduct(kProductIDFifty, ProductType.Consumable, new IDs() { { kProductNameAppleFifty, AppleAppStore.Name }, { kProductNameGooglePlayFifty, GooglePlay.Name }, });// Continue adding the non-consumable product.
        builder.AddProduct(kProductIDFiveHundred, ProductType.Consumable, new IDs() { { kProductNameAppleFiveHundred, AppleAppStore.Name }, { kProductNameGooglePlayFiveHundred, GooglePlay.Name }, });// And finish adding the subscription product.
        builder.AddProduct(kProductIDTwoThousand, ProductType.Consumable, new IDs() { { kProductNameAppleTwoThousand, AppleAppStore.Name }, { kProductNameGooglePlayTwoThousand, GooglePlay.Name }, });// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyConsumable(int amount)
    {
        // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
        if (amount == 50)
            BuyProductID(kProductIDFifty);
        else if(amount == 500)
            BuyProductID(kProductIDFiveHundred);
        else if(amount == 2000)
            BuyProductID(kProductIDTwoThousand);
    }

    void BuyProductID(string productId)
    {
        // If the stores throw an unexpected exception, use try..catch to protect my logic here.
        try
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // Complete the unexpected exception handling ...
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDFifty, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            MainMenu.instance.coins += 50;
        }

        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDFiveHundred, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            MainMenu.instance.coins += 500;
        }

        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDTwoThousand, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            MainMenu.instance.coins += 2000;
        }

        InfoManager.SetInfo("coins", MainMenu.instance.coins.ToString());

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    void Start()
    {
        itemDatabase = items;
    }

    public void Open()
    {
        boughtItems = GetBoughtItems(InfoManager.GetInfo("bought"));

        Close();
        selectedItem = -1;
        layout.position = new Vector2(0, layout.position.y);
        for (int i = 0; i < items.Length; i++)
        {
            if (boughtItems.Contains(i) && !items[i].repurshable)
                return;

            GameObject newShopItemObj = Instantiate(Resources.Load("ShopItem")) as GameObject;
            newShopItemObj.transform.SetParent(layout);
            newShopItemObj.transform.position = Vector3.zero;
            newShopItemObj.transform.localScale = Vector3.one;
            ShopItem newShopItem = newShopItemObj.GetComponent<ShopItem>();
            newShopItem.title = items[i].title;
            newShopItem.iap = true;
            if (items[i].repurshable)
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
            Debug.Log("Purchasing item...");
            
            if (selectedItem != -1)
            {
                // IAP
                if (selectedItem == 0)
                    BuyConsumable(50);
                else if (selectedItem == 1)
                    BuyConsumable(500);
                else if (selectedItem == 2)
                    BuyConsumable(2000);
            }
        }
    }
    
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
            float cost = items[selectedItem].cost;
            selectedCostText.text = "$" + cost.ToString();
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
