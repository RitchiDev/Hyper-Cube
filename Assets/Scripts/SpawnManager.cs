using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private Platform m_Prefab;
    [SerializeField] private float m_SpawnIncrement = 8f;
    [SerializeField] private float m_SpawnHeight = -6f;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        m_SpawnHeight -= m_SpawnIncrement;
    }

    public Platform InstantiatePlatform()
    {
        m_SpawnHeight += m_SpawnIncrement;
        //Debug.Log(m_SpawnHeight + " / " + m_SpawnIncrement);
        return Instantiate(m_Prefab, new Vector3(0, m_SpawnHeight, 0), Quaternion.identity);
    }
}
