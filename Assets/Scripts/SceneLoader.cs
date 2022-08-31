using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(int targetScene) => SceneManager.LoadScene(targetScene);
    public static void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public static void QuitGame() => Application.Quit();
    public static void NextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            RestartScene();
    }
}
