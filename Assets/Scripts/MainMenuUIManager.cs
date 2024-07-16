using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    public void changescene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    //quit the application
    public void quit()
    {
        Application.Quit();
    }
}
