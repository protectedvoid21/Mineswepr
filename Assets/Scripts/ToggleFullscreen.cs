using UnityEngine;

public class ToggleFullscreen : MonoBehaviour {
    public void OnValueChanged(bool value) {
        if(value == true) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            PlayerPrefs.SetInt("Fullscreen", 0);
        }
    }
}