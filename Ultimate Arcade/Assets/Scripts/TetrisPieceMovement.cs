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
    public int xOffset = 4;
    int[,] TetrisGrid;
    int[,] CurrentPosition;

    [SerializeField] private bool CurrentlyControlling = false;
    [SerializeField] private bool OnExistingBlock = false;
    [SerializeField] private float allowedTimeOnBlock = 1.0f; //variable will be updated/initialized as game progressess

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
        InvokeRepeating("BlockDescent", 1.0f, 0.5f);
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
        Debug.Log("Descending");
        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                if (TetrisGrid[x, y] == 1)
                {
                    if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 2)
                    {
                        Invoke("LastSecondChanges", 1.5f);
                        CancelInvoke("BlockDescent");
                        return;
                    }
                    else if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset - 1 + y - 1] == 3)
                    {
                        OnExistingBlock = true;
                        return;
                    }
                }
            }
        }
        OnExistingBlock = false;
        allowedTimeOnBlock = 1.0f;
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

    void LastSecondChanges()
    {
        for (int x = 0; x < CurrentPosition.GetLength(1); x++)
        {
            for (int y = 0; y < CurrentPosition.GetLength(0); y++)
            {
                if (CurrentPosition[y, x] == 1)
                {
                    GridManager.GridSize[y, x] = 3;
                }
            }
        }

        CurrentlyControlling = false;
        PieceGenerator.GenerateBlock();
    }

    void RapidUpdates()
    {
        for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < TetrisGrid.GetLength(0); x++)
            {
                if (TetrisGrid[x, y] == 1)
                {
                    if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 2)
                    {
                        Invoke("LastSecondChanges", 1.5f);
                        CancelInvoke("BlockDescent");
                        return;
                    }
                    else if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 3)
                    {
                        //OnExistingBlock = true;
                        return;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentlyControlling)
        {
            GetInput();
            UpdateBlockPos();
        }
        if (OnExistingBlock)
        {
            allowedTimeOnBlock -= Time.deltaTime;
            //UpdateCoordinates();
            RapidUpdates();
            if (allowedTimeOnBlock <= 0)
            {
                LastSecondChanges();
                CancelInvoke("BlockDescent");
                gameObject.transform.position = CurrentPos;
                CurrentlyControlling = false;
                OnExistingBlock = false;
            }
        }

        //used to show block type
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
                for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < TetrisGrid.GetLength(0); x++)
                    {
                        if (TetrisGrid[x, y] == 1)
                        {
                            if (GridManager.GridSize[x + xOffset + 1, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 2 || GridManager.GridSize[x + xOffset + 1, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 3)
                            {
                                return;
                            }
                        }
                    }
                }
                xOffset += 1;
                CurrentPos.x += 1;
            }
            else if (DirToGo.x < 0)
            {
                for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < TetrisGrid.GetLength(0); x++)
                    {
                        if (TetrisGrid[x, y] == 1)
                        {
                            if (GridManager.GridSize[x + xOffset - 1, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 2 || GridManager.GridSize[x + xOffset - 1, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 3)
                            {
                                return;
                            }
                        }
                    }
                }
                xOffset -= 1;
                CurrentPos.x -= 1;
            }
            UpdateCoordinates();
            gameObject.transform.position = CurrentPos;
        }
    }

    void UpdateCoordinates()
    {
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
    }
}
