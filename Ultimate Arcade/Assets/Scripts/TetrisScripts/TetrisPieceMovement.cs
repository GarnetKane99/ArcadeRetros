using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//This class handles the movement and rotation of the Tetrominoes
public class TetrisPieceMovement : MonoBehaviour
{
    //These coordinate pieces will be linked with the grids
    public GameObject[] CoordinatePieces;
    public Vector2 DirToGo;
    public Vector2 CurrentPos;
    public Vector3 CurrentRot;

    private TetrisBoardManager GridManager;
    private TetrisMainGeneration PieceGenerator;
    public int yOffset = -1;
    public int xOffset = 4;
    int[,] TetrisGrid;
    int[,] CurrentPosition;
    //zOffset is used to calculate which rotation the block will be moving into
    public int zOffset = 0;

    [SerializeField] private bool CurrentlyControlling = false;
    [SerializeField] private bool OnExistingBlock = false;
    [SerializeField] private float allowedTimeOnBlock = 1.0f; //variable will be updated/initialized as game progressess

    [SerializeField] private float decreaseSpeed = 1.0f;
    [SerializeField] private TetrisScoreHandler ScoreHandler;
    private bool CorrectYPos = false;
    private bool SpacePressed = false;
    private bool StopDescent = false;

    public bool GhostPiece = false;

    [SerializeField] private GameObject GhostyBoyToSpawn;
    [SerializeField] private GameObject GhostBoy;
    [SerializeField] private AudioSource QuickPlaceSound;
    [SerializeField] private TetrisPauseHandler PauseFound;
    private void Awake()
    {
        CurrentlyControlling = true;
        CurrentPos = transform.position;
        CurrentRot = transform.rotation.eulerAngles;

        if (FoundPiece() == "Tetris_I")
        {
            xOffset -= 1;
        }
    }

    void Start()
    {
        SetupGrid(FoundPiece());
        GridManager = FindObjectOfType<TetrisBoardManager>();
        PieceGenerator = FindObjectOfType<TetrisMainGeneration>();
        ScoreHandler = FindObjectOfType<TetrisScoreHandler>();
        PauseFound = FindObjectOfType<TetrisPauseHandler>();

        if (GridManager != null)
        {
            InitializeBlockSpawnPos();
        }
        if (ScoreHandler != null)
        {
            GetScoreForSpeed();
        }

        GameObject Spawned = Instantiate(GhostyBoyToSpawn, CurrentPos, Quaternion.identity);
        GhostBoy = Spawned;
        GhostBoy.GetComponent<GhostPiece>().yOffset = yOffset;
        GhostBoy.GetComponent<GhostPiece>().xOffset = xOffset;
        GhostBoy.GetComponent<GhostPiece>().TetrisGrid = TetrisGrid;
        GhostBoy.GetComponent<GhostPiece>().CurrentPosition = CurrentPosition;
    }

    void GetScoreForSpeed()
    {
        if (ScoreHandler.GetLevel() < 20)
        {
            for (int i = 0; i < ScoreHandler.GetLevel(); i++)
            {
                decreaseSpeed -= 0.05f;
            }
        }
        else
        {
            decreaseSpeed = 0.05f;
            for (int i = ScoreHandler.GetLevel() - 1; i < ScoreHandler.GetLevel(); i++)
            {
                decreaseSpeed -= 0.005f;
            }
        }
        decreaseSpeed = Mathf.Clamp(decreaseSpeed, 0.0005f, 1f);
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
                TetrisGrid = new int[5, 5]
                {
                    {0,0,0,0,0 },
                    {0,0,1,0,0 },
                    {0,0,1,0,0 },
                    {0,0,1,0,0 },
                    {0,0,1,0,0 }
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
                    else if (OnPlacedBlock(x, y))
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

    void LastSecondChanges()
    {
        for (int x = 0; x < CurrentPosition.GetLength(1); x++)
        {
            for (int y = 0; y < CurrentPosition.GetLength(0); y++)
            {
                if (CurrentPosition[y, x] == 1)
                {
                    switch (FoundPiece())
                    {
                        case "Tetris_I":
                            GridManager.GridSize[y, x] = 3;
                            break;
                        case "Tetris_O":
                            GridManager.GridSize[y, x] = 4;
                            break;
                        case "Tetris_L":
                            GridManager.GridSize[y, x] = 5;
                            break;
                        case "Tetris_J":
                            GridManager.GridSize[y, x] = 6;
                            break;
                        case "Tetris_Z":
                            GridManager.GridSize[y, x] = 7;
                            break;
                        case "Tetris_S":
                            GridManager.GridSize[y, x] = 8;
                            break;
                        case "Tetris_T":
                            GridManager.GridSize[y, x] = 9;
                            break;
                    }
                    GridManager.UpdateIndividualPieces(y, x);
                }
            }
        }

        CurrentlyControlling = false;
        /*        if (PieceGenerator.enabled)
                {
                    PieceGenerator.GenerateBlock();
                }*/
        GridManager.FindFinishedRow();
        Destroy(GhostBoy);
        Destroy(gameObject);
    }

    void BlockDescentFast()
    {
        while (!StopDescent)
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
                            LastSecondChanges();
                            return;
                        }
                        else if (OnPlacedBlock(x, y))
                        {
                            StopDescent = true;
                            LastSecondChanges();
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
    }

    void Update()
    {
        if (!SpacePressed)
        {
            if (CurrentlyControlling && yOffset <= -4)
            {
                GetInput();
                UpdateBlockPos();
                UpdateBlockRot();
            }
            else
            {
                gameObject.transform.position = CurrentPos;
            }
            if (yOffset > -4)
            {
                BlockDescent();
            }
            if (!CorrectYPos && yOffset <= -4)
            {
                InvokeRepeating("BlockDescent", 0.5f, decreaseSpeed);
                CorrectYPos = true;
            }

            if (OnExistingBlock)
            {
                allowedTimeOnBlock -= Time.deltaTime;
                //UpdateCoordinates();
                //RapidUpdates();
                if (allowedTimeOnBlock <= 0)
                {
                    LastSecondChanges();
                    CancelInvoke("BlockDescent");
                    gameObject.transform.position = CurrentPos;
                    CurrentlyControlling = false;
                    OnExistingBlock = false;
                }
            }
        }
        else
        {
            if (!StopDescent)
            {
                QuickPlaceSound.Play();
                CancelInvoke("BlockDescent");
                BlockDescentFast();
                SpacePressed = false;
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

        //        Debug.Log(sb.ToString());
    }

    void GetInput()
    {
        if (!PauseFound.currentlyPaused)
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpacePressed = true;
            }
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
                            if (GridManager.GridSize[x + xOffset + 1, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 2 || OnPlacedBlock(x + 1, y + 1))
                            {
                                return;
                            }
                        }
                    }
                }
                xOffset += 1;
                CurrentPos.x += 1;
                Destroy(GhostBoy);
            }
            else if (DirToGo.x < 0)
            {
                for (int y = TetrisGrid.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < TetrisGrid.GetLength(0); x++)
                    {
                        if (TetrisGrid[x, y] == 1)
                        {
                            if (GridManager.GridSize[x + xOffset - 1, GridManager.GridSize.GetLength(1) + yOffset + y - 1] == 2 || OnPlacedBlock(x - 1, y + 1))
                            {
                                return;
                            }
                        }
                    }
                }
                xOffset -= 1;
                CurrentPos.x -= 1;
                Destroy(GhostBoy);
            }
            UpdateCoordinates();
            gameObject.transform.position = CurrentPos;
            GameObject Spawned = Instantiate(GhostyBoyToSpawn, CurrentPos, Quaternion.identity);
            GhostBoy = Spawned;
            GhostBoy.GetComponent<GhostPiece>().yOffset = yOffset;
            GhostBoy.GetComponent<GhostPiece>().xOffset = xOffset;
            GhostBoy.GetComponent<GhostPiece>().TetrisGrid = TetrisGrid;
            GhostBoy.GetComponent<GhostPiece>().CurrentPosition = CurrentPosition;
            GhostBoy.GetComponent<GhostPiece>().CurrentRot = CurrentRot;
            GhostBoy.transform.rotation = Quaternion.Euler(GhostBoy.GetComponent<GhostPiece>().CurrentRot);
        }
    }

    bool OnPlacedBlockRot(int x, int y)
    {
        //Debug.Log((int)GridManager.GridSize.GetValue(1) + yOffset + y);
        if (GridManager.GridSize[x, y] == 3
        || GridManager.GridSize[x, y] == 4
        || GridManager.GridSize[x, y] == 5
        || GridManager.GridSize[x, y] == 6
        || GridManager.GridSize[x, y] == 7
        || GridManager.GridSize[x, y] == 8
        || GridManager.GridSize[x, y] == 9)
        {
            return true;
        }

        return false;
    }

    void UpdateBlockRot()
    {
        if (DirToGo.y != 0)
        {
            if (TetrisPieceName() != "Tetris_O" && TetrisPieceName() != "Tetris_I")
            {
                //Pieces J,L,T,S,Z
                if (DirToGo.y > 0)
                {
                    int tempZoffset = zOffset;
                    switch (zOffset)
                    {
                        case 0:
                            zOffset = 1;
                            break;
                        case 1:
                            zOffset = 2;
                            break;
                        case 2:
                            zOffset = 3;
                            break;
                        case 3:
                            zOffset = 0;
                            break;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        if (TestPositiveRotation(i))
                        {
                            Destroy(GhostBoy);
                            GameObject Spawned = Instantiate(GhostyBoyToSpawn, CurrentPos, Quaternion.identity);
                            GhostBoy = Spawned;
                            GhostBoy.GetComponent<GhostPiece>().yOffset = yOffset;
                            GhostBoy.GetComponent<GhostPiece>().xOffset = xOffset;
                            GhostBoy.GetComponent<GhostPiece>().TetrisGrid = TetrisGrid;
                            GhostBoy.GetComponent<GhostPiece>().CurrentPosition = CurrentPosition;
                            GhostBoy.GetComponent<GhostPiece>().CurrentRot = CurrentRot;
                            GhostBoy.transform.rotation = Quaternion.Euler(GhostBoy.GetComponent<GhostPiece>().CurrentRot);
                            return;
                        }
                    }
                    zOffset = tempZoffset;
                }
                else if (DirToGo.y < 0)
                {
                    int tempZoffset = zOffset;
                    switch (zOffset)
                    {
                        case 1:
                            zOffset = 0;
                            break;
                        case 2:
                            zOffset = 1;
                            break;
                        case 3:
                            zOffset = 2;
                            break;
                        case 0:
                            zOffset = 3;
                            break;
                    }


                    for (int i = 0; i < 5; i++)
                    {
                        if (TestNegativeRotation(i))
                        {
                            Destroy(GhostBoy);
                            GameObject Spawned = Instantiate(GhostyBoyToSpawn, CurrentPos, Quaternion.identity);
                            GhostBoy = Spawned;
                            GhostBoy.GetComponent<GhostPiece>().yOffset = yOffset;
                            GhostBoy.GetComponent<GhostPiece>().xOffset = xOffset;
                            GhostBoy.GetComponent<GhostPiece>().TetrisGrid = TetrisGrid;
                            GhostBoy.GetComponent<GhostPiece>().CurrentPosition = CurrentPosition;
                            GhostBoy.GetComponent<GhostPiece>().CurrentRot = CurrentRot;
                            GhostBoy.transform.rotation = Quaternion.Euler(GhostBoy.GetComponent<GhostPiece>().CurrentRot);
                            return;
                        }
                    }
                    zOffset = tempZoffset;
                }
            }
            //I Piece rotation algorithm
            else if (TetrisPieceName() == "Tetris_I")
            {
                if (DirToGo.y > 0)
                {
                    int tempZoffset = zOffset;
                    switch (zOffset)
                    {
                        case 0:
                            zOffset = 1;
                            break;
                        case 1:
                            zOffset = 2;
                            break;
                        case 2:
                            zOffset = 3;
                            break;
                        case 3:
                            zOffset = 0;
                            break;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        if (TestIPositiveRotation(i))
                        {
                            Destroy(GhostBoy.gameObject);
                            GameObject Spawned = Instantiate(GhostyBoyToSpawn, CurrentPos, Quaternion.identity);
                            GhostBoy = Spawned;
                            GhostBoy.GetComponent<GhostPiece>().yOffset = yOffset;
                            GhostBoy.GetComponent<GhostPiece>().xOffset = xOffset;
                            GhostBoy.GetComponent<GhostPiece>().TetrisGrid = TetrisGrid;
                            GhostBoy.GetComponent<GhostPiece>().CurrentPosition = CurrentPosition;
                            GhostBoy.GetComponent<GhostPiece>().CurrentRot = CurrentRot;
                            GhostBoy.transform.rotation = Quaternion.Euler(GhostBoy.GetComponent<GhostPiece>().CurrentRot);
                            return;
                        }
                    }
                    zOffset = tempZoffset;
                }
                else if (DirToGo.y < 0)
                {
                    int tempZoffset = zOffset;
                    switch (zOffset)
                    {
                        case 1:
                            zOffset = 0;
                            break;
                        case 2:
                            zOffset = 1;
                            break;
                        case 3:
                            zOffset = 2;
                            break;
                        case 0:
                            zOffset = 3;
                            break;
                    }


                    for (int i = 0; i < 5; i++)
                    {
                        if (TestINegativeRotation(i))
                        {
                            Destroy(GhostBoy.gameObject);
                            GameObject Spawned = Instantiate(GhostyBoyToSpawn, CurrentPos, Quaternion.identity);
                            GhostBoy = Spawned;
                            GhostBoy.GetComponent<GhostPiece>().yOffset = yOffset;
                            GhostBoy.GetComponent<GhostPiece>().xOffset = xOffset;
                            GhostBoy.GetComponent<GhostPiece>().TetrisGrid = TetrisGrid;
                            GhostBoy.GetComponent<GhostPiece>().CurrentPosition = CurrentPosition;
                            GhostBoy.GetComponent<GhostPiece>().CurrentRot = CurrentRot;
                            GhostBoy.transform.rotation = Quaternion.Euler(GhostBoy.GetComponent<GhostPiece>().CurrentRot);
                            return;
                        }
                    }
                    zOffset = tempZoffset;
                }
            }
        }
    }

    bool TestPositiveRotation(int TestNum)
    {
        Vector2[,] offsetFound = new Vector2[,]
        {
            {new Vector2(0,0),new Vector2(-1,0), new Vector2(-1,1), new Vector2(0,-2), new Vector2(-1,-2) },
            {new Vector2(0,0), new Vector2(1,0), new Vector2(1,-1), new Vector2(0,2), new Vector2(1,2) },
            {new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,-2), new Vector2(1,-2)},
            {new Vector2(0,0), new Vector2(-1, 0), new Vector2(-1,-1), new Vector2(0,2), new Vector2(-1,2) }
        };

        int tempXOffset = (int)offsetFound[zOffset, TestNum].x;
        int tempYOffset = (int)offsetFound[zOffset, TestNum].y;

        int[,] tempVal = TetrisGrid;
        int matSize = tempVal.GetLength(0);

        //positive 90 algorithm
        for (int x = 0; x < matSize / 2; x++)
        {
            for (int y = x; y < matSize - x - 1; y++)
            {
                int temp = tempVal[x, y];
                tempVal[x, y] = tempVal[matSize - 1 - y, x];
                tempVal[matSize - 1 - y, x] = tempVal[matSize - 1 - x, matSize - 1 - y];
                tempVal[matSize - 1 - x, matSize - 1 - y] = tempVal[y, matSize - 1 - x];
                tempVal[y, matSize - 1 - x] = temp;
            }
        }

        //check if updated grid is able to rotate
        for (int y = tempVal.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < tempVal.GetLength(0); x++)
            {
                if (tempVal[x, y] == 1)
                {
                    //if checks fail (i.e. wall/piece near it before it rotates), rotate will fail and get reverted
                    if (GridManager.GridSize[x + xOffset + tempXOffset, GridManager.GridSize.GetLength(1) + yOffset + y - 1 + tempYOffset] == 2 || OnPlacedBlockRot(x + tempXOffset + xOffset, y + tempYOffset + GridManager.GridSize.GetLength(1) + yOffset - 1))
                    {
                        //Debug.Log("Test Case : " + TestNum + " Failed X: " + tempXOffset + "\nFailed Y: " + tempYOffset);
                        //negative 90 algorithm
                        for (int x1 = 0; x1 < matSize / 2; x1++)
                        {
                            for (int y1 = x1; y1 < matSize - x1 - 1; y1++)
                            {
                                int temp = tempVal[x1, y1];
                                tempVal[x1, y1] = tempVal[y1, matSize - 1 - x1];
                                tempVal[y1, matSize - 1 - x1] = tempVal[matSize - 1 - x1, matSize - 1 - y1];
                                tempVal[matSize - 1 - x1, matSize - 1 - y1] = tempVal[matSize - 1 - y1, x1];
                                tempVal[matSize - 1 - y1, x1] = temp;
                            }
                        }
                        return false;
                    }
                }
            }
        }

        CurrentRot.z -= 90;
        CurrentPos.x += tempXOffset;
        xOffset += tempXOffset;
        CurrentPos.y += tempYOffset;
        yOffset += tempYOffset;
        TetrisGrid = tempVal;
        UpdateRotationCoordinates(tempXOffset, tempYOffset);
        transform.rotation = Quaternion.Euler(CurrentRot);
        transform.position = CurrentPos;
        return true;
    }

    bool TestNegativeRotation(int TestNum)
    {
        Vector2[,] offsetFound = new Vector2[,]
        {
            {new Vector2(0,0),new Vector2(1,0), new Vector2(1,-1), new Vector2(0,2), new Vector2(1,2) },
            {new Vector2(0,0), new Vector2(-1,0), new Vector2(-1,1), new Vector2(0,-2), new Vector2(-1,-2) },
            {new Vector2(0,0), new Vector2(-1,0), new Vector2(-1,-1), new Vector2(0,2), new Vector2(-1,2)},
            {new Vector2(0,0), new Vector2(1, 0), new Vector2(1,1), new Vector2(0,-2), new Vector2(1,-2) }
        };

        int tempXOffset = (int)offsetFound[zOffset, TestNum].x;
        int tempYOffset = (int)offsetFound[zOffset, TestNum].y;

        int[,] tempVal = TetrisGrid;
        int matSize = tempVal.GetLength(0);

        //negative 90 algorithm
        for (int x = 0; x < matSize / 2; x++)
        {
            for (int y = x; y < matSize - x - 1; y++)
            {
                int temp = tempVal[x, y];
                tempVal[x, y] = tempVal[y, matSize - 1 - x];
                tempVal[y, matSize - 1 - x] = tempVal[matSize - 1 - x, matSize - 1 - y];
                tempVal[matSize - 1 - x, matSize - 1 - y] = tempVal[matSize - 1 - y, x];
                tempVal[matSize - 1 - y, x] = temp;
            }
        }

        //check if updated grid is able to rotate
        for (int y = tempVal.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < tempVal.GetLength(0); x++)
            {
                if (tempVal[x, y] == 1)
                {
                    //if checks fail (i.e. wall/piece near it before it rotates), rotate will fail and get reverted
                    if (GridManager.GridSize[x + xOffset + tempXOffset, GridManager.GridSize.GetLength(1) + yOffset + y - 1 + tempYOffset] == 2 || OnPlacedBlockRot(x + tempXOffset + xOffset, y + tempYOffset + GridManager.GridSize.GetLength(1) + yOffset - 1))
                    {
                        //positive 90 algorithm
                        for (int x1 = 0; x1 < matSize / 2; x1++)
                        {
                            for (int y1 = x1; y1 < matSize - x1 - 1; y1++)
                            {
                                int temp = tempVal[x1, y1];
                                tempVal[x1, y1] = tempVal[matSize - 1 - y1, x1];
                                tempVal[matSize - 1 - y1, x1] = tempVal[matSize - 1 - x1, matSize - 1 - y1];
                                tempVal[matSize - 1 - x1, matSize - 1 - y1] = tempVal[y1, matSize - 1 - x1];
                                tempVal[y1, matSize - 1 - x1] = temp;
                            }
                        }
                        return false;
                    }
                }
            }
        }

        CurrentRot.z += 90;
        CurrentPos.x += tempXOffset;
        xOffset += tempXOffset;
        CurrentPos.y += tempYOffset;
        yOffset += tempYOffset;
        TetrisGrid = tempVal;
        UpdateRotationCoordinates(tempXOffset, tempYOffset);
        transform.rotation = Quaternion.Euler(CurrentRot);
        transform.position = CurrentPos;
        return true;
    }

    bool TestIPositiveRotation(int TestNum)
    {
        Vector2[,] offsetFound = new Vector2[,]
        {
            {new Vector2(0,0),new Vector2(1,0), new Vector2(-2,0), new Vector2(1,-2), new Vector2(-2,1) },
            {new Vector2(0,0), new Vector2(-2,0), new Vector2(1,0), new Vector2(-2,-1), new Vector2(1,2) },
            {new Vector2(0,0), new Vector2(-1,0), new Vector2(2,0), new Vector2(-1,2), new Vector2(2,-1)},
            {new Vector2(0,0), new Vector2(2,0), new Vector2(-1,0), new Vector2(2,1), new Vector2(-1,-2) }
        };

        int tempXOffset = (int)offsetFound[zOffset, TestNum].x;
        int tempYOffset = (int)offsetFound[zOffset, TestNum].y;

        int[,] tempVal = TetrisGrid;
        int matSize = tempVal.GetLength(0);

        //positive 90 algorithm
        for (int x = 0; x < matSize / 2; x++)
        {
            for (int y = x; y < matSize - x - 1; y++)
            {
                int temp = tempVal[x, y];
                tempVal[x, y] = tempVal[matSize - 1 - y, x];
                tempVal[matSize - 1 - y, x] = tempVal[matSize - 1 - x, matSize - 1 - y];
                tempVal[matSize - 1 - x, matSize - 1 - y] = tempVal[y, matSize - 1 - x];
                tempVal[y, matSize - 1 - x] = temp;
            }
        }

        //check if updated grid is able to rotate
        for (int y = tempVal.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < tempVal.GetLength(0); x++)
            {
                if (tempVal[x, y] == 1)
                {
                    if (transform.position.x >= -3 && transform.position.x <= 3)
                    {
                        if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + y + yOffset - tempYOffset] == 2 || OnPlacedBlockRot(x + tempXOffset + xOffset, y + tempYOffset + GridManager.GridSize.GetLength(1) + yOffset - 1))
                        {
                            //negative 90 algorithm
                            for (int x1 = 0; x1 < matSize / 2; x1++)
                            {
                                for (int y1 = x1; y1 < matSize - x1 - 1; y1++)
                                {
                                    int temp = tempVal[x1, y1];
                                    tempVal[x1, y1] = tempVal[y1, matSize - 1 - x1];
                                    tempVal[y1, matSize - 1 - x1] = tempVal[matSize - 1 - x1, matSize - 1 - y1];
                                    tempVal[matSize - 1 - x1, matSize - 1 - y1] = tempVal[matSize - 1 - y1, x1];
                                    tempVal[matSize - 1 - y1, x1] = temp;
                                }
                            }
                            return false;
                        }
                    }
                    else if (transform.position.x == 4 || transform.position.x == -4)
                    {
                        for (int x1 = 0; x1 < matSize / 2; x1++)
                        {
                            for (int y1 = x1; y1 < matSize - x1 - 1; y1++)
                            {
                                int temp = tempVal[x1, y1];
                                tempVal[x1, y1] = tempVal[y1, matSize - 1 - x1];
                                tempVal[y1, matSize - 1 - x1] = tempVal[matSize - 1 - x1, matSize - 1 - y1];
                                tempVal[matSize - 1 - x1, matSize - 1 - y1] = tempVal[matSize - 1 - y1, x1];
                                tempVal[matSize - 1 - y1, x1] = temp;
                            }
                        }
                        return false;
                    }
                }
            }
        }
        CurrentRot.z -= 90;

        CurrentPos.x += tempXOffset;
        xOffset += tempXOffset;
        CurrentPos.y += tempYOffset;
        yOffset += tempYOffset;
        TetrisGrid = tempVal;
        UpdateRotationCoordinates(tempXOffset, tempYOffset);
        transform.rotation = Quaternion.Euler(CurrentRot);
        transform.position = CurrentPos;
        return true;
    }

    bool TestINegativeRotation(int TestNum)
    {
        Vector2[,] offsetFound = new Vector2[,]
        {
            {new Vector2(0,0),new Vector2(2,0), new Vector2(-1,0), new Vector2(2,1), new Vector2(-1,-2) },
            {new Vector2(-1,0), new Vector2(1,0), new Vector2(-2,0), new Vector2(1,-2), new Vector2(-2,1) },
            {new Vector2(-1,1), new Vector2(-2,0), new Vector2(1,0), new Vector2(-2,-1), new Vector2(1,2)},
            {new Vector2(0,1), new Vector2(-1, 0), new Vector2(2,0), new Vector2(-1,2), new Vector2(2,-1) }
        };

        int tempXOffset = (int)offsetFound[zOffset, TestNum].x;
        int tempYOffset = (int)offsetFound[zOffset, TestNum].y;

        int[,] tempVal = TetrisGrid;
        int matSize = tempVal.GetLength(0);

        //negative 90 algorithm
        for (int x = 0; x < matSize / 2; x++)
        {
            for (int y = x; y < matSize - x - 1; y++)
            {
                int temp = tempVal[x, y];
                tempVal[x, y] = tempVal[y, matSize - 1 - x];
                tempVal[y, matSize - 1 - x] = tempVal[matSize - 1 - x, matSize - 1 - y];
                tempVal[matSize - 1 - x, matSize - 1 - y] = tempVal[matSize - 1 - y, x];
                tempVal[matSize - 1 - y, x] = temp;
            }
        }
        //check if updated grid is able to rotate
        for (int y = tempVal.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < tempVal.GetLength(0); x++)
            {
                if (tempVal[x, y] == 1)
                {
                    if (transform.position.x >= -3 && transform.position.x <= 3)
                    {
                        if (GridManager.GridSize[x + xOffset, GridManager.GridSize.GetLength(1) + y + yOffset - tempYOffset] == 2 || OnPlacedBlockRot(x + tempXOffset + xOffset, y + tempYOffset + GridManager.GridSize.GetLength(1) + yOffset - 1))
                        {
                            for (int x1 = 0; x1 < matSize / 2; x1++)
                            {
                                for (int y1 = x1; y1 < matSize - x1 - 1; y1++)
                                {
                                    int temp = tempVal[x1, y1];
                                    tempVal[x1, y1] = tempVal[matSize - 1 - y1, x1];
                                    tempVal[matSize - 1 - y1, x1] = tempVal[matSize - 1 - x1, matSize - 1 - y1];
                                    tempVal[matSize - 1 - x1, matSize - 1 - y1] = tempVal[y1, matSize - 1 - x1];
                                    tempVal[y1, matSize - 1 - x1] = temp;
                                }
                            }
                            return false;
                        }
                    }
                    else if (transform.position.x == 4 || transform.position.x == -4)
                    {
                        for (int x1 = 0; x1 < matSize / 2; x1++)
                        {
                            for (int y1 = x1; y1 < matSize - x1 - 1; y1++)
                            {
                                int temp = tempVal[x1, y1];
                                tempVal[x1, y1] = tempVal[matSize - 1 - y1, x1];
                                tempVal[matSize - 1 - y1, x1] = tempVal[matSize - 1 - x1, matSize - 1 - y1];
                                tempVal[matSize - 1 - x1, matSize - 1 - y1] = tempVal[y1, matSize - 1 - x1];
                                tempVal[y1, matSize - 1 - x1] = temp;
                            }
                        }
                        return false;
                    }
                }
            }
        }

        CurrentRot.z += 90;
        CurrentPos.x += tempXOffset;
        xOffset += tempXOffset;
        CurrentPos.y += tempYOffset;
        yOffset += tempYOffset;
        TetrisGrid = tempVal;
        UpdateRotationCoordinates(tempXOffset, tempYOffset);
        transform.rotation = Quaternion.Euler(CurrentRot);
        transform.position = CurrentPos;
        return true;
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

    void UpdateRotationCoordinates(int xOff, int yOff)
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
                    GridManager.GridSize[x + xOffset + xOff, GridManager.GridSize.GetLength(1) + yOffset + y - 1 + yOff] = TetrisGrid[x, y];
                    CurrentPosition[x + xOffset + xOff, CurrentPosition.GetLength(1) + yOffset + y - 1 + yOff] = TetrisGrid[x, y];
                }
            }
        }
    }

    string TetrisPieceName()
    {
        return name.Split('(')[0];
    }
}
