using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMapGenerator : MonoBehaviour
{
    private int[,] MapCoordinates;
    [SerializeField] private GameObject[] MAP_TILES;
    [SerializeField] private GameObject PACMAN_NODE;
    [SerializeField] private GameObject PACMAN_PELLET;
    [SerializeField] private GameObject ParentMapObject, ParentNodeObject, PelletParent;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMapCoordinates();
        StartCoroutine(CreateMap());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CreateMap()
    {
        for (int x = 0; x < MapCoordinates.GetLength(0); x++)
        {
            for (int y = 0; y < MapCoordinates.GetLength(1); y++)
            {
                switch (MapCoordinates[x, y])
                {
                    case 0:
                        CreateNode(y, x);
                        break;
                    case 50:
                        break;
                    default:
                        DrawMap(y, x, MapCoordinates[x, y]);
                        yield return new WaitForSeconds(0.01f);
                        break;
                }
            }
        }
    }

    void CreateNode(int x, int y)
    {
        GameObject Node = Instantiate(PACMAN_NODE, new Vector2(x-13, y-7), Quaternion.identity);
        GameObject Pellet = Instantiate(PACMAN_PELLET, new Vector2(x - 13, y - 7), Quaternion.identity);
        Node.transform.parent = ParentNodeObject.transform;
        Pellet.transform.parent = PelletParent.transform;
    }

    void InitializeMapCoordinates()
    {
        //starting from bottom
        MapCoordinates = new int[,]
        {
            {3,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,19,4 },//row 1
            {18,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,17 }, //row 2
            {18,0,7,22,22,22,22,22,22,22,22,8,0,7,8,0,7,22,22,22,22,22,22,22,22,8,0,17 }, //row 3
            {18,0,5,21,21,21,21,16,15,21,21,6,0,25,26,0,5,21,21,16,15,21,21,21,21,6,0,17 }, //row 4
            {18,0,0,0,0,0,0,25,26,0,0,0,0,25,26,0,0,0,0,25,26,0,0,0,0,0,0,17 }, //row 5
            {9,22,8,0,7,8,0,25,26,0,7,22,22,14,13,22,22,8,0,25,26,0,7,8,0,7,22,10 }, //row 6
            {11,21,6,0,25,26,0,5,6,0,5,21,21,21,21,21,21,6,0,5,6,0,25,26,0,5,21,12 }, // row 7
            {18,0,0,0,25,26,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,25,26,0,0,0,17 }, //row 8
            {18,0,7,22,14,26,0,7,22,22,22,8,0,7,8,0,7,22,22,22,8,0,25,13,22,8,0,17 }, //row 9
            {18,0,5,21,21,6,0,5,21,21,21,6,0,25,26,0,5,21,21,21,6,0,5,21,21,6,0,17 }, //row 10
            {18,0,0,0,0,0,0,0,0,0,0,0,0,25,26,0,0,0,0,0,0,0,0,0,0,0,0,17}, //row 11
            {1,20,20,20,20,8,0,7,8,0,7,22,22,14,13,22,22,8,0,7,8,0,7,20,20,20,20,2 }, //row 12
            {50,50,50,50,50,18,0,25,26,0,5,21,21,21,21,21,21,6,0,25,26,0,17,50,50,50,50,50 }, //row 13
            {50,50,50,50,50,18,0,25,26,0,0,0,0,0,0,0,0,0,0,25,26,0,17,50,50,50,50,50 }, //row 14
            {50,50,50,50,50,18,0,25,26,0,32,33,33,33,33,33,33,34,0,25,26,0,17,50,50,50,50,50 }, //row 15
            {19,19,19,19,19,6,0,5,6,0,30,50,50,50,50,50,50,31,0,5,6,0,5,19,19,19,19,19 }, //row 16
            {0,0,0,0,0,0,0,0,0,0,30,50,50,50,50,50,50,31,0,0,0,0,0,0,0,0,0,0 }, //row 17
            {20,20,20,20,20,8,0,7,8,0,30,50,50,50,50,50,50,31,0,7,8,0,7,20,20,20,20,20 }, //row 18
            {50,50,50,50,50,18,0,25,26,0,27,28,35,50,50,36,28,29,0,25,26,0,17,50,50,50,50,50 }, // row 19
            {50,50,50,50,50,18,0,25,26,0,0,0,0,0,0,0,0,0,0,25,26,0,17,50,50,50,50,50 }, //row 20
            {50,50,50,50,50,18,0,25,13,22,22,8,0,7,8,0,7,22,22,14,26,0,17,50,50,50,50,50 }, //row 21
            {3,19,19,19,19,6,0,25,15,21,21,6,0,25,26,0,5,21,21,16,26,0,5,19,19,19,19,4 }, //row 22
            {18,0,0,0,0,0,0,25,26,0,0,0,0,25,26,0,0,0,0,25,26,0,0,0,0,0,0,17 }, //row 23
            {18,0,7,22,22,8,0,25,26,0,7,22,22,14,13,22,22,8,0,25,26,0,7,22,22,8,0,17 }, //row 24
            {18,0,5,21,21,6,0,5,6,0,5,21,21,21,21,21,21,6,0,5,6,0,5,21,21,6,0,17 }, //row 25
            {18,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,17 }, //row 26
            {18,0,7,22,22,8,0,7,22,22,22,8,0,7,8,0,7,22,22,22,8,0,7,22,22,8,0,17 }, //row 27
            {18,0,25,37,37,26,0,25,37,37,37,26,0,25,26,0,25,37,37,37,26,0,25,37,37,26,0,17 }, //row 28
            {18,0,5,21,21,6,0,5,21,21,21,6,0,25,26,0,5,21,21,21,6,0,5,21,21,6,0,17 }, //row 29
            {18,0,0,0,0,0,0,0,0,0,0,0,0,25,26,0,0,0,0,0,0,0,0,0,0,0,0,17 }, //row 30
            {1,20,20,20,20,20,20,20,20,20,20,20,20,14,13,20,20,20,20,20,20,20,20,20,20,20,20,2 } //row 31
        };
    }

    void DrawMap(int x, int y, int Pos)
    {
        GameObject Border = Instantiate(MAP_TILES[Pos - 1], new Vector2(x - 13, y - 7), Quaternion.identity);
        Border.transform.parent = ParentMapObject.transform;
    }
}
