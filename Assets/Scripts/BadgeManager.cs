using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BadgeManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject badgePopupPanel;   
    public Image badgeIconImage;         
    public TextMeshProUGUI badgeNameText;
    
    [Header("Badge Data")]
    public List<Badge> allBadges;        

    // --- NEW: A waiting list for badges ---
    private Queue<Badge> badgeQueue = new Queue<Badge>();
    private bool isShowingBadges = false;

    void Start()
    {
        if (badgePopupPanel != null) badgePopupPanel.SetActive(false);
    }

    // --- 1. CALL THIS TO "EARN" THE BADGE (BUT NOT SHOW IT YET) ---
    public void UnlockBadge(string badgeID)
    {
        // Check if we already own it permanently
        if (PlayerPrefs.GetInt("Badge_" + badgeID, 0) == 1)
        {
            return; // Already have it, ignore.
        }

        // Check if it's already in the queue waiting to be shown
        foreach(Badge b in badgeQueue)
        {
            if (b.id == badgeID) return; 
        }

        // Find the badge data
        Badge badge = allBadges.Find(b => b.id == badgeID);
        
        if (badge != null)
        {
            // Save it PERMANENTLY right now
            PlayerPrefs.SetInt("Badge_" + badgeID, 1);
            PlayerPrefs.Save();

            // Add it to the "Waiting List" instead of showing immediately
            badgeQueue.Enqueue(badge);
        }
    }

    // --- 2. CALL THIS WHEN YOU CLOSE THE COMPUTER ---
    public void ShowPendingBadges()
    {
        if (!isShowingBadges && badgeQueue.Count > 0)
        {
            StartCoroutine(ProcessBadgeQueue());
        }
    }

    IEnumerator ProcessBadgeQueue()
    {
        isShowingBadges = true;

        // Loop through everything in the waiting list
        while (badgeQueue.Count > 0)
        {
            Badge b = badgeQueue.Dequeue(); // Get the next badge

            // Setup UI
            if (badgeNameText != null) badgeNameText.text = "UNLOCKED: " + b.displayName;
            if (badgeIconImage != null) badgeIconImage.sprite = b.icon;
            if (badgePopupPanel != null) badgePopupPanel.SetActive(true);

            // Wait 3 seconds
            yield return new WaitForSeconds(3f);

            // Hide
            if (badgePopupPanel != null) badgePopupPanel.SetActive(false);
            
            // Small pause between multiple badges
            yield return new WaitForSeconds(0.5f);
        }

        isShowingBadges = false;
    }
}

[System.Serializable]
public class Badge
{
    public string id;           
    public string displayName;  
    public Sprite icon;         
}