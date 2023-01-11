using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] private KeyCode pauseKey = KeyCode.P;

    [Header("UI elemnts")]
    [SerializeField] GameObject[] menuOnElements;
    [SerializeField] GameObject[] menuOffElements;

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
        if(Input.GetKeyDown(pauseKey))
            ChangeMenuState();
    }

    public void ChangeMenuState()
    {
        activeMenu = !activeMenu;

        if(activeMenu)
        {
            foreach (var item in menuOnElements)
                item.SetActive(true);
            foreach (var item in menuOffElements)
                item.SetActive(false);
        }
        else
        {
            foreach (var item in menuOnElements)
                item.SetActive(false);
            foreach (var item in menuOffElements)
                item.SetActive(true);
        }
    }

    public void Mute()
    {
        SoundManager.Instance.MuteAudio();
    }
}
