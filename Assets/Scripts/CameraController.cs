using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private float m_MoveSpeed = 3f;
    [SerializeField] private float m_LerpTime = 5f;
    [SerializeField] private float m_FollowPlayerLerpTime = 1f;
    private float m_CurrentLerpTime;
    private bool m_IsBoosting;

    private Transform m_NewTarget;
    private Transform m_OldTarget;
    private Vector3 m_CameraOffset;

    private void Awake()
    {
        if(Instance)
        {
            Debug.LogError("An instance of this " + Instance.ToString() + " already exists");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void LateUpdate()
    {
        if(!m_NewTarget)
        {
            return;
        }

        if (transform.position.y >= (m_NewTarget.position.y + m_CameraOffset.y) - 0.1f)
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * m_MoveSpeed;
        }
        else
        {
            Vector3 desiredPosition = m_NewTarget.position + m_CameraOffset;
            desiredPosition.x = transform.position.x;
            desiredPosition.z = transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_LerpTime * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    public void SetNewTarget(Transform target, bool firstTarget = false, bool isPlayer = false)
    {
        m_CurrentLerpTime = isPlayer ? m_FollowPlayerLerpTime : m_LerpTime;

        //Debug.Log("Set New Target");
        m_OldTarget = m_NewTarget;
        m_NewTarget = target;

        //Debug.Log(lerpTime);
        if(firstTarget)
        {
            m_CameraOffset = transform.position - m_NewTarget.position;
        }
    }
}
