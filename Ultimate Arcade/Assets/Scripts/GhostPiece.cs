using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    //These coordinate pieces will be linked with the grids
    public GameObject[] CoordinatePieces;
    public Vector2 DirToGo;
    public Vector2 CurrentPos;
    public Vector3 CurrentRot;

    [SerializeField] private TetrisBoardManager GridManager;
    public int yOffset = -1;
    public int xOffset = 4;
    public int[,] TetrisGrid;
    public int[,] CurrentPosition;
    //zOffset is used to calculate which rotation the block will be moving into
    public int zOffset = 0;

    private bool StopDescent = false;

    private void Awake()
    {
        CurrentPos = transform.position;
        CurrentRot = transform.rotation.eulerAngles;
        GridManager = FindObjectOfType<TetrisBoardManager>();

        if (FoundPiece() == "Tetris_I")
        {
            xOffset -= 1;
        }
    }

    string FoundPiece()
    {
        return name.Split('(')[0];
    }

    public void GetGridType(int[,] found)
    {
        TetrisGrid = found;
        CurrentPosition = TetrisGrid;
    }

    bool OnPlacedBlock(int x, int y)
    {
        if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 3
        || GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 4
        || GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 5
        || GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 6
        || GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 7
        || GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 8
        || GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 9)
        {
            return true;
        }
        return false;
    }

    void BlockDescentFast()
    {
        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                if (TetrisGrid[x, y] == 1)
                {
                    if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 2)
                    {
                        StopDescent = true;
                        return;
                    }
                    else if (OnPlacedBlock(x, y))
                    {
                        StopDescent = true;
                        return;
                    }
                }
            }
        }
        yOffset--;

        for (int y = CurrentPosition.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < CurrentPosition.GetLength(0); x++)
            {
                if (CurrentPosition[x, y] == 1)
                {
                    GridManager.GridSize[x, y] = 0;
                    CurrentPosition[x, y] = 0;
                }
            }
        }

        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                if (TetrisGrid[x, y] == 1)
                {
                    GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset + y - 1] = TetrisGrid[x, y];
                    CurrentPosition[x + xOffset, CurrentPosition.GetLength(1) + yOffset + y - 1] = TetrisGrid[x, y];
                }
            }
        }

        CurrentPos.y -= 1;
        gameObject.transform.position = CurrentPos;
    }

    void Update()
    {
        BlockDescentFast();

        StringBuilder sb = new StringBuilder();
        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                sb.Append(TetrisGrid[x, y]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }

        //Debug.Log(sb.ToString());
    }
}
