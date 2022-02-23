using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Camera m_Camera;
    [SerializeField] private PlayerController m_PlayerPrefab;
    [SerializeField] private int m_BoostAmount = 20;
    [SerializeField] private Vector3 m_BoostTargetOffset;
    [SerializeField] private float m_BoostSpeed = 0.3f;
    private float m_BoostTime;
    public float BoostSpeed => m_BoostSpeed;

    private PlayerController m_Player;
    private Transform m_BoostTarget;
    private bool m_IsBoosting;
    public bool PlayerIsBoosting => m_IsBoosting;

    [SerializeField] private int m_MaxPlatformsBeforeShrink = 30;
    [SerializeField] private float m_MaxPlatformSpeed = 10f;
    [SerializeField] private float m_StartPlatformSpeed = 7f;
    [SerializeField] private float m_SpeedIncrement = 0.2f;

    [SerializeField] private float m_MaxPlatformSize = 7f;
    [SerializeField] private float m_MinPlatformSize = 1f;
    [SerializeField] private float m_ShrinkIncrement = 0.2f;
    private int m_PlatformsBeforeShrink;
    private float m_NewPlatformSize;
    private float m_NewSpeed;

    [SerializeField] private Vector2 m_SpawnPosition;

    private void Awake()
    {
        if(Instance)
        {
            Debug.LogError("An instance of " + Instance.ToString() + " already existed!");
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

        m_PlatformsBeforeShrink = m_MaxPlatformsBeforeShrink;
        m_NewPlatformSize = m_MaxPlatformSize;
        m_NewSpeed = m_StartPlatformSpeed;

        m_Player = Instantiate(m_PlayerPrefab, m_SpawnPosition, m_PlayerPrefab.transform.rotation);
    }

    private void Update()
    {
        if(m_IsBoosting)
        {
            float progress = (Time.time - m_BoostTime) / m_BoostSpeed;
            Vector3 desiredPosition = m_BoostTarget.position + m_BoostTargetOffset;
            m_Player.transform.position = Vector3.Lerp(m_Player.transform.position, desiredPosition, Mathf.Clamp01(progress));

            Debug.Log(progress);
            if (progress >= 1)
            {
                //m_BoostTime = Time.time;
                m_IsBoosting = false;
                m_Player.enabled = true;
                m_Player.gameObject.GetComponent<Collider2D>().enabled = true;

                CameraController.Instance.SetNewTarget(m_BoostTarget);
            }

            //if (m_Player.transform.position.y >= (m_BoostTarget.position.y + m_BoostTargetOffset.y) - 1f)
            //{
            //    m_IsBoosting = false;
            //    CameraController.Instance.SetNewTarget(m_BoostTarget);
            //    m_Player.gameObject.GetComponent<Collider2D>().enabled = true;
            //    m_Player.enabled = true;
            //}
        }
    }

    public void Boost()
    {
        m_IsBoosting = true;
        m_BoostTime = Time.time;
        CameraController.Instance.SetNewTarget(m_Player.transform);

        if (m_Player.enabled)
        {
            m_Player.enabled = false;
            m_Player.Rigidbody.velocity = Vector2.zero;
            m_Player.gameObject.GetComponent<Collider2D>().enabled = false;
        }

        Platform[] platforms = FindObjectsOfType<Platform>();
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < m_BoostAmount; i++)
        {
            ScoreManager.Instance.AddScore(1);
            if (i >= m_BoostAmount - 1)
            {
                //Debug.Log(i);
                SpawnPlatform(false, true);
                SpawnPlatform(true);

                return;
            }

            SpawnPlatform(false);

        }
    }

    public void SpawnPlatform(bool doMove = true, bool boostTarget = false)
    {
        if(m_PlatformsBeforeShrink > 0)
        {
            m_PlatformsBeforeShrink = Mathf.Clamp(m_PlatformsBeforeShrink - 1, 0, m_MaxPlatformsBeforeShrink);
        }
        else
        {
            m_PlatformsBeforeShrink = m_MaxPlatformsBeforeShrink;
        }

        if(boostTarget)
        {
            Platform platform = SpawnManager.Instance.InstantiatePlatform();
            platform.SetUp(GetNewSize(), GetNewSpeed(), doMove);
            m_BoostTarget = platform.transform;

            return;
        }

        SpawnManager.Instance.InstantiatePlatform().SetUp(GetNewSize(), GetNewSpeed(), doMove);
    }

    private float GetNewSize()
    {
        if(m_PlatformsBeforeShrink <= 0)
        {
            m_NewPlatformSize = Mathf.Clamp(m_NewPlatformSize - m_ShrinkIncrement, m_MinPlatformSize, m_MaxPlatformSize);
        }

        return m_NewPlatformSize;
    }

    private float GetNewSpeed()
    {
        if(m_PlatformsBeforeShrink <= 0)
        {
            m_NewSpeed = Mathf.Clamp(m_NewSpeed + m_SpeedIncrement, m_StartPlatformSpeed, m_MaxPlatformSpeed);
        }

        return m_NewSpeed;
    }

    public void GameOver()
    {
        Instantiate(m_Player.DeathEffect, m_Player.transform.position, m_Player.transform.rotation);
        m_Player.gameObject.SetActive(false);
        StartCoroutine(TimedRetry());
    }

    private IEnumerator TimedRetry()
    {
        yield return new WaitForSeconds(0.95f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
