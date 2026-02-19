/***********************************************************************************************************
 * Produced by Madfireon:               https://www.madfireongames.com/									   *
 * Facebook:                            https://www.facebook.com/madfireon/								   *
 * Contact us:                          https://www.madfireongames.com/contact							   *
 * Madfireon Unity Asset Store catalog: https://bit.ly/2JjKCtw											   *
 * Developed by Swapnil Rane:           https://in.linkedin.com/in/swapnilrane24                           *
 ***********************************************************************************************************/

using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class TrafficGameManager : MonoBehaviour
{
    public static TrafficGameManager instance; // FIXED NAME

    private TrafficGameData data; // FIXED NAME

    public bool gameStarted = false, gameOver = false, retry = false, tutorialShowned = false;
    public bool fbConnected = false, loginReward, shareReward, likeReward;
    public int totalCars, totalCoins, gamesPlayed = 0, lastDistance, giftPoints;
    public float normalSpeed, turboSpeed, fuel, turboTime, magnetTime, doubleCoinTime;
    public GameObject playerCar;

    #region Data Stored On Device
    [Header("Dont Make Changes")]
    public bool isGameStartedFirstTime, canShowAds, isMusicOn;

    public CarData[] carDatas;
    public int selectedCar, GDPRConset;
    public int turboUpgrade, magnetUpgrade, doubleCoinUpgrade;
    public int currentCoinsEarned;
    public float currentDistance;
    public int coinAmount, totalCoinsSpend, totalCarCollisions;  
    public int bestDistance = 0;                                  
    #endregion

    void Awake()
    {
        MakeSingleton();
        InitializeGameVariables();
    }

    void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void InitializeGameVariables()
    {
        Load();
        if (data != null)
        {
            isGameStartedFirstTime = data.getIsGameStartedFirstTime();
        }
        else
        {
            isGameStartedFirstTime = true;
        }

        if (isGameStartedFirstTime)
        {
            isGameStartedFirstTime = false;
            DefaultData();
            SetData();
            Save();
            Load();
        }
        else
        {
            GetData();
        }
    }

    void DefaultData()
    {
        isMusicOn = true;
        canShowAds = true;
        tutorialShowned = false;
        fbConnected = false;
        loginReward = false;
        shareReward = false;
        likeReward = false;
        bestDistance = 0;
        coinAmount = totalCoins;
        totalCarCollisions = totalCoinsSpend = 0;
        giftPoints = 0;
        carDatas = new CarData[totalCars];
        selectedCar = 0;
        GDPRConset = 0;
        for (int i = 0; i < carDatas.Length; i++)
        {
            carDatas[i].unlocked = false;
        }

        carDatas[0].unlocked = true;

        turboUpgrade = 0;
        magnetUpgrade = 0;
        doubleCoinUpgrade = 0;

        data = new TrafficGameData(); // FIXED NAME
    }

    void SetData()
    {
        data.setIsGameStartedFirstTime(isGameStartedFirstTime);
        data.setCanShowAds(canShowAds);
        data.setTutorialShowned(true);
        data.setFBConnected(fbConnected);
        data.setFBLoginReward(loginReward);
        data.setFBShareReward(shareReward);
        data.setFBLikeReward(likeReward);
        data.setIsMusicOn(isMusicOn);
        data.setBestDistance(bestDistance);
        data.setCoins(coinAmount);
        data.setSelectedCar(selectedCar);
        data.setCarData(carDatas);
        data.setTurboUpgrade(turboUpgrade);
        data.setMagnetUpgrade(magnetUpgrade);
        data.setDoubleCoinUpgrade(doubleCoinUpgrade);
        data.setTotalCarCollisions(totalCarCollisions);
        data.setGiftPoints(giftPoints);
        data.setTotalCoinsSpend(totalCoinsSpend);
        data.setGDPRConset(GDPRConset);
    }

    void GetData()
    {
        isGameStartedFirstTime = data.getIsGameStartedFirstTime();
        canShowAds = data.getCanShowAds();
        tutorialShowned = data.getTutorialShowned();
        fbConnected = data.getFBConnected();
        loginReward = data.getFBLoginReward();
        shareReward = data.getFBShareReward();
        likeReward = data.getFBLikeReward();
        bestDistance = data.getBestDistancee();
        coinAmount = data.getCoins();
        selectedCar = data.getSelectedCar();
        carDatas = data.getCarData();
        turboUpgrade = data.getTurboUpgrade();
        magnetUpgrade = data.getMagnetUpgrade();
        doubleCoinUpgrade = data.getDoubleCoinUpgrade();
        totalCoinsSpend = data.getTotalCoinsSpend();
        giftPoints = data.getGiftPoints();
        totalCarCollisions = data.getTotalCarCollisions();
        isMusicOn = data.getIsMusicOn();
        GDPRConset = data.getGDPRConset();
    }

    public void Save()
    {
        FileStream file = null;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/TrafficGameData.dat"); // FIXED SAVE NAME

            if (data != null)
            {
                SetData();
                bf.Serialize(file, data);
            }
        }
        catch (Exception) { }
        finally
        {
            if (file != null) file.Close();
        }
    }

    public void Load()
    {
        FileStream file = null;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/TrafficGameData.dat", FileMode.Open); // FIXED SAVE NAME
            data = (TrafficGameData)bf.Deserialize(file); // FIXED NAME
        }
        catch (Exception) { }
        finally
        {
            if (file != null) file.Close();
        }
    }

    public void ResetGameManager()
    {
        isGameStartedFirstTime = true;
        DefaultData();
        SetData();
        Save();
        Load();
        Debug.Log("GameManager Reset");
    }
}

[Serializable]
public struct CarData
{
    public bool unlocked;
    public int carLevel;
}

[Serializable]
class TrafficGameData // FIXED CLASS NAME
{
    private bool isGameStartedFirstTime, canShowAds, isMusicOn, tutorialShowned;
    private bool fbConnected = false, loginReward, shareReward, likeReward;
    private int bestDistance, GDPRConset = 0;
    private int turboUpgrade, magnetUpgrade, doubleCoinUpgrade, giftPoints;
    private int selectedCar;
    private CarData[] carDatas;
    private int coins, totalCoinsSpend, totalCarCollisions; 

    public void setIsGameStartedFirstTime(bool isGameStartedFirstTime) { this.isGameStartedFirstTime = isGameStartedFirstTime; }
    public bool getIsGameStartedFirstTime() { return this.isGameStartedFirstTime; }
    public void setFBLoginReward(bool loginReward) { this.loginReward = loginReward; }
    public bool getFBLoginReward() { return this.loginReward; }
    public void setFBShareReward(bool shareReward) { this.shareReward = shareReward; }
    public bool getFBShareReward() { return this.shareReward; }
    public void setFBLikeReward(bool likeReward) { this.likeReward = likeReward; }
    public bool getFBLikeReward() { return this.likeReward; }
    public void setFBConnected(bool fbConnected) { this.fbConnected = fbConnected; }
    public bool getFBConnected() { return this.fbConnected; }
    public void setTutorialShowned(bool tutorialShowned) { this.tutorialShowned = tutorialShowned; }
    public bool getTutorialShowned() { return this.tutorialShowned; }
    public void setCanShowAds(bool canShowAds) { this.canShowAds = canShowAds; }
    public bool getCanShowAds() { return this.canShowAds; }
    public void setCarData(CarData[] carDatas) { this.carDatas = carDatas; }
    public CarData[] getCarData() { return carDatas; }
    public void setIsMusicOn(bool isMusicOn) { this.isMusicOn = isMusicOn; }
    public bool getIsMusicOn() { return this.isMusicOn; }
    public void setBestDistance(int bestDistance) { this.bestDistance = bestDistance; }
    public int getBestDistancee() { return this.bestDistance; }
    public void setGiftPoints(int giftPoints) { this.giftPoints = giftPoints; }
    public int getGiftPoints() { return this.giftPoints; }
    public void setCoins(int coins) { this.coins = coins; }
    public int getCoins() { return this.coins; }
    public void setGDPRConset(int GDPRConset) { this.GDPRConset = GDPRConset; }
    public int getGDPRConset() { return this.GDPRConset; }
    public void setTotalCoinsSpend(int totalCoinsSpend) { this.totalCoinsSpend = totalCoinsSpend; }
    public int getTotalCoinsSpend() { return this.totalCoinsSpend; }
    public void setTotalCarCollisions(int totalCarCollisions) { this.totalCarCollisions = totalCarCollisions; }
    public int getTotalCarCollisions() { return this.totalCarCollisions; }
    public void setSelectedCar(int selectedSkin) { this.selectedCar = selectedSkin; }
    public int getSelectedCar() { return this.selectedCar; }
    public void setTurboUpgrade(int turboUpgrade) { this.turboUpgrade = turboUpgrade; }
    public int getTurboUpgrade() { return this.turboUpgrade; }
    public void setMagnetUpgrade(int magnetUpgrade) { this.magnetUpgrade = magnetUpgrade; }
    public int getMagnetUpgrade() { return this.magnetUpgrade; }
    public void setDoubleCoinUpgrade(int doubleCoinUpgrade) { this.doubleCoinUpgrade = doubleCoinUpgrade; }
    public int getDoubleCoinUpgrade() { return this.doubleCoinUpgrade; }
}