using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField]
    private Text valueText;

    [SerializeField]
    private string valueName;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(valueName))
        {
            Debug.LogWarning($"Value : {valueName} does not exist");
        }

        float valueOnStart = PlayerPrefs.GetFloat(valueName, 0.5f);
        gameObject.GetComponent<Slider>().value = valueOnStart;
        valueText.text = valueOnStart.ToString("0.00");
    }

    public void OnSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(valueName, value);
        AudioManager.instance.AdjustVolume();
        valueText.text = value.ToString("0.00");
    }
}