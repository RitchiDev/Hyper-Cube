using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    [SerializeField] private GameObject m_InGameMenu;
    [SerializeField] private GameObject m_PauseMenu;

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

    private void Start()
    {
        m_PauseMenu.SetActive(false);
        m_InGameMenu.SetActive(true);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        if(Time.timeScale > 0)
        {
            m_InGameMenu.SetActive(false);
            m_PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            m_PauseMenu.SetActive(false);
            m_InGameMenu.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
