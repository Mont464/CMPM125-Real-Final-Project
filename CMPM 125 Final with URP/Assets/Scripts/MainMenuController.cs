using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Tutorial found at: https://www.youtube.com/watch?v=nUNHJMdDuXE

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup CreditsPanel;

    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        CreditsPanel.alpha = 1;
        CreditsPanel.blocksRaycasts = true;
    }

    public void Back()
    {
        CreditsPanel.alpha = 0;
        CreditsPanel.blocksRaycasts = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
