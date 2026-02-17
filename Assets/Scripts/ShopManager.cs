using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;      
    public GameObject purchasePopup;       
    public Image popupImage;               
    public TextMeshProUGUI itemNameText;   // NEW: The text that shows the item name
    public Button equipButton;             
    public TextMeshProUGUI equipButtonText;

    [Header("Game Objects")]
    public GameObject petObject;           
    public Image pcScreenImage;            

    [Header("Shop Items")]
    public ShopItem[] items;               

    private int currentItemIndex;          

    void Start()
    {
        UpdateScoreUI();
        if (purchasePopup != null) purchasePopup.SetActive(false);
    }

    // --- TRIGGER: BUY BUTTON (On the Plank) ---
    public void BuySpecificItem(int index)
    {
        if (index < 0 || index >= items.Length) return;
        ShopItem item = items[index];

        if (!item.isOwned)
        {
            if (SaveManager.TrySpendCoins(item.price))
            {
                item.isOwned = true;
                UpdateScoreUI();
                OpenEquipPopup(index); 
            }
            else Debug.Log("Not enough money!");
        }
        else
        {
            OpenEquipPopup(index);
        }
    }

    // --- ACTION: OPEN POPUP ---
    void OpenEquipPopup(int index)
    {
        currentItemIndex = index;
        ShopItem item = items[index];

        // 1. Show the Name
        if (itemNameText != null) itemNameText.text = item.itemName;

        // 2. Show the Image
        if (popupImage != null)
        {
            popupImage.sprite = item.icon;
            popupImage.preserveAspect = true; 
        }

        // 3. Update Button
        if (item.isEquipped)
        {
            equipButtonText.text = "Equipped";
            equipButton.interactable = false; 
        }
        else
        {
            equipButtonText.text = "Equip";
            equipButton.interactable = true;
        }

        purchasePopup.SetActive(true);
    }

    // --- TRIGGER: EQUIP BUTTON ---
    public void OnEquipButtonClicked()
    {
        ShopItem item = items[currentItemIndex];
        item.isEquipped = true;

        if (currentItemIndex == 0) // PET
        {
            if (petObject != null) petObject.SetActive(true);
            PlayerPrefs.SetInt("PetEnabled", 1);
        }
        else if (currentItemIndex == 1) // PC WALLPAPER
        {
            if (pcScreenImage != null) pcScreenImage.sprite = item.icon;
            PlayerPrefs.SetInt("WallpaperID", currentItemIndex);
        }

        PlayerPrefs.Save();
        equipButtonText.text = "Equipped";
        equipButton.interactable = false; 
    }

    public void ClosePopup()
    {
        purchasePopup.SetActive(false);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + SaveManager.GetCoins();
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemName; // Make sure this is filled out in the Inspector!
    public int price;
    public Sprite icon; 
    public bool isOwned;
    public bool isEquipped;
}