using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUI;
    public Timer TimerScript;
    
    public void ContinueGame()
    {
        if (TimerScript.TimeLeft > 0)
            Time.timeScale = 1;
        PauseMenuUI.SetActive(false);
    }

    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Debug.Log("Game closed");
        Application.Quit();
    }

}
