using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TetrisBoardManager : MonoBehaviour
{
    public int[,] GridSize;
    public Sprite TileSprite;
    public GameObject BorderParent;
    public List<GameObject> Children;
    private List<GameObject> BorderChild;
    private List<GameObject> BorderInner;

    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private TetrisScoreHandler Score;
    [SerializeField] private List<GameObject> Pieces;
    [SerializeField] private TetrisMainGeneration MainGen;

    // Start is called before the first frame update
    void Start()
    {
        BorderChild = new List<GameObject>();
        BorderInner = new List<GameObject>();
        InitializeGrid();
        Children = new List<GameObject>();
        MainGen = GetComponent<TetrisMainGeneration>();
    }

    void InitializeGrid()
    {
        GridSize = new int[11, 22];
        for (int x = 0; x < GridSize.GetLength(0); x++)
        {
            for (int y = 0; y < GridSize.GetLength(1); y++)
            {
                if (x == 0 || x == GridSize.GetLength(0) - 1 || y == 0)
                {
                    DrawGrid(x, y, 1);
                    GridSize[x, y] = 2;
                }
                else
                {
                    DrawGrid(x, y, 0);
                    GridSize[x, y] = 0;
                }
            }
        }
    }

    public void DrawGrid(int x, int y, int val)
    {
        GameObject Tile = new GameObject();
        Tile.transform.position = new Vector2(x - 5, y - 9);
        var Sprite = Tile.AddComponent<SpriteRenderer>();
        if (val == 0)
        {
            Sprite.color = new Color(1, 1, 1, 25f / 255f);
            BorderInner.Add(Tile);
        }
        else
        {
            BorderChild.Add(Tile);
        }
        Sprite.sprite = TileSprite;
        Tile.transform.parent = BorderParent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = GridSize.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < GridSize.GetLength(0); x++)
            {
                if (y == GridSize.GetLength(1) - 1)
                {
                    GridSize[x, y] = 0;
                }
                sb.Append(GridSize[x, y]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        //Debug.Log(sb.ToString());
    }

    public void UpdateBlocks()
    {
        foreach (GameObject Child in Children)
        {
            Destroy(Child.gameObject);
        }
        Children.Clear();
        for (int y = GridSize.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < GridSize.GetLength(0); x++)
            {
                switch (GridSize[x, y])
                {
                    case 3:
                        GameObject TetrisI = Instantiate(Pieces[0], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisI.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisI);
                        break;
                    case 4:
                        GameObject TetrisO = Instantiate(Pieces[1], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisO.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisO);
                        break;
                    case 5:
                        GameObject TetrisL = Instantiate(Pieces[2], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisL.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisL);
                        break;
                    case 6:
                        GameObject TetrisJ = Instantiate(Pieces[3], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisJ.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisJ);
                        break;
                    case 7:
                        GameObject TetrisZ = Instantiate(Pieces[4], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisZ.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisZ);
                        break;
                    case 8:
                        GameObject TetrisS = Instantiate(Pieces[5], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisS.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisS);
                        break;
                    case 9:
                        GameObject TetrisT = Instantiate(Pieces[6], new Vector2(x - 5, y - 9), Quaternion.identity);
                        TetrisT.GetComponent<TetrisIndividualPieceHandle>().Pos = new Vector2(x - 5, y - 9);
                        Children.Add(TetrisT);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void FindFinishedRow()
    {
        UpdateBlocks();
        for (int y = GridSize.GetLength(1) - 1; y >= 0; y--)
        {
            int Found = 0;
            for (int x = 0; x < GridSize.GetLength(0); x++)
            {
                if (y != 0 && y != GridSize.GetLength(1) - 1)
                {
                    if (FoundRow(x, y))
                    {
                        Found++;
                    }
                }
                else if (y == GridSize.GetLength(1) - 1)
                {
                    if (FoundRow(x, y-1))
                    {
                        EndGame();
                        return;
                    }
                }

            }

            if (Found == 9)
            {
                DeleteRow(y);
            }
        }
    }

    void EndGame()
    {
        foreach (GameObject BorderChildren in BorderChild)
        {
            SpriteRenderer ColourFound = BorderChildren.GetComponent<SpriteRenderer>();
            ColourFound.color = new Color(ColourFound.color.r, ColourFound.color.g, ColourFound.color.b, 100f / 255f);
        }
        foreach(GameObject BorderIn in BorderInner)
        {
            SpriteRenderer ColourFound = BorderIn.GetComponent<SpriteRenderer>();
            ColourFound.color = new Color(ColourFound.color.r, ColourFound.color.g, ColourFound.color.b, ColourFound.color.a - 10f / 255f);
        }
        foreach (GameObject TileChildren in Children)
        {
            SpriteRenderer ColourFound = TileChildren.GetComponent<SpriteRenderer>();
            ColourFound.color = new Color(ColourFound.color.r, ColourFound.color.g, ColourFound.color.b, 100f / 255f);
        }

        MainGen.enabled = false;
        GameOverScreen.SetActive(true);
    }

    bool FoundRow(int x, int y)
    {
        if (GridSize[x, y] == 3
            || GridSize[x, y] == 4
            || GridSize[x, y] == 5
            || GridSize[x, y] == 6
            || GridSize[x, y] == 7
            || GridSize[x, y] == 8
            || GridSize[x, y] == 9)
        {
            return true;
        }

        return false;
    }

    void DeleteRow(int RowFound)
    {
        Score.SetCurrentScore();
        Score.SetCurrentLevel();

        int[,] tempArray = GridSize;

        for (int y = 0; y < tempArray.GetLength(1); y++)
        {
            for (int x = 1; x < tempArray.GetLength(0) - 1; x++)
            {
                if (y >= RowFound)
                {
                    if (y < tempArray.GetLength(1) - 1)
                    {
                        if (tempArray[x, y + 1] != 2)
                        {
                            tempArray[x, y] = tempArray[x, y + 1];
                        }
                    }
                }
            }
        }

        DestroyChildren(RowFound);
        GridSize = tempArray;
    }

    void DestroyChildren(int Row)
    {
        List<GameObject> TempList = new List<GameObject>();
        TempList = Children;

        for (int i = TempList.Count - 1; i >= 0; i--)
        {
            if ((int)TempList[i].transform.position.y == Row - 9)
            {
                Destroy(TempList[i].gameObject);
                TempList[i] = TempList[TempList.Count - 1];
                TempList.RemoveAt(TempList.Count - 1);
            }
        }
        TempList = Children;
        for (int i = 0; i < TempList.Count; i++)
        {
            if ((int)TempList[i].transform.position.y > Row - 9)
            {
                TempList[i].transform.position = new Vector2(Mathf.FloorToInt(TempList[i].transform.position.x), Mathf.FloorToInt(TempList[i].transform.position.y - 1));
                TempList[i].GetComponent<TetrisIndividualPieceHandle>().GetYPos();
            }
        }
        Children = TempList;
    }
}
