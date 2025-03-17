using UnityEngine;

public class OptionExecution : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MainVolume"))
        {
            PlayerPrefs.SetFloat("MainVolume", 0.5f);
        }

        if (PlayerPrefs.GetInt("Fullscreen", 1) == 1)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}