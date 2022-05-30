using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNavigationManager : MonoBehaviour {
    [SerializeField] private GameObject firstPage;
    [SerializeField] private GameObject secondPage;

    public void Back() {
        SceneManager.LoadScene("Menu");
    }

    public void FirstPage() {
        firstPage.SetActive(true);
        secondPage.SetActive(false);
    }

    public void SecondPage() {
        firstPage.SetActive(false);
        secondPage.SetActive(true);
    }
}
