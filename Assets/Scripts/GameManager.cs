using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private BlockSpawner blockSpawner;
    
    public bool IsFirstClicked { get; private set; }

    private int activeBlocks;
    private int mineCountTextValue;
    private int minutes;
    private int seconds;
    [SerializeField] private Text mineText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text wonTimeText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject gameWonPanel;
    
    private void Awake() {
        Time.timeScale = 1;
        blockSpawner = FindObjectOfType<BlockSpawner>();
        mineText.text = blockSpawner.bombCount.ToString();
        mineCountTextValue = blockSpawner.bombCount;
        activeBlocks = blockSpawner.height * blockSpawner.width;
        InvokeRepeating("ChangeTimeText", 0f, 1f);
    }

    public void FirstClick() {
        IsFirstClicked = true;
    }

    private void ChangeTimeText() {
        if(seconds == 60) {
            minutes++;
            seconds = 0;
        }
        string secondsText = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
        timeText.text = $"{minutes}:{secondsText}";
        seconds++;
    }

    public void AddMineToText(int value) {
        mineCountTextValue += value;
        mineText.text = mineCountTextValue.ToString();
    }

    public void DecreaseBombCount() {
        activeBlocks--;

        if(activeBlocks == blockSpawner.bombCount) {
            Win();
        }
    }
    
    private void Win() {
        gameWonPanel.SetActive(true);
        wonTimeText.text = timeText.text;
        timeText.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void GameOver() {
        blockSpawner.RevealAllBombs();
        CancelInvoke("ChangeTimeText");
        restartButton.SetActive(true);
    }
}
