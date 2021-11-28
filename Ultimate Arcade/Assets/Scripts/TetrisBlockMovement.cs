using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlockMovement : MonoBehaviour
{
    //Factors that affect the speed of the block
    //Level and Score (might change to public shortly)
    //Maybe write up an algorithm for default speed in which the blocks fall down?
    private int SCORE, LEVEL;

    public Vector2 CurrentPos;
    public Vector3 CurrentRot;
    public Vector2 DirToGo;

    [Header("Individual Blocks")]
    public GameObject[] IndividualPieces;

    //For offsetting (to be used later)
    public Vector2 RotationValue;
    public int[,] BlockType;

    void Start()
    {
        ArraySpecification(FoundPiece());
        CurrentPos = transform.position;
        InvokeRepeating("BlockDescent", 1.0f, 1.0f);
    }

    void Update()
    {
        GetInput();
        UpdateBlockPos();
        UpdateBlockRot();

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < BlockType.GetLength(1); i++)
        {
            for (int j = 0; j < BlockType.GetLength(0); j++)
            {
                sb.Append(BlockType[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    void ArraySpecification(string PieceType)
    {
        switch (PieceType)
        {
            case "Tetris_L":
                BlockType = new int[3, 3]
                {
                    {0,0,1 },
                    {1,1,1 },
                    {0,0,0 }
                };
                break;
            case "Tetris_J":
                BlockType = new int[3, 3]
                {
                    {1,0,0 },
                    {1,1,1 },
                    {0,0,0 }
                };
                break;
            case "Tetris_T":
                BlockType = new int[3, 3]
                {
                    {0,1,0 },
                    {1,1,1 },
                    {0,0,0 }
                };
                break;
            case "Tetris_S":
                BlockType = new int[3, 3]
                {
                    {0,1,1 },
                    {1,1,0 },
                    {0,0,0 }
                };
                break;
            case "Tetris_Z":
                BlockType = new int[3, 3]
                {
                    {1,1,0 },
                    {0,1,1 },
                    {0,0,0 }
                };
                break;
            case "Tetris_I":
                BlockType = new int[4, 4]
                {
                    {0,0,0,0 },
                    {1,1,1,1 },
                    {0,0,0,0 },
                    {0,0,0,0 }
                };
                break;
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DirToGo.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            DirToGo.x = 1;
        }
        else
        {
            DirToGo.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            DirToGo.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            DirToGo.y = -1;
        }
        else
        {
            DirToGo.y = 0;
        }
    }

    void UpdateBlockPos()
    {
        if (DirToGo.x != 0)
        {
            if (DirToGo.x < 0)
            {
                NegativeMovement();
            }
            else if (DirToGo.x > 0)
            {
                PositiveMovement();
            }
            gameObject.transform.position = CurrentPos;
        }
    }
    //movement -1 on x
    void NegativeMovement()
    {
        foreach (GameObject Piece in IndividualPieces)
        {
            if (!Piece.GetComponent<PieceHandler>().TryNegativeMovement())
            {
                return;
            }
        }
        CurrentPos.x -= 1;
    }
    //movement +1 on x
    void PositiveMovement()
    {
        foreach (GameObject Piece in IndividualPieces)
        {
            if (!Piece.GetComponent<PieceHandler>().TryPositiveMovement())
            {
                return;
            }
        }
        CurrentPos.x += 1;
    }

    void UpdateBlockRot()
    {
        if (DirToGo.y != 0)
        {
            switch (FoundPiece())
            {
                case "Tetris_O":
                    return;
                case "Tetris_I":
                    break;
                default:
                    if (DirToGo.y > 0)
                    {
                        TestOne();
                    }
                    else if (DirToGo.y < 0)
                    {

                    }
                    break;
            }
            gameObject.transform.position = CurrentPos;
            gameObject.transform.rotation = Quaternion.Euler(CurrentRot);
        }
    }

    void TestOne()
    {
        int N = BlockType.Length;

        //Transpose the Matrix
        for (int i = 0; i < BlockType.GetLength(0); i++)
        {
            for (int j = 0; j < i; j++)
            {
                int temp = BlockType[i, j];
                BlockType[i, j] = BlockType[j, i];
                BlockType[j, i] = temp;
            }
        }

        for (int i = 0; i < BlockType.GetLength(0); i++)
        {
            for (int j = 0; j < N / 2; j++)
            {
                int temp = BlockType[i, j];
                BlockType[i, j] = BlockType[i, N - j - 1];
                BlockType[i, N - j - 1] = temp;
            }
        }
    }

    string FoundPiece()
    {
        return name.Split('(')[0];
    }

    void BlockDescent()
    {
        CurrentPos.y -= 1;
        gameObject.transform.position = CurrentPos;
    }
}
