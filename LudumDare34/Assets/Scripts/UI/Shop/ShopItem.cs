using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour
{

    public Text selectedCostText;
    public Text selectedTitleText;
    public Image selectedIconImage;

    public Button selectedButton;

    public float cost;
    public string title;
    public string desc;
    public int index;
    public bool maxed;
    public int timesBought;
    public Sprite icon;
    public bool iap = false;

    void Update()
    {
        selectedCostText.text = (iap) ? "$" : "";
        selectedCostText.text += cost.ToString();
        selectedTitleText.text = title;
        selectedIconImage.sprite = icon;
        selectedButton.interactable = !maxed;
        if(maxed)
            selectedCostText.text = "Max";
    }
}
