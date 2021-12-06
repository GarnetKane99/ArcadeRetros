using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TetrisBoardManager : MonoBehaviour
{
    public int[,] GridSize;
    public Sprite TileSprite;
    public GameObject BorderParent;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
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
}
