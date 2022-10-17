using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //Singelton
    public static SceneLoader Instance { get; private set; }

    public int levelIndex = 0;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    void Update()
    {
#if(UNITY_EDITOR)
        if(Input.GetKeyDown(KeyCode.R))
            RestartScene();
#endif
    }

    //Fast funktions
    public static void LoadScene(int targetScene) => SceneManager.LoadScene(targetScene);
    public static void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public static void QuitGame() => Application.Quit();
    public static void NextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
}
