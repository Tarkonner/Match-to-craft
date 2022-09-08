using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Don't show UI i scnes")]
    [SerializeField] private int[] notShowScenes;

    [Header("UI elemnts")]
    [SerializeField] GameObject menuElement;

    private bool activeMenu = false;
    public bool ActiveMenu { get { return activeMenu; }}



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
       
    private void Update()
    {
        bool takeInput = true;
        foreach (int item in notShowScenes)
        {
            if(SceneManager.GetActiveScene().buildIndex == item)
            {
                takeInput = false;
                break;
            }
        }
        if (!takeInput)
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
            ChangeMenuState();
    }

    public void ChangeMenuState()
    {
        activeMenu = !activeMenu;

        menuElement.SetActive(activeMenu);
    }

    public void Mute()
    {
        SoundManager.Instance.MuteAudio();
    }
}
