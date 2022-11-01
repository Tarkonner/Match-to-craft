using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSceneLoaderFunktions : MonoBehaviour
{
    public void LoadScene(int level)
    {
        SceneLoader.Instance.LoadScene(level);
    }

    public void LoadLevel(int index)
    {
        SceneLoader.Instance.LoadLevel(index);
    }
}
