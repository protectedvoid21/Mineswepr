using UnityEngine;
using UnityEngine.UI;

public class TutorialExampleScroll : MonoBehaviour
{
    [System.Serializable]
    private class ExampleSteps
    {
        [TextArea]
        public string explanationText;

        public Sprite stepSprite;
    }

    [SerializeField]
    private ExampleSteps[] steps;

    private int _currentIndex;

    [SerializeField]
    private Text stepText;

    [SerializeField]
    private Image stepImage;

    [SerializeField]
    private GameObject previousButton;

    [SerializeField]
    private GameObject nextButton;

    private void OnEnable()
    {
        _currentIndex = 0;
        previousButton.SetActive(false);
        nextButton.SetActive(true);

        SetStep();
    }

    private void SetStep()
    {
        stepText.text = steps[_currentIndex].explanationText;
        stepImage.sprite = steps[_currentIndex].stepSprite;
    }

    public void ChangePage(int amount)
    {
        _currentIndex += amount;
        previousButton.SetActive(_currentIndex != 0);
        nextButton.SetActive(_currentIndex != steps.Length - 1);

        SetStep();
    }
}