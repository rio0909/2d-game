using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

public class ShopManager : MonoBehaviour
{
    // grabbing all the UI stuff so it actually works
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
    
    // new shop ui assets cuz the old ones were kinda mid mumu
    [Header("Shop UI Assets")]
    public Sprite boughtButtonSprite; 

    private int currentItemIndex;          

    void OnEnable()
    {
        // load saved data (finally fixed that weird memory loss bug)
        LoadShopProgress();

        UpdateScoreUI();
        
        // update the button pictures so they look right
        UpdateShopButtonsUI(); 
        
        // reset the views so nothing overlaps
        if (shopMainView != null) shopMainView.SetActive(true);
        if (purchasePopup != null) purchasePopup.SetActive(false);
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    // the big buy item button logic
    public void BuySpecificItem(int index)
    {
        if (index < 0 || index >= items.Length) return;
        ShopItem item = items[index];

        // check if they already bought it waaaa
        if (item.isOwned)
        {
            // do absolutely nothing. the button shouldn't be clickable anyway.
            return; 
        }

        // they actually wanna buy it
        if (SaveManager.TrySpendCoins(item.price))
        {
            // success! they had enough money
            item.isOwned = true;
            
            // save the receipt so they don't yell at me later
            PlayerPrefs.SetInt("ShopItem_" + index, 1);
            PlayerPrefs.Save();

            UpdateScoreUI();
            
            // change the button pic and turn it off instantly
            UpdateShopButtonsUI(); 
            
            OpenEquipPopup(index); 
            if (shopMainView != null) shopMainView.SetActive(false);
        }
        else
        {
            // fail - show the broke warning mumu
            StartCoroutine(ShowWarning());
        }
    }

    // helper to load the saved items back in
    void LoadShopProgress()
    {
        for (int i = 0; i < items.Length; i++)
        {
            // check if we own it (1 means yes)
            if (PlayerPrefs.GetInt("ShopItem_" + i, 0) == 1)
            {
                items[i].isOwned = true;
            }
        }

        // gotta check which wallpaper they have on
        int currentWallpaper = PlayerPrefs.GetInt("WallpaperID", 0);
        if (items.Length > 1) 
        {
            items[1].isEquipped = (currentWallpaper == 1); 
        }
        
         // check if the little pet is out
         if (items.Length > 0)
        {
             items[0].isEquipped = (PlayerPrefs.GetInt("PetEnabled", 0) == 1);
        }
    }
    
    // helper to swap out button images when they buy stuff
    void UpdateShopButtonsUI()
    {
        for (int i = 0; i < items.Length; i++)
        {
            // if we own the item, and i remembered to assign the image in the inspector...
            if (items[i].isOwned && items[i].buyButtonImage != null)
            {
                // swap the sprite to the bought one
                if (boughtButtonSprite != null)
                {
                    items[i].buyButtonImage.sprite = boughtButtonSprite;
                }
                
                // kill the button so they don't spam click it waaaa
                Button btn = items[i].buyButtonImage.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }
        }
    }

    // flash the warning text if they are broke
    IEnumerator ShowWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            warningText.gameObject.SetActive(false);
        }
    }

    // setup the popup window for equipping
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

        // update button text depending on if it's equipped
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

    // equip item button logic mumu
    public void OnEquipButtonClicked()
    {
        ShopItem item = items[currentItemIndex];
        
        // mark as equipped locally
        item.isEquipped = true;
        
        // update visuals immediately so it feels responsive
        equipButtonText.text = "Equipped";
        equipButton.interactable = false;

        // actually do the thing
        if (currentItemIndex == 0) // pet
        {
            if (petObject != null) petObject.SetActive(true);
            PlayerPrefs.SetInt("PetEnabled", 1);
        }
        else if (currentItemIndex == 1) // wallpaper
        {
            PlayerPrefs.SetInt("WallpaperID", 1);
            
            // force pc to update if it's in the scene
            OSManager computer = FindObjectOfType<OSManager>();
            if (computer != null) computer.CheckAndSetWallpaper();
        }

        PlayerPrefs.Save();
    }

    // get this popup off my screen waaaa
    public void ClosePopup()
    {
        if (purchasePopup != null) purchasePopup.SetActive(false);
        gameObject.SetActive(false); 
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Credits: " + SaveManager.GetCoins();
    }
} // please don't delete this bracket

// this has to be outside the class or unity will throw a fit
[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public Sprite icon; 
    public bool isOwned;
    public bool isEquipped;
    
    // reference to the button's image in the UI so we can swap it later mumu
    public Image buyButtonImage; 
}