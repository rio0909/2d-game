using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

public class ShopManager : MonoBehaviour
{
    // --- UI REFERENCES ---
    [Header("UI Groups")]
    public GameObject shopMainView;        
    public GameObject purchasePopup;       
    public TextMeshProUGUI warningText;    

    [Header("Popup References")]
    public Image popupImage;               
    public TextMeshProUGUI itemNameText;   
    public Button equipButton;             
    public TextMeshProUGUI equipButtonText;

    [Header("Main UI")]
    public TextMeshProUGUI scoreText;      

    [Header("Game Objects")]
    public GameObject petObject;           
    public Image pcScreenImage;            

    [Header("Shop Items")]
    public ShopItem[] items;               

    private int currentItemIndex;          

    void OnEnable()
    {
        // 1. LOAD SAVED DATA (Fixes the "Memory Loss")
        LoadShopProgress();

        UpdateScoreUI();
        
        // Reset the views
        if (shopMainView != null) shopMainView.SetActive(true);
        if (purchasePopup != null) purchasePopup.SetActive(false);
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    // --- BUTTON: BUY ITEM ---
    public void BuySpecificItem(int index)
    {
        if (index < 0 || index >= items.Length) return;
        ShopItem item = items[index];

        // 1. IF ALREADY OWNED
        if (item.isOwned)
        {
            OpenEquipPopup(index);
            if (shopMainView != null) shopMainView.SetActive(false);
            return;
        }

        // 2. IF BUYING NEW
        if (SaveManager.TrySpendCoins(item.price))
        {
            // Success!
            item.isOwned = true;
            
            // --- SAVE RECEIPT ---
            PlayerPrefs.SetInt("ShopItem_" + index, 1);
            PlayerPrefs.Save();
            // --------------------

            UpdateScoreUI();
            
            OpenEquipPopup(index); 
            if (shopMainView != null) shopMainView.SetActive(false);
        }
        else
        {
            // Fail
            StartCoroutine(ShowWarning());
        }
    }

    // --- HELPER: LOAD SAVED ITEMS ---
    void LoadShopProgress()
    {
        for (int i = 0; i < items.Length; i++)
        {
            // Check if we own it (1 = Yes)
            if (PlayerPrefs.GetInt("ShopItem_" + i, 0) == 1)
            {
                items[i].isOwned = true;
            }
        }

        // Check which wallpaper is equipped
        int currentWallpaper = PlayerPrefs.GetInt("WallpaperID", 0);
        if (items.Length > 1) 
        {
            items[1].isEquipped = (currentWallpaper == 1); 
        }
        
        // Check if Pet is enabled
         if (items.Length > 0)
        {
             items[0].isEquipped = (PlayerPrefs.GetInt("PetEnabled", 0) == 1);
        }
    }

    // --- HELPER: FLASH WARNING ---
    IEnumerator ShowWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            warningText.gameObject.SetActive(false);
        }
    }

    // --- HELPER: SETUP POPUP ---
    void OpenEquipPopup(int index)
    {
        currentItemIndex = index;
        ShopItem item = items[index];

        if (itemNameText != null) itemNameText.text = item.itemName;
        if (popupImage != null)
        {
            popupImage.sprite = item.icon;
            popupImage.preserveAspect = true; 
        }

        // Update Button Text
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

    // --- BUTTON: EQUIP ITEM ---
    public void OnEquipButtonClicked()
    {
        ShopItem item = items[currentItemIndex];
        
        // Mark as equipped locally
        item.isEquipped = true;
        
        // Update visuals immediately
        equipButtonText.text = "Equipped";
        equipButton.interactable = false;

        // Apply Logic
        if (currentItemIndex == 0) // PET
        {
            if (petObject != null) petObject.SetActive(true);
            PlayerPrefs.SetInt("PetEnabled", 1);
        }
        else if (currentItemIndex == 1) // WALLPAPER
        {
            PlayerPrefs.SetInt("WallpaperID", 1);
            
            // Force PC to update if it's in the scene
            OSManager computer = FindObjectOfType<OSManager>();
            if (computer != null) computer.CheckAndSetWallpaper();
        }

        PlayerPrefs.Save();
    }

    // --- BUTTON: CLOSE POPUP ---
    public void ClosePopup()
    {
        if (purchasePopup != null) purchasePopup.SetActive(false);
        gameObject.SetActive(false); 
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Credits: " + SaveManager.GetCoins();
    }
} // <--- THIS CLOSING BRACKET ENDS THE CLASS

// --- THIS MUST BE OUTSIDE THE CLASS ---
[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public Sprite icon; 
    public bool isOwned;
    public bool isEquipped;
}