using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanGhostController : MonoBehaviour
{
    public List<GameObject> Path;
    public PacmanAIManager PacAIManager;
    public PacmanMapGenerator PacMap;
    public GameObject CurrentNode;

    public bool pathFound = false;

    [SerializeField] private Sprite LeftLook, RightLook, UpLook, DownLook;
    private SpriteRenderer CurSprite;

    Vector2 PrevPos, CurPos;

    // Start is called before the first frame update
    void Start()
    {
        Path = new List<GameObject>();
        Invoke("GeneratePath", 1.0f);
        CurPos = transform.position;
        PrevPos = CurPos;
        CurSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //FindPath();
        if (pathFound)
        {
            FindPath();
        }

        CreatePath();
        UpdateSprite();
    }

    void UpdateSprite()
    {
        CurPos = transform.position;
        if (PrevPos.x < CurPos.x && PrevPos.y == CurPos.y)
        {
            CurSprite.sprite = RightLook;
        }
        else if (PrevPos.x > CurPos.x && PrevPos.y == CurPos.y)
        {
            CurSprite.sprite = LeftLook;
        }
        else if (PrevPos.y < CurPos.y && PrevPos.x == CurPos.x)
        {
            CurSprite.sprite = UpLook;
        }
        else if (PrevPos.y > CurPos.y && PrevPos.x == CurPos.x)
        {
            CurSprite.sprite = DownLook;
        }
        PrevPos = CurPos;
    }

    void CreatePath()
    {
        /*        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetMouseButtonDown(0))
                {
                    Path = new List<GameObject>();
                    pathFound = false;
                    Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacAIManager.FindNearestNode(MousePos, PacMap.NodeList), PacMap.NodeList);
                    pathFound = true;
                }*/

        if (Path.Count == 1)
        {
            pathFound = false;
            Path = new List<GameObject>();
            int RandomPath = Random.Range(0, PacMap.NodeList.Count);
            Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacMap.NodeList[RandomPath], PacMap.NodeList);
            pathFound = true;
        }
    }

    public void GeneratePath()
    {
        int RandomPath = Random.Range(0, PacMap.NodeList.Count);
        Path = PacAIManager.BreadthFirstSearch(CurrentNode, PacMap.NodeList[RandomPath], PacMap.NodeList);
        pathFound = true;
    }

    public void FindPath()
    {
        if (Path != null)
        {
            if (Path.Count > 0)
            {
                for (int i = 0; i < Path.Count - 1; i++)
                {
                    //for (int j = i + 1; j < Path.Count; j++)
                    //{
                    Debug.DrawLine(Path[i].transform.position, Path[i + 1].transform.position, new Color(0, 0, 1));
                    //}
                }


                int x = 0;
                transform.position = Vector2.MoveTowards(transform.position, Path[0].transform.position, 5.0f * Time.deltaTime);
                if (transform.position == Path[x].transform.position)
                {
                    if (Path.Count > 1)
                    {
                        CurrentNode = Path[x + 1];
                        Path.RemoveAt(x);
                    }
                }
            }
        }
    }
}
