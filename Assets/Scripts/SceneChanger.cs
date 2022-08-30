using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public int targetScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            SceneLoader.LoadScene(targetScene);
    }
}
