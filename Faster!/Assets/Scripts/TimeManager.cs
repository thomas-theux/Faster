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

    public static float LightIsOnTimer = 0;
    public bool ShowPopUps = false;

    private bool timerIsRunning = false;
    private bool increasingLevelBar = false;
    private bool increasingLevelText = false;
    public float levelBarSmoothSpeed = 3.0f;
    public float levelTextSmoothSpeed = 6.0f;

    public List<float> timesArr = new List<float>();
    public List<int> rankingArr = new List<int>();


    public void InitializeLightTimer() {
        SetRandomTime();
        timerIsRunning = true;

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            timesArr.Add(0);

            // Revert texts back to basic and display proper badge
            GameManager.PlayerInstances[i].GetComponent<BulbManager>().LevelText.GetComponent<TMP_Text>().text = GameManager.PlayerLevelArr[i] + "";
            GameManager.PlayerInstances[i].GetComponent<BulbManager>().LevelText.GetComponent<TMP_Text>().color = ColorManager.KeyWhite;

            float newFillAmount = GameManager.PlayerLevelArr[i] / (float)GameManager.LevelMax;
            GameManager.PlayerInstances[i].GetComponent<BulbManager>().PlayerBadge.GetComponent<Image>().fillAmount = newFillAmount;
            GameManager.PlayerInstances[i].GetComponent<BulbManager>().PlayerBadge.GetComponent<Image>().color = ColorManager.KeyWhite;
        }
    }


    private void Update() {
        // Start counting down to turn the light on
        if (timerIsRunning) {
            TimerCountdown();
        }

        // Start counting the time when light is turned on
        if (LightIsOn) {
            LightTimerRunning();
        }
    }


    public void AllPlayersDone() {
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


    private void LightTimerRunning() {
        LightIsOnTimer += Time.deltaTime;
    }


    public IEnumerator ShowPopUpsDelay() {
        ShowPopUps = true;

        yield return new WaitForSeconds(1.0f);

        ShowTimePopUps();
        GameManager.ReadyToReset = true;
    }


    private void ShowTimePopUps() {
        AudioManager.instance.Play("PopUp");

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            BulbManager bulbManagerScript = GameManager.PlayerInstances[i].GetComponent<BulbManager>();

            GameObject newPopUp = bulbManagerScript.TimePopUp;
            newPopUp.SetActive(true);

            // float playerTime = timesArr[i];
            float playerTime = GameManager.PlayerInstances[i].GetComponent<PlayerSheet>().PlayerTime;
            
            if (playerTime < 0) {
                bulbManagerScript.PopUpTime.GetComponent<TMP_Text>().text = "-";
            } else {
                bulbManagerScript.PopUpTime.GetComponent<TMP_Text>().text = playerTime.ToString("F1");
            }
        }

        FindWinner();
    }


    private void FindWinner() {
        // Reset index of winner
        findWinnerIndex = -1;
        bool foundWinner = false;

        List<float> playerTimes = new List<float>();
        List<float> sortedTimes = new List<float>();

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            // float newPlayerTime = GameManager.PlayerInstances[i].GetComponent<PlayerSheet>().Playertime;
            float newPlayerTime = GameManager.PlayerInstances[i].GetComponent<PlayerSheet>().PlayerTime;
            playerTimes.Add(newPlayerTime);
            sortedTimes.Add(newPlayerTime);
        }

        sortedTimes.Sort();

        // Find best time and ID of player
        for (int i = 0; i < GameManager.PlayerCount; i++) {
            if (!foundWinner) {
                if (sortedTimes[i] != -1) {
                    if (i+1 < GameManager.PlayerCount) {
                        if (sortedTimes[i] != sortedTimes[i+1]) {
                            float bestTime = sortedTimes[i];
                            findWinnerIndex = playerTimes.IndexOf(bestTime);
                        }
                        foundWinner = true;
                    }
                }
            }
        }

        if (findWinnerIndex > -1) {
            bulbManagerScriptWinner = GameManager.PlayerInstances[findWinnerIndex].GetComponent<BulbManager>();
            GameManager.PlayerLevelArr[findWinnerIndex]++;

            StartCoroutine(DelayThenWinnerParticle());
        } else {
            StartCoroutine(ContinueDelay());
        }
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
        LightIsOnTimer = 0;
        ShowPopUps = false;
        timerIsRunning = false;
        timesArr.Clear();
        rankingArr.Clear();
    }

}
