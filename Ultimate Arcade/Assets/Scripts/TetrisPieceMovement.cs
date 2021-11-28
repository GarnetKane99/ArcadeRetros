using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TetrisPieceMovement : MonoBehaviour
{
    //These coordinate pieces will be linked with the grids 
    public GameObject[] CoordinatePieces;
    public Vector2 DirToGo;
    public Vector2 CurrentPos;

    private TetrisBoardManager GridManager;
    private TetrisMainGeneration PieceGenerator;
    public int yOffset = -1;//, xOffset = -10;
    int[,] TetrisGrid;
    int[,] CurrentPosition;

    [SerializeField]
    private bool CurrentlyControlling = false;

    private void Awake()
    {
        CurrentlyControlling = true;
        CurrentPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupGrid(FoundPiece());
        GridManager = FindObjectOfType<TetrisBoardManager>();
        PieceGenerator = FindObjectOfType<TetrisMainGeneration>();
        if (GridManager != null)
        {
            InitializeBlockSpawnPos();
        }
        InvokeRepeating("BlockDescent", 1.0f, 1.0f);
    }

    string FoundPiece()
    {
        return name.Split('(')[0];
    }

    void SetupGrid(string Found)
    {
        switch (Found)
        {
            case "Tetris_L":
                TetrisGrid = new int[3, 3]
                {
                    {0,1,0 },
                    {0,1,0 },
                    {0,1,1 }
                };
                break;
            case "Tetris_J":
                TetrisGrid = new int[3, 3]
                {
                    {0,1,1 },
                    {0,1,0 },
                    {0,1,0 }
                };
                break;
            case "Tetris_T":
                TetrisGrid = new int[3, 3]
                {
                    {0,1,0 },
                    {0,1,1 },
                    {0,1,0 }
                };
                break;
            case "Tetris_S":
                TetrisGrid = new int[3, 3]
                {
                    {0,1,0 },
                    {0,1,1 },
                    {0,0,1 }
                };
                break;
            case "Tetris_Z":
                TetrisGrid = new int[3, 3]
                {
                    {0,0,1 },
                    {0,1,1 },
                    {0,1,0 }
                };
                break;
            case "Tetris_I":
                TetrisGrid = new int[4, 4]
                {
                    {0,0,1,0 },
                    {0,0,1,0 },
                    {0,0,1,0 },
                    {0,0,1,0 }
                };
                break;
            case "Tetris_O":
                TetrisGrid = new int[2, 2]
                {
                    {1,1 },
                    {1,1 }
                };
                break;
        }
    }

    void InitializeBlockSpawnPos()
    {
        CurrentPosition = new int[GridManager.GridSize.GetLength(0), GridManager.GridSize.GetLength(1)];
    }

    void BlockDescent()
    {
        yOffset--;
        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                if (TetrisGrid[x, y] == 1)
                {
                    if (GridManager.GridSize[x + 4, GridManager.GridSize.GetLength(1) + yOffset+y-1] == 2)
                    {
                        Invoke("LastSecondChanges", 1.5f);
                        CancelInvoke("BlockDescent");
                        return;
                    }
                }
            }
        }

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

        /*        for (int x = 0; x < CurrentPosition.GetLength(0); x++)
                {
                    for (int y = 0; y < CurrentPosition.GetLength(1); y++)
                    {
                        if (CurrentPosition[x, y] == 1)
                        {
                            GridManager.GridSize[x, y] = 0;
                            CurrentPosition[x, y] = 0;
                        }
                    }
                }*/

        /*        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < TetrisGrid.GetLength(0); x++)
                    {
                        if (TetrisGrid[x, y] == 1)
                        {
                            GridManager.GridSize[x, y] = TetrisGrid[x, y];
                        }
                    }
                }*/

        /*        for (int y = GridManager.GridSize.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < GridManager.GridSize.GetLength(0); x++)
                    {
                        if (TetrisGrid[x, y] == 1)
                        {
                            GridManager.GridSize[x + xOffset, y + yOffset] = TetrisGrid[x, y];
                        }
                    }
                }*/

        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                if (TetrisGrid[x, y] == 1)
                {
                    GridManager.GridSize[x + 4, GridManager.GridSize.GetLength(1) + yOffset + y-1] = TetrisGrid[x, y];
                    CurrentPosition[x + 4, CurrentPosition.GetLength(1) + yOffset + y-1] = TetrisGrid[x, y];
                }
            }
        }

        CurrentPos.y -= 1;
        gameObject.transform.position = CurrentPos;
    }

    void LastSecondChanges()
    {
        for (int x = 0; x < CurrentPosition.GetLength(1); x++)
        {
            for (int y = 0; y < CurrentPosition.GetLength(0); y++)
            {
                if (CurrentPosition[y, x] == 1)
                {
                    GridManager.GridSize[y, x] = 2;
                }
            }
        }

        CurrentlyControlling = false;
        PieceGenerator.GenerateBlock();
    }

    void UpdateTetrisGrid(string Found)
    {
        switch (Found)
        {
            case "Tetris_L":
                TetrisGrid = new int[3, 3]
                {
                    {0,0,2 },
                    {2,2,2 },
                    {0,0,0 }
                };
                break;
            case "Tetris_J":
                TetrisGrid = new int[3, 3]
                {
                    {2,0,0 },
                    {2,2,2 },
                    {0,0,0 }
                };
                break;
            case "Tetris_T":
                TetrisGrid = new int[3, 3]
                {
                    {0,2,0 },
                    {2,2,2 },
                    {0,0,0 }
                };
                break;
            case "Tetris_S":
                TetrisGrid = new int[3, 3]
                {
                    {0,2,2 },
                    {2,2,0 },
                    {0,0,0 }
                };
                break;
            case "Tetris_Z":
                TetrisGrid = new int[3, 3]
                {
                    {2,2,0 },
                    {0,2,2 },
                    {0,0,0 }
                };
                break;
            case "Tetris_I":
                TetrisGrid = new int[4, 4]
                {
                    {0,0,0,0 },
                    {2,2,2,2 },
                    {0,0,0,0 },
                    {0,0,0,0 }
                };
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentlyControlling)
        {
            GetInput();
            UpdateBlockPos();
            //UpdateCoordinates();
        }

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

        Debug.Log(sb.ToString());
    }

    void GetInput()
    {
        //left and right movement
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
        //up and down movement
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
            if (DirToGo.x > 0)
            {
                //xOffset += 1;
                CurrentPos.x += 1;
            }
            else if (DirToGo.x < 0)
            {
                //xOffset -= 1;
                CurrentPos.x -= 1;
            }
        }
    }

    void UpdateCoordinates()
    {
        /*        //Tetris piece update
                gameObject.transform.position = CurrentPos;
                //Grid coordinates
                for (int y = 0; y < TetrisGrid.GetLength(1); y++)
                {
                    for (int x = 0; x < TetrisGrid.GetLength(0); x++)
                    {
                        if (GridManager.GridSize[y - yOffset, x + xOffset] == 2)
                        {
                            return;
                        }
                    }
                }

                for (int x = 0; x < CurrentPosition.GetLength(1); x++)
                {
                    for (int y = 0; y < CurrentPosition.GetLength(0); y++)
                    {
                        if (CurrentPosition[y, x] == 1)
                        {
                            GridManager.GridSize[y, x] = 0;
                            CurrentPosition[y, x] = 0;
                        }
                    }
                }

                for (int y = 0; y < TetrisGrid.GetLength(0); y++)
                {
                    for (int x = 0; x < TetrisGrid.GetLength(1); x++)
                    {
                        if (TetrisGrid[y, x] == 1)
                        {
                            GridManager.GridSize[y - yOffset, x + xOffset] = TetrisGrid[y, x];
                            CurrentPosition[y - yOffset, x + xOffset] = TetrisGrid[y, x];
                        }
                    }
                }*/
    }
}
