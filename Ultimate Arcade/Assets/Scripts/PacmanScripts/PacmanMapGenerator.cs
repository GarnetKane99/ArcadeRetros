using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMapGenerator : MonoBehaviour
{
    private int[,] MapCoordinates;
    [SerializeField] private GameObject[] MAP_TILES;
    [SerializeField] private GameObject PACMAN_NODE;
    [SerializeField] private GameObject PACMAN_PELLET, PACMAN_PELLET_LARGE;
    [SerializeField] private GameObject ParentMapObject, ParentNodeObject, PelletParent;
    [SerializeField] private GameObject Inky, Pinky, Blinky, Clyde, PacmanPrefab;
    [SerializeField] private PacmanAIManager AIManager;

    public List<GameObject> NodeList;
    private List<GameObject> SpecialNode;
    int NodeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        NodeList = new List<GameObject>();
        SpecialNode = new List<GameObject>();
        InitializeMapCoordinates();
        StartCoroutine(CreateMap());
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
                        CreateNodeToo(y, x);
                        break;
                    case 51:
                        CreateLargePellet(y, x);
                        break;
                    case 55:
                        CreateSpecialNode(y, x);
                        break;
                    case 60:
                        break;
                    default:
                        DrawMap(y, x, MapCoordinates[x, y]);
                        yield return new WaitForSeconds(0.01f);
                        break;
                }

                if (x == MapCoordinates.GetLength(0) - 1 && y == MapCoordinates.GetLength(1) - 1)
                {
                    FindConnections();
                }
            }
        }
    }

    void CreateNode(int x, int y)
    {
        GameObject Node = Instantiate(PACMAN_NODE, new Vector2(x - 13, y - 7), Quaternion.identity);
        GameObject Pellet = Instantiate(PACMAN_PELLET, new Vector2(x - 13, y - 7), Quaternion.identity);
        Node.transform.parent = ParentNodeObject.transform;
        Node.name = "Node: " + NodeCount;
        NodeCount++;
        Pellet.transform.parent = PelletParent.transform;
        NodeList.Add(Node);
    }

    void CreateNodeToo(int x, int y)
    {
        GameObject Node = Instantiate(PACMAN_NODE, new Vector2(x - 13, y - 7), Quaternion.identity);
        Node.transform.parent = ParentNodeObject.transform;
        Node.name = "Node: " + NodeCount;
        NodeCount++;
        NodeList.Add(Node);
    }

    void CreateSpecialNode(int x, int y)
    {
        GameObject Node = Instantiate(PACMAN_NODE, new Vector2(x - 13, y - 7), Quaternion.identity);
        Node.transform.parent = ParentNodeObject.transform;
        Node.name = "Node: " + NodeCount;
        NodeCount++;
        NodeList.Add(Node);
        SpecialNode.Add(Node);
    }
    void CreateLargePellet(int x, int y)
    {
        GameObject LargePellete = Instantiate(PACMAN_PELLET_LARGE, new Vector2(x - 13, y - 7), Quaternion.identity);
        LargePellete.transform.parent = PelletParent.transform;

        GameObject Node = Instantiate(PACMAN_NODE, new Vector2(x - 13, y - 7), Quaternion.identity);
        Node.transform.parent = ParentNodeObject.transform;
        Node.name = "Node: " + NodeCount;
        NodeCount++;
        NodeList.Add(Node);
    }

    void FindConnections()
    {
        AIManager.TotalNodeCount = NodeList.Count;
        for (int i = 0; i < NodeList.Count; i++)
        {
            for (int j = i + 1; j < NodeList.Count; j++)
            {
                if (Vector2.Distance(NodeList[i].transform.position, NodeList[j].transform.position) > 0 &&
                Vector2.Distance(NodeList[i].transform.position, NodeList[j].transform.position) <= 1.0f)
                {
                    AddConnections(NodeList[i], NodeList[j]);
                    AddConnections(NodeList[j], NodeList[i]);
                }
            }
        }

        for (int i = 0; i < SpecialNode.Count; i++)
        {
            for (int j = i + 1; j < SpecialNode.Count; j++)
            {
                //Debug.Log("Called");
                AddConnections(SpecialNode[i], SpecialNode[j]);
                AddConnections(SpecialNode[j], SpecialNode[i]);
            }
        }
        StartCoroutine(SpawnGhosts());
    }

    IEnumerator SpawnGhosts()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject InkyGO = Instantiate(Inky, new Vector2(-1.5f, 9), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        GameObject BlinkyGO = Instantiate(Blinky, new Vector2(0.5f, 12), Quaternion.identity);
        PacmanGhostController BlinkyController = BlinkyGO.GetComponent<PacmanGhostController>();
        BlinkyController.PacAIManager = AIManager;
        BlinkyController.PacMap = this;
        foreach (GameObject Node in NodeList)
        {
            if (Node.transform.position.x - 0.5f == BlinkyGO.transform.position.x && Node.transform.position.y == BlinkyGO.transform.position.y)
            {
                BlinkyController.CurrentNode = Node;
                break;
            }
        }
        GameObject PinkyGO = Instantiate(Pinky, new Vector2(0.5f, 9), Quaternion.identity);

        GameObject PacmanGO = Instantiate(PacmanPrefab, new Vector2(0.5f, 0), Quaternion.identity);
        PacmanController PacController = PacmanGO.GetComponent<PacmanController>();
        yield return new WaitForSeconds(0.5f);
        GameObject ClydeGO = Instantiate(Clyde, new Vector2(2.5f, 9), Quaternion.identity);
        PacController.GameStarted = true;
    }

    void AddConnections(GameObject From, GameObject To)
    {
        From.GetComponent<PacNodeController>().ConnectedNodes.Add(To);
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
            {18,51,0,0,25,26,0,0,0,0,0,0,0,50,50,0,0,0,0,0,0,0,25,26,0,0,51,17 }, //row 8
            {18,0,7,22,14,26,0,7,22,22,22,8,0,7,8,0,7,22,22,22,8,0,25,13,22,8,0,17 }, //row 9
            {18,0,5,21,21,6,0,5,21,21,21,6,0,25,26,0,5,21,21,21,6,0,5,21,21,6,0,17 }, //row 10
            {18,0,0,0,0,0,0,0,0,0,0,0,0,25,26,0,0,0,0,0,0,0,0,0,0,0,0,17}, //row 11
            {1,20,20,20,20,8,0,7,8,50,7,22,22,14,13,22,22,8,50,7,8,0,7,20,20,20,20,2 }, //row 12
            {60,60,60,60,60,18,0,25,26,50,5,21,21,21,21,21,21,6,50,25,26,0,17,60,60,60,60,60 }, //row 13
            {60,60,60,60,60,18,0,25,26,50,50,50,50,50,50,50,50,50,50,25,26,0,17,60,60,60,60,60 }, //row 14
            {60,60,60,60,60,18,0,25,26,50,32,33,33,33,33,33,33,34,50,25,26,0,17,60,60,60,60,60 }, //row 15
            {19,19,19,19,19,6,0,5,6,50,30,60,60,60,60,60,60,31,50,5,6,0,5,19,19,19,19,19 }, //row 16
            {55,50,50,50,50,50,0,50,50,50,30,60,60,60,60,60,60,31,50,50,50,0,50,50,50,50,50,55 }, //row 17
            {20,20,20,20,20,8,0,7,8,50,30,60,60,60,60,60,60,31,50,7,8,0,7,20,20,20,20,20 }, //row 18
            {60,60,60,60,60,18,0,25,26,50,27,28,35,60,60,36,28,29,50,25,26,0,17,60,60,60,60,60 }, // row 19
            {60,60,60,60,60,18,0,25,26,50,50,50,50,50,50,50,50,50,50,25,26,0,17,60,60,60,60,60 }, //row 20
            {60,60,60,60,60,18,0,25,13,22,22,8,50,7,8,50,7,22,22,14,26,0,17,60,60,60,60,60 }, //row 21
            {3,19,19,19,19,6,0,25,15,21,21,6,50,25,26,50,5,21,21,16,26,0,5,19,19,19,19,4 }, //row 22
            {18,0,0,0,0,0,0,25,26,0,0,0,0,25,26,0,0,0,0,25,26,0,0,0,0,0,0,17 }, //row 23
            {18,0,7,22,22,8,0,25,26,0,7,22,22,14,13,22,22,8,0,25,26,0,7,22,22,8,0,17 }, //row 24
            {18,0,5,21,21,6,0,5,6,0,5,21,21,21,21,21,21,6,0,5,6,0,5,21,21,6,0,17 }, //row 25
            {18,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,17 }, //row 26
            {18,0,7,22,22,8,0,7,22,22,22,8,0,7,8,0,7,22,22,22,8,0,7,22,22,8,0,17 }, //row 27
            {18,51,25,37,37,26,0,25,37,37,37,26,0,25,26,0,25,37,37,37,26,0,25,37,37,26,51,17 }, //row 28
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
