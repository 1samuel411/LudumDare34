using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour
{

    public Text selectedCostText;
    public Text selectedTitleText;
    public Image selectedIconImage;

    public Button selectedButton;

    public int cost;
    public string title;
    public string desc;
    public int index;
    public Sprite icon;

    void Start()
    {

    }

    void Update()
    {
        selectedCostText.text = cost.ToString();
        selectedTitleText.text = title;
        selectedIconImage.sprite = icon;
    }
}
