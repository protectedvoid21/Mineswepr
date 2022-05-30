using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    [SerializeField] private GameObject pauseCanvas;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Switch();
        }
    }

    private void Switch() {
        Time.timeScale = pauseCanvas.activeSelf ? 1f : 0f;
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
    }

    public void Resume() {
        Switch();
    }

    public void Exit() {
        SceneManager.LoadScene("Menu");
    }
}