using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI elemnts")]
    [SerializeField] GameObject menuElement;

    private bool activeMenu = false;
    public bool ActiveMenu { get { return activeMenu; }}



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
       
    private void Update()
    {
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
