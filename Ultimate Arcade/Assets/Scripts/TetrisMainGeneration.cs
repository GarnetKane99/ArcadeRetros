using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisMainGeneration : MonoBehaviour
{
    //An array containing the different Tetris Pieces
    public GameObject[] POSSIBLE_PIECES;
    public GameObject[] POSSIBLE_NEXTPIECES;
    public Transform SPAWN_POS;
    public Transform NEXT_PIECE;
    public int RandomNextBlock;
    GameObject NextBlock;
    // Start is called before the first frame update
    void Start()
    {
        GenerateFirstBlock();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateFirstBlock()
    {
        int RandomBlock = Random.Range(0, 7);
        //int RandomBlock = 0;
        GameObject CurPiece = Instantiate(POSSIBLE_PIECES[RandomBlock], SPAWN_POS);
        RandomNextBlock = Random.Range(0, 7);
        //RandomNextBlock = 0;
        GameObject NextPiece = Instantiate(POSSIBLE_NEXTPIECES[RandomNextBlock], NEXT_PIECE);
        NextBlock = NextPiece;
        CurPiece.transform.parent = null;
    }

    public void GenerateBlock()
    {
        Destroy(NextBlock);
        GameObject CurPiece = Instantiate(POSSIBLE_PIECES[RandomNextBlock], SPAWN_POS);
        RandomNextBlock = Random.Range(0, 7);
        //RandomNextBlock = 0;
        GameObject NextPiece = Instantiate(POSSIBLE_NEXTPIECES[RandomNextBlock], NEXT_PIECE);
        NextBlock = NextPiece;
        CurPiece.transform.parent = null;
    }
}
