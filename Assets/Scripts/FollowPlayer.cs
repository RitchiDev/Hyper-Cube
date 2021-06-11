using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private enum FollowAxis
    {
        notSet,
        x,
        y,
        z,
        all
    }

    [SerializeField] private FollowAxis m_FollowAxis;
    private Transform m_Player;
    private bool m_DestroyMesh;
    private Vector3 m_Offset;

    private void Awake()
    {
        if(m_DestroyMesh)
        {
            DestroyMeshRenderer();
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void LateUpdate()
    {
        switch (m_FollowAxis)
        {
            case FollowAxis.notSet:
                break;
            case FollowAxis.x:
                FollowPlayerX();
                break;
            case FollowAxis.y:
                FollowPlayerY();
                break;
            case FollowAxis.z:
                FollowPlayerZ();
                break;
            case FollowAxis.all:
                FollowPlayerAllAxis();
                break;
            default:
                break;
        }
    }

    private void FollowPlayerX()
    {
        Vector3 playerPos = m_Player.transform.position;
        Vector3 position = transform.position;
        transform.position = new Vector3(playerPos.x, position.y, position.z);
    }

    private void FollowPlayerY()
    {
        Vector3 playerPos = m_Player.transform.position;
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, playerPos.y, position.z);
    }

    private void FollowPlayerZ()
    {
        Vector3 playerPos = m_Player.transform.position;
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, playerPos.z);
    }

    private void FollowPlayerAllAxis()
    {
        Vector3 playerPos = m_Player.transform.position;
        transform.position = playerPos + m_Offset;
    }

    private void DestroyMeshRenderer()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer)
        {
            Destroy(meshRenderer);
        }
    }

    private void FindPlayer()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        m_Offset = transform.position - m_Player.position;
    }
}
