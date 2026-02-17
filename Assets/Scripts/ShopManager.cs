using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Needed for the timer

public class ShopManager : MonoBehaviour
{
    // --- NEW SLOTS START HERE ---
    [Header("UI Groups (Drag Objects Here)")]
    public GameObject shopMainView;        // Drag 'Shop_Main_View' container here
    public GameObject purchasePopup;       // Drag 'PurchasePopup' panel here
    public TextMeshProUGUI warningText;    // Drag 'Text_Warning' here
    // ---------------------------

    [Header("Popup References")]
    public Image popupImage;               // Drag the Image inside the popup
    public TextMeshProUGUI itemNameText;   // Drag the Text inside the popup
    public Button equipButton;             // Drag the Button inside the popup
    public TextMeshProUGUI equipButtonText;// Drag the Text inside that button

    [Header("Main UI")]
    public TextMeshProUGUI scoreText;      

    [Header("Game Objects")]
    public GameObject petObject;           // Drag CyberPet here
    // We don't use this for the wallpaper anymore, but I left it so your inspector doesn't break
    public Image pcScreenImage;            

    [Header("Shop Items")]
    public ShopItem[] items;               

    private int currentItemIndex;          

    void OnEnable()
    {
        UpdateScoreUI();
        
        // Reset the views when game starts so only the Shop is visible
        if (shopMainView != null) shopMainView.SetActive(true);
        if (purchasePopup != null) purchasePopup.SetActive(false);
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    // --- BUTTON: BUY ITEM ---
    public void BuySpecificItem(int index)
    {
        if (index < 0 || index >= items.Length) return;
        ShopItem item = items[index];

        if (!item.isOwned)
        {
            // Try to spend money
            if (SaveManager.TrySpendCoins(item.price))
            {
                // SUCCESS: Mark owned, update score
                item.isOwned = true;
                UpdateScoreUI();
                
                // Switch Views: Hide Main Shop, Show Popup
                OpenEquipPopup(index); 
                if (shopMainView != null) shopMainView.SetActive(false);
            }
            else
            {
                // FAIL: Show Warning
                StartCoroutine(ShowWarning());
            }
        }
        else
        {
            // ALREADY OWNED: Just switch views
            OpenEquipPopup(index);
            if (shopMainView != null) shopMainView.SetActive(false);
        }
    }

    // --- HELPER: FLASH WARNING TEXT ---
    IEnumerator ShowWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f); // Wait 1.5 seconds
            warningText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Warning Text slot is empty!");
        }
    }

    // --- HELPER: SETUP POPUP ---
    void OpenEquipPopup(int index)
    {
        currentItemIndex = index;
        ShopItem item = items[index];

        // Set Text and Image
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (popupImage != null)
        {
            popupImage.sprite = item.icon;
            popupImage.preserveAspect = true; 
        }

        // Set Button State
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

    // --- BUTTON: EQUIP ITEM (THIS IS FIXED) ---
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
            // 1. Save the receipt (Remember that we own it)
            PlayerPrefs.SetInt("WallpaperID", 1);
            PlayerPrefs.Save();

            // 2. LIVE UPDATE: Find the computer and force it to update NOW
            // This finds the OSManager script in your scene and runs the update function
            OSManager computer = FindObjectOfType<OSManager>();
            if (computer != null)
            {
                computer.CheckAndSetWallpaper(); 
            }
        }

        PlayerPrefs.Save();
        
        // Update Button Visuals
        equipButtonText.text = "Equipped";
        equipButton.interactable = false; 
    }

   // --- BUTTON: CLOSE POPUP ---
    public void ClosePopup()
    {
        // 1. Hide the popup (Optional, but good for safety)
        if (purchasePopup != null) purchasePopup.SetActive(false);

        // 2. IMPORTANT: Turn off the ENTIRE Shop Object
        // This makes sure 'OnEnable' runs the next time you open it.
        gameObject.SetActive(false); 
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Credits: " + SaveManager.GetCoins();
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public Sprite icon; 
    public bool isOwned;
    public bool isEquipped;
}