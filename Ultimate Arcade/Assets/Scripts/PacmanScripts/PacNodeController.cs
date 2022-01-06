using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacNodeController : MonoBehaviour
{
    public List<GameObject> ConnectedNodes;
    public bool Visited = false;
    public int Distance, Previous;

    private void Awake()
    {
        ConnectedNodes = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
