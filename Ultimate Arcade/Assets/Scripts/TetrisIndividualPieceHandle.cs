using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisIndividualPieceHandle : MonoBehaviour
{
    public int yDefaultOffset = 0;
    public bool UpdatePos = false;
    [SerializeField] private bool foundX = false;
    [SerializeField] private int xFound = 0;
    public Vector2 Pos;

    // Start is called before the first frame update
    void Start()
    {
        //Pos = new Vector2(100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdatePos)
        {
            BeginUpdate();
        }   
    }
    public void GetXPos()
    {
        Pos.x = Mathf.Round(transform.position.x);
    }

    public void GetYPos()
    {
        Pos.y = Mathf.Round(transform.position.y);
    }

    void BeginUpdate()
    {        
        transform.position = Pos;
    }
}