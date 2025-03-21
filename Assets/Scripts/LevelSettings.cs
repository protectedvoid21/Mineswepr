﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour
{
    [Serializable]
    private class Difficulty
    {
        public int width;
        public int height;
        public int bombs;
    }

    [SerializeField]
    private Difficulty[] difficulties;

    private int _difficultyIndex;

    private string[] _defaultTexts = new string[3];

    [SerializeField]
    private Text widthText;

    [SerializeField]
    private Text heightText;

    [SerializeField]
    private Text bombText;

    private void Awake()
    {
        _defaultTexts[0] = widthText.text;
        _defaultTexts[1] = heightText.text;
        _defaultTexts[2] = bombText.text;
    }

    public void SetDifficultyNumber(int number)
    {
        if (number < 0 || number >= difficulties.Length)
        {
            Debug.LogError("Number is incorrect");
        }
        else
        {
            widthText.text = $"{_defaultTexts[0]} : {difficulties[number].width}";
            heightText.text = $"{_defaultTexts[1]} : {difficulties[number].height}";
            bombText.text = $"{_defaultTexts[2]} : {difficulties[number].bombs}";
            _difficultyIndex = number;
        }
    }

    public void RunGame()
    {
        PlayerPrefs.SetInt("Width", difficulties[_difficultyIndex].width);
        PlayerPrefs.SetInt("Height", difficulties[_difficultyIndex].height);
        PlayerPrefs.SetInt("Bombs", difficulties[_difficultyIndex].bombs);

        SceneManager.LoadScene("Game");
    }
}