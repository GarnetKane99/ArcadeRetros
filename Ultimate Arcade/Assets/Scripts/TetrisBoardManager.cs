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

    [SerializeField] private TetrisScoreHandler Score;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
        Children = new List<GameObject>();
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
        Debug.Log(sb.ToString());
    }

    public void FindFinishedRow()
    {
        for(int y = GridSize.GetLength(1)-1; y >= 0; y--)
        {
            int Found = 0;
            for(int x = 0; x < GridSize.GetLength(0); x++)
            {
                if(y != 0 && y != GridSize.GetLength(1) - 1)
                {
                    if(GridSize[x,y] == 3)
                    {
                        Found++;
                    }
                }
            }

            if(Found == 9)
            {
                DeleteRow(y);
            }
        }
    }
    // * IDEA *
    // Have a list of game objects - give each letter a score from 3 to 10 -
    // Can 'destroy' the letters and reinstatiate them in new positions purely according
    // to the grid position rather than the actual vector position
    // Use similar the DrawGrid part!!!!
    void DeleteRow(int RowFound)
    {
        Score.SetCurrentScore();
        Score.SetCurrentLevel();

        int[,] tempArray = GridSize;

        for (int y = 0; y < tempArray.GetLength(1); y++)
        {
            for (int x = 1; x < tempArray.GetLength(0)-1; x++)
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
    }
}
