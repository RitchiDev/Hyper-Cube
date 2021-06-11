using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererDestroyer : MonoBehaviour
{
    private void Awake()
    {
        DestroyMesh();
    }

    private void DestroyMesh()
    {
        Destroy(GetComponent<MeshRenderer>());
    }
}
