using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private BlockSpawner _blockSpawner;

    public bool IsFirstClicked { get; private set; }

    private int _activeBlocks;
    private int _mineCountTextValue;
    private int _minutes;
    private int _seconds;

    [SerializeField]
    private Text mineText;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text wonTimeText;

    [SerializeField]
    private GameObject restartButton;

    [SerializeField]
    private GameObject gameWonPanel;

    private void Awake()
    {
        Time.timeScale = 1;
        _blockSpawner = FindObjectOfType<BlockSpawner>();
        mineText.text = _blockSpawner.BombCount.ToString();
        _mineCountTextValue = _blockSpawner.BombCount;
        _activeBlocks = _blockSpawner.Height * _blockSpawner.Width;
        InvokeRepeating("ChangeTimeText", 0f, 1f);
    }

    public void FirstClick()
    {
        IsFirstClicked = true;
    }

    private void ChangeTimeText()
    {
        if (_seconds == 60)
        {
            _minutes++;
            _seconds = 0;
        }

        string secondsText = _seconds < 10 ? "0" + _seconds.ToString() : _seconds.ToString();
        timeText.text = $"{_minutes}:{secondsText}";
        _seconds++;
    }

    public void AddMineToText(int value)
    {
        _mineCountTextValue += value;
        mineText.text = _mineCountTextValue.ToString();
    }

    public void DecreaseBombCount()
    {
        _activeBlocks--;

        if (_activeBlocks == _blockSpawner.BombCount)
        {
            Win();
        }
    }

    private void Win()
    {
        gameWonPanel.SetActive(true);
        wonTimeText.text = timeText.text;
        timeText.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        _blockSpawner.RevealAllBombs();
        CancelInvoke("ChangeTimeText");
        restartButton.SetActive(true);
    }
}