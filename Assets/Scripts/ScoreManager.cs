using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private float m_GrowTime;
    [SerializeField] private float m_ShrinkTime;
    [SerializeField] private Vector2 m_GrowthSize;
    private int m_Score;
    private IEnumerator m_GrowCoroutine;

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
        AddScore(0, false);
    }

    public void AddScore(int amount, bool doGrow = true)
    {
        m_Score = Mathf.Clamp(m_Score + amount, 0, 99999);
        m_ScoreText.text = m_Score.ToString();

        if(doGrow)
        {
            m_GrowCoroutine = InflateText();
            StartCoroutine(m_GrowCoroutine);
        }
    }

    private IEnumerator InflateText()
    {
        m_ScoreText.transform.DOScale(m_GrowthSize, m_GrowTime);
        yield return new WaitForSeconds(m_GrowTime);
        m_ScoreText.transform.DOScale(Vector2.one, m_ShrinkTime);
    }
}
