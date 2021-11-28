using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    public List<GameObject> TilesInRow;
    public bool Populated = false;

    void Start()
    {
        //TilesInRow = new List<GameObject>();
    }

    void Update()
    {
        CheckRows();
    }

    void CheckRows()
    {
        if (TilesInRow.Count != 0)
        {
            for (int i = 0; i < TilesInRow.Count; i++)
            {
                if (TilesInRow[i].GetComponent<TileHandler>().Populated)
                {
                    continue;
                }
                else
                {
                    return;
                }
            }
            //If it reaches this part of the code, assume that all tiles in row are populated 
            //and objects can get destroyed
        }
    }
}
