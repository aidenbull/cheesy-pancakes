using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject credits;

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("SampleScene 1");
    }

    public void OnCreditsButtonPressed()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void OnCreditsBackButtonPressed()
    {
        mainMenu.SetActive(true);
        credits.SetActive(false);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
