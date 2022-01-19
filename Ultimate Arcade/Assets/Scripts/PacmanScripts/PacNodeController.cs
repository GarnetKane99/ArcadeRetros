using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacNodeController : MonoBehaviour
{
    public List<GameObject> ConnectedNodes;
    public bool Visited = false;

    private void Awake()
    {
        ConnectedNodes = new List<GameObject>();
    }
}
