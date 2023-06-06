using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadOtherScenes());
    }

    void Update()
    {
        
    }
    private IEnumerator LoadOtherScenes()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Doors", LoadSceneMode.Additive);
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.5f);
        SceneManager.UnloadSceneAsync("Preload");
        yield break;
    }
}
