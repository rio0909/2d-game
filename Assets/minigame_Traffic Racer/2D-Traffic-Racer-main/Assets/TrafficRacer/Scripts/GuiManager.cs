/***********************************************************************************************************
 * Produced by Madfireon:               https://www.madfireongames.com/                                    *
 * Facebook:                            https://www.facebook.com/madfireon/                                *
 * Contact us:                          https://www.madfireongames.com/contact                             *
 * Madfireon Unity Asset Store catalog: https://bit.ly/2JjKCtw                                             *
 * Developed by Swapnil Rane:           https://in.linkedin.com/in/swapnilrane24                           *
 ***********************************************************************************************************/

/***********************************************************************************************************
* NOTE:- This script controls game menu                                                                    *
***********************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GuiManager : MonoBehaviour {

    public static GuiManager instance;

    #region Serialized Variables
    [SerializeField] [Header("Game Menu Elements")]
    private GameMenu gameMenu;                                                                          
    [SerializeField][Header("-----------------------")][Header("Main Menu Elements")]
    private MainMenu mainMenu;                                                                          
    [SerializeField] [Header("-----------------------")] [Header("GameOver Menu Elements")]
    private GameOverMenu gameOverMenu;                                                                  
    [SerializeField] [Header("-----------------------")] [Header("Tutorial Menu Elements")]
    private Tutorial tutorialElements;                                                                  
    [SerializeField] [Header("-----------------------")] [Header("Facebook Menu Elements")]
    public FacebookPanel facebookElements;                                                              
    [SerializeField] [Header("-----------------------")] [Header("Speed Increaser")]
    private SpeedIncreaser speedIncreaser;                                                              

    [SerializeField] [Header("-----------------------")] private GameObject mainMenuObj;                
    [SerializeField] private GameObject gameMenuObj, gameoverMenu, reviveMenu, carShopPanel, turboEffect, powerupShop, gdprAdmobPanel;
    [SerializeField] private ScrollTexture road;                                                        
    [SerializeField] private float currentSpeed;                                                        
    [SerializeField] private Button noAdsbtn;                                                           
    #endregion

    /*---------------------------------------------------Private Variables------------------------------------------------------------*/

    #region Private Variables
    private float currentFuel, maxEnemyCarGap = 4;                                                      
    private float currentMagnetTime, currentTurboTime, currentDoubleCoinTime;                           
    private float countDown = 4, distanceMultiplier, giftBarTime;                                       
    private bool fuelSpawned = false, startCountDown, revived = false, giftBarActive = false;   
    private bool magnetActive, turboActive, doubleCoinActive, shieldActive;                             
    private bool magnetSpawned, turboSpawned, doubleCoinSpawned, shieldSpawned;                         
    private GameObject coinObject, coinUIObj;                                                           
    private int coinIncreaser = 1;                                                                      
    private int currentTipIndex = 0;
    #endregion

    /*----------------------------------------------------Getter And Setter----------------------------------------------------------*/

    #region Getter And Setter
    public Button NoAdsBtn          { get { return noAdsbtn;       } }
    public GameObject GDPRpanel     { get { return gdprAdmobPanel; } }
    public float DistanceMultiplier { get { return distanceMultiplier; } set { distanceMultiplier = value; } }
    public float CurrentFuel        { get { return currentFuel;    } }
    public float CurrentSpeed       { get { return currentSpeed; }       set { currentSpeed       = value; } }
    public float MaxEnemyCarGap     { get { return maxEnemyCarGap; }     set { maxEnemyCarGap     = value; } }
    public bool  FuelSpawned        { get { return fuelSpawned; }        set { fuelSpawned        = value; } }

    public bool MagnetActive        { get { return magnetActive;     } }
    public bool TurboActive         { get { return turboActive;      } }
    public bool DoubleCoinActive    { get { return doubleCoinActive; } }
    public bool ShieldActive        { get { return shieldActive;     }   set { shieldActive       = value; } }

    public bool MagnetSpawned       { get { return magnetSpawned; }      set { magnetSpawned      = value; } }
    public bool TurboSpawned        { get { return turboSpawned; }       set { turboSpawned       = value; } }
    public bool DoubleCoinSpawned   { get { return doubleCoinSpawned; }  set { doubleCoinSpawned  = value; } }
    public bool ShieldSpawned       { get { return shieldSpawned; }      set { shieldSpawned      = value; } }
    #endregion

    [HideInInspector]
    public managerVars vars;

    void Awake ()
    {
        if (instance == null)
            instance = this;

        vars = Resources.Load<managerVars>("managerVarsContainer");         
    }

    private void Start()
    {
        tutorialElements.previousButton.SetActive(false);

        if (TrafficGameManager.instance.GDPRConset == 0 && TrafficGameManager.instance.canShowAds == true)
        {
            gdprAdmobPanel.SetActive(true);                                                                 
        }

        if (TrafficGameManager.instance.canShowAds == false)                                               
        {
            noAdsbtn.interactable = false;                                                                  
        }

        if (TrafficGameManager.instance.isMusicOn == true)                                                 
        {
            AudioListener.volume = 0.35f;                                                                       
            mainMenu.soundBtnImg.sprite = mainMenu.soundOff;                                                
        }
        else
        {
            AudioListener.volume = 0;                                                                       
            mainMenu.soundBtnImg.sprite = mainMenu.soundOn;                                                 
        }

        TrafficGameManager.instance.gameStarted = false;                                                       
        TrafficGameManager.instance.gameOver = false;                                                          
        TrafficGameManager.instance.currentCoinsEarned = 0;                                                    
        gameMenu.coinText.text = "" + TrafficGameManager.instance.currentCoinsEarned;                          
        speedIncreaser.milestone = speedIncreaser.milestoneIncreaser;                                   
        MoveUI(mainMenuObj.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);  
        SoundManager.instance.PlayFX("PanelSlide");                                                     
        TrafficGameManager.instance.playerCar = GameObject.FindGameObjectWithTag("Player");                    
        TrafficGameManager.instance.currentDistance = 0;                                                       
        gameMenu.distanceText.text = "" + TrafficGameManager.instance.currentDistance;                         
        road.ScrollSpeed = currentSpeed;                                                                
        mainMenu.coinText.text = "" + TrafficGameManager.instance.coinAmount;                                  
        PlayerController.instance.SetCarSprite();                                                       

        // --- BULLETPROOF AUTO-START CHECK ---
        if (PlayerPrefs.GetInt("AutoStartRace", 0) == 1)                                                         
        {
            PlayerPrefs.SetInt("AutoStartRace", 0);                                                        
            PlayBtn();                                                                                      
        }
        else if (TrafficGameManager.instance.retry == true)
        {
            TrafficGameManager.instance.retry = false;
            PlayBtn();
        }
    }

    void Update ()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))                                                               
            Application.Quit();                                                                             
#endif
        if (countDown > 0 && startCountDown == true && TrafficGameManager.instance.gameStarted == false)
        {
            countDown -= Time.deltaTime;                                                                    
            if (countDown <= 4 && countDown > 3)                                                        
            {
                if (gameMenu.countDownText.text != "3")                                                 
                    SoundManager.instance.PlayFX("CD3");                                                

                gameMenu.countDownText.text = "3";                                                      
            }   
            else if (countDown <= 3 && countDown > 2)                                                   
            {
                if (gameMenu.countDownText.text != "2")                                                 
                    SoundManager.instance.PlayFX("CD2");                                                

                gameMenu.countDownText.text = "2";                                                      
            }
            else if (countDown <= 2 && countDown > 1)                                                   
            {
                if (gameMenu.countDownText.text != "1")                                                 
                    SoundManager.instance.PlayFX("CD1");                                                

                gameMenu.countDownText.text = "1";                                                      
            }
            else if (countDown <= 1)
            {
                if (gameMenu.countDownText.text != "GO!!")
                    SoundManager.instance.PlayFX("CDGO");

                gameMenu.countDownText.text = "GO!!";
            }

            if (countDown <= 0)                                                                         
            {   
                SoundManager.instance.PlayNarrationFX("GO");                                            
                startCountDown = false;                                                                 
                gameMenu.countDownText.gameObject.SetActive(false);                                     
                TrafficGameManager.instance.gameStarted = true;                                                
                Spawner.instance.SpawnObjects();                                                        
            }
        }

        if (TrafficGameManager.instance.gameOver == true && giftBarActive == true)                             
        {
            GiftBar();                                                                                  
        }

        if (TrafficGameManager.instance.gameStarted == true && TrafficGameManager.instance.gameOver == false)         
        {
            TrafficGameManager.instance.currentDistance += Time.deltaTime * distanceMultiplier;                
            gameMenu.distanceText.text = "" + Mathf.RoundToInt(TrafficGameManager.instance.currentDistance);   

            if (currentFuel > 0)                                                                        
            {
                currentFuel -= Time.deltaTime;                                                          
            }

            if (currentFuel <= 0 && TrafficGameManager.instance.gameOver == false)                             
            {
                SoundManager.instance.PlayNarrationFX("GameOver");                                      
                TrafficGameManager.instance.gameOver = true;                                                   
                GameOverMethod();                                                                       
            }

            gameMenu.fuelSlider.value = currentFuel / TrafficGameManager.instance.fuel;                        
            IncreaseSpeed();                                                                            

            if (magnetActive) MagnetBar();                                                              
            if (turboActive)    TurboBar();                                                             
            if (doubleCoinActive) DoubleCoinBar();                                                      
        }
    }

    #region Button Methods
    public void PlayBtn()                                                                               
    {
        SoundManager.instance.PlayFX("BtnClick");                                                       
        MoveUI(mainMenuObj.GetComponent<RectTransform>(), new Vector2(-480, 0), 0.5f, 0f, Ease.OutFlash);   
        MoveUI(gameMenuObj.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);      
        SoundManager.instance.PlayGameMusic();                                                          

        if (TrafficGameManager.instance.tutorialShowned == false)
            tutorialElements.tutorialPanel.SetActive(true);
        else
        {
            PlayMethod();
        }
    }

    void PlayMethod()
    {      
        currentFuel = TrafficGameManager.instance.fuel;     
        startCountDown = true;                                                                          
    }

    public void TipButton(string value)
    {
        if (value == "Next")
        {
            if (currentTipIndex < tutorialElements.tips.Length - 1)
            {
                currentTipIndex++;
                for (int i = 0; i < tutorialElements.tips.Length; i++)
                {
                    tutorialElements.previousButton.SetActive(true);
                    tutorialElements.tipImage.sprite = tutorialElements.tips[i];
                }
                tutorialElements.tipImage.sprite = tutorialElements.tips[currentTipIndex];

                if (currentTipIndex == tutorialElements.tips.Length - 1)
                {
                    tutorialElements.nextbutton.SetActive(false);
                    tutorialElements.closeBtn.SetActive(true);
                }

                if (currentTipIndex < tutorialElements.tips.Length - 1)
                {
                    tutorialElements.nextbutton.SetActive(true);
                    tutorialElements.closeBtn.SetActive(false);
                }
            }
        }

        if (value == "Back")
        {
            if (currentTipIndex > 0)
            {
                currentTipIndex--;
                for (int i = 0; i < tutorialElements.tips.Length; i++)
                {
                    tutorialElements.closeBtn.SetActive(false);
                    tutorialElements.nextbutton.SetActive(true);
                    tutorialElements.tipImage.sprite = tutorialElements.tips[i];
                }
                tutorialElements.tipImage.sprite = tutorialElements.tips[currentTipIndex];
            }

            if (currentTipIndex <= 0)
            {
                tutorialElements.previousButton.SetActive(false);
            }

            if (currentTipIndex > 0)
                tutorialElements.previousButton.SetActive(true);
        }

        if (value == "Close")
        {
            tutorialElements.tutorialPanel.SetActive(false);
            TrafficGameManager.instance.tutorialShowned = true;
            TrafficGameManager.instance.Save();
            PlayMethod();
        }
    }

    public void GDPRConsetBtn(int value)                                                                
    {
        TrafficGameManager.instance.GDPRConset = value;                                                        
        TrafficGameManager.instance.Save();                                                                    
        gdprAdmobPanel.SetActive(false);                                                                
    }

    public void MenuBtns(string value)                                                                  
    {
        SoundManager.instance.PlayFX("BtnClick");                                                       
        if (value == "facebook")                                                                        
        {
        }
        else if (value == "sound")                                                                      
        {
            if (TrafficGameManager.instance.isMusicOn == true)                                                 
            {
                TrafficGameManager.instance.isMusicOn = false;                                                 
                AudioListener.volume = 0;                                                               
                mainMenu.soundBtnImg.sprite = mainMenu.soundOn;                                         
            }
            else                                                                                        
            {
                TrafficGameManager.instance.isMusicOn = true;                                                  
                AudioListener.volume = 0.35f;                                                               
                mainMenu.soundBtnImg.sprite = mainMenu.soundOff;                                        
            }
            TrafficGameManager.instance.Save();                                                                
        }
        else if (value == "moregames")                                                                  
        {   
            Application.OpenURL(vars.moregamesUrl);                                                     
        }
        else if (value == "rate")                                                                       
        {
#if UNITY_ANDROID
            Application.OpenURL(vars.rateButtonUrl);                                                    
#endif
        }
        else if (value == "noads")                                                                      
        {
        }
        else if (value == "restore")                                                                    
        {
        }
        else if (value == "revive")                                                                     
        {
        }
        else if (value == "gameover")                                                                   
        {
            GameOver();                                                                                 
            MoveUI(reviveMenu.GetComponent<RectTransform>(), new Vector2(0, -800), 0.5f, 0f, Ease.OutFlash);    
            MoveUI(gameoverMenu.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);     
        }
        else if (value == "replay")                                                                     
        {
            Time.timeScale = 1f; 
            MonoBehaviour loader = FindObjectOfType<MiniGameLoader>(); // Safely find your main room script
            
            if (loader != null) {
                // Hand the timer over to the safe Main Room so it survives the deletion!
                loader.StartCoroutine(PerformRestart(gameObject.scene.name, true));
            } else {
                PlayerPrefs.SetInt("AutoStartRace", 1);
                SceneManager.LoadScene(gameObject.scene.name);
            }
        }
        else if (value == "home")                                                                       
        {
            Time.timeScale = 1f; 
            MonoBehaviour loader = FindObjectOfType<MiniGameLoader>(); 
            
            if (loader != null) {
                // Hand the timer over to the safe Main Room so it survives the deletion!
                loader.StartCoroutine(PerformRestart(gameObject.scene.name, false));
            } else {
                PlayerPrefs.SetInt("AutoStartRace", 0);
                SceneManager.LoadScene(gameObject.scene.name);
            }
        }
        else if (value == "share")                                                                      
        {
        }
        else if (value == "leaderboard")                                                                 
        {
        }
        else if (value == "openShop")                                                                   
        {
            CarShop.instance.InitializeCarShop();                                                       
            SoundManager.instance.PlayFX("BtnClick");                                                   
            MoveUI(mainMenuObj.GetComponent<RectTransform>(), new Vector2(-480, 0), 0.5f, 0f, Ease.OutFlash);   
            MoveUI(carShopPanel.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);     
        }
        else if (value == "closeShop")                                                                  
        {
            SoundManager.instance.PlayFX("BtnClick");                                                   
            MoveUI(carShopPanel.GetComponent<RectTransform>(), new Vector2(480, 0), 0.5f, 0f, Ease.OutFlash);   
            MoveUI(mainMenuObj.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);      
        }
        else if (value == "openPWShop")                                                                 
        {
            SoundManager.instance.PlayFX("BtnClick");
            MoveUI(mainMenuObj.GetComponent<RectTransform>(), new Vector2(480, 0), 0.5f, 0f, Ease.OutFlash);    
            MoveUI(powerupShop.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);      
        }
        else if (value == "closePWShop")                                                                
        {
            SoundManager.instance.PlayFX("BtnClick");                                                   
            MoveUI(powerupShop.GetComponent<RectTransform>(), new Vector2(-480, 0), 0.5f, 0f, Ease.OutFlash);   
            MoveUI(mainMenuObj.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0f, Ease.OutFlash);      
        }
        else if (value == "opengiftPanel")                                                              
        {
            SoundManager.instance.PlayFX("BtnClick");                                                   
            TrafficGameManager.instance.giftPoints = 0;                                                        
            TrafficGameManager.instance.Save();                                                                
            gameOverMenu.giftPanel.SetActive(true);                                                     
        }
        else if (value == "closegiftPanel")                                                             
        {
            gameOverMenu.giftbtn.interactable = true;                                                   
            SoundManager.instance.PlayFX("BtnClick");                                                   
            gameOverMenu.giftPanel.SetActive(false);                                                    
        }   
        else if (value == "collectGift")                                                                
        {
            SoundManager.instance.PlayFX("BtnClick");                                                   
            gameOverMenu.collectBtn.interactable = false;                                               

            coinUIObj = ObjectPooling.instance.GetPickUpFXPooledObject("CoinUIFX");                     
            coinUIObj.transform.position = gameOverMenu.collectBtn.transform.position;                  
            coinUIObj.SetActive(true);                                                                  
            float r = Random.Range(0.25f, 0.8f);                                                        
            coinUIObj.transform.DOMove(gameOverMenu.coinImg.transform.position, r).OnComplete(DeactivateUICoin).SetEase(Ease.Linear);  
        }
    }

    // --- THE PERFECT RELOAD TIMER ---
    private IEnumerator PerformRestart(string sceneName, bool autoStart)
    {
        PlayerPrefs.SetInt("AutoStartRace", autoStart ? 1 : 0);
        
        // 1. Tell the old crashed scene to delete itself FIRST
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        
        // 2. WAIT until the old scene is 100% wiped from memory (killing the old Game Managers)
        while (!unloadOp.isDone)
        {
            yield return null;
        }
        
        // 3. ONLY THEN, spawn the brand new scene so the new Managers take over perfectly!
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    #endregion

    /*--------------------------------------------------Basic Methods----------------------------------------------------------------*/

#region Basic Methods

    public void ActivateMagnet()
    {
        currentMagnetTime = TrafficGameManager.instance.magnetTime;                
        gameMenu.magnetUI.SetActive(true);                                                              
        magnetActive = true;                                                                            
    }

    void MagnetBar()
    {
        if (currentMagnetTime > 0)                                                                      
        {
            currentMagnetTime -= Time.deltaTime;                                                        
            gameMenu.magnetImg.fillAmount = currentMagnetTime / TrafficGameManager.instance.magnetTime;        
        }
            
        if (currentMagnetTime <= 0)                                                                     
        {
            PlayerController.instance.DeactivateEffects("magnet");                                      
            gameMenu.magnetUI.SetActive(false);                                                         
            magnetActive = false;                                                                       
        }
    }

    public void ActivateTurbo()
    {
        currentSpeed += 1.5f;                                                                           
        road.ScrollSpeed = currentSpeed;                                                                
        turboEffect.SetActive(true);                                                                    
        currentTurboTime = TrafficGameManager.instance.turboTime;                  
        gameMenu.turboUI.SetActive(true);                                                               
        turboActive = true;                                                                             
        distanceMultiplier += TrafficGameManager.instance.turboSpeed;                                          
    }

    void TurboBar()
    {
        if (currentTurboTime > 0)                                                                       
        {
            currentTurboTime -= Time.deltaTime;                                                         
            gameMenu.turboImg.fillAmount = currentTurboTime / TrafficGameManager.instance.turboTime;           
        }

        if (currentTurboTime <= 0)                                                                      
        {
            currentSpeed -= 1.5f;                                                                       
            road.ScrollSpeed = currentSpeed;                                                            
            turboEffect.SetActive(false);                                                               
            distanceMultiplier = TrafficGameManager.instance.normalSpeed;                                      
            PlayerController.instance.DeactivateEffects("turbo");                                       
            gameMenu.turboUI.SetActive(false);                                                          
            turboActive = false;                                                                        
        }
    }

    public void ActivateDoubleCoin()
    {
        coinIncreaser = 2;                                                                              
        currentDoubleCoinTime = TrafficGameManager.instance.doubleCoinTime;       
        gameMenu.doubleCoinUI.SetActive(true);                                                          
        doubleCoinActive = true;                                                                        
    }

    void DoubleCoinBar()
    {
        if (currentDoubleCoinTime > 0)                                                                  
        {
            currentDoubleCoinTime -= Time.deltaTime;                                                    
            gameMenu.doubleCoinImg.fillAmount = currentDoubleCoinTime / TrafficGameManager.instance.doubleCoinTime;    
        }

        if (currentDoubleCoinTime <= 0)                                                                 
        {
            coinIncreaser = 1;                                                                          
            gameMenu.doubleCoinUI.SetActive(false);                                                     
            doubleCoinActive = false;                                                                   
        }
    }

    void IncreaseSpeed()                                                                                
    {
        if (TrafficGameManager.instance.currentDistance >= speedIncreaser.milestone)                           
        {
            speedIncreaser.milestone += speedIncreaser.milestoneIncreaser;                              

            if (currentSpeed < speedIncreaser.maxSpeed)                                                 
            {
                currentSpeed = currentSpeed * speedIncreaser.speedMultiplier;                           
                road.ScrollSpeed = currentSpeed;                                                        
            }

            if (maxEnemyCarGap > 2)                                                                     
                maxEnemyCarGap -= 0.5f;                                                                 
        }
    }
                                                                                                        
    public void IncreaseCoin(int value, GameObject _coinObject)                                         
    {   
        TrafficGameManager.instance.currentCoinsEarned += coinIncreaser * value;                               
        gameMenu.coinText.text = "" + TrafficGameManager.instance.currentCoinsEarned;                          
        coinObject = _coinObject;                                                                       
        coinObject.transform.DOMove(gameMenu.coinImg.transform.position, 0.5f).OnComplete(DeactivateCoin).SetEase(Ease.Linear);  
    }

    public void UpdateTotalCoins()
    {
        mainMenu.coinText.text = "" + TrafficGameManager.instance.coinAmount;                                  
    }

    void DeactivateCoin()
    {
        coinObject.SetActive(false);
    }

    void DeactivateUICoin()
    {
        SoundManager.instance.CarFX("Coin");                                                            
        TrafficGameManager.instance.coinAmount += 50;                                                          
        TrafficGameManager.instance.Save();                                                                    
        coinUIObj.SetActive(false);                                                                     
        gameOverMenu.coinText.text = "" + TrafficGameManager.instance.coinAmount;
        UpdateTotalCoins();                                                                             
    }

    public void RestoreFuel()                                                                           
    {   
        currentFuel = TrafficGameManager.instance.fuel;                                                        
        fuelSpawned = false;                                                                            
    }

    void GiftBar()
    {
        giftBarTime += Time.deltaTime;                                                                  
        var percent = giftBarTime / 1f;                                                                 
        float barFill = TrafficGameManager.instance.giftPoints / 500f;                                         

        gameOverMenu.giftBar.fillAmount = Mathf.Lerp(0, barFill, percent);                              
    }

    public void GameOverMethod()
    {
        MoveUI(gameMenuObj.GetComponent<RectTransform>(), new Vector2(480,0), 0.5f, 0f, Ease.OutFlash); 
        
        GameOver();                                                                             
        MoveUI(gameoverMenu.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0.25f, Ease.OutFlash);  
        SoundManager.instance.PlayFX("PanelSlide");                                                 
    }

    void GameOver()
    {
        if (gameOverMenu.coinText != null) 
            gameOverMenu.coinText.text = "" + TrafficGameManager.instance.coinAmount;                              
            
        if (gameOverMenu.coinEarnedText != null) 
            gameOverMenu.coinEarnedText.text = "+" + TrafficGameManager.instance.currentCoinsEarned;               
            
        if (gameOverMenu.scoreText != null) 
            gameOverMenu.scoreText.text = "" + Mathf.CeilToInt(TrafficGameManager.instance.currentDistance);       

        if (TrafficGameManager.instance.currentDistance > TrafficGameManager.instance.bestDistance)                   
            TrafficGameManager.instance.bestDistance = Mathf.CeilToInt(TrafficGameManager.instance.currentDistance);  

        TrafficGameManager.instance.lastDistance = Mathf.CeilToInt(TrafficGameManager.instance.currentDistance);      
        TrafficGameManager.instance.giftPoints += TrafficGameManager.instance.lastDistance;                           

        if (TrafficGameManager.instance.giftPoints > 500)                                                      
        {
            TrafficGameManager.instance.giftPoints = 500;                                                      
            if (gameOverMenu.giftbtn != null) 
                gameOverMenu.giftbtn.interactable = true;                                                   
        }

        TrafficGameManager.instance.coinAmount += TrafficGameManager.instance.currentCoinsEarned;                     
        TrafficGameManager.instance.Save();                                                                    
        giftBarActive = true; 
        
        if (gameOverMenu.giftInfoText != null) 
            gameOverMenu.giftInfoText.text = TrafficGameManager.instance.giftPoints + "/500 To Next Gift";         
            
        if (gameOverMenu.hiScoreText != null) 
            gameOverMenu.hiScoreText.text = "" + TrafficGameManager.instance.bestDistance;                         
    }

    public void Revive()
    {
        foreach (GameObject enemyCar in GameObject.FindGameObjectsWithTag("Enemy"))                     
        {   
            enemyCar.SetActive(false);                                                                  
        }

        MoveUI(reviveMenu.GetComponent<RectTransform>(), new Vector2(0, -800), 0.5f, 0f, Ease.OutFlash);    
        MoveUI(gameMenuObj.GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f, 0.25f, Ease.OutFlash);   
        TrafficGameManager.instance.playerCar.SetActive(true);                                                 
        gameMenu.countDownText.gameObject.SetActive(true);                                              
        currentFuel = TrafficGameManager.instance.fuel;                                                        
        gameMenu.fuelSlider.value = currentFuel / TrafficGameManager.instance.fuel;                            
        revived = true;                                                                                 
        TrafficGameManager.instance.gameStarted = false;                                                       
        countDown = 4;                                                                                  
        startCountDown = true;                                                                          
        TrafficGameManager.instance.gameOver = false;                                                          
    }

    public void PickUpPop(string value)
    {
        if (gameMenu.pickUpPopText != null && gameMenu.pickUpPopUI != null)
        {
            gameMenu.pickUpPopText.text = value;                                                            
            MoveUI(gameMenu.pickUpPopUI.GetComponent<RectTransform>(), new Vector3(0, 100, 0), 0.5f, 0f, Ease.OutExpo);     
            MoveUI(gameMenu.pickUpPopUI.GetComponent<RectTransform>(), new Vector3(500, 100, 0), 0.5f, 1f, Ease.OutExpo);   
        }
    }

#endregion

    /*--------------------------------------------------Tween Code-------------------------------------------------------------------*/

#region TweenCode

    void MoveUI(RectTransform _transform, Vector2 position, float moveTime, float delayTime, Ease ease)
    {
        _transform.DOAnchorPos(position, moveTime).SetDelay(delayTime).SetEase(ease);
    }

#endregion

    /*--------------------------------------------------Struct-----------------------------------------------------------------------*/

#region Struct
    [System.Serializable]
    protected class GameMenu
    {
        public Image coinImg, magnetImg, turboImg, doubleCoinImg;                                       
        public Slider fuelSlider;                                                                       
        public Text coinText, distanceText, countDownText, pickUpPopText;                               
        public GameObject pickUpPopUI, magnetUI, doubleCoinUI, turboUI;                                 
    }


    [System.Serializable]
    protected class MainMenu
    {
        public Image soundBtnImg;                                                                       
        public Text coinText;                                                                           
        public Sprite soundOn, soundOff;                                                                
    }

    [System.Serializable]
    protected class GameOverMenu
    {
        public Image soundBtnImg, giftBar;                                                              
        public Text coinText, scoreText, hiScoreText, coinEarnedText, giftInfoText;                     
        public Button rewardAdsbtn, giftbtn, collectBtn;                                                
        public GameObject giftPanel, coinImg;                                                           
    }

    [System.Serializable]
    protected class SpeedIncreaser
    {
        public float milestone, milestoneIncreaser, maxSpeed, speedMultiplier;                          
    }

    [System.Serializable]
    protected class Tutorial
    {
        public GameObject tutorialPanel, closeBtn, nextbutton, previousButton;
        public Image tipImage;
        public Sprite[] tips;
    }

    [System.Serializable]
    public class FacebookPanel
    {
        public Text shareBtnText, likeBtnText, coinInfoText;
        public GameObject fbPanel, coinRewardPanel;
        public Image profileImage;
    }
#endregion

}