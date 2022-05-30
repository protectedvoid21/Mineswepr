using UnityEngine;
using UnityEngine.UI;

public class TutorialExampleScroll : MonoBehaviour {
    [System.Serializable]
    private class ExampleSteps {
        [TextArea]
        public string explanationText;
        public Sprite stepSprite;
    }

    [SerializeField] private ExampleSteps[] steps;
    private int currentIndex;

    [SerializeField] private Text stepText;
    [SerializeField] private Image stepImage;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;

    private void OnEnable() {
        currentIndex = 0;
        previousButton.SetActive(false);
        nextButton.SetActive(true);

        SetStep();
    }

    private void SetStep() {
        stepText.text = steps[currentIndex].explanationText;
        stepImage.sprite = steps[currentIndex].stepSprite;
    }

    public void ChangePage(int amount) {
        currentIndex += amount;
        previousButton.SetActive(currentIndex != 0);
        nextButton.SetActive(currentIndex != steps.Length - 1);

        SetStep();
    }
}