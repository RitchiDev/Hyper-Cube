using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_InteractEffect;

    public virtual void SpawnInteractEffect()
    {
        Instantiate(m_InteractEffect, transform.position, m_InteractEffect.transform.rotation);
    }
}
