using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisMainGeneration : MonoBehaviour
{
    //An array containing the different Tetris Pieces
    public GameObject[] POSSIBLE_PIECES;
    public Transform SPAWN_POS;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateBlock()
    {
        int RandomBlock = Random.Range(0, 7);
        //int RandomBlock = 6;
        GameObject CurPiece = Instantiate(POSSIBLE_PIECES[RandomBlock], SPAWN_POS);
        CurPiece.transform.parent = null;
    }
}
