using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour {
    [Serializable]
    private class Difficulty {
        public int width;
        public int height;
        public int bombs;
    }
    [SerializeField] private Difficulty[] difficulties;
    private int difficultyIndex;

    private string[] defaultTexts = new string[3];
    [SerializeField] private Text widthText;
    [SerializeField] private Text heightText;
    [SerializeField] private Text bombText;

    private void Awake() {
        defaultTexts[0] = widthText.text;
        defaultTexts[1] = heightText.text;
        defaultTexts[2] = bombText.text;
    }
    
    public void SetDifficultyNumber(int number) {
        if(number < 0 || number >= difficulties.Length) {
            Debug.LogError("Number is incorrect");
        }
        else {
            widthText.text = $"{defaultTexts[0]} : {difficulties[number].width}";
            heightText.text = $"{defaultTexts[1]} : {difficulties[number].height}";
            bombText.text = $"{defaultTexts[2]} : {difficulties[number].bombs}";
            difficultyIndex = number;
        }
    }
    
    public void RunGame() {
        PlayerPrefs.SetInt("Width", difficulties[difficultyIndex].width);
        PlayerPrefs.SetInt("Height", difficulties[difficultyIndex].height);
        PlayerPrefs.SetInt("Bombs", difficulties[difficultyIndex].bombs);

        SceneManager.LoadScene("Game");
    }
}