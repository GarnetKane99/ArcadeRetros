using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{
    public GameObject TileToCreate;
    public GameObject ParentTile;
    GameObject CurTile;
    private int curY = default;
    private int prevY = default;
    List<GameObject> TilesToAdd;

    void Start()
    {
        TilesToAdd = new List<GameObject>();
        CreateTiles();
    }

    void CreateTiles()
    {
        for (int y = -9; y < 14; y++)
        {
            curY = y;
            for(int x = -4; x < 6; x++)
            {
                Vector2 Pos = new Vector2(x, y);
                GameObject Tile = Instantiate(TileToCreate, Pos, Quaternion.identity);
                Tile.transform.parent = ParentTile.transform;
                TilesToAdd.Add(Tile);

                if (prevY != curY)
                {
                    TilesToAdd.Remove(Tile);
                    if (CurTile != null)
                    {
                        CurTile.GetComponent<TileHandler>().TilesInRow.Add(CurTile);
                        for (int i = 0; i < TilesToAdd.Count; i++)
                        {
                            CurTile.GetComponent<TileHandler>().TilesInRow.Add(TilesToAdd[i]);
                        }
                    }
                    TilesToAdd = new List<GameObject>();
                    CurTile = Tile;
                    prevY = curY;
                }
            }
        }
    }

    void Update()
    {
        
    }
}
