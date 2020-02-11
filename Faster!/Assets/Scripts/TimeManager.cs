using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Rewired;

public class TimeManager : MonoBehaviour {

    public GameObject lightBulbGO;
    public Sprite LevelUpBadge;
    // public TMP_Text BestTimeEver;
    // public GameObject MSText;

    private BulbManager bulbManagerScriptWinner;
    private int findWinnerIndex;

    public static bool LightIsOn = false;

    private float delayTime = 0;
    public float minDelay = 1.0f;
    public float maxDelay = 10.0f;

    private float winnerParticleDelay = 0.5f;
    private float increaseLevelBarDelay = 0.0f;
    private float displayLevelDelay = 1.1f;
    private float popUpDelay = 2.0f;

    private float lightOnTime = 0;
    private bool ShowPopUps = false;

    private bool timerIsRunning = false;
    private bool increasingLevelBar = false;
    private bool increasingLevelText = false;
    public float levelBarSmoothSpeed = 3.0f;
    public float levelTextSmoothSpeed = 6.0f;

    public List<float> timesArr = new List<float>();


    public void InitializeLightTimer() {
        SetRandomTime();
        timerIsRunning = true;

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            timesArr.Add(0);

            // Revert texts back to basic and display proper badge
            GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().LevelText.GetComponent<TMP_Text>().text = GameManager.PlayerLevelArr[i] + "";
            // GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().LevelText.GetComponent<TMP_Text>().fontSize = 32;
            GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().LevelText.GetComponent<TMP_Text>().color = ColorManager.KeyWhite;

            float newFillAmount = GameManager.PlayerLevelArr[i] / (float)GameManager.LevelMax;
            GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().PlayerBadge.GetComponent<Image>().fillAmount = newFillAmount;
            GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().PlayerBadge.GetComponent<Image>().color = ColorManager.KeyWhite;
        }

        // Display best time ever
        // if (GameManager.BestTimeEver < 999999) {
        //     BestTimeEver.text = GameManager.BestTimeEver.ToString("F1");

        //     if (!MSText.activeSelf) {
        //         MSText.SetActive(true);
        //     }
        // }
    }


    private void Update() {
        // Start counting down to turn the light on
        if (timerIsRunning) {
            TimerCountdown();
        }

        // Start counting the time when light is turned on
        if (LightIsOn) {
            LightOnTime();
        }

        // Show time popups
        if (GameManager.PlayerDoneCount == GameManager.PlayerCount) {
            if (!ShowPopUps) {
                StartCoroutine(ShowPopUpsDelay());
            }
        }
    }


    private void FixedUpdate() {
        if (increasingLevelBar) {
            SmoothedLevelBarIncrease();
        }

        if (increasingLevelText) {
            SmoothedLevelTextIncrease();
        }
    }


    private void SmoothedLevelBarIncrease() {
        float desiredFillAmount = GameManager.PlayerLevelArr[findWinnerIndex] / (float)GameManager.LevelMax;
        float smoothedFillAMount = Mathf.Lerp(bulbManagerScriptWinner.PlayerBadge.GetComponent<Image>().fillAmount, desiredFillAmount, levelBarSmoothSpeed * Time.deltaTime);
        bulbManagerScriptWinner.PlayerBadge.GetComponent<Image>().fillAmount = smoothedFillAMount;

        // if (bulbManagerScriptWinner.PlayerBadge.GetComponent<Image>().fillAmount >= desiredFillAmount - (desiredFillAmount / 50)) {
        if (bulbManagerScriptWinner.PlayerBadge.GetComponent<Image>().fillAmount >= desiredFillAmount) {
            increasingLevelBar = false;
        }
    }


    private void SmoothedLevelTextIncrease() {
        float desiredFontSize = 48;
        float smoothedFontSize = Mathf.Lerp(bulbManagerScriptWinner.LevelText.GetComponent<TMP_Text>().fontSize, desiredFontSize, levelTextSmoothSpeed * Time.deltaTime);
        bulbManagerScriptWinner.LevelText.GetComponent<TMP_Text>().fontSize = smoothedFontSize;

        if (bulbManagerScriptWinner.LevelText.GetComponent<TMP_Text>().fontSize >= desiredFontSize) {
            increasingLevelText = false;
        }
    }


    private void SetRandomTime() {
        delayTime = Random.Range(minDelay, maxDelay);
    }


    private void TimerCountdown() {
        delayTime -= Time.deltaTime;

        if (delayTime <= 0) {
            AudioManager.instance.Play("LightOn");

            timerIsRunning = false;
            lightBulbGO.SetActive(true);
            lightBulbGO.GetComponent<Image>().color = ColorManager.KeyWhite;
            LightIsOn = true;
        }
    }


    private void LightOnTime() {
        lightOnTime += Time.deltaTime;
    }


    public void GetTime(int playerID, bool rightOnTime) {
        float newTime = 999999;

        if (rightOnTime) {
            newTime = lightOnTime;

            // Calculate to milliseconds
            newTime *= 1000;
            newTime = Mathf.Round(newTime * 100f) / 100f;
            // newTime *= 1000;
            // newTime = Mathf.Round(newTime);
        }

        // timerDict[playerID]["Player ID"] = playerID;
        // timerDict[playerID]["Player Time"] = newTime;
        timesArr[playerID] = newTime;

        // Hashtable dictEntry = new Hashtable();

        // dictEntry["Player ID"] = playerID;
        // dictEntry["Player Time"] = newTime;

        // timerDict.Add(dictEntry);

        // print((float)timerDict[playerID]["Player Time"]);
        // print("Player " + playerID + ": " + newTime);
    }


    private IEnumerator ShowPopUpsDelay() {
        ShowPopUps = true;

        yield return new WaitForSeconds(1.0f);

        ShowTimePopUps();
        GameManager.ReadyToReset = true;
    }


    private void ShowTimePopUps() {
        AudioManager.instance.Play("PopUp");

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            BulbManager bulbManagerScript = GameManager.PlayerLightsArr[i].GetComponent<BulbManager>();

            GameObject newPopUp = bulbManagerScript.TimePopUp;
            newPopUp.SetActive(true);

            // float playerTime = (float)timerDict[i]["Player Time"];
            float playerTime = timesArr[i];
            
            if (playerTime > 99999) {
                bulbManagerScript.PopUpTime.GetComponent<TMP_Text>().text = "–";
            } else {
                bulbManagerScript.PopUpTime.GetComponent<TMP_Text>().text = playerTime.ToString("F1");
            }
        }

        FindWinner();
    }


    private void FindWinner() {
        // Reset index of winner
        findWinnerIndex = -1;

        float bestTime = 9999999;

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            if (timesArr[i] < bestTime) {
                bestTime = timesArr[i];
            }
        }

        findWinnerIndex = timesArr.IndexOf(bestTime);
        GameManager.PlayerLevelArr[findWinnerIndex]++;

        // Find best time ever
        if (bestTime < GameManager.BestTimeEver) {
            GameManager.BestTimeEver = bestTime;
            // BestTimeEver.text = GameManager.BestTimeEver.ToString("F1");
        }
        
        bulbManagerScriptWinner = GameManager.PlayerLightsArr[findWinnerIndex].GetComponent<BulbManager>();


        StartCoroutine(DelayThenWinnerParticle());

        // WinnerParticleEffect();

        // IncreaseLevelBar();

        // DisplayNewLevel();


        // CheckForGameOver();

        // if (!MSText.activeSelf) {
        //     MSText.SetActive(true);
        // }

        // StartCoroutine(ContinueDelay());
    }


    private IEnumerator DelayThenWinnerParticle() {
        yield return new WaitForSeconds(winnerParticleDelay);
        WinnerParticleEffect();
    }


    // Instantiate particle effect for winner
    private void WinnerParticleEffect() {
        AudioManager.instance.Play("WinnerSound");
        bulbManagerScriptWinner.WinnerParticles.SetActive(true);
        StartCoroutine(DelayThenIncreaseLevelBar());
    }


    private IEnumerator DelayThenIncreaseLevelBar() {
        yield return new WaitForSeconds(increaseLevelBarDelay);
        IncreaseLevelBar();
    }


    // Increase fill amount of level bar
    private void IncreaseLevelBar() {
        bulbManagerScriptWinner.PlayerBadge.GetComponent<Image>().sprite = LevelUpBadge;
        // bulbManagerScriptWinner.PlayerBadge.GetComponent<Image>().color = ColorManager.KeyPurple;
        // AudioManager.instance.Play("IncreaseLevel");
        increasingLevelBar = true;

        StartCoroutine(DelayThenDisplayNewLevel());
    }

    private IEnumerator DelayThenDisplayNewLevel() {
        yield return new WaitForSeconds(displayLevelDelay);
        DisplayNewLevel();
    }


    // Add one level, display the current level, and color
    private void DisplayNewLevel() {
        // AudioManager.instance.Stop("IncreaseLevel");
        AudioManager.instance.Play("NextLevelSound");

        bulbManagerScriptWinner.LevelText.GetComponent<TMP_Text>().text = GameManager.PlayerLevelArr[findWinnerIndex] + "";
        increasingLevelText = true;
        // bulbManagerScriptWinner.LevelText.GetComponent<TMP_Text>().fontSize = 48;
        bulbManagerScriptWinner.LevelText.GetComponent<TMP_Text>().color = ColorManager.KeyPurple;

        CheckForGameOver();
    }


    // Check if the game is over due to a player reaching the max level
    private void CheckForGameOver() {
        if (GameManager.PlayerLevelArr[findWinnerIndex] == GameManager.LevelMax) {
            GameManager.GameOver = true;
        }

        StartCoroutine(ContinueDelay());
    }


    private IEnumerator ContinueDelay() {
        yield return new WaitForSeconds(popUpDelay);
        MenuManager.PlayNextRound();
    }


    private void OnDisable() {
        CleanUpLightTimer();
    }


    public void CleanUpLightTimer() {
        LightIsOn = false;
        delayTime = 0;
        lightOnTime = 0;
        ShowPopUps = false;
        timerIsRunning = false;
        timesArr.Clear();
    }

}
