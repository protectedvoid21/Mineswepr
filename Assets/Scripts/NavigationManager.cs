using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour {
    public void StartGame() {
        SceneManager.LoadScene("LevelSelect");
    }

    public void RunGame() {
        SceneManager.LoadScene("Game");
    }

    public void Tutorial() {
        SceneManager.LoadScene("Tutorial");
    }

    public void Options() {
        SceneManager.LoadScene("Options");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void BackToMenu() {
        SceneManager.LoadScene("Menu");
    }
}