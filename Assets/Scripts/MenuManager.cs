using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingPanel;
    public void Play()
    {
        SceneManager.LoadScene("Start");
    }
    public void Setting()
    {
        settingPanel.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
