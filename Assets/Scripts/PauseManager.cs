using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingPanel;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.StopSound();
            Pause();
        }
    }
    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Setting()
    {
        settingPanel.SetActive(true);
    }
    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
