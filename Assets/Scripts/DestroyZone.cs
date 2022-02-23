using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyZone : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_DeathEffect;
    [SerializeField] private Transform m_Target;
    private Vector3 m_Offset;

    private void Awake()
    {
        m_Offset = transform.position - m_Target.position;
    }

    private void LateUpdate()
    {
        if(GameManager.Instance.PlayerIsBoosting)
        {
            return;
        }

        Vector3 targetPos = m_Target.transform.position;
        transform.position = targetPos + m_Offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if(player)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Platform platform = other.GetComponent<Platform>();
        if(platform)
        {
            platform.gameObject.SetActive(false);
        }
    }
}
