using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMainMenu : MonoBehaviour
{
    public static TempMainMenu Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of this " + Instance.ToString() + " already exists");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartGame(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
