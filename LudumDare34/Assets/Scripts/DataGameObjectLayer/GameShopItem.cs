using UnityEngine;
using UnityEngine.UI;

public class GameShopItem : MonoBehaviour {

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

    public void SetCost(float itemCost) {
        cost = itemCost;
        selectedCostText.text = cost.ToString();
    }

    public void SetTitle(string titleName) {
        selectedTitleText.text = titleName;
        title = titleName;
    }

    public void SetIcon(Sprite iconSprite) {
        icon = iconSprite;
        selectedIconImage.overrideSprite = iconSprite;
    }

    public void SetMaxed(bool isMaxed) {
        maxed = isMaxed;
        if(maxed)
            selectedCostText.text = "Max";
        selectedButton.interactable = !maxed;
    }
}
